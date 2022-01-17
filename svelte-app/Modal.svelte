<script>
    import { createEventDispatcher, onDestroy, tick } from "svelte";
    import { fly, fade, slide, crossfade } from "svelte/transition";
    import { backOut } from "svelte/easing";
    import { Editor, Delta, normalizeRange, Source } from "typewriter-editor";
    import { paragraph, header, list, blockquote } from "typewriter-editor/lib/typesetting/lines";
    import { link, bold, italic } from "typewriter-editor/lib/typesetting/formats";
    import { br } from "typewriter-editor/lib/typesetting/embeds";
    import { format, line } from "typewriter-editor/lib/typesetting";
    import { h } from "typewriter-editor/lib/rendering/vdom";
    import { placeholder, smartEntry, smartQuotes } from "typewriter-editor/lib/modules";
    import asRoot from "typewriter-editor/lib/asRoot";
    import Toolbar from "typewriter-editor/lib/Toolbar.svelte";
    import categories from "./categories.js";
    import { cards, token } from "./stores.js";
    
    export let card = {};

    let lastSelection = [0, 0];
    
    const dispatch = createEventDispatcher();

    function lineCommand(format) {
        const { doc } = editor;
        if (!lastSelection) return;
        const change = editor.change.delete(lastSelection);
        if (lastSelection[0] === lastSelection[1] && doc.getLineAt(lastSelection[0]).length === 1) {
            change
                .insert(lastSelection[0], "\n", { ...doc.getLineFormat(lastSelection[0]) })
                .formatLine(lastSelection[0], format);
        } else {
            const delta = new Delta()
                .insert('\n', doc.getLineAt(lastSelection[0]).attributes)
                .insert('\n', format);
            change.insertContent(lastSelection[0], delta);
            change.select(lastSelection[0] + 2);
        }
        editor.update(change);
    }

    const underline = format({
        name: "underline",
        selector: "u",
        styleSelector: "[style*='text-decoration:underline'], [style*='text-decoration: underline']",
        commands: editor => () => editor.toggleTextFormat({ underline: true }),
        shortcuts: "Mod+U",
        render: (attributes, children) => h("span", { style: "text-decoration:underline" }, children),
    });

    const image = line({
        name: "image",
        selector: "p.image",
        frozen: true,
        commands: () => lineCommand,
        fromDom: (node) => ({ image: node.firstElementChild.src, alt: node.firstElementChild.alt }),
        render: line => h("p", { class: "image" }, [
            h("img", { src: line.image, alt: line.alt })
        ])
    });

    const video = line({
        name: "video",
        selector: "p.iframe-container iframe[src^='https://www.youtube.com/embed/'], p.iframe-container iframe[src^='https://player.vimeo.com/video/']",
        frozen: true,
        commands: () => lineCommand,
        fromDom: node => {
            return { video: node.src };
        },
        render: embed => {
            return h("p", { class: "iframe-container" }, h("iframe", { src: embed.video, frameborder: 0, allowfullscreen: "" }));
        }
    });

    const editor = new Editor({
        types: {
            lines: [paragraph, header, list, blockquote, image, video],
            formats: [link, bold, italic, underline],
            embeds: [br]
        }
    });

    editor.setDelta(new Delta(JSON.parse(card.Content).ops));
    editor.enabled = false;

    smartEntry()(editor);
    smartQuotes(editor);
    placeholder(() => editing ? "type hier..." : "(leeg)")(editor);
    let editing = false;

    $: editing, editStart();
    function editStart() {
        if (editing) {
            editor.enabled = true;
            let l = editor.doc.length-1;
            editor.select(l, Source.api);
            lastSelection = [l, l];
            decorator = editor.modules.decorations.getDecorator("highlight");
        } else {
            editor.enabled = false;
        }
    }

    let currentInsert = null;
    let decorator;

    let linkUrl = "";
    let linkText = "";
    $: linkText = editor.doc.selection && currentInsert && editor.getText(lastSelection) || linkText;

    $: lastSelection, editor.modules.decorations && currentInsert && updateDecorator();

    let videoUrl = "";
    const youtubeRegex = /^(?:https?:\/\/)?(?:www\.)?(?:(?:youtube\.\w{2,3}\/(?:watch.*(?:\?|&)v=|embed\/))|(?:youtu.be))([^&#]+)/;
    const vimeoRegex = /^(?:https?:\/\/)?(?:www\.)?(?:(?:vimeo\.com)|(?:player\.vimeo\.com\/video))\/([^?#]+)/;
    const dailymotionRegex = /^(?:https?:\/\/)?(?:www\.)?dailymotion\.com\/(?:embed\/)?video\/([^?#]+)/;


    function insert(what) {
        if (what === "link") {
            linkUrl = "";
            linkText = editor.getText(lastSelection)
        } else if (what === "video") {
            videoUrl = "";
        }
        currentInsert = what;
        updateDecorator();
        editor.select(null, Source.api);
    }

    function applyInsert(e) {
        e.preventDefault();
        if (currentInsert === "link")
            editor.insert(linkText, { link: linkUrl }, lastSelection);
        else if (currentInsert === "video")
            editor.commands.video({ video: getEmbedLink(videoUrl) });
        currentInsert = null;
    }

    function cancelInsert(e) {
        e.target.closest("form").setAttribute("novalidate", "");
        currentInsert = null;
        decorator.remove();
        editor.select(lastSelection, Source.api)
    }

    function updateDecorator() {
        decorator.remove();
        decorator = editor.modules.decorations.getDecorator("highlight");
        if (lastSelection[0] === lastSelection[1])
            decorator.insertDecoration(lastSelection[0]);
        else
            decorator.decorateText(lastSelection);
        decorator.apply();
    }

    function uploadImage(e) {
        currentInsert = null;
        let reader = new FileReader();
        reader.onloadend = () => editor.commands.image({image: reader.result});
        reader.readAsDataURL(e.target.files[0]);
    }

    function checkVideo(e) {
        if (youtubeRegex.test(videoUrl) || vimeoRegex.test(videoUrl) || dailymotionRegex.test(videoUrl))
            e.target.setCustomValidity("");
        else
            e.target.setCustomValidity("Geen geldige link");
    }

    function getEmbedLink(url) {
        let match;
        if (match = youtubeRegex.exec(url))
            return "https://www.youtube.com/embed/" + match[1];
        else if (match = vimeoRegex.exec(url))
            return "https://player.vimeo.com/video/" + match[1];
        else if (match = dailymotionRegex.exec(url))
            return "https://www.dailymotion.com/embed/video/" + match[1];
    }

    function clearFormat() {
        let change = editor.change;
        let range = normalizeRange(editor.doc.selection);
        if (range[0] === range[1])
            range = editor.doc.getLineRange(range[0]);
        for (let line of editor.doc.getLinesAt([range[0], range[1] + (editor.getText()[editor.doc.selection[1]] === "\n")], true)) {
            if (!editor.typeset.lines.findByAttributes(line.attributes)?.frozen)
                change.formatLine(editor.doc.getLineRange(line), {})
        }
        for (let format of editor.typeset.formats.list)
            change.formatText(range, {[format.name]: false});
        change.select(range);
        editor.update(change);
        editor.root?.focus();
    }

    function focusInsert(e) {
        e.focus();
    }

    function insertImagesFromDataTransfer(e) {
        for (let file of (e.clipboardData || e.dataTransfer).files) {
            if (file.type.startsWith("image")) {
                e.stopPropagation();
                e.preventDefault();
                let reader = new FileReader();
                reader.onloadend = () => editor.commands.image({image: reader.result});
                reader.readAsDataURL(file);
            }
        }
    }

    function resize() {
        document.querySelectorAll(".buttons > div").forEach(div => {
            if (div.getBoundingClientRect().y == div.nextElementSibling?.getBoundingClientRect().y)
                div.classList.add("vr");
            else
                div.classList.remove("vr");
        });
    }

    $: editing && tick().then(resize);

    const [send, receive] = crossfade({
        duration: d => Math.sqrt(d * 1000)
    });

    let modal;
    let grey;
    let innerHeight;

    let startY;
    let startElement;
    let lastY;
    let dragging = false;
    let firstMove = true;
    let mouseDown = false;

    function touchStart(e) {
        startY = lastY = e.touches?.[0].clientY ?? e.clientY;
        startElement = e.target;
        if (!(e.target.closest(".bottom") && !e.touches) && !grey.scrollTop)
            mouseDown = true;
    }

    function touchMove(e) {
        if (mouseDown) {
            lastY = e.touches?.[0].clientY ?? e.clientY;
            
            if (firstMove) {
                modal.style.transitionDuration = grey.style.transitionDuration = "0s";
                firstMove = false;
            }
            
            if (!dragging && Math.abs(lastY - startY) > 25) {
                dragging = true;
                startY = lastY;
            }
            
            if (dragging) {
                let offset = -Math.min(startY - lastY, 0);
                modal.style.transform = `translateY(${offset}px)`;
                grey.style.background = `rgba(0, 0, 0, ${(1 - offset / innerHeight) * .95})`;
            }
        }
    }

    function touchEnd(e) {
        modal.style.transitionDuration = grey.style.transitionDuration = "";
        if (dragging && lastY - startY > innerHeight/4 && !editing) {
            dispatch("back");
        } else if (Math.abs(lastY - startY) < 25 && e.target === grey && !startElement.closest(".modal") && !editing) {
            dispatch("back");
            e.preventDefault();
        } else {
            grey.style.background = modal.style.transform = "";
        }
        dragging = mouseDown = false;
        firstMove = true;
    }

    let message = "Er is iets misgegaan. Probeer het later opnieuw.";
    
    function saveCard() {
        card.Content = JSON.stringify(editor.getDelta());
        fetch("/saveCard", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": $token
            },
            body: JSON.stringify(card)
        })
        .then(res => res.text())
        .then(res => res !== "Success" && alert(message))
        .catch(() => alert(message));
    }

    let deleting = false;

    function deleteCard() {
        fetch("/deleteCard", {
            method: "POST",
            headers: {
                "Authorization": $token,
                "Id": card.Id
            }
        })
        .then(res => res.text())
        .then(res => {
            if (res === "Success") {
                $cards = $cards.filter(c => c.Id !== card.Id);
            } else {
                alert(message);
            }
        });
    }

    document.body.style.overscrollBehaviorY = "none";

    onDestroy(() => {
        document.body.style.overscrollBehaviorY = "";
        editor.destroy();
    });
</script>

<svelte:window bind:innerHeight on:touchstart={touchStart} on:mousedown={touchStart} on:touchmove={touchMove} on:mousemove={touchMove} on:touchend={touchEnd} on:mouseup={touchEnd} on:resize={resize}/>

<div class="grey" bind:this={grey} on:click={(e) => !editing && e.target.matches(".grey") && !startElement.closest(".modal") && dispatch("back")} in:fade={{ duration: 200 }} out:fade={{ duration: 200, delay: 200 }}>
    <div class="modal" bind:this={modal} in:fly={{ y: 100, duration: 300, delay: 200, easing: backOut }} out:fly={{ y: 1000, duration: 300 }}>
        <div class={"top " + categories[card.Type].color}>
            {#if !editing}
                <button on:click|stopPropagation={() => dispatch("back")} title="terug">
                    <span class="material-icons">arrow_back</span>
                </button>
            {/if}

            {#if $token}
                <div class="right">
                    <div class="deleting" class:open={deleting}>
                        <button title="toepassen" on:click={deleteCard}><span class="material-icons">done</span></button>
                        {#if deleting}
                            <button title="annuleren" on:click={() => deleting = false}><span class="material-icons">close</span></button>
                        {:else}
                            <button on:click|stopPropagation={() => deleting = true} title="verwijderen">
                                <span class="material-icons">delete</span>
                            </button>
                        {/if}
                    </div>
                    <button on:click|stopPropagation={() => { editing = !editing; if (!editing) saveCard() }} title={editing ? "opslaan" : "bewerken"}>
                        <span class="material-icons">{editing ? "save" : "edit"}</span>
                    </button>
                </div>
            {/if}
            <h1 title={editing ? "titel" : undefined}>
                <span class="material-icons">{categories[card.Type].icon}</span>
                {#if editing}
                    <input type="text" bind:value={card.Title} placeholder="titel" />
                {:else}
                    {card.Title || "(geen titel)"}
                {/if}
            </h1>
        </div>
        <div class="bottom">
            {#if editing}
                <div class="options" transition:slide>
                    <span class="material-icons" title="categorie">category</span>
                    <div class="types">
                        {#each categories as c, i}
                            <label title="categorie: {c.name}"><input type="radio" bind:group={card.Type} value={i}/><span class="material-icons">{c.icon}</span></label>
                        {/each}
                    </div>
                    <label for="description" title="beschrijving"><span class="material-icons">short_text</span></label><input type="text" id="description" bind:value={card.Description} title="beschrijving" placeholder="beschrijving" />
                </div>
                <Toolbar {editor} let:active let:commands let:focus let:selection>
                    <div class="toolbar" transition:slide>
                        <div class="buttons">
                            <div>
                                <button title="undo" on:click={commands.undo} disabled={selection, !editor.modules?.history?.hasUndo()}><span class="material-icons">undo</span></button>
                                <button title="redo" on:click={commands.redo} disabled={selection, !editor.modules?.history?.hasRedo()}><span class="material-icons">redo</span></button>
                            </div>
                            <div>
                                <button title="vet" class:active={active.bold} on:click={commands.bold} disabled={!focus}><span class="material-icons">format_bold</span></button>
                                <button title="schuin" class:active={active.italic} on:click={commands.italic} disabled={!focus}><span class="material-icons">format_italic</span></button>
                                <button title="onderstreept" class:active={active.underline} on:click={commands.underline} disabled={!focus}><span class="material-icons">format_underline</span></button>
                            </div>
                            <div>
                                <button title="titel" class:active={active.header === 1} on:click={commands.header1} disabled={!focus}><span class="material-icons">title</span></button>
                                <button title="ongeordende lijst" class:active={active.list === "bullet"} on:click={commands.bulletList} disabled={!focus}><span class="material-icons">format_list_bulleted</span></button>
                                <button title="geordende lijst" class:active={active.list === "ordered"} on:click={commands.orderedList} disabled={!focus}><span class="material-icons">format_list_numbered</span></button>
                                <button title="citaat" class:active={active.blockquote} on:click={commands.blockquote} disabled={!focus}><span class="material-icons">format_quote</span></button>
                            </div>
                            <div>
                                <button title="link" on:click={() => insert("link")} disabled={!focus}><span class="material-icons">link</span></button>
                                <button title="afbeelding" on:click={() => insert("image")} disabled={!focus}><span class="material-icons">image</span></button>
                                <button title="video" on:click={() => insert("video")} disabled={!focus}><span class="material-icons">movie</span></button>
                            </div>
                            <div>
                                <button title="opmaak wissen" on:click={clearFormat} disabled={!focus}><span class="material-icons">format_clear</span></button>
                            </div>
                        </div>
                        {#if currentInsert}
                            <form on:submit={applyInsert} transition:slide>
                                {#if currentInsert === "link"}
                                    <label title="link"><span class="material-icons">link</span><input type="url" bind:value={linkUrl} placeholder="link" use:focusInsert required/></label>
                                    <label title="tekst"><span class="material-icons">text_fields</span><input type="text" bind:value={linkText} placeholder="tekst" required/></label>
                                {:else if currentInsert === "image"}
                                    <label title="selecteer een afbeelding"><span class="material-icons">file_upload</span><input type="file" accept=".apng,.avif,.gif,.jpg,.jpeg,.jfif,.pjpeg,.pjp,.png,.svg,.webp" on:change={uploadImage} required /></label>
                                {:else if currentInsert === "video"}
                                    <label title="link"><span class="material-icons">link</span><input type="url" bind:value={videoUrl} placeholder="link" on:input={checkVideo} use:focusInsert required/></label>
                                {/if}
                                <div>
                                    <button type="submit" title="toepassen"><span class="material-icons">done</span></button>
                                    <button type="button" title="annuleren" on:click={cancelInsert}><span class="material-icons">close</span></button>
                                </div>
                            </form>
                        {/if}
                    </div>
                </Toolbar>
            {/if}
            <div class="rich-text" use:asRoot={editor} on:select={() => lastSelection = normalizeRange(editor.doc.selection) ?? lastSelection} on:paste={insertImagesFromDataTransfer} on:drop|preventDefault={insertImagesFromDataTransfer} />
        </div>
    </div>
</div>

<style>
    .grey {
        position: fixed;
        width: 100%;
        height: 100%;
        z-index: 999;
        top: 0;
        background: rgba(0, 0, 0, .95);
        transition: 0.5s;
        display: flex;
        align-items: center;
        justify-content: center;
        overflow-y: scroll;
        overflow-y: overlay;
    }

    :global(.dark-mode) .modal {
        background: black;
    }

    .modal {
        position: absolute;
        display: flex;
        flex-direction: column;
        top: 100px;
        width: 100%;
        min-height: calc(100% - 100px);
        font-family: sans-serif;
        border-radius: 8px 8px 0 0;
        box-shadow: 0 200px 0 0 white;
        transition: .2s cubic-bezier(0.1, 1.75, 0.5, 1);
    }

    :global(.dark-mode) .modal {
        box-shadow: 0 200px 0 0 #222;
    }

    @media (min-width: 700px) {
        .modal {
            width: 75%;
        }
    }
    
    @media (min-width: 1200px) {
        .modal {
            width: 50%;
        }
    }

    .top {
        background: #e7334c;
        height: 150px;
        display: flex;
        align-items: flex-end;
        border-radius: 8px 8px 0 0;
        transition: background-color .5s;
    }

    .top button {
        background: none;
        border: none;
        color: white;
        padding: 12px;
        cursor: pointer;
    }

    .top > button, .top .right {
        position: absolute;
        top: 0;
    }

    .top .right {
        right: 0;
    }

    .deleting {
        display: inline-flex;
        justify-content: flex-end;
        border-radius: 48px;
        padding: 6px;
        gap: 16px;
        overflow: hidden;
        width: 24px;
        transition: width .2s, background .3s;
    }

    .deleting.open {
        background: rgba(0, 0, 0, .25);
        width: 64px;
        transition: width .2s, background .1s;
    }

    .deleting button {
        padding: 0 !important;
        display: flex;
        align-items: center;
    }

    .top h1 {
        display: flex;
        align-items: center;
        width: 100%;
        margin: 16px;
        gap: 5px;
        font-size: 24px;
        color: white;
    }

    .top h1 input {
        flex-grow: 1;
        padding: 0;
        font-size: 24px;
        background: none;
        border: none;
        color: white;
        font-weight: bold;
        width: 0;
    }

    .bottom {
        position: relative;
        background: white;
        flex-grow: 1;
    }

    :global(.dark-mode) .bottom {
        background: #222;
    }

    .options {
        display: grid;
        grid-template-columns: min-content auto;
        align-items: center;
        gap: 4px;
        margin: 1em 1em 4px 1em;
        text-align: right;
    }

    .options .types {
        display: inline-flex;
        background: #ddd;
        padding: 4px;
        width: min-content;
        border-radius: 4px;
    }

    :global(.dark-mode) .options .types {
        background: #333;
    }

    .options .types input[type="radio"] {
        display: none;
    }

    .options .types input[type="radio"]:checked + span {
        color: #e7334c;
    }

    .toolbar {
        position: sticky;
        top: 0;
        padding: 4px 16px;
        gap: 4px;
        background: white;
    }

    :global(.dark-mode) .toolbar {
        background: #222;
    }

    .buttons {
        display: flex;
        flex-wrap: wrap;
        gap: 4px;
    }

    .buttons > div {
        margin: 0 2px;
    }

    .toolbar .buttons > :global(.vr:after) {
        content: "";
        position: absolute;
        display: inline-block;
        width: 2px;
        height: 1.5em;
        margin-top: 0.25em;
        margin-left: 3px;
        background: #ddd;
        border-radius: 1px;
    }

    :global(.dark-mode) .toolbar .buttons > :global(.vr:after) {
        background: #333;
    }

    .toolbar button {
        border: none;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        height: 30px;
        width: 30px;
        background: none;
        border-radius: 4px;
        cursor: pointer;
        transition: background .2s;
    }

    .toolbar button:disabled {
        opacity: .5;
        cursor: default;
    }

    :global(.dark-mode) .toolbar button {
        color: white;
    }

    .toolbar button:not(:disabled):hover {
        background: #eee;
    }

    :global(.dark-mode) .toolbar button:not(:disabled):hover {
        background: #333;
    }

    .toolbar button.active {
        background: #ccc;
    }

    :global(.dark-mode) .toolbar button.active {
        background: #444;
    }

    .toolbar form {
        display: flex;
        flex-wrap: wrap;
        gap: 4px;
    }

    .toolbar label {
        display: flex;
        align-items: center;
    }

    .rich-text {
        float: left;
        box-sizing: border-box;
        width: 100%;
        padding: 0 1em;
        margin: 4px 0;
        border-radius: 2px;
        box-shadow: 0 0 0 0 rgba(3, 169, 244, .5);
        transition: box-shadow .2s;
        background: inherit;
        user-select: text;
    }

    .rich-text:focus {
        outline: 0 solid transparent;
        box-shadow: 0 0 0 4px rgba(3, 169, 244, .5);
    }

    .rich-text :global(.placeholder) {
        position: relative;
        cursor: text;
    }

    .rich-text :global(.placeholder)::before {
        content: attr(data-placeholder);
        position: absolute;
        left: 0;
        right: 0;
        opacity: 0.5;
    }

    .bottom :global(blockquote) {
        border-left: 4px solid #ccc;
        padding-left: 0.5em;
    }

    :global(.dark-mode) .bottom :global(blockquote) {
        border-left-color: #444;
    }

    .bottom :global(img), .bottom :global(iframe) {
        display: block;
        margin: 0 auto;
        max-width: 100%;
        width: fit-content;
    }

    .bottom :global(.iframe-container) {
        position: relative;
        padding-bottom: 56.25%;
        height: 0;
        overflow: hidden;
        max-width: 100%;
    }
    
    .rich-text :global(.iframe-container iframe) {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: black;
    }

    .rich-text.focus :global(.iframe-container iframe) {
        pointer-events: none;
    }

    .rich-text :global(.highlight) {
        background: #ffff0080;
    }

    .rich-text :global(.highlight.embed) {
        outline: 1px solid #ffff0080;
        pointer-events: none;
    }
</style>
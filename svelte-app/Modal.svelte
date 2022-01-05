<script>
    import { createEventDispatcher, onDestroy } from "svelte";
    import { fly, fade, slide, crossfade } from "svelte/transition";
    import { backOut } from "svelte/easing";
    import { Editor, Delta, normalizeRange, Source } from "typewriter-editor";
    import { paragraph, header, list, blockquote } from "typewriter-editor/lib/typesetting/lines";
    import { link, bold, italic } from "typewriter-editor/lib/typesetting/formats";
    import { br } from "typewriter-editor/lib/typesetting/embeds";
    import { line } from "typewriter-editor/lib/typesetting";
    import { h } from "typewriter-editor/lib/rendering/vdom";
    import { placeholder, smartEntry, smartQuotes } from "typewriter-editor/lib/modules";
    import asRoot from "typewriter-editor/lib/asRoot";
    import Toolbar from "typewriter-editor/lib/Toolbar.svelte";
    import categories from "./categories.js";
    
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

    const editor = window.editor = new Editor({
        html: card.Content,
        types: {
            lines: [paragraph, header, list, blockquote, image, video],
            formats: [link, bold, italic],
            embeds: [br]
        }
    });

    let smartEntryDestroy = smartEntry()(editor).destroy;
    let smartQuotesDestroy = smartQuotes(editor).destroy;
    placeholder("type hier...")(editor);
    let editing = false;
    editor.on("root", () => {
        let l = editor.doc.length-1;
        editor.select(l, Source.api);
        lastSelection = [l, l];
        editor.root.addEventListener("select", () => {
            if (editor.doc.selection !== null)
                lastSelection = normalizeRange(editor.doc.selection)
        });
        editor.root.addEventListener("paste", e => insertImagesFromDataTransfer(e));
        editor.root.addEventListener("drop", e => { e.preventDefault(); insertImagesFromDataTransfer(e) });
    });

    let currentInsert = null;

    let linkUrl = "";
    let linkText = "";

    let videoUrl = "";
    const youtubeRegex = /^(?:https?:\/\/)?(?:www\.)?(?:(?:youtube\.\w{2,3}\/(?:watch.*(?:\?|&)v=|embed\/))|(?:youtu.be))([^&#]+)/;
    const vimeoRegex = /^(?:https?:\/\/)?(?:www\.)?(?:(?:vimeo\.com)|(?:player\.vimeo\.com\/video))\/([^?#]+)/;
    const dailymotionRegex = /^(?:https?:\/\/)?(?:www\.)?dailymotion\.com\/(?:embed\/)?video\/([^?#]+)/;


    function insert(what) {
        if (what === "link") {
            linkUrl = "";
            linkText = editor.doc.getText(lastSelection);
        } else if (what === "video") {
            videoUrl = "";
        }
        currentInsert = what;
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
        editor.select(lastSelection, Source.api)
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

    const [send, receive] = crossfade({
        duration: d => Math.sqrt(d * 1000)
    });

    document.body.style.overscrollBehaviorY = "none";

    onDestroy(() => {
        document.body.style.overscrollBehaviorY = "";

        smartEntryDestroy();
        smartQuotesDestroy();
        editor.destroy();
    });

    let modal;
    let grey;
    let innerHeight;

    let startY;
    let lastY;
    let dragging = false;
    let firstMove = true;
    let mouseDown = false;

    function touchStart(e) {
        startY = lastY = e.touches?.[0].clientY ?? e.clientY;
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
        } else if (Math.abs(lastY - startY) < 25 && e.target === grey && !editing) {
            dispatch("back");
            e.preventDefault();
        } else {
            grey.style.background = modal.style.transform = "";
        }
        dragging = mouseDown = false;
        firstMove = true;
    }

    function saveCard() {
        card.Content = editor.getHTML();
        fetch("/saveCard", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(card)
        });
    }
</script>

<svelte:window bind:innerHeight on:touchstart={touchStart} on:mousedown={touchStart} on:touchmove={touchMove} on:mousemove={touchMove} on:touchend={touchEnd} on:mouseup={touchEnd}/>

<div class="grey" bind:this={grey} on:click={(e) => { if (!editing && e.target.matches(".grey")) dispatch("back") }} in:fade={{ duration: 200 }} out:fade={{ duration: 200, delay: 200 }}>
    <div class="modal" bind:this={modal} in:fly={{ y: 100, duration: 300, delay: 200, easing: backOut }} out:fly={{ y: 1000, duration: 300 }}>
        <div class={"top " + categories[card.Type].color}>
            {#if !editing}
                <button on:click|stopPropagation={() => dispatch("back")} title="terug">
                    <span class="material-icons">arrow_back</span>
                </button>
            {/if}

            <button on:click|stopPropagation={() => { editing = !editing; if (!editing) saveCard() }} style="right:0" title={editing ? "opslaan" : "bewerken"}>
                <span class="material-icons">{editing ? "save" : "edit"}</span>
            </button>
            <h1 title={editing ? "titel" : undefined}>
                <span class="material-icons">{categories[card.Type].icon}</span>
                {#if editing}
                    <input type="text" bind:value={card.Title} />
                {:else}
                    {card.Title}
                {/if}
            </h1>
        </div>
        <div class="bottom">
            {#if editing}
                <div class="options" transition:fade>
                    <span class="material-icons" title="categorie">category</span>
                    <div class="types">
                        {#each categories as c, i}
                            <label title="categorie: {c.name}"><input type="radio" bind:group={card.Type} value={i}/><span class="material-icons">{c.icon}</span></label>
                        {/each}
                    </div>
                    <label for="description" title="beschrijving"><span class="material-icons">short_text</span></label><input type="text" id="description" bind:value={card.Description} title="beschrijving" placeholder="beschrijving" />
                </div>
                    <Toolbar {editor} let:active let:commands let:focus let:selection>
                        <div class="toolbar" transition:fade>
                            <div class="buttons">
                                <div>
                                    <button title="undo" on:click={commands.undo} disabled={selection, !editor.modules?.history?.hasUndo()}><span class="material-icons">undo</span></button>
                                    <button title="redo" on:click={commands.redo} disabled={selection, !editor.modules?.history?.hasRedo()}><span class="material-icons">redo</span></button>
                                </div>
                                <span class="vr"></span>
                                <div>
                                    <button title="vet" class:active={active.bold} on:click={commands.bold} disabled={!focus}><span class="material-icons">format_bold</span></button>
                                    <button title="schuin" class:active={active.italic} on:click={commands.italic} disabled={!focus}><span class="material-icons">format_italic</span></button>
                                </div>
                                <span class="vr"></span>
                                <div>
                                    <button title="titel" class:active={active.header === 1} on:click={commands.header1} disabled={!focus}><span class="material-icons">title</span></button>
                                    <button title="ongeordende lijst" class:active={active.list === "bullet"} on:click={commands.bulletList} disabled={!focus}><span class="material-icons">format_list_bulleted</span></button>
                                    <button title="geordende lijst" class:active={active.list === "ordered"} on:click={commands.orderedList} disabled={!focus}><span class="material-icons">format_list_numbered</span></button>
                                    <button title="citaat" class:active={active.blockquote} on:click={commands.blockquote} disabled={!focus}><span class="material-icons">format_quote</span></button>
                                </div>
                                <span class="vr"></span>
                                <div>
                                    <button title="link" on:click={() => insert("link")} disabled={!focus}><span class="material-icons">link</span></button>
                                    <button title="afbeelding" on:click={() => insert("image")} disabled={!focus}><span class="material-icons">image</span></button>
                                    <button title="video" on:click={() => insert("video")} disabled={!focus}><span class="material-icons">movie</span></button>
                                </div>
                                <span class="vr"></span>
                                <div>
                                    <button title="opmaak wissen" on:click={clearFormat} disabled={!focus}><span class="material-icons">format_clear</span></button>
                                </div>
                            </div>
                            {#if currentInsert}
                                <form on:submit={applyInsert} transition:slide>
                                    {#if currentInsert === "link"}
                                        <label title="link"><span class="material-icons">link</span><input type="url" bind:value={linkUrl} placeholder="link" use:focusInsert required select/></label>
                                        <label title="tekst"><span class="material-icons">text_fields</span><input type="text" bind:value={linkText} placeholder="tekst" required/></label>
                                    {:else if currentInsert === "image"}
                                        <label title="selecteer een afbeelding"><input type="file" accept=".apng,.avif,.gif,.jpg,.jpeg,.jfif,.pjpeg,.pjp,.png,.svg,.webp" on:change={uploadImage} /></label>
                                    {:else if currentInsert === "video"}
                                        <label title="link"><span class="material-icons">link</span><input type="url" bind:value={videoUrl} placeholder="link" on:input={checkVideo} use:focusInsert required select/></label>
                                    {/if}
                                    <button type="submit" title="toepassen"><span class="material-icons">done</span></button>
                                    <button type="button" title="annuleren" on:click={cancelInsert}><span class="material-icons">close</span></button>
                                </form>
                            {/if}
                        </div>
                    </Toolbar>
                
                <div class="rich-text" use:asRoot={editor}  in:receive={{key: "content"}} out:send={{key: "content"}}/>
            {:else}
                <div class="content" in:receive={{key: "content"}} out:send={{key: "content"}}>{@html card.Content}</div>
            {/if}
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
        position: absolute;
        top: 0;
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

    .content {
        position: absolute;
        top: 0;
        left: 0;
        user-select: text;
        box-sizing: border-box;
        padding: 0 16px;
        float: left;
        width: 100%;
        background: inherit;
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

    .options .types label {
        display: flex;
        align-items: center;
        gap: 4px;
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

    .toolbar .buttons {
        display: flex;
        flex-wrap: wrap;
        gap: 4px;
    }

    .toolbar .buttons .vr {
        width: 2px;
        border-radius: 1px;
        background: #ddd;
    }

    :global(.dark-mode) .toolbar .buttons .vr {
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
        border-radius: 5px;
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
    
    .bottom :global(.iframe-container iframe) {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        pointer-events: none;
    }

    .bottom :global(hr) {
        margin: 1em 0;
    }

    .bottom :global(hr.selected), .bottom :global(.iframe-container.selected), .bottom :global(p.selected img) {
        outline: 4px solid rgba(3, 169, 244, .5);
    }
</style>
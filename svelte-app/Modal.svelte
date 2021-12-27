<script>
    import { createEventDispatcher, onDestroy } from "svelte";
    import { fly, fade } from "svelte/transition";
    import { backOut } from "svelte/easing";
    import { Editor } from "typewriter-editor";
    import asRoot from "typewriter-editor/lib/asRoot";
    import Toolbar from "typewriter-editor/lib/Toolbar.svelte";
    import categories from "./categories.js";
    
    export let card = {};
    
    const dispatch = createEventDispatcher();
    
    const editor = new Editor();
    editor.setHTML(card.Content);
    let editing = false;

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

    document.body.style.overscrollBehaviorY = "none";
    onDestroy(() => {
        document.body.style.overscrollBehaviorY = "";
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
                grey.style.setProperty("--x", 1 - offset / (innerHeight/4));
            }
        }
    }

    function touchEnd(e) {
        modal.style.transitionDuration = grey.style.transitionDuration = "";
        if (dragging && lastY - startY > innerHeight/4) {
            dispatch("back");
        } else if (Math.abs(lastY - startY) < 25 && e.target === grey) {
            dispatch("back");
            e.preventDefault();
        } else {
            grey.style.setProperty("--x", "");
            modal.style.transform = "";
        }
        dragging = mouseDown = false;
        firstMove = true;
    }
</script>

<svelte:window bind:innerHeight on:touchstart={touchStart} on:mousedown={touchStart} on:touchmove={touchMove} on:mousemove={touchMove} on:touchend={touchEnd} on:mouseup={touchEnd}/>

<div class="grey" bind:this={grey} on:click={(e) => { if (!editing && e.target.matches(".grey")) dispatch("back") }} in:fade={{ duration: 200 }} out:fade={{ duration: 200, delay: 200 }}>
    <div class="modal" bind:this={modal} on:click={() => dispatch("click")} in:fly={{ y: 100, duration: 300, delay: 200, easing: backOut }} out:fly={{ y: 1000, duration: 300 }} tabindex="0">
        <div class={"top " + categories[card.Type].color}>
            {#if !editing}
                <button on:click|stopPropagation={() => dispatch("back")}>
                    <span class="material-icons">arrow_back</span>
                </button>
            {/if}

            <button on:click|stopPropagation={() => { editing = !editing; if (!editing) saveCard() }} style="right:0">
                <span class="material-icons">{editing ? "done" : "edit"}</span>
            </button>
            <h1>
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
                Beschrijving: <input type="text" bind:value={card.Description}>
                <Toolbar {editor} let:active let:commands>
                    <div class="toolbar">
                        <button title="titel" class:active={active.header === 1} on:click={commands.header1}><span class="material-icons">title</span></button>
                        <button title="vet" class:active={active.bold} on:click={commands.bold}><span class="material-icons">format_bold</span></button>
                        <button title="schuin" class:active={active.italic} on:click={commands.italic}><span class="material-icons">format_italic</span></button>
                        <button title="citaat" class:active={active.blockquote} on:click={commands.blockquote}><span class="material-icons">format_quote</span></button>
                    </div>
                </Toolbar>
                
                <div class="rich-text" use:asRoot={editor} />
            {:else}
                {@html card.Content}
            {/if}
        </div>
    </div>
</div>

<style>
    .grey {
        --x: 1;
        position: fixed;
        width: 100%;
        height: 100%;
        z-index: 999;
        top: 0;
        background-color: rgba(0, 0, 0, calc(var(--x) * .95));
        transition: 0.5s;
        display: flex;
        align-items: center;
        justify-content: center;
        overflow-y: overlay;
    }

    @supports ((-webkit-backdrop-filter: none) or (backdrop-filter: none)) {
        .grey {
            background-color: rgba(0, 0, 0, calc(var(--x) * .8));
            backdrop-filter: blur(calc(var(--x) * 10px));
            -webkit-backdrop-filter: blur(calc(var(--x) * 10px));
        }
    }

    :global(.dark-mode) .modal {
        background: black;
    }

    .modal {
        box-shadow: 2px 2px 5px rgba(0, 0, 0, .1);
        font-family: sans-serif;
        border-radius: 8px 8px 0 0;
        position: fixed;
        width: 100%;
        min-height: calc(100%);
        top: 100px;
        display: flex;
        flex-direction: column;
        transition: .2s cubic-bezier(0.1, 1.75, 0.5, 1);
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
        margin: 16px;
        font-size: 24px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
    }

    .top h1 input {
        font-size: 24px;
        padding: 0;
        background: none;
        border: none;
        color: white;
        font-weight: bold;
    }

    .bottom {
        background: white;
        padding: 16px;
        user-select: text;
        flex-grow: 1;
    }

    .toolbar {
        position: sticky;
        top: 0;
        padding: 0 16px;
        background: white;
        width: 100%;
        transform: translateX(-16px);
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

    .toolbar button:hover {
        background: #eee;
    }

    .toolbar button.active {
        background: #ccc;
    }

    :global(.bottom) .underline {
        text-decoration: underline;
    }
    
    :global(.bottom) :global(p) {
        margin-top: 0;
        margin-bottom: 0;
    }
</style>
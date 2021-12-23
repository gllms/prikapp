<script>
    import { createEventDispatcher } from "svelte";
    import { fly } from "svelte/transition";
    import { Editor } from "typewriter-editor";
    import asRoot from "typewriter-editor/lib/asRoot";
    import Toolbar from "typewriter-editor/lib/Toolbar.svelte";
    
    export let card = {};
    export let modal = false;
    export let editing = false;
    export let index = 0;
    
    const dispatch = createEventDispatcher();
    let open = true;
    
    const editor = new Editor();
    editor.setHTML(card.Content);

    let categories = [
        {
            "name": "video",
            "icon": "movie",
            "color": "darkblue"
        },
        {
            "name": "text",
            "icon": "article",
            "color": "lightblue"
        },
        {
            "name": "image",
            "icon": "image",
            "color": "red"
        }
    ];

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

{#if open}
    <div class="card" class:modal on:click={() => dispatch("click")} in:fly={{ y: 100, duration: 300, delay: 200 + 100*index }} out:fly={{ y: 1000, duration: 300 }} tabindex="0">
        <div class={"top " + categories[card.Type].color}>
            {#if modal}
                {#if !editing}
                    <button on:click|stopPropagation={() => dispatch("back")}>
                        <span class="material-icons">arrow_back</span>
                    </button>
                {/if}

                <button on:click|stopPropagation={() => { editing = !editing; if (!editing) saveCard() }} style="right:0">
                    <span class="material-icons">{editing ? "done" : "edit"}</span>
                </button>
            {/if}
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
        {#if modal}
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
        {:else}
            {card.Description}
        {/if}
        </div>
    </div>
{/if}

<style>
    :global(.dark-mode) .card {
        background: black;
    }

    .card {
        border-radius: 8px;
        background: white;
        box-shadow: 2px 2px 5px rgba(0, 0, 0, .1);
        font-family: sans-serif;
        cursor: pointer;
        transition: .2s;
    }

    .card.modal {
        position: fixed;
        cursor: default;
        width: 100%;
        min-height: calc(100% - 100px);
        top: 100px;
        border-radius: 8px 8px 0 0;
    }

    @media (min-width: 700px) {
        .card.modal {
            width: 75%;
        }
    }
    
    @media (min-width: 1200px) {
        .card.modal {
            width: 50%;
        }
    }

    .card:not(.modal):hover, .card:not(.modal):focus {
        box-shadow: 8px 8px 10px rgba(0, 0, 0, .05);
        transform: scale(1.01);
        z-index: 0;
    }

    .top {
        background: #e7334c;
        height: 100px;
        display: flex;
        align-items: flex-end;
        border-radius: 8px 8px 0 0;
    }

    .card.modal .top {
        border-radius: 8px 8px 0 0;
        height: 150px;
    }

    .darkblue {
        background: #142d49;
    }

    .lightblue {
        background: #79b9d5;
    }

    .red {
        background: #e7334c;
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
        margin: 8px;
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

    .card.modal h1 {
        margin: 16px;
    }

    .bottom {
        margin: 16px;
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
    
    :global(.bottom) p {
        margin-top: 0;
        margin-bottom: 0;
    }
</style>
<script>
    import { createEventDispatcher } from "svelte";
    import { fly } from "svelte/transition";
    
    export let card = {};
    export let modal = false;

    const dispatch = createEventDispatcher();

    let categories = {
        "video": {
            "icon": "movie",
            "color": "darkblue"
        },
        "text": {
            "icon": "article",
            "color": "lightblue"
        }
    };
</script>

<div class="card{modal ? ' modal' : ''}" on:click={() => dispatch("click")} transition:fly={{ y: 1000, duration: 300, delay: 200, opacity: 1 }} tabindex="0">
    <div class={"top " + categories[card.Type].color}>
        {#if modal}
            <button on:click|stopPropagation={() => dispatch("back") }>
                <span class="material-icons">arrow_back</span>
            </button>
        {/if}
        <h1><span class="material-icons">{categories[card.Type].icon}</span>{card.Title}</h1>
    </div>
    <div class="bottom">
    {#if modal}
        {card.Content}
    {:else}
        {card.Description}
    {/if}
    </div>
</div>

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

    h1 {
        margin: 8px;
        font-size: 24px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
    }

    .card.modal h1 {
        margin: 16px;
    }

    .bottom {
        margin: 16px;
    }
</style>
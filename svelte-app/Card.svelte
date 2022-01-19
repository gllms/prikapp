<script>
    import { createEventDispatcher } from "svelte";
    import { fly } from "svelte/transition";
    import categories from "./categories.js";
    
    export let card = {};
    export let index = 0;
    
    const dispatch = createEventDispatcher();
</script>

<div class="card" on:click={() => dispatch("click")} in:fly={{ y: 100, duration: 300, delay: 100*index }} tabindex="0">
    <div class={"top " + categories[card.Type].color}>
        <h1>
            <span class="material-icons">{categories[card.Type].icon}</span>
            {card.Title || "(geen titel)"}
        </h1>
    </div>
    <div class="bottom">
        {card.Description || "(geen beschrijving)"}
    </div>
</div>

<style>
    :global(.dark-mode) .card {
        background: #222;
    }

    .card {
        border-radius: 8px;
        background: white;
        box-shadow: 2px 2px 5px rgba(0, 0, 0, .1);
        font-family: sans-serif;
        cursor: pointer;
        transition: .2s;
        height: 100%;
    }

    .card:hover, .card:focus {
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

    .top h1 {
        margin: 8px;
        font-size: 1.5em;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
    }

    .bottom {
        margin: 16px;
    }
</style>
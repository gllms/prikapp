<script>
    import Card from "./Card.svelte";
    import { fade } from "svelte/transition";
    import { overlayCount } from "./stores.js";

    let cards = [
        {
            Type: "video",
            Title: "De weg van het bloed",
            Description: "In deze video wordt uitgelegd wat er allemaal met jouw bloed gebeurt",
            Content: "In deze video wordt uitgelegd wat er allemaal met jouw bloed gebeurt"
        },
        {
            Type: "text",
            Title: "In de wachtkamer",
            Description: "Uitleg over wat je moet doen in de wachtkamer",
            Content: "In de wachtkamer moet je wachten."
        }
    ];

    let currentCard = null;
    $: document.body.style.overflow = $overlayCount ? "hidden" : "overlay";
</script>

<div class="cards">
    {#each cards as card}
        <Card card={card} on:click={() => { currentCard = card; $overlayCount++ }}/>
    {/each}
</div>

{#if currentCard}
    <div class="grey" on:click={(e) => { if (e.target.matches(".grey")) { currentCard = null; $overlayCount-- } }} transition:fade={{ duration: 200 }}>
        <Card card={currentCard} modal={true} on:back={() => { currentCard = null; $overlayCount-- }}/>
    </div>
{/if}

<style>
    .cards {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
        gap: 20px;
        padding: 20px;
        max-width: 800px;
        margin: 0 auto;
    }

    .grey {
        position: fixed;
        width: 100%;
        height: 100%;
        z-index: 999;
        top: 0;
        background-color: rgba(0, 0, 0, .95);
        backdrop-filter: blur(10px);
        transition: 0.5s;
        display: flex;
        align-items: center;
        justify-content: center;
        overflow-y: auto;
    }

    @supports ((-webkit-backdrop-filter: none) or (backdrop-filter: none)) {
        .grey {
            background-color: rgba(0, 0, 0, .8);
            backdrop-filter: blur(10px);
        }
    }
</style>
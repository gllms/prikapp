<script>
    import Card from "./Card.svelte";
    import Loading from "./Loading.svelte";
    import { fade } from "svelte/transition";
    import { overlayCount } from "./stores.js";

    let cards = [];
    let promise = fetch("./cards.json").then(response => response.json()).then(response => cards = response);

    let currentCard = null;
    let editing = false;

    $: document.body.style.overflow = $overlayCount ? "hidden" : "overlay";
    $: currentCard, cards = cards;
</script>

<div class="cards">
    {#await promise}
        <Loading />
    {:then}
        {#each cards as card, index (card.Id)}
            <Card card={card} {index} on:click={() => { currentCard = card; $overlayCount++ }}/>
        {/each}
    {:catch error}
        <p>Laden van kaarten mislukt</p>
    {/await}
</div>

{#if currentCard}
    <div class="grey" on:click={(e) => { if (!editing && e.target.matches(".grey")) { currentCard = null; $overlayCount-- } }} in:fade={{ duration: 200 }} out:fade={{ duration: 200, delay: 200 }}>
        <Card card={currentCard} modal on:back={() => { currentCard = null; $overlayCount-- }} bind:editing />
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
            -webkit-backdrop-filter: blur(10px);
        }
    }
</style>
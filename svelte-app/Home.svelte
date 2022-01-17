<script>
    import Card from "./Card.svelte";
    import Modal from "./Modal.svelte";
    import Loading from "./Loading.svelte";
    import { fade } from "svelte/transition";
    import { overlayCount } from "./stores.js";
    import { onDestroy } from "svelte";
    import { cards } from "./stores.js";

    let promise = fetch("./cards.json").then(response => response.json()).then(response => $cards = response);

    let currentCard = null;
    $: currentCard, $cards = $cards;
    $: $cards, checkCurrentCard();
    function checkCurrentCard() {
        if (!$cards.includes(currentCard))
            currentCard = null;
    }

    onDestroy(() => currentCard && $overlayCount--);
</script>

<div class="cards">
    {#await promise}
        <Loading />
    {:then}
        {#each $cards as card, index (card.Id)}
            <Card card={card} {index} on:click={() => { currentCard = card; $overlayCount++ }}/>
        {/each}
    {:catch}
        <p>Laden van kaarten mislukt</p>
    {/await}
</div>

{#if currentCard}
    <Modal card={currentCard} on:back|once={() => { currentCard = null; $overlayCount-- }} />
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
</style>
<script>
    import Card from "./Card.svelte";

    export let cards = [];
    let currentCard = null;
    $: console.log(currentCard);
</script>

<div class="cards">
    {#each cards as card}
        <Card card={card} on:click={() => currentCard = card} />
    {/each}
</div>



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
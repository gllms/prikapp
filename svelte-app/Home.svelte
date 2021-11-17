<script>
    import Card from "./Card.svelte";

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
<script>
    import Menu from "./Menu.svelte";
    import Home from "./Home.svelte";

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
</script>

<Menu />

<Home cards={cards}/>

<style>
    :global(body) {
        background: #eee;
        margin-top: 48px;
    }
</style>

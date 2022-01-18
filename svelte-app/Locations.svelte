<script>
    import MediaQuery from "./MediaQuery.svelte";
    import Loading from "./Loading.svelte";
    import { slide } from "svelte/transition";
    import { cubicOut } from "svelte/easing";

    let name = "";
    let range = 15;
    let currentPostcode = "";
    $: currentPostcode = /\d{4}/.test(name) ? /\d{4}/.exec(name)[0] : "";

    let locations = [];
    fetch("./build/locations.json").then(response => response.json()).then(response => locations = response);
    $: if (coords[currentPostcode]) {
        locations.sort((a, b) => distance(coords[currentPostcode], coords[a.Postalcode.substring(0, 4)]) - distance(coords[currentPostcode], coords[b.Postalcode.substring(0, 4)]));
        locations = locations;
    }
    $: filteredLocations = currentPostcode ? coords[currentPostcode] ? locations.filter(location => distance(coords[currentPostcode], coords[location.Postalcode.substring(0, 4)]) < range) : [] : locations;

    let coords = {};
    fetch("./postcodes.json").then(response => response.json()).then(response => coords = response);

    function distance(c1, c2) {
        const dLat = (c2.Lat - c1.Lat) * Math.PI/180;
        const dLon = (c2.Lon - c1.Lon) * Math.PI/180;
        const result = Math.sin(dLat/2) * Math.sin(dLat/2) + Math.cos(c1.Lat * Math.PI/180) * Math.cos(c2.Lat * Math.PI/180) * Math.sin(dLon/2) * Math.sin(dLon/2);
        return 12742 * Math.atan2(Math.sqrt(result), Math.sqrt(1-result));
    }

    let selected = -1;
</script>

<div class="cards">
    <div class="search">
        <label title="postcode"><span class="material-icons">mail</span><input type="text" bind:value={name} disabled={!coords || !locations} placeholder="0000 AB"/></label>
        <label title="zoekradius"><span class="material-icons">adjust</span><input type="range" bind:value={range} min="0" max="400" disabled={!currentPostcode} /><input type="text" pattern="\d+" bind:value={range} min="0" max="400" disabled={!currentPostcode} style="width:3em;text-align:right"/>km</label>
    </div>
    {#if locations.length}
        {#if filteredLocations.length}
            {#each filteredLocations as location, i}
                <div class="card" class:selected={i === selected} on:click={() => selected = selected === i ? -1 : i}>
                    <div class="top">
                        <h1><span class="material-icons">push_pin</span> {@html location.LocationName}</h1>
                    </div>
                    <div class="bottom">
                        <p>
                            <span class="material-icons">place</span>
                            <span>
                                {#if i === selected}
                                    <span in:slide={{ delay: 200, easing: cubicOut }} out:slide={{ easing: cubicOut }}>
                                        <span>{@html location.Address}</span>
                                        <span>{@html location.Postalcode}</span>
                                    </span>
                                {/if}
                                <span>{@html location.Place}</span>
                            </span>
                        </p>
                        {#if i === selected}
                            <div in:slide={{ delay: 200, easing: cubicOut }} out:slide={{ easing: cubicOut }}>
                                <p>
                                    <span class="material-icons">access_time</span><span>{@html location.OpeningHours}</span>
                                </p>
                                <p>
                                    <span class="material-icons">info</span><span>{@html location.Particularities}</span>
                                </p>
                            </div>
                        {/if}
                    </div>
                </div>
            {/each}
        {:else}
            <p>Geen locaties gevonden</p>
        {/if}
    {:else}
        <Loading />
    {/if}
</div>

<div class="grey" class:open={selected > -1} on:click={() => selected = -1}></div>

<style>
    .search {
        padding: 32px 0 0;
        grid-column: span 2;
    }

    .search label {
        display: inline-flex;
        margin: 8px;
    }

    .cards {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
        gap: 20px;
        padding: 20px;
        max-width: 800px;
        margin: 0 auto;
    }

    .card {
        border-radius: 8px;
        background: white;
        box-shadow: 2px 2px 5px rgba(0, 0, 0, .1);
        font-family: sans-serif;
        cursor: pointer;
        transition: .2s;
        height: fit-content;
        transition: all .2s .1s, margin-bottom .5s;
    }

    .card.selected {
        margin-bottom: -1000px;
        transform: scale(1.01) translateY(-50px) !important;
        z-index: 1000;
        transition: all .2s, margin-bottom .5s;
    }

    :global(.dark-mode) .card {
        background: #222;
    }

    @media (max-width: 659px) {
        .search {
            grid-column: span 1;
        }

        .card.selected {
            margin-bottom: -48.59px;
            margin-top: 50px;
        }
    }

    .card:hover, .card:focus {
        box-shadow: 8px 8px 10px rgba(0, 0, 0, .05);
        transform: scale(1.01);
    }

    .top {
        background: #142d49;
        height: 72px;
        display: flex;
        align-items: flex-end;
        border-radius: 8px 8px 0 0;
    }

    .top h1 {
        margin: 8px;
        font-size: 24px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
    }

    .bottom {
        padding: 8px;
        gap: 8px;
    }

    .bottom p {
        display: flex;
        align-items: center;
        gap: 8px;
        margin: 8px;
    }

    .bottom span {
        display: flex;
        flex-direction: column;
    }

    .bottom span .material-icons {
        margin-left: -32px;
        margin-right: 8px;
    }

    .bottom div {
        display: flex;
        flex-direction: column;
    }

    .grey {
        position: fixed;
        width: 100%;
        height: 100%;
        z-index: 9999;
        top: 0;
        background: rgba(0, 0, 0, .95);
        opacity: 0;
        transition: opacity 1s;
        pointer-events: none;
        z-index: 999;
    }

    .grey.open {
        opacity: 1;
        pointer-events: all;
    }
</style>
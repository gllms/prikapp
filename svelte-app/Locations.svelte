<script>
    import MediaQuery from "./MediaQuery.svelte";
    import Loading from "./Loading.svelte";

    let name = "";
    let range = 15;
    let currentPostcode = "";
    $: currentPostcode = /\d{4}/.test(name) ? /\d{4}/.exec(name)[0] : "";
    $: if (coords[currentPostcode]) {
        locations.sort((a, b) => distance(coords[currentPostcode], coords[a.Postalcode.substring(0, 4)]) - distance(coords[currentPostcode], coords[b.Postalcode.substring(0, 4)]));
        locations = locations;
    }

    let locations = [];
    fetch("./locations.json").then(response => response.json()).then(response => locations = response);

    let coords = {};
    fetch("./postcodes.json").then(response => response.json()).then(response => coords = response);

    function distance(c1, c2) {
        const dLat = (c2.Lat - c1.Lat) * Math.PI/180;
        const dLon = (c2.Lon - c1.Lon) * Math.PI/180;
        const result = Math.sin(dLat/2) * Math.sin(dLat/2) + Math.cos(c1.Lat * Math.PI/180) * Math.cos(c2.Lat * Math.PI/180) * Math.sin(dLon/2) * Math.sin(dLon/2);
        return 12742 * Math.atan2(Math.sqrt(result), Math.sqrt(1-result));
    }
</script>

<div class="search">
    <p>Zoek op postcode:</p>
    <label>postcode: <input type="text" bind:value={name} disabled={!coords || !locations} placeholder="0000 AB"/></label>
    <label>radius: <input type="number" bind:value={range} min="0"/></label>
</div>

<MediaQuery query="(min-width: 480px)" let:matches>
    {#if matches}
        {#if locations.length}
            <div class="location-table">
                <table>
                    <thead>
                        <tr>
                            <th>Plaats</th>
                            <th>Naam Locatie</th>
                            <th>Adres</th>
                            <th>Postcode</th>
                            <th>Openingstijden</th>
                            <th>Bijzonderheden</th>
                        </tr>
                    </thead>
                    <tbody>
                        {#each locations as location}
                            {#if currentPostcode == "" || coords[currentPostcode] && distance(coords[currentPostcode], coords[location.Postalcode.substring(0, 4)]) < range}
                            <tr>
                                <td>{@html location.Place}</td>
                                <td>{@html location.LocationName}</td>
                                <td>{location.Address}</td>
                                <td>{location.Postalcode}</td>
                                <td>{@html location.OpeningHours}</td>
                                <td>{location.Particularities}</td>
                            </tr>
                            {/if}
                        {/each}
                    </tbody>
                    
                </table>
            </div>
        {:else}
            <Loading />
        {/if}
    {:else}
        {#if locations.length}
            {#each locations as location}
                {#if currentPostcode == "" || coords[currentPostcode] && distance(coords[currentPostcode], coords[location.Postalcode.substring(0, 4)]) < range}
                    <div class="location-card">
                        <table class="lightblue">
                            <tr>
                                <th>Plaats</th>
                                <td>{@html location.Place}</td>
                            </tr>
                            <tr>
                                <th>Naam</th>
                                <td>{@html location.LocationName}</td>
                            </tr>
                            <tr>
                                <th>Adres</th>
                                <td>{location.Address}</td>
                            </tr>
                            <tr>
                                <th>Postcode</th>
                                <td>{location.Postalcode}</td>
                            </tr>
                            <tr>
                                <th>Openingstijden</th>
                                <td>{@html location.OpeningHours}</td>
                            </tr>
                            <tr>
                                <th>Bijzonderheden</th>
                                <td>{location.Particularities}</td>
                            </tr>
                        </table>
                    </div>
                {/if}
            {/each}
        {:else}
            <Loading />
        {/if}
    {/if}
</MediaQuery>



<style>
    @media only screen and (max-width: 480px) {
        .search {
            text-align: center;
            margin-bottom: 15px;
            background-color: #f6f6f6;
            width: max-content;
            padding: 15px;
            margin-left: auto;
            margin-right: auto;
        }

        .location-card {
            padding: 5px;
        }

        table {
            width: 100%;
            border-radius: 8px;
            margin: 0 auto 10px auto;
            border-collapse:collapse;
            color: black;
        }

        tr:first-child {
            background-color: #e7334c;
        }

        table, th, td {
            padding: 5px;
            border: 1px solid white;
        }

        th {
            width: 100px;
            text-align: right;
            font-weight: bolder;
        }

        td {
            text-align: left;
            font-weight: 50;
        }
    }

    @media only screen and (min-width: 480px) {
        .location-table {
            margin: auto;
            padding: 32px;
        }

        .search {
            text-align: center;
            margin-bottom: 15px;
            background-color: #f6f6f6;
            width: max-content;
            padding: 15px;
            margin-left: auto;
            margin-right: auto;
        }

        table {
            width: 100%;
            margin: 0 auto 10px auto;
            border-collapse:collapse;
            color: black;
        }

        table, th, td {
            padding: 5px;
        }

        thead tr {
            position: sticky;
            top: 48px;
        }

        thead tr th {
            text-align: center;
        }

        th {
            width: 100px;
            text-align: left;
            font-weight: bolder;
            background-color: #e7334c;
        }

        th:first-child {
            border-radius: 8px 0 0 0;
        }

        th:last-child {
            border-radius: 0 8px 0 0;
        }

        td {
            text-align: left;
            font-weight: 50;
        }

        tr:nth-child(2n) {
            background: #ddd;
        }
    }
</style>
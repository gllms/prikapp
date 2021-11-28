<script>
    let name = "";
    let locations = "Loading...";
    const Http = new XMLHttpRequest();
    const url = 'https://localhost:5001/locations.json';

    
    Http.open("GET", url);
    Http.send();

    Http.onreadystatechange = (e) => {
        console.log(Http.responseText)
        locations = JSON.parse(Http.responseText);
    }
</script>

<div class="search">
    <p>Zoek op postcode:</p>
    <input type='text' bind:value={name}/>
</div>


<!-- <table class="data-table">
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
            <tr>
                <td>{@html location.Place}</td>
                <td>{location.LocationName}</td>
                <td>{location.Address}</td>
                <td>{location.Postalcode}</td>
                <td>{@html location.OpeningHours}</td>
                <td>{location.Particularities}</td>
            </tr>
        {/each}
    </tbody>
    
</table> -->

{#each locations as location}
    <div class="location-card">
        <table class="lightblue">
            <tr>
                <th>Plaats</th>
                <td>{@html location.Place}</td>
            </tr>
            <tr>
                <th>Naam</th>
                <td>{location.LocationName}</td>
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
{/each}

<style>
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
        font-weight: 550;
    }
    
    /* colors */

    .darkblue {
        background: #142d49;
    }

    .lightblue {
        background: #79b9d5;
    }

    .red {
        background: #e7334c;
    }
</style>
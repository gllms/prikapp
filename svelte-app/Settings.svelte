<script>
    import { ageChoice, freqChoice, themeChoice } from "./stores.js";

    let firstTime = true;
    let timeout;
    $: {
        if (!firstTime) {
            document.body.classList.add("transforming");
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                document.body.classList.remove("transforming");
            }, 1000);
            if($themeChoice == "dark") {
                window.document.body.classList.add("dark-mode");
                document.documentElement.style.setProperty("--color-scheme", "dark");
            } else if ($themeChoice == "light") {
                window.document.body.classList.remove("dark-mode");
                document.documentElement.style.setProperty("--color-scheme", "light");
            }
        }
        firstTime = false;
    }
</script>

<div class="top">
    <h1><span class="material-icons">settings</span>Instellingen</h1>
</div>

<div class="settings">
    <div class="legend">Leeftijd</div>
    <label>Jong<input on:click={() => $ageChoice = "jong"} class="radios" type="radio"  bind:group={$ageChoice} value="jong" /></label>
    <label>Middelbaar<input on:click= {() => $ageChoice = "middelbaar"} class="radios" type="radio"  bind:group={$ageChoice} value="middelbaar" /></label>
    <label>Oud<input on:click= {() => $ageChoice = "oud"} class="radios" type="radio"  bind:group={$ageChoice} value="oud" /></label>

    <div class="legend">Frequentie</div>
    <label>Incidenteel<input on:click= {() => $freqChoice = "incidenteel"} class="radios" type="radio" bind:group={$freqChoice} value="incidenteel" /></label>
    <label>Regelmatig<input on:click= {() => $freqChoice = "regelmatig"} class="radios" type="radio" bind:group={$freqChoice} value="regelmatig" /></label>

    <div class="legend">Thema</div>
    <label>Licht<input on:click= {() => $themeChoice = "light"} class="radios" type="radio" bind:group={$themeChoice} value="light" /></label>
    <label>Donker<input on:click= {() => $themeChoice = "dark"} class="radios" type="radio" bind:group={$themeChoice} value="dark" /></label>
</div>

<style>
    .legend {
        font-size: 14px;
        font-weight: bold;
        margin: 32px 0 8px 16px;
        transition: color 1s;
    }

    .settings {
        display: block;
        max-width: 500px;
        margin: 0 auto;
        color: black;
    }

    :global(.dark-mode) .settings {
        color: white;
    }

    label {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 12px 16px;
        border-bottom: 2px solid #ddd;
        transition: background .2s, color 1s;
    }

    label:hover {
        background: #ddd;
    }

    :global(.dark-mode) label {
        border-bottom: 2px solid #222;
    }

    :global(.dark-mode) label:hover {
        background: #222;
    }

    .top {
        position: sticky;
        top: 0;
        background: #e7334c;
        height: 102px;
        display: flex;
        align-items: flex-end;
    }

    .top h1 {
        margin: 16px;
        font-size: 24px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
    }

    :global(.transforming), :global(.transforming) * {
        transition: 1s;
    }
</style>

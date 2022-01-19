<script>
    import { textSize, themeChoice } from "./stores.js";

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
    <div class="legend">Tekstgrootte</div>
    <label>Normaal<input class="radios" type="radio"  bind:group={$textSize} value="100%" /></label>
    <label>Groot<input class="radios" type="radio"  bind:group={$textSize} value="110%" /></label>
    <label>Enorm<input class="radios" type="radio"  bind:group={$textSize} value="120%" /></label>

    <div class="legend">Thema</div>
    <label>Licht<input class="radios" type="radio" bind:group={$themeChoice} value="light" /></label>
    <label>Donker<input class="radios" type="radio" bind:group={$themeChoice} value="dark" /></label>
</div>

<style>
    .legend {
        font-size: 0.9em;
        font-weight: bold;
        margin: 32px 0 8px 16px;
        transition: color 1s;
        color: black;
    }

    :global(.dark-mode) .legend {
        color: white;
    }

    .settings {
        display: block;
        max-width: 500px;
        margin: 0 auto;
        color: black;
        transition: font-size .2s;
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
        color: black;
    }

    label:hover {
        background: #ddd;
    }

    :global(.dark-mode) label {
        border-bottom: 2px solid #222;
        color: white;
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
        font-size: 1.5em;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
        transition: font-size .2s;
    }

    :global(.transforming), :global(.transforming) * {
        transition: 1s;
    }
</style>

<script>
    import { ageChoice, freqChoice, themeChoice } from "./stores.js";

    function ageSave(ageValue){
        localStorage.setItem("age", ageValue);
    }

    function freqSave(freqValue){
        localStorage.setItem("freq", freqValue);
    }

    function themeSave(themeValue) {
        if(themeValue == "Dark") {
            window.document.body.classList.add("dark-mode");
            localStorage.setItem("theme", "donker");
        }
        else if (themeValue == "Light") {
            window.document.body.classList.remove("dark-mode");
            localStorage.setItem("theme", "licht");
        }
    }     
</script>


<nav class="navBar">
    <button>
        <span class="material-icons settingsArrow">arrow_back</span>
    </button>

    <img src="logo.png" alt="logo"/>
    <div class="cog">
        <span class="material-icons cogWheel">settings</span>
        <div class="settingsText">Instellingen</div>
    </div>
</nav>

<div role="radiogroup" class="group-container">

    <div class="legend">
        Leeftijd
    </div>
    
    <label><input on:click={() => ageSave("jong")} class="radios" type="radio"  bind:group={$ageChoice} value="jong" /> Jong</label><br>
    <label><input on:click= {() => ageSave("middelbaar")} class="radios" type="radio"  bind:group={$ageChoice} value="middelbaar" /> Middelbaar</label><br>
    
    <label><input on:click= {() => ageSave("oud")} class="radios" type="radio"  bind:group={$ageChoice} value={"oud"} /> Oud</label>

    <div class="legend">
        Frequentie
    </div>

    <label><input on:click= {() => freqSave("incidenteel")} class="radios" type="radio" bind:group={$freqChoice} value="incidenteel" /> Incidenteel</label><br>
    <label><input on:click= {() => freqSave("regelmatig")} class="radios" type="radio" bind:group={$freqChoice} value="regelmatig" /> Regelmatig</label><br>

    <div class="legend">
        Thema
    </div>

    <label><input on:click= {() => themeSave("Light")} class="radios" type="radio" bind:group={$themeChoice} value="licht" /> Licht</label><br>
    <label><input on:click= {() => themeSave("Dark")} class="radios" type="radio" bind:group={$themeChoice} value="donker" /> Donker</label><br>
</div>

<style>
    :global(body) {
        background-color: #eee;
        color: black;
        transition: background-color 0.3s;
    }
    :global(body.dark-mode) {
        background-color: #222;
        color: white;
    }

    .legend {
        font-size: 24px;
    }

    .group-container {
        position: absolute;
        top: 130px;
        font-size: 20px;
        margin: 32px;
    }

    nav button {
        background: none;
        border: none;
        color: white;
        padding: 12px;
        cursor: pointer;
    }

    img {
        position: absolute;
        height: 32px;
        margin: 8px;
        left: 50%;
        transform: translateX(-50%);
    }

    .navBar {
        position: fixed;
        top: 0;
        background: #e7334c;
        height: 130px;
        width: 100%;
        box-sizing: border-box;
    }

    .cog {
        position: absolute;
        bottom: 5px;
        color: white;
    }

    .cogWheel {
        position: absolute;
        left: 10px;
        bottom: 3px;
    }

    .settingsText {
        position: absolute;
        left: 35px;
        bottom: 0;
        font-size: 25px;
    }
</style>

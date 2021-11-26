<script>
    import { ageChoice } from "./store.js";
    import { freqChoice } from "./store.js";
    import { themeChoice } from "./store.js";
	const ageOptions = [{
		value: "jong",
		label: "Jong",
	}, {
		value: "middelbaar",
		label: "Middelbaar",
	}, {
		value: "oud",
		label: "Oud",
	}]
    const frequentie = [{
		value: "vaak",
		label: "Vaak",
	}, {
		value: "incidenteel",
		label: "Incidenteel",
	}]
    const themeOptions = [{
        value: "licht",
        label: "Licht",
    },  {
        value: "donker",
        label: "Donker",
    }]
    function ageSave(ageValue){
        localStorage.setItem('age', ageValue);
    }
    function freqSave(freqValue){
        localStorage.setItem('freq', freqValue);
    }
    function themeSave(themeValue) {
        if(!(window.document.body.classList.contains('dark-mode'))&& themeValue=="Dark"){
            window.document.body.classList.toggle('dark-mode');
            localStorage.setItem('theme', 'donker');
        }
        else if (window.document.body.classList.contains('dark-mode')&& themeValue=="Light"){
            window.document.body.classList.toggle('dark-mode');
            localStorage.setItem('theme', 'licht');
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

    <input on:click= {() => ageSave("jong")} class="radios" type="radio"  bind:group={$ageChoice} value={"jong"} />
    <label for="age"> {"Jong"} </label><br>
    <input on:click= {() => ageSave("middelbaar")} class="radios" type="radio"  bind:group={$ageChoice} value={"middelbaar"} />
    <label for="age"> {"Middelbaar"} </label><br>
    <input on:click= {() => ageSave("oud")} class="radios" type="radio"  bind:group={$ageChoice} value={"oud"} />
    <label for="age"> {"Oud"} </label>
	<div class="legend">
        Frequentie
	</div>


    <input on:click= {() => freqSave("incidenteel")} class="radios" type="radio" bind:group={$freqChoice} value={"incidenteel"} />
    <label for="freq"> {"Incidenteel"} </label><br>
    <input on:click= {() => freqSave("regelmatig")} class="radios" type="radio" bind:group={$freqChoice} value={"regelmatig"} />
    <label for="freq"> {"Regelmatig"} </label><br>



    <div class="legend">
        Thema
    </div>

    <input on:click= {() => themeSave("Light")} class="radios" type="radio" bind:group={$themeChoice} value={"licht"} />
    <label for="the"> {"Licht"} </label><br>
    <input on:click= {() => themeSave("Dark")} class="radios" type="radio" bind:group={$themeChoice} value={"donker"} />
    <label for="the"> {"Donker"} </label><br>
</div>
















<style>
    :global(body) {
        background-color: #eee;
        color: black;
        transition: background-color 0.3s;
    }
    :global(body.dark-mode) {
		background-color: black;
		color: white;
	}






















    .legend{
        font-size: 24px;
    }
    .group-container{
        position: absolute;
        top: 300px;
        font-size: 20px;
        padding-left: 5%;
    }
    button{
        background: none;
        border: none;
    }
    .settingsArrow{
        font-size: 40px;
        color: white;
    }
    img{
        position: absolute;
        height: 32px;
        margin: 8px;
        left: 50%;
        transform: translateX(-50%);
    }
    .navBar{
        position: fixed;
        top: 0;
        background: #e7334c;
        height: 130px;
        width: 100%;
        box-sizing: border-box;
    }
    .cog{
        position: absolute;
        bottom: 5px;
        color: white;
    }
    .cogWheel{
        position: absolute;
        left: 10px;
        bottom: 3px;
    }
    .settingsText{
        position: absolute;
        left: 35px;
        bottom: 0;
        font-size: 25px;
    }
</style>

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
    let userSelectedAge = ageOptions[$ageChoice].value;
    let userSelectedFreq = frequentie[$freqChoice].value;
    let userSelectedTheme = themeOptions[$themeChoice].value;
    function toggle(themeButton) {
        if(!(window.document.body.classList.contains('dark-mode'))&& themeButton=="Dark"){
            window.document.body.classList.toggle('dark-mode');
            localStorage.setItem('theme', 'dark');
        }
        else if (window.document.body.classList.contains('dark-mode')&& themeButton=="Light"){
            window.document.body.classList.toggle('dark-mode');
            localStorage.setItem('theme', 'light');
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

    <input class="radios" type="radio"  bind:group={userSelectedAge} value={"jong"} />
    <label for="age"> {"Jong"} </label><br>
    <input class="radios" type="radio"  bind:group={userSelectedAge} value={"middelbaar"} />
    <label for="age"> {"Middelbaar"} </label><br>
    <input class="radios" type="radio"  bind:group={userSelectedAge} value={"oud"} />
    <label for="age"> {"Oud"} </label>
	<div class="legend">
        Frequentie
	</div>

	{#each frequentie as { value, label }}
        <input class="radios" type="radio" bind:group={userSelectedFreq} value={value} />
        <label for="fre"> {label} </label><br>
    {/each}


    <div class="legend">
        Thema
    </div>

    <input on:click= {() => toggle("Light")} class="radios" type="radio" bind:group={userSelectedTheme} value={"licht"} />
    <label for="the"> {"Licht"} </label><br>
    <input on:click= {() => toggle("Dark")} class="radios" type="radio" bind:group={userSelectedTheme} value={"donker"} />
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
    :global(nav.dark-mode) {
		color: black;
        background-color: white;
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

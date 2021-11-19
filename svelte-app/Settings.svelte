<script>
    import Agechoice from './store.js';
    import Testing from "./store.js";
    let Test;
    Agechoice.subscribe(age=> {
        Test = age;
    });
    let testing;
    Testing.subscribe(bage=> {
        testing = bage;
    });
	const Ageoptions = [{
		value: 'j',
		label: 'Jong',
	}, {
		value: 'm',
		label: 'Middelbaar',
	}, {
		value: 'o',
		label: 'Oud',
	}]
    const frequentie = [{
		value: 'v',
		label: 'Vaak',
	}, {
		value: 'l',
		label: 'Incidenteel',
	}]
    const Theme = [{
        value: 'l',
        label: 'licht',
    },  {
        value: 'd',
        label: 'donker',
    }]
    export let userSelectedAge = Ageoptions[Test].value;
    export let userSelectedFreq = frequentie[0].value;
    export let userSelectedTheme = Theme[1].value;
	export let fontSize = 16;
	export let flexDirection = 'column';

    const uniqueID = Math.floor(Math.random() * 100)

    const slugify = (str = "") =>
        str.toLowerCase().replace(/ /g, "-").replace(/\./g, "");
</script>


<nav class="topBar">
    <button>
        <span class="material-icons settingsArrow">arrow_back</span>
    </button>

    <img src="logo.png" alt="logo"/>
    <div class="cog">
        <span class="material-icons cogWheel">settings</span>
        <div class="settingsText">Instellingen</div>
    </div>
</nav>

<div role="radiogroup" 
				 class="group-container"
				 aria-labelledby={`label-${uniqueID}`}
				 style="font-size:{fontSize}px; flex-direction:{flexDirection}" 
				 id={`group-${uniqueID}`}>
        <div class="legend" 
						 id={`label-${uniqueID}`}>Agechoice
				</div>
  {#each Ageoptions as { value, label }}
    <input
      class="sr-only"
      type="radio"
      id={slugify(label)}
      bind:group={userSelectedAge}
      value={value} />
    <label for={slugify(label)}> {label} </label>
  {/each}
	<div class="legend" 
						 id={`label-${uniqueID}`}>Frequentie
	</div>
	{#each frequentie as { value, label }}
    <input
      class="sr-only"
      type="radio"
      id={slugify(label)}
      bind:group={userSelectedFreq}
      value={value} />
    <label for={slugify(label)}> {label} </label>
  {/each}
  <div class="legend" 
						 id={`label-${uniqueID}`}>{testing}
	</div>
  {#each Theme as { value, label }}
    <input
      class="sr-only"
      type="radio"
      id={slugify(label)}
      bind:group={userSelectedTheme}
      value={value} />
    <label for={slugify(label)}> {label} </label>
  {/each}
</div>
















<style>
    .group-container{
        position: absolute;
        top: 300px;
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
    .topBar{
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
    .settings{
        position: absolute;
        top: 150px;
    }
    #jong:checked{
        color: brown;
    }
</style>

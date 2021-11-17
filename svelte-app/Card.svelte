<script>
    import { createEventDispatcher } from 'svelte';
    
    export let card = {};

    const dispatch = createEventDispatcher();

    let categories = {
        "video": {
            "icon": "movie",
            "color": "darkblue"
        },
        "text": {
            "icon": "article",
            "color": "lightblue"
        }
    };
</script>

<div class="card" on:click={() => dispatch("click")} tabindex="0">
    <div class={"top " + categories[card.Type].color}>
        <h1><span class="material-icons">{categories[card.Type].icon}</span>{card.Title}</h1>
    </div>
    <p>{card.Description}</p>
</div>

<style>
    .card {
        border-radius: 8px;
        background: white;
        box-shadow: 2px 2px 5px rgba(0, 0, 0, .1);
        font-family: sans-serif;
        cursor: pointer;
        transition: .2s;
    }

    .card:hover, .card:focus {
        box-shadow: 8px 8px 5px rgba(0, 0, 0, .05);
        transform: scale(1.01);
        z-index: 0;
    }

    .top {
        background: #e7334c;
        height: 100px;
        display: flex;
        align-items: flex-end;
        border-radius: 8px 8px 0 0;
    }

    .darkblue {
        background: #142d49;
    }

    .lightblue {
        background: #79b9d5;
    }

    .red {
        background: #e7334c;
    }

    h1 {
        margin: 8px;
        font-size: 24px;
        color: white;
        display: flex;
        align-items: center;
        gap: 5px;
    }

    p {
        margin: 16px;
    }
</style>
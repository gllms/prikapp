<script>
    import Menu from "./Menu.svelte";
    import Home from "./Home.svelte";
    import Settings from "./Settings.svelte";
    import NotFound from "./NotFound.svelte";
    import { themeChoice  } from "./store.js";

    let path = location.pathname.split(/[/?#]/g)[1];
    let routing = {
        "": Home,
        "settings": Settings,
    };

    if(!(window.document.body.classList.contains('dark-mode'))&& $themeChoice=="donker"){
        window.document.body.classList.toggle('dark-mode');
        /*window.document.p.classList.toggle("dark-mode");*/
    }
    else if (window.document.body.classList.contains('dark-mode')&& $themeChoice=="licht"){
        window.document.body.classList.toggle('dark-mode');
    }
</script>

<Menu />

<svelte:component this={routing[path] ?? NotFound} />

<style>
    :global(body) {
        background: #eee;
        margin-top: 48px;
    }
    :global(body.dark-mode){
        background: black;
        margin-top: 48px;
    }
</style>

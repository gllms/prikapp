<script>
    import Menu from "./Menu.svelte";
    import Home from "./Home.svelte";
    import Settings from "./Settings.svelte";
    import NotFound from "./NotFound.svelte";
    import { themeChoice  } from "./stores.js";

    let path = location.pathname.split(/[/?#]/g)[1];
    let routing = {
        "": Home,
        "settings": Settings,
    };

    if($themeChoice == "donker"){
        window.document.body.classList.add('dark-mode');
    }
    else if ($themeChoice == "licht"){
        window.document.body.classList.remove('dark-mode');
    }
</script>

<Menu />

<svelte:component this={routing[path] ?? NotFound} />

<style>
    :global(body) {
        background: #eee;
        margin-top: 48px;
    }
</style>

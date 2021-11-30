<script>
    import Menu from "./Menu.svelte";
    import Home from "./Home.svelte";
    import Settings from "./Settings.svelte";
    import Locations from "./Locations.svelte";
    import NotFound from "./NotFound.svelte";

    let path = location.pathname.split(/[/?#]/g)[1];
    let routing = {
        "": Home,
        "settings": Settings,
        "locations": Locations,
    };
</script>

<Menu />

<svelte:component this={routing[path] ?? NotFound} />

<style>
    :global(body) {
        background: #eee;
        margin-top: 48px;
    }
</style>

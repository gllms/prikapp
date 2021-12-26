<script>
    import Menu from "./Menu.svelte";
    import Home from "./Home.svelte";
    import Settings from "./Settings.svelte";
    import Locations from "./Locations.svelte";
    import NotFound from "./NotFound.svelte";
    import { themeChoice, currentPage } from "./stores.js";
    import { onDestroy } from "svelte";

    $currentPage = location.pathname.split(/[/?#]/g)[1];
    $: document.title = ($currentPage || "home") + " - prikapp";

    let routing = {
        "": Home,
        "instellingen": Settings,
        "locaties": Locations,
    };

    if ($themeChoice == "donker") {
        window.document.body.classList.add("dark-mode");
    }

    window.addEventListener("popstate", handlePopState);
    onDestroy(() => {
        window.removeEventListener("popstate", handlePopState);
    });

    function handlePopState(e) {
        $currentPage = location.pathname.split(/[/?#]/g)[1];
    }
</script>

<Menu />

<svelte:component this={routing[$currentPage] ?? NotFound} />
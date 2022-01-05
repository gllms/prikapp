<script>
    import Menu from "./Menu.svelte";
    import Home from "./Home.svelte";
    import Settings from "./Settings.svelte";
    import Locations from "./Locations.svelte";
    import NotFound from "./NotFound.svelte";
    import Toast from "./Toast.svelte";
    import { themeChoice, currentPage, overlayCount } from "./stores.js";
    import { onDestroy } from "svelte";

    $currentPage = location.pathname.split(/[/?#]/g)[1];
    $: document.title = ($currentPage || "home") + " - prikapp";

    let routing = {
        "": Home,
        "instellingen": Settings,
        "locaties": Locations,
    };

    if ($themeChoice == "dark") {
        document.body.classList.add("dark-mode");
        document.documentElement.style.setProperty("--color-scheme", "dark");
    }

    $: document.body.style.overflow = $overlayCount ? "hidden" : "overlay";

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

<Toast />
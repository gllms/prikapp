<script>
    import { currentPage } from "./stores";

    export let href;
    $: if (href) {
        href = href.replace(/^\//, "").split(/[?#]/g)[0];
    }

    function handleClick(e) {
        e.preventDefault();
        $currentPage = href;
        history.pushState({}, document.title, "/" + href);
    }
</script>

<a {href} on:click={handleClick} draggable="false">
    <slot />
</a>
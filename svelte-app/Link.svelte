<script>
    import { currentPage } from "./stores";

    export let href;

    function handleClick(e) {
        e.preventDefault();
        $currentPage = href.replace(/^\//, "").split(/[?#]/g)[0];
        history.pushState({}, document.title, "/" + $currentPage);
    }
</script>

<a {href} on:click={handleClick} title={href} draggable="false">
    <slot />
</a>
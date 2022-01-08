<script>
    import { onDestroy } from "svelte";

    export let query;

    let matches = false;
    let callback = e => matches = e.matches;
    
    let mql = window.matchMedia(query);
    mql.addEventListener("change", callback);
    matches = mql.matches;
    
    onDestroy(() => mql && callback && mql.removeEventListener("change", callback));
</script>

<slot {matches} />
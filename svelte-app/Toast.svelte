<script>
    import { fly } from "svelte/transition";
    import { backOut } from "svelte/easing";

    let showToast = false;
    let timeoutTrue;
    let timeoutFalse;
    let value;
    let icon = undefined;

    function touchStart(e) {
        let el = e.target.closest("[title]");
        if (el) {
            clearTimeout(timeoutTrue);
            clearTimeout(timeoutFalse);
            timeoutTrue = setTimeout(() => {
                value = el.title || el.getAttribute("href");
                icon = [el, ...el.children].find(c => c.matches(".material-icons"))?.innerText;
                showToast = true;
            }, 500);
            timeoutFalse = setTimeout(() => showToast = false, 3000);
        }
    }

    function touchEnd(e) {
        clearTimeout(timeoutTrue);
    }

    function contextMenu(e) {
        if (!e.target.closest("input[type='text'], input[type='number'], input[type='url'], .rich-text"))
            e.preventDefault();
    }
</script>

<svelte:window on:touchstart={touchStart} on:touchend={touchEnd}/>
<svelte:body on:contextmenu={contextMenu} />

{#if showToast}
    <div transition:fly={{ y: 200, easing: backOut }}>
        {#if icon}
            <span class="material-icons">{icon}</span>
        {/if}
        {value}
    </div>
{/if}

<style>
    div {
        position: fixed;
        bottom: 1em;
        left: 50%;
        width: fit-content;
        max-width: calc(100% - 4em);
        transform: translateX(-50%);
        padding: .5em 1em;
        border-radius: 4em;
        background: #ddd;
        color: #444;
        box-shadow: 2px 2px 5px rgba(0, 0, 0, .1);
        text-align: center;
        z-index: 9999;
        display: flex;
        align-items: center;
        gap: 4px;
    }

    :global(.dark-mode) div {
        background: #333;
        color: #ddd;
    }
</style>
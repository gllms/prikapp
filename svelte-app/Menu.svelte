<script>
    import { overlayCount } from "./stores";
    import { fade, fly } from "svelte/transition";

    let navOpen = false;
    
    function handleNav() {
        navOpen = !navOpen;
        $overlayCount += (navOpen ? 1 : -1);
        document.body.style.overflow = $overlayCount ? "hidden" : "overlay";
    }
</script>

<nav>
    <button on:click={handleNav}>
        <span class="material-icons">menu</span>
    </button>
    <img src="/images/icons-192.png" alt="logo"/>
</nav>

{#if navOpen}
    <div class="grey" on:click={handleNav} in:fade={{ duration: 200 }} out:fade={{ duration: 200, delay: 200 }}></div>

    <div class="sidenav" in:fly={{ x: -400, duration: 200, delay: 200, opacity: 1 }} out:fly={{ x:-400, duration: 200, opacity: 1 }}>
        <div class="top">
            <button on:click={handleNav}>
                <span class="material-icons">close</span>
            </button>
            <p>Menu</p>
        </div>
        <div class="bottom">
            <a href="/"><span class="material-icons">view_day</span>Home</a>
            <a href="/locations"><span class="material-icons">place</span>Locaties</a>
            <a href="/login"><span class="material-icons">person</span>Inloggen</a>
            <a href="/settings"><span class="material-icons">settings</span>Instellingen</a>    </div>
    </div>
{/if}

<style>
    nav {
        position: fixed;
        background: #e7334c;
        height: 48px;
        width: 100%;
        top: 0;
        box-sizing: border-box;
        z-index: 999;
        user-select: none;
    }

    /* Hamburger Menu icon */	
    nav button, .sidenav .top button {
        background: none;
        border: none;
        color: white;
        padding: 12px;
        cursor: pointer;
    }

    nav img {
        position: absolute;
        height: 32px;
        margin: 8px 0;
        left: 50%;
        transform: translateX(-50%);
        user-select: none;
    }

    .grey {
        position: fixed;
        width: 100%;
        height: 100%;
        z-index: 9999;
        top: 0;
        background: rgba(0, 0, 0, .95);
        transition: 0.5s;
    }

    @supports ((-webkit-backdrop-filter: none) or (backdrop-filter: none)) {
        .grey {
            background: rgba(0, 0, 0, .8);
            backdrop-filter: blur(10px);
        }
    }

    /* The side navigation menu */
    .sidenav {
        height: 100%; 
        position: fixed;
        z-index: 9999;
        top: 0;
        background: white;
        font-family: sans-serif;
        overflow-x: hidden; /* Disable horizontal scroll */
        transition: 0.5s;
        width: min(400px, calc(100% - 48px));
    }

    :global(.dark-mode) .sidenav {
        background: #222;
    }

    /* The navigation menu links */
    .sidenav .top {
        display: flex;
        align-items: flex-end;
        background: #e7334c;
        height: 150px;
    }

    .sidenav .top p {
        margin: 0;
        padding: 8px 16px;
        font-size: 36px;
        color: white;
    }

    /* Position and style the close button (top right corner) */
    .sidenav .top button {
        position: absolute;
        right: 0;
        top: 0;
    }

    .sidenav .bottom a {
        display: flex;
        align-items: center;
        gap: 8px;
        text-decoration: none;
        font-size: 16px;
        padding: 16px 16px;
        color: black;
        transition: 0.3s;
    }
    
    :global(.dark-mode) .sidenav .bottom a {
        color: white;
    }

    /* When you mouse over the navigation links, change their color */
    .sidenav .bottom a:hover {
        background: lightgrey;
    }
</style>
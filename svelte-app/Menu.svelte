<script>
    import { cards, overlayCount, currentPage, token } from "./stores.js";
    import Link from "./Link.svelte";
    import Loading from "./Loading.svelte";
    import { fly } from "svelte/transition";

    let navOpen = false;

    function handleNav(open) {
        if (navOpen !== open)
            $overlayCount += (open ? 1 : -1);
        navOpen = open;
    }

    let sideNav;
    let grey;
    let sideNavWidth;

    let startX;
    let lastX;
    let wasOpen = false;
    let dragging = false;
    let firstMove = true;
    let mouseDown = false;

    function touchStart(e) {
        startX = lastX = e.touches?.[0].clientX ?? e.clientX;
        wasOpen = navOpen;
        mouseDown = true;
        
        if (!wasOpen && startX < 16 && !e.target.closest("nav")) {
            dragging = true;
            handleNav(true);
            sideNav.style.transform = `translateX(${-sideNavWidth + lastX}px)`;
            grey.style.opacity = 0;
        }
    }

    function touchMove(e) {
        if (mouseDown) {
            lastX = e.touches?.[0].clientX ?? e.clientX;
            
            if (firstMove) {
                sideNav.style.transitionDuration = grey.style.transitionDuration = "0s";
                firstMove = false;
            }
            
            if (!dragging && wasOpen && Math.abs(lastX - startX) > 25) {
                dragging = true;
                startX = lastX;
            }
            
            if (dragging) {
                let offset = wasOpen ? Math.min(lastX - startX, 0) : Math.min(-sideNavWidth + Math.max(lastX, 0), 0);
                sideNav.style.transform = `translateX(${offset}px)`;
                grey.style.opacity = 1 - -offset / sideNavWidth;
            }
        }
    }

    function touchEnd(e) {
        sideNav.style.transitionDuration = grey.style.transitionDuration = grey.style.opacity = "";
        if (dragging)
            handleNav(lastX - startX > (wasOpen ? -100 : 100));
        if (Math.abs(lastX - startX) < 25 && e.target === grey) {
            handleNav(false);
            e.preventDefault();
        }
        dragging = mouseDown = false;
        firstMove = true;
        sideNav.style.transform = "";
    }

    let loggingOut = false;
    function logOut() {
        if (loggingOut) return;
        loggingOut = true;
        fetch("/logOut", {
            method: "POST",
            headers: {
                "Authorization": $token
            }
        }).then(res => {
            setTimeout(() => {
                $token = "";
                loggingOut = false;
            }, 1000);
        }).catch(err => {
            alert("Er is iets misgegaan bij het uitloggen. Probeer het later opnieuw.");
            loggingOut = false;
        });
    }

    function createCard() {
        fetch("/createCard", {
            method: "POST",
            headers: {
                "Authorization": $token
            }
        })
        .then(res => res.text())
        .then(res => {
            if (res) {
                $cards = [{
                    Id: parseInt(res),
                    Type: 0,
                    Title: "",
                    Description: "",
                    Content: "{}"
                }, ...$cards];
            } else {
                alert("Er is iets misgegaan. Probeer het later opnieuw.");
            }
        });
    }
</script>

<svelte:window on:touchstart={touchStart} on:mousedown={touchStart} on:touchmove={touchMove} on:mousemove={touchMove} on:touchend={touchEnd} on:mouseup={touchEnd} />

<nav>
    <button on:click={() => handleNav(true)} title="menu">
        <span class="material-icons">menu</span>
    </button>
    <img src="/images/icons-192.png" alt="logo" title="logo"/>
    {#if $token}
        <div transition:fly={{ x: 200 }}>
            {#if $currentPage == ""}
                <button on:click={createCard} title="kaart toevoegen">
                    <span class="material-icons">add</span>
                </button>
            {/if}
            {#if loggingOut}
                <Loading white/>
            {:else}
                <button on:click={logOut} title="uitloggen"><span class="material-icons">logout</span></button>
            {/if}
        </div>
    {/if}
</nav>

<div class="grey" class:open={navOpen} bind:this={grey}></div>

<div class="sidenav" class:open={navOpen} bind:this={sideNav} bind:clientWidth={sideNavWidth}>
    <div class="top">
        <p>Menu</p>
    </div>
    <div class="bottom" on:click={ e => e.target.closest("a") && handleNav(false) }>
        <Link href="/"><span class="material-icons">view_day</span>Home</Link>
        <Link href="/locaties"><span class="material-icons">place</span>Locaties</Link>
        <Link href="/instellingen"><span class="material-icons">settings</span>Instellingen</Link>
        {#if !$token}
            <Link href="/login"><span class="material-icons">login</span>Inloggen</Link>
        {/if}
    </div>
</div>

<style>
    nav {
        position: fixed;
        background: #e7334c;
        height: 48px;
        width: 100%;
        top: 0;
        box-sizing: border-box;
        z-index: 999;
    }

    nav button {
        background: none;
        border: none;
        color: white;
        padding: 12px;
        cursor: pointer;
    }

    nav :nth-child(3) {
        display: flex;
        float: right;
    }

    nav :global(.loading) {
        margin: 4px !important;
    }

    nav img {
        position: absolute;
        height: 32px;
        margin: 8px 0;
        left: 50%;
        transform: translateX(-50%);
    }

    .grey {
        position: fixed;
        width: 100%;
        height: 100%;
        z-index: 9999;
        top: 0;
        background: rgba(0, 0, 0, .95);
        opacity: 0;
        transition: 0.5s;
        pointer-events: none;
    }

    .grey.open {
        opacity: 1;
        pointer-events: all;
    }

    /* The side navigation menu */
    .sidenav {
        height: 100%;
        position: fixed !important;
        z-index: 9999;
        top: 0;
        background: white;
        font-family: sans-serif;
        transition: 0.5s cubic-bezier(0.1, 1.4, 0.5, 1);
        width: min(400px, calc(100% - 48px));
        transform: translateX(-400px);
        box-shadow: -20px 0 0 0 white;
    }

    .sidenav.open {
        transform: initial;
    }

    :global(.dark-mode) .sidenav {
        background: #222;
        box-shadow: -20px 0 0 0 #222;
    }

    /* The navigation menu links */
    .sidenav .top {
        display: flex;
        align-items: flex-end;
        background: #e7334c;
        height: 150px;
        box-shadow: -20px 0 0 0 #e7334c;
    }

    .sidenav .top p {
        margin: 0;
        padding: 8px 16px;
        font-size: 2.25em;
        color: white;
    }

    .sidenav .bottom :global(a) {
        display: flex;
        align-items: center;
        gap: 8px;
        text-decoration: none;
        font-size: 1em;
        padding: 16px;
        color: #222;
        transition: 0.1s;
    }
    
    :global(.dark-mode) .sidenav .bottom :global(a) {
        color: white;
    }

    /* When you mouse over the navigation links, change their color */
    .sidenav .bottom :global(a):hover {
        background: #eee;
    }

    :global(.dark-mode) .sidenav .bottom :global(a):hover {
        background: #111;
    }
</style>
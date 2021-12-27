<script>
    import { overlayCount } from "./stores";
    import Link from "./Link.svelte";

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
        
        if (!wasOpen && startX < 25 && !e.target.closest("nav")) {
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
        if (startX === lastX && e.target === grey) {
            handleNav(false);
            e.preventDefault();
        }
        dragging = mouseDown = false;
        firstMove = true;
        sideNav.style.transform = "";
    }
</script>

<svelte:window on:touchstart={touchStart} on:mousedown={touchStart} on:touchmove={touchMove} on:mousemove={touchMove} on:touchend={touchEnd} on:mouseup={touchEnd} />

<nav>
    <button on:click={() => handleNav(true)}>
        <span class="material-icons">menu</span>
    </button>
    <img src="/images/icons-192.png" alt="logo"/>
</nav>

<div class="grey" class:open={navOpen} bind:this={grey}></div>

<div class="sidenav" class:open={navOpen} bind:this={sideNav} bind:clientWidth={sideNavWidth}>
    <div class="top">
        <p>Menu</p>
    </div>
    <div class="bottom" on:click={ e => e.target.matches("a") && handleNav(false) }>
        <Link href="/"><span class="material-icons">view_day</span>Home</Link>
        <Link href="/locaties"><span class="material-icons">place</span>Locaties</Link>
        <Link href="/login"><span class="material-icons">person</span>Inloggen</Link>
        <Link href="/instellingen"><span class="material-icons">settings</span>Instellingen</Link>
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

    /* Hamburger Menu icon */	
    nav button {
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
    }

    .grey {
        position: fixed;
        display: none;
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

    .grey.open {
        display: block;
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
        transform: translateX(-400px);
    }

    .sidenav.open {
        transform: initial;
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

    .sidenav .bottom :global(a) {
        display: flex;
        align-items: center;
        gap: 8px;
        text-decoration: none;
        font-size: 16px;
        padding: 16px 16px;
        color: black;
        transition: 0.3s;
    }
    
    :global(.dark-mode) .sidenav .bottom :global(a) {
        color: white;
    }

    /* When you mouse over the navigation links, change their color */
    .sidenav .bottom :global(a):hover {
        background: lightgrey;
    }
</style>
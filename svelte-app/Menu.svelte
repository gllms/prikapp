<script>
    let navOpen = false;
    
    function handleNav() {
        navOpen = !navOpen;
        document.body.style.overflow = navOpen ? "hidden" : "auto";
    }
    
    function handleNavWithKey(e) {
        if (e.code === "F1") {
            e.preventDefault();
            navOpen = !navOpen;
        }
    }
</script>

<!-- Use keyboard to handle the sidenav -->
<svelte:window on:keydown={handleNavWithKey} />

<nav>
    <button on:click={handleNav}>
        <span class="material-icons">menu</span>
    </button>
    <img src="logo.png" alt="logo"/>
</nav>
  
<div class:grey={navOpen} on:click={handleNav}></div>

<div class="sidenav" class:open={navOpen}>
    <div class="top">
        <button on:click={handleNav}>
            <span class="material-icons">close</span>
        </button>
        <p>Menu</p>
    </div>
    <div class="bottom">
        <a href="#d"><span class="material-icons">place</span>Locaties</a>
        <a href="#c"><span class="material-icons">person</span>Inloggen</a>
        <a href="#b"><span class="material-icons">settings</span>Instellingen</a>
    </div>
</div>

<style>
    nav {
        position: fixed;
        background: rgba(231, 51, 76, .8);
        backdrop-filter: blur(10px);
        height: 48px;
        width: 100%;
        top: 0;
        box-sizing: border-box;
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
        margin: 8px;
        left: 50%;
        transform: translateX(-50%);
    }

    .grey {
        width: 100%;
        height: 100%;
        position: fixed;
        z-index: 9999;
        top: 0;
        background-color: rgba(0, 0, 0, .8);
        backdrop-filter: blur(10px);
        transition: 0.5s;
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
        width: min(400px, 90%);
        left: -100%;
    }

    .sidenav.open {
        left: 0;
    }

    /* The navigation menu links */
    .sidenav .top {
        display: flex;
        align-items: flex-end;
        background-color: #e7334c;
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
        padding: 16px 16px;
        text-decoration: none;
        font-size: 16px;
        color: black;
        transition: 0.3s;
    }

    /* When you mouse over the navigation links, change their color */
    .sidenav .bottom a:hover {
        background: lightgrey;
    }
</style>
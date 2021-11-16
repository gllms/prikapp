<script>
  // 	let navWidth = 0;
    let navOpen = false;
    
    function handleNav() {
      navOpen = !navOpen;
  // 		navWidth === 0 ? navWidth = 40 : navWidth = 0;
    }
    
  function handleNavWithKey(e) {
    console.log(e.code);
    if (e.code === "F1") {
      navOpen = !navOpen;
    }
  }	
    
  // 	const openNav = () => {
  // 		navOpen = !navOpen;
  // 		navWidth = 40;
  // 	}
    
  // 	const closeNav = () => {
  // 		navOpen = !navOpen;
  // 		navWidth = 0;
  // 	}	
  </script>
  
  <div class="grey" class:grey={navOpen} on:click={handleNav}></div>
  <div class="sidenav" class:open={navOpen}>
    <div class="top">
      <a href="#a" class="closebtn" on:click={handleNav}>&times;</a>
      <p1 class="text">Menu</p1>
    </div>
    <div class="bottom">
      <a class="test" href="#b">Instellingen</a>
        <a href="#c">Inloggen</a>
      <a href="#d">Locaties</a>
    </div>
  </div>
  
   <!-- Use to open the sidenav -->
   <!-- 	<span on:click={handleNav}>open</span> -->
    
    <!-- Use Menu Icon to open the sidenav -->
  <nav>
  
    <div class="container" class:change={navOpen} on:click={handleNav}>
      <span class="material-icons">menu</span>
    </div>
    <img src="logo.png" alt="logo"/>
  
  </nav>
  
  
  <!-- Use keyboard to handle the sidenav -->
  <svelte:window on:keydown={handleNavWithKey} />
  
  <style>
    .grey{
      height: 100%; 
      width: 0; /* 0 width - change this with JavaScript */
      position: fixed;
      z-index: 1;
      top: 0;
      right: 0;
      background-color: black;
      opacity: 0.7;
      overflow-x: hidden; /* Disable horizontal scroll */
      padding-top: 60px;
      transition: 0.5s;
    }
  /* Hamburger Menu icon */	
  .container {
    background: none;
    border: none;
    color: white;
    padding: 12px;
  }	
  /* span {
    position: absolute;
    right: 20%;
    display: inline-block;
    cursor: pointer;
  }	 */
    
  /* The side navigation menu */
  .sidenav {
    height: 100%; 
    width: 0; /* 0 width - change this with JavaScript */
    position: fixed;
    z-index: 1;
    top: 0;
    left: 0;
    background: #e7334c;
    overflow-x: hidden; /* Disable horizontal scroll */
    padding-top: 60px;
    transition: 0.5s;
  }
  
  /* The navigation menu links */
  .sidenav a {
    padding: 0px 8px 8px 32px;
    text-decoration: none;
    font-size: 25px;
    color: black;
    display: block;
    transition: 0.3s;
  }
    .bottom a {
      text-decoration: underline;
    }
  
  /* When you mouse over the navigation links, change their color */
  .sidenav a:hover {
    color: grey;
  }
  
  /* Position and style the close button (top right corner) */
  .sidenav .closebtn {
    position: absolute;
    top: 0;
    right: 25px;
    font-size: 36px;
    margin-left: 50px;
  }
    .text{
      padding: 8px 8px 8px 32px;
      font-size: 36px;
    }
    .test{
      margin-top: 50px;
    }
  
  .grey{
    width: 100%		
  }
    
  .open {
    width: 75%;
  }	
  
  nav {
    background: #e7334c;
    height: 48px;
    box-sizing: border-box;
    }
    
  img {
    position: absolute;
    height: 32px;
    margin: 8px;
    left: 50%;
    transform: translateX(-50%);
      }
  
  </style>
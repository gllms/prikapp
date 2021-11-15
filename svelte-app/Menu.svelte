<script>
  let navOpen = false;

  function handleNav() {
    navOpen = !navOpen;
  }
</script>

<nav>

  <div class="OpenButton" class:change={navOpen} on:click={handleNav}>
    <span class="material-icons">menu</span>
  </div>

  <img src="logo.png" alt="logo"/>
  
</nav>

<div id="mySideNav" class="TopSideNav" class:open={navOpen}>
  <div class="CloseButton" on:click={handleNav}>
    X
  </div>
  <h2 class="MenuText">
    Menu
  </h2>
</div>

<div id="mySideNav" class="GreyedOut" class:open={navOpen}></div>

<div id=mySideNav class="BottomSideNav" class:open={navOpen}>
  <a href="Info">Informatie</a>
  <a href="Inst">Instellingen</a>
  <a href="Inloggen">Inloggen</a>
</div>



<style>
/* The side navigation menu */

.OpenButton {
  position: absolute;
  padding: 25px 0px 0px 450px;
  display: inline-block;
  cursor: pointer;
}
.TopSideNav {
height: 40%; 
border: 2px solid grey;
width: 0%; /* 0 width - change this with JavaScript */
position: fixed;
z-index: 0;
top: 0;
left: 0;
background-color: #e7334c; 
overflow-x: hidden; /* Disable horizontal scroll */
padding-top: 60px;
transition: 0.5s;
}
.MenuText{
  padding-left:50px;
  color: white;
}
.GreyedOut{
  height: 100%; 
  border: 2px solid grey;
  width: 0%; /* 0 width - change this with JavaScript */
  position: fixed;
  z-index: 0;
  top: 0;
  right: 0;
  background-color: black;
  opacity: 0.7;
  overflow-x: hidden; /* Disable horizontal scroll */
  padding-top: 60px;
  transition: 0.5s;
}

.BottomSideNav{
  height: 60%;
  border: 2px solid grey;
  width: 0%; /* 0 width - change this with JavaScript */
  position: fixed;
  z-index: 0;
  top: 1;
  left: 0;
  background-color: white; 
  overflow-x: hidden; /* Disable horizontal scroll */
  padding-top: 60px;
  transition: 0.5s;
}

/* The navigation menu links */
#mySideNav a {
padding: 8px 8px 8px 32px;
text-decoration: none;
font-size: 25px;
color: #333;
display: block;
transition: 0.3s;
}

/* When you mouse over the navigation links, change their color */
#mySideNav a:hover {
color: white;
}

/* Position and style the close button (top right corner) */

.open{
  width: 49.5%;
}

  nav {
      background: #e7334c;
      height: 48px;
      box-sizing: border-box;
  }

  .OpenButton {
      background: none;
      border: none;
      color: white;
      padding: 12px;
  }

  img {
      position: absolute;
      height: 32px;
      margin: 8px;
      left: 50%;
      transform: translateX(-50%);
  }
</style>
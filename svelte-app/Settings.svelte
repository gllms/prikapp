<script>
    
</script>




<nav class="topBar">
    <button>
        <span class="material-icons settingsArrow">arrow_back</span>
    </button>

    <img src="logo.png" alt="logo"/>
    <div class="cog">
        <span class="material-icons cogWheel">settings</span>
        <div class="settingsText">Instellingen</div>
    </div>
</nav>
<div class="settings">
    <form>
        <!--Leeftijd setting-->
        <div>Leeftijd</div>
        <input type="radio" id="jong" name="Leeftijd" value="Jong" checked>
        <label for="html">Jong</label><br>
        <input type="radio" id="middelbaar" name="Leeftijd" value="Middelbaar">
        <label for="css">Middelbaar</label><br>
        <input type="radio" id="oud" name="Leeftijd" value="Oud">
        <label for="javascript">Oud</label>
        <!--Frequentie setting-->
        <div>Frequentie</div>
        <input type="radio" id="incidenteel" name="frequentie" value="Incidenteel" checked>
        <label for="html">Incidenteel</label><br>
        <input type="radio" id="regelmatig" name="frequentie" value="Regelmatig">
        <label for="css">Regelmatig</label><br>
        <!--Thema setting-->
        <div>Thema</div>
        <input type="radio" id="licht" name="thema" value="Licht">
        <label for="html">Licht</label><br>
        <input type="radio" id="donker" name="thema" value="Donker">
        <label for="css">Donker</label><br>
    </form> 
</div>

<style>
    button{
        background: none;
        border: none;
    }
    .settingsArrow{
        font-size: 40px;
        color: white;
    }
    img{
        position: absolute;
        height: 32px;
        margin: 8px;
        left: 50%;
        transform: translateX(-50%);
    }
    .topBar{
        position: fixed;
        top: 0;
        background: #e7334c;
        height: 130px;
        width: 100%;
        box-sizing: border-box;
    }
    .cog{
        position: absolute;
        bottom: 5px;
        color: white;
    }
    .cogWheel{
        position: absolute;
        left: 10px;
        bottom: 3px;
    }
    .settingsText{
        position: absolute;
        left: 35px;
        bottom: 0;
        font-size: 25px;
    }
    .settings{
        position: absolute;
        top: 150px;
    }
    #jong:checked{
        color: brown;
    }
</style>

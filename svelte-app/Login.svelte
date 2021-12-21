<script>
    let username, password
</script>

<form class="LoginBox Form">
    <h2>Inloggen</h2>
    <p>Gebruikersnaam</p>
    <input label="username" bind:value={username} type="text" placeholder="Uw gebruikersnaam" required />
    <p>Wachtwoord</p>
    <input label="password" bind:value={password} type="password" placeholder="Uw wachtwoord" required />
    <button type="submit">Inloggen</button>
</form>




<style>
    .LoginBox {
        width: 320px;
        height: 420px;
        background: #e7334c;
        color: white;
        top: 50%;
        left: 50%;
        position: absolute;
        transform: translate(-50%, -50%);
        box-sizing: border-box;
        border-radius: 8px;
        padding: 70px 30px;
    }
    h2 {
        margin: 0;
        padding: 0 0 20px;
        text-align: center;
        font-size: 22px;
    }
    .LoginBox p{
        margin: 0;
        padding: 0;
        font-weight: bold;
    }
    .LoginBox input{
        width: 100%;
        margin-bottom: 20px;
        border: none;
        border-bottom: 1px solid #fff;
        background: transparent;
        outline: none;
        height: 40px;
        color: #fff;
        font-size: 16px;
    }
    .LoginBox button{
        width: 100%;
        border: none;
        outline: none;
        height: 40px;
        background: white;
        font-size: 18px;
        border-radius: 10px;
    }
</style>
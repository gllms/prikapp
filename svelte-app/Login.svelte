<script>
    import { token } from "./stores.js";
    import goToPage from "./goToPage.js";

    let username, password;
    let p = new Promise(() => {});

    $token && goToPage("/");

    function login() {
        p = fetch("/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "Username": username,
                "Password": password.split("").reduce((a,b)=>{a=((a<<5)-a)+b.charCodeAt(0);return a&a},0)
            })
        })
        .then(response => response.text())
        .then(t => {
            $token = t;
            t && goToPage("/");
            return t;
        });
    }
</script>

<form on:submit|preventDefault={login(username, password)}>
    <h1>Login</h1>
    <label><span class="material-icons">person</span><input bind:value={username} type="text" placeholder="gebruikersnaam" required /></label>
    <label><span class="material-icons">lock</span><input bind:value={password} type="password" placeholder="wachtwoord" required /></label>
    <button type="submit">Inloggen</button>
    {#await p then t}
        {#if !t}
            <p class="error">Inloggen mislukt</p>
        {/if}
    {/await}
</form>

<style>
    form {
        display: flex;
        flex-direction: column;
        gap: 8px;
        background: white;
        top: 50%;
        left: 50%;
        position: absolute;
        transform: translate(-50%, -50%);
        border-radius: 8px;
        padding: 16px 32px 32px 32px;
        box-shadow: 2px 2px 5px rgba(0, 0, 0, .1);
    }

    h1 {
        margin: 0;
        text-align: center;
        font-size: 24px;
    }

    button {
        width: 100%;
        border: none;
        outline: none;
        padding: 8px;
        background: #e7334c;
        color: white;
        font-size: 16px;
        border-radius: 4px;
        cursor: pointer;
    }

    .error {
        color: red;
        margin: 0;
    }
</style>
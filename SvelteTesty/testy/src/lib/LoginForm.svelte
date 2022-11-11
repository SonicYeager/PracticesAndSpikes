<script>
    let username
    let password
    
    export let onAttemptLogin
    </script>
    
    <section>
        <header>
            <h2>
                Login
            </h2>
        </header>
        <form>
            <input type="text" placeholder="username" bind:value={username}>
            <input type="password" placeholder="password" bind:value={password}>
        </form>
        <footer>
            <button on:click={onAttemptLogin(username, password)}>Login</button>
        </footer>
    </section>
    
    <style>
        section {
            width: 100%;
            height: 100%;
        }
        h2 {
            color: var(--background-dark-color);
            margin: 0;
        }
        header {
            display: flex;
            flex-flow: column nowrap;
            text-align: center;
            align-self: flex-start;
            margin: 0;
            width: 100%;
            height: min-content;
            border-bottom: 1px solid whitesmoke;
            padding: .5em;
        }
        form {
            display: flex;       
            align-self: center;
            flex-flow: column nowrap; 
            align-items: center;
            justify-items: center;
            height: max-content;
            width: 100%;
            gap: .5em;
            padding: .5em;
        }
        input {
            border: none;
            box-shadow: none;
            padding: .5em;
            border-radius: .5em;
            width: 50%;
        }
        footer{
            display: flex;
            flex-flow: row nowrap;
            justify-content: center;
            align-self: flex-end;
            width: 100%;
            height: min-content;
        }
        button {
            border-radius: .5em;
            align-items: center;
            margin: .5em;
            padding: .5em;
            box-shadow: none;
            border: none;
            width: 50%;
        }
        button:hover{
            transform: scale(1.05);
            transition: 300ms;
        }
        button:active{
            transform: scale(.95);
            transition: 150ms;
        }
    </style>
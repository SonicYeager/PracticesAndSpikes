<script>
  import svelteLogo from './assets/svelte.svg'
  import Card from './lib/Card.svelte';
  import LoginForm from './lib/LoginForm.svelte';

  let title = "Ich bims eins Testy";

  function attemptLogin(username, password){
    title = username + " " + password
  }
</script>

<main>
  <link rel="stylesheet" href="node_modules/svelte-material-ui/bare.css" />
  <h1>{title}</h1>
  <div class="card">
    <Card>
      <LoginForm onAttemptLogin={attemptLogin}/>
    </Card>
  </div>
</main>

<style>
  .card {
    display: flex;
    align-self: center;
    align-content: center;
    justify-content: center;
  }
  h1 {
    color: var(--primary-dark-color);
  }
  :global(*){
    box-sizing: border-box;
  }
</style>

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
  :global(*){
    box-sizing: border-box;
  }
</style>

<script>
  import svelteLogo from './assets/svelte.svg'
    import Card from './lib/Card.svelte';
</script>

<main>
  <h1>Ich bims eins Testy</h1>

  <div class="card">
    <Card cardTitle="I am a leftist!" cardSubtitle="Mine!"/>
    <Card cardTitle="I am a rightist!" cardSubtitle="Mine!"/>
  </div>
</main>

<style>
  .card {
    display: flex;
    gap: 1em;
  }
  h1 {
    color: var(--primary-dark-color);
  }
  :global(*){
    box-sizing: border-box;
  }
</style>

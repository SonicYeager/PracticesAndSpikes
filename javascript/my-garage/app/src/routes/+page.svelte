<script>
    import Card from "$lib/components/Card.svelte";

    export let data

    $: ({GetGarages} = data)
</script>

<div class="container">
    <div class="elements">
        {#if $GetGarages.fetching}
            <p>Loading...</p>
        {:else if $GetGarages.error}
            <p>Error: {$GetGarages.error.message}</p>
        {:else}
            {#each $GetGarages.data.garages.edges as garage}
                <Card {garage}/>
            {/each}
        {/if}
    </div>
    <div class="button-container">
        <button
                disabled={!$GetGarages.pageInfo.hasNextPage}
                on:click={async () => await GetGarages.loadNextPage()}
        >
            Load More
        </button>
    </div>
</div>

<style>
    .container {
        display: flex;
        flex-direction: column;
        justify-content: space-evenly;
        height: 100%;
        padding-top: 60px;
        padding-bottom: 60px;
    }

    .elements {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-around;
        margin: 0 15vw;
    }

    .button-container {
        display: flex;
        justify-content: center;
        align-items: center;
        margin-top: 20px;
    }

    .button-container button {
        background: #1e1e1e;
        color: #f5f5f5;
        border: none;
        padding: 10px 20px;
        border-radius: 5px;
        cursor: pointer;
        transition: all 0.3s ease-in-out;
    }

    .button-container button:hover {
        background: #f5f5f5;
        color: #1e1e1e;
    }

    .button-container button:disabled {
        background: #666;
        cursor: not-allowed;
    }
</style>
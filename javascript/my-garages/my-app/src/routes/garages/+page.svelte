<script lang="ts">
    import type {PageData} from './$types';

    export let data: PageData;
</script>

<svelte:head>
    <title>About</title>
    <meta name="description" content="About this app"/>
</svelte:head>

<div>
    <h1>Garages</h1>

    <div class="gallery">
    <!-- Here should be cards of garages fetched from an graphql request -->
    {#each data.post.garages as garage (garage.id)}
        <div class="card">
            <div class="card-content">
                <p class="designation">{garage.designation}</p>
            </div>
            <div class="card-footer">
                <small class="id">{garage.id}</small>
            </div>
        </div>
    {/each}
    </div>
</div>

<style>
    .gallery {
        /* Use flexbox to layout the cards */
        display: flex;
        flex-wrap: wrap;
        justify-content: space-around;
    }
    
    .card {
        /* Define the size and layout of the card */
        width: 300px;
        height: 200px;
        padding: 20px 20px 10px 20px; /* top right bottom left */
        margin: 10px;
        box-sizing: border-box;
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;

        /* Add a soft color gradient */
        background: radial-gradient(circle at center, #ffffff 70%, #eef6fd 100%);

        /* Add some border radius for rounded corners */
        border-radius: 10px;

        /* Add some box shadow for a card-like effect */
        box-shadow: 0px 0px 15px rgba(0, 0, 0, 0.1);

        /* Add a transition for smooth hover effect */
        transition: transform 0.3s ease-in-out, box-shadow 0.3s ease-in-out;
    }

    .card:hover {
        /* Scale the card up slightly */
        transform: scale(1.05);

        /* Increase the box shadow */
        box-shadow: 0px 0px 20px rgba(0, 0, 0, 0.15);
    }

    .card-content {
        /* Fill the rest of the card */
        flex-grow: 1;

        /* Center the text */
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .designation {
        /* Style the designation text */
        font-size: 1.2em;
        text-align: center;
    }

    .card-footer {
        /* Center the footer text */
        display: flex;
        justify-content: center;

        /* Add top margin */
        margin-top: auto;
    }

    .id {
        /* Style the id text */
        font-size: 0.8em;
    }
</style>
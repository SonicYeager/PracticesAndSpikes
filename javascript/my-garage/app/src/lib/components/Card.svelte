<script>
    export let garage;
    $: garage = garage.node;
</script>

<div class="card">
    <h2>{garage.id}</h2>
    <p>{garage.designation}</p>
    <!-- Display other garage properties here -->
</div>

<style>
    .card {
        background: #1e1e1e;
        border-radius: 10px;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
        padding: 20px;
        margin: 15px;
        transition: all 0.3s ease-in-out;
        position: relative;
        overflow: hidden;
        color: #f5f5f5;
    }

    .card:hover {
        box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
        transform: scale(1.03);
    }

    .card h2 {
        margin-top: 0;
        color: #f5f5f5;
        font-size: 24px;
    }

    .card p {
        color: #f5f5f5;
        font-size: 16px;
    }
</style>
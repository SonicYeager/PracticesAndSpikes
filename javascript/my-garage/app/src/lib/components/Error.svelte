<script>
    export let message;
</script>

<div class="error-container">
    <div class="error-card">
        <p>{message}</p>
    </div>
</div>

<style>
    .error-container {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100vh; /* Full height of the viewport */
        width: 100vw; /* Full width of the viewport */
    }

    .error-card {
        background-color: #f8d7da;
        color: #721c24;
        padding: 20px;
        border-radius: 5px;
    }
</style>
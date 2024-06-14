<script>
    // You can import and use any components or scripts here
    import Footer from "$lib/components/Footer.svelte";
    import Header from "$lib/components/Header.svelte";
</script>

<Header/>

<main>
    <slot></slot>
</main>

<Footer/>

<style>
    :global(body), :global(html) {
        display: flex;
        flex-direction: column;
        height: 100vh;
        margin: 0;
        padding: 0;
        background-color: #1d1d1d;
        color: #f5f5f5;
    }

    header {
        position: sticky;
        top: 0;
    }

    main {
        flex: 1;
    }

    footer {
        position: sticky;
        bottom: 0;
    }
</style>
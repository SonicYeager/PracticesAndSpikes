<script>
export let cardTitle
export let cardSubtitle
export let cardContent = "{Card}"
</script>

<section>
    <article class="cardHeading">
        <h2>
            {cardTitle}
        </h2>
        <h4>
            {cardSubtitle}
        </h4>
    </article>
    <article class="cardContent">
        {cardContent}
    </article>
</section>

<style>
    section {
        display: flex;
        flex-flow: row wrap;
        height: 12em;
        aspect-ratio: 1/1;
        border-radius: 1em;
        box-shadow: -1px 1px 3px whitesmoke;
        background-color: var(--tertiary-dark-color);
    }
    h2, h4 {
        color: var(--on-tertiary-dark-color);
        margin: 0;
    }
    h4 {
        font-style: italic;
    }
    .cardHeading {
        display: flex;
        flex-flow: column nowrap;
        text-align: left;
        margin: 0;
        height: 4em;
        width: 100%;
        border-bottom: 1px solid whitesmoke;
        padding: .5em;
    }
    .cardContent{
        display: flex;
        flex-flow: row wrap;
        align-items: center;
        justify-content: center;
        height: calc(100% - 5em);
        width: 100%;
        padding: .5em;
    }
    section:hover{
        transform: scale(1.1);
        transition: 300ms cubic-bezier(0.89,-0.43, 0.27, 1.35);
    }
</style>
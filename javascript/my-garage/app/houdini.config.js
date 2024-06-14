/// <references types="houdini-svelte">

/** @type {import('houdini').ConfigFile} */
const config = {
    "watchSchema": {
        "url": "http://localhost:5000/graphql"
    },
    "plugins": {
        "houdini-svelte": {}
    },
    scalars: {
        // the name of the scalar we are configuring
        DateTime: {
            // the corresponding typescript type
            type: 'Date',
        },
        Decimal: {
            type: 'number',
        }
    }
}

export default config

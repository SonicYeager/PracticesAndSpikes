const app = Vue.createApp({
    data() {
        return {
            continuousOutput: "empty",
            confirmedOutput: "empty"
        };
    },
    methods: {
        showAlert() {
            alert('This is an alert!');
        },
        addConfirmed(event) {
            this.confirmedOutput = event.target.value;
        },
        addContinuous(event) {
            this.continuousOutput = event.target.value;
        }
    }
});

app.mount('#assignment');
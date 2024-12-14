const app = Vue.createApp({
    data() {
        return {
            counter: 0,
            name: "",
            confirmedName: "",
        };
    },
    methods: {
        add(num) {
            this.counter += num;
        },
        remove(num) {
            this.counter -= num;
        },
        updateName(event, def) {
            this.name = event.target.value + def;
        },
        submitForm(event) {
            alert("Submitted!");
        },
        confirmInput() {
            this.confirmedName = this.name;
        },
    },
});

app.mount('#events');

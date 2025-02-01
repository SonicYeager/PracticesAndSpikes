const app = Vue.createApp({
    data() {
        return {
            name: "Tobias",
            age: 23,
            imageUrl: "https://img.freepik.com/fotos-kostenlos/schulbedarf-auf-weissem-hoelzernem-hintergrund_24837-174.jpg?ga=GA1.1.268479560.1734126365&semt=ais_hybrid"
        }
    },
    methods: {
        calculateAge() {
            return this.age + 5;
        },
        randomNum() {
            return Math.random();
        }
    }
});

app.mount("#assignment");
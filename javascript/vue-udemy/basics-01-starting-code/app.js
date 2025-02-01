const app = Vue.createApp({
    data() {
        return {
            courseGoalA: 'Hello, Vue.js!',
            courseGoalB: '<h2>Whatever you want to learn!</h2>',
            vueLink: 'https://vuejs.org/'
        };
    },
    methods: {
        outputGoal() {
            const randomNumber = Math.random();
            if (randomNumber < 0.5) {
                return this.courseGoalA;
            } else {
                return this.courseGoalB;
            }
        }
    }
});

app.mount('#user-goal');
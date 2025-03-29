import { createApp } from 'vue';
import { createStore } from 'vuex';

import App from './App.vue';

const store = createStore({
  state() {
    return {
      count: 0
    };
  },
  mutations: {
    increment(state) {
      state.count++;
    },
    increase(state, payload) {
      state.count += payload;
    }
  },
  actions: {
    increment(context) {
      setTimeout(() => {
        context.commit('increment');
      }, 2000);
    },
    increase(context, payload) {
      setTimeout(() => {
        context.commit('increase', payload);
      }, 2000);
    }
  },
  getters: {
    count(state) {
      return state.count;
    },
    doubleCount(state) {
      return state.count * 2;
    }
  }
});

const app = createApp(App);

app.use(store);

app.mount('#app');

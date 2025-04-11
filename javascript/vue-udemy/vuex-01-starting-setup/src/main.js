import { createApp } from 'vue';
import { createStore } from 'vuex';

import App from './App.vue';

const store = createStore({
  state() {
    return {
      count: 0,
      isLoggedIn: false
    };
  },
  mutations: {
    increment(state) {
      state.count++;
    },
    increase(state, payload) {
      state.count += payload;
    },
    setAuth(state, payload) {
      state.isLoggedIn = payload.isAuth;
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
    },
    login(context) {
      context.commit('setAuth', { isAuth: true });
    },
    logout(context) {
      context.commit('setAuth', { isAuth: false });
    }
  },
  getters: {
    count(state) {
      return state.count;
    },
    doubleCount(state) {
      return state.count * 2;
    },
    userIsAuthenticated(state) {
      return state.isLoggedIn;
    }
  }
});

const app = createApp(App);

app.use(store);

app.mount('#app');

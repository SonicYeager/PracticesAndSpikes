import { createStore } from 'vuex';
import coaches from '@/store/modules/coaches';
import requests from '@/store/modules/requests';

const store = createStore({
  modules: {
    coaches: coaches,
    requests: requests,
  },
  state() {
    return {
      userId: new Date().toISOString(),
    };
  },
  getters: {
    userId(state) {
      return state.userId;
    },
  },
});

export default store;

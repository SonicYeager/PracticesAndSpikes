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
      userId: 'c3',
    };
  },
  getters: {
    userId(state) {
      return state.userId;
    },
  },
});

export default store;

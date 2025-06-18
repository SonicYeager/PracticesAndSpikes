import { createStore } from 'vuex';
import coaches from '@/store/modules/coaches';

const store = createStore({
  modules: {
    coaches: coaches,
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

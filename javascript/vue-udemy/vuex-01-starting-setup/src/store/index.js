import { createStore } from 'vuex';
import mutations from '@/store/mutations';
import actions from '@/store/actions';
import getters from '@/store/getters';
import counter from '@/store/counter/index';

const store = createStore({
  modules: {
    counter: counter,
  },
  state() {
    return {
      isLoggedIn: false,
    };
  },
  mutations: mutations,
  actions: actions,
  getters: getters,
});

export default store;

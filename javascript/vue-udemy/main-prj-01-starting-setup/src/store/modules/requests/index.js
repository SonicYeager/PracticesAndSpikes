import { createStore } from 'vuex';
import coaches from '@/store/modules/coaches';

const store = createStore({
  modules: {
    coaches: coaches,
  },
});

export default store;

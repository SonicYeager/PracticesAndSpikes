import { createStore } from 'vuex';
import coaches from '@/store/modules/coaches';
import requests from '@/store/modules/requests';
import auth from '@/store/modules/auth';

const store = createStore({
  modules: {
    coaches: coaches,
    requests: requests,
    auth: auth,
  },
});

export default store;

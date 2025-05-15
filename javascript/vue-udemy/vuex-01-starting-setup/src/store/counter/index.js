import actions from '@/store/counter/actions';
import getters from '@/store/counter/getters';
import mutations from '@/store/counter/mutations';

export default {
  state() {
    return {
      count: 0,
    };
  },
  mutations: mutations,
  actions: actions,
  getters: getters,
};

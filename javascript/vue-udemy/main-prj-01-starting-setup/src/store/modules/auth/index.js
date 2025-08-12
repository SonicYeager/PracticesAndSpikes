import mutations from './mutations';
import actions from './actions';
import getters from './getters';

export default {
  state() {
    return {
      userId: 'c1',
    };
  },
  mutations: mutations,
  actions: actions,
  getters: getters,
};

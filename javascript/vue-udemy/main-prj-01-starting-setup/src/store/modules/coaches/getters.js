export default {
  coaches(state) {
    return state.coaches;
  },
  hasCoaches(state) {
    return state.coaches && state.coaches.length > 0;
  },
  isCoach(state, getters, rootState, rootGetters) {
    const coaches = getters.coaches;
    return coaches.some((c) => c.id === rootGetters.userId);
  },
  shouldUpdate(state) {
    if (!state.lastFetch) {
      return true;
    }
    const currentTime = new Date().getTime();
    return (currentTime - state.lastFetch) / 1000 > 60;
  },
};

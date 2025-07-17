export default {
  addCoach(state, coach) {
    state.coaches.push(coach);
  },
  setCoaches(state, coaches) {
    state.coaches = coaches;
  },
  setFetchTimestamp(state) {
    state.lastFetch = new Date().getTime();
  },
};

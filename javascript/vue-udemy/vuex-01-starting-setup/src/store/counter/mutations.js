export default {
  increment(state) {
    state.count++;
  },
  increase(state, payload) {
    state.count += payload;
  },
};

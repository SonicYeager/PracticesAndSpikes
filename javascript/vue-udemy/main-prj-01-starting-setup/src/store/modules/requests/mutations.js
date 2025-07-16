export default {
  addRequest(state, request) {
    state.requests.push(request);
  },
  setRequests(state, requests) {
    state.requests = requests;
  },
};

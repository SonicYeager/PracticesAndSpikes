export default {
  async contactCoach(context, payload) {
    const newRequest = {
      userEmail: payload.email,
      message: payload.message,
    };

    const result = await fetch(
      `https://vue-http-demo-sy-default-rtdb.europe-west1.firebasedatabase.app/requests/${payload.coachId}.json`,
      {
        method: 'POST',
        body: JSON.stringify(newRequest),
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );

    if (!result.ok) {
      const error = new Error(result.statusText || 'Failed to send request');
      throw error;
    }

    const data = await result.json();
    newRequest.id = data.name;

    context.commit('addRequest', newRequest);
  },
  async fetchRequests(context) {
    const coachId = context.rootGetters.userId;

    const response = await fetch(
      `https://vue-http-demo-sy-default-rtdb.europe-west1.firebasedatabase.app/requests/${coachId}.json`
    );

    if (!response.ok) {
      const error = new Error(
        response.statusText || 'Failed to fetch requests'
      );
      throw error;
    }

    const data = await response.json();
    const requests = [];
    for (const key in data) {
      const request = {
        id: key,
        coachId: coachId,
        userEmail: data[key].userEmail,
        message: data[key].message,
      };
      requests.push(request);
    }

    context.commit('setRequests', requests);
  },
};

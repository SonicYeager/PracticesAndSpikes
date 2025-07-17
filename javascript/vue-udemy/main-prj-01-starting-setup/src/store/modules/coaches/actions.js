export default {
  async addCoach(context, coach) {
    const userId = context.rootGetters.userId;
    const mapped = {
      firstName: coach.first,
      lastName: coach.last,
      hourlyRate: coach.rate,
      areas: coach.areas,
      description: coach.desc,
    };

    const response = await fetch(
      `https://vue-http-demo-sy-default-rtdb.europe-west1.firebasedatabase.app/coaches/${userId}.json`,
      {
        method: 'PUT',
        body: JSON.stringify(mapped),
      }
    );

    //const data = await response.json();

    if (!response.ok) {
      const error = new Error(response.statusText || 'Failed to add coach');
      throw error;
    }

    context.commit('addCoach', {
      ...mapped,
      id: userId,
    });
  },
  async fetchCoaches(context, payload) {
    if (!payload.forceRefresh && !context.getters.shouldUpdate) {
      return;
    }

    const response = await fetch(
      'https://vue-http-demo-sy-default-rtdb.europe-west1.firebasedatabase.app/coaches.json'
    );

    if (!response.ok) {
      const error = new Error(response.statusText || 'Failed to fetch coaches');
      throw error;
    }

    const data = await response.json();
    const coaches = [];

    for (const key in data) {
      const coach = {
        id: key,
        firstName: data[key].firstName,
        lastName: data[key].lastName,
        hourlyRate: data[key].hourlyRate,
        areas: data[key].areas,
        description: data[key].description,
      };
      coaches.push(coach);
    }

    context.commit('setCoaches', coaches);
    context.commit('setFetchTimestamp');
  },
};

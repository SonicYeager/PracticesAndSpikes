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
};

export default {
  addCoach(context, coach) {
    const mapped = {
      id: context.rootGetters.userId,
      firstName: coach.first,
      lastName: coach.last,
      hourlyRate: coach.rate,
      areas: coach.areas,
      description: coach.desc,
    };
    context.commit('addCoach', mapped);
  },
};

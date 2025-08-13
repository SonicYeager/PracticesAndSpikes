export default {
  login() {},
  async signup(context, payload) {
    const response = await fetch(
      'https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyAd-ZgOvPvFN9UGD6UEcBo5nAY3imKz1D0',
      {
        method: 'POST',
        body: JSON.stringify({
          email: payload.email,
          password: payload.password,
          returnSecureToken: true,
        }),
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );
    const data = await response.json();

    if (!response.ok) {
      const error = new Error(data.error.message || 'Failed to sign up');
      throw error;
    }

    context.commit('setUser', {
      token: data.idToken,
      userId: data.localId,
      tokenExpiration: data.expiresIn,
    });
  },
};

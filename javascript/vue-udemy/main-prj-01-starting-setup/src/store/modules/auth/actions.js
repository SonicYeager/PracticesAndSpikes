import auth from '@/store/modules/auth/index';

export default {
  async logout(context) {
    context.commit('setUser', {
      token: null,
      userId: null,
      tokenExpiration: null,
    });
  },

  async login(context, payload) {
    return await auth(context, {
      ...payload,
      mode: 'login',
    });
  },

  async signup(context, payload) {
    return await auth(context, {
      ...payload,
      mode: 'signup',
    });
  },

  async tryAutoLogin(context) {
    const token = localStorage.getItem('token');
    const userId = localStorage.getItem('userId');
    const expirationDate = localStorage.getItem('tokenExpiration');

    if (token && userId && expirationDate) {
      context.commit('setUser', {
        token: token,
        userId: userId,
        tokenExpiration: expirationDate,
      });
    }
  },

  async auth(context, payload) {
    const mode = payload.mode;
    let url =
      'https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyAd-ZgOvPvFN9UGD6UEcBo5nAY3imKz1D0';

    if (mode === 'signup') {
      url =
        'https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyAd-ZgOvPvFN9UGD6UEcBo5nAY3imKz1D0';
    }

    const response = await fetch(url, {
      method: 'POST',
      body: JSON.stringify({
        email: payload.email,
        password: payload.password,
        returnSecureToken: true,
      }),
      headers: {
        'Content-Type': 'application/json',
      },
    });
    const data = await response.json();

    if (!response.ok) {
      const error = new Error(data.error.message || 'Failed to log in');
      throw error;
    }

    localStorage.setItem('token', data.idToken);
    localStorage.setItem('userId', data.localId);
    const expirationDate = new Date().getTime() + +data.expiresIn * 1000;
    localStorage.setItem('tokenExpiration', expirationDate);

    context.commit('setUser', {
      token: data.idToken,
      userId: data.localId,
      tokenExpiration: data.expiresIn,
    });
  },
};

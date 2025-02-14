import { createRouter, createWebHistory } from 'vue-router';

import TeamsList from '@/pages/teams/TeamsList.vue';
import UsersList from '@/pages/users/UsersList.vue';
import TeamMembers from '@/components/teams/TeamMembers.vue';
import TeamsFooter from '@/pages/teams/TeamsFooter.vue';
import UsersFooter from '@/pages/users/UsersFooter.vue';

const router = createRouter({
  routes: [
    { path: '/', redirect: '/teams' },
    {
      name: 'teams',
      path: '/teams',
      meta: { needsAuth: true },
      components: {
        default: TeamsList,
        footer: TeamsFooter,
      },
      children: [
        {
          name: 'team-members',
          path: ':teamId',
          component: TeamMembers,
          props: true,
        },
      ],
    },
    {
      path: '/users',
      components: {
        default: UsersList,
        footer: UsersFooter,
      },
      beforeEnter(to, from, next) {
        next();
      },
    },
    { path: '/:notFound(.*)', redirect: '/teams' },
  ],
  linkActiveClass: 'active',
  history: createWebHistory(),
  scrollBehavior(to, from, savedPostion) {
    if (savedPostion) {
      return savedPostion;
    }
    return { left: 0, top: 0 };
  },
});

router.beforeEach(function (to, from, next) {
  if (to.meta.needsAuth) {
    console.log('Needs auth!');
    next();
  }
  next();
});

router.afterEach(function (to, from) {
  // sending analytics data
  console.log(to, from);
});

export default router;

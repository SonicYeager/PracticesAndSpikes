import { createRouter, createWebHistory } from 'vue-router';
import CoachesList from '@/pages/coaches/CoachesList.vue';
import CoachDetails from '@/pages/coaches/CoachDetails.vue';
import CoachRegistration from '@/pages/coaches/CoachRegistration.vue';
import ContactCoach from '@/pages/requests/ContactCoach.vue';
import NotFound from '@/pages/NotFound.vue';

const router = createRouter(
  {
    history: createWebHistory(),
    routes: [
      { path: '/', redirect: '/coaches'},
      { path: '/coaches', component: CoachesList},
      { path: '/coaches/:id', component: CoachDetails, children: [
        { path: 'contact', component: ContactCoach},
        ]},
      { path: '/register', component: CoachRegistration},
      { path: '/requests', component: CoachRegistration},
      { path: '/:notFound(.*)', component: NotFound},
    ]
  }
);

export default router;
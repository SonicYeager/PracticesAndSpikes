import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from "@/views/DashboardView.vue";
import Preferences from "@/views/PreferencesView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/dashboard'
    },
    {
      path: '/home',
      redirect: '/dashboard'
    },
    {
      path: '/start',
      redirect: '/dashboard'
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard,
    },
    {
      path: '/preferences',
      name: 'preferences',
      component: Preferences,
    },
  ],
})

export default router

import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from "@/views/DashboardView.vue";
import Preferences from "@/views/PreferencesView.vue";
import Topics from "@/views/TopicsView.vue";

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
      path: '/topics',
      name: 'topics',
      component: () => import('../views/TopicsView.vue')
    },
    {
      path: '/settings',
      name: 'settings',
      component: () => import('../views/SettingsView.vue')
    },
    {
      path: '/preferences',
      name: 'preferences',
      component: Preferences,
    },
  ],
})

export default router

import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import Qualifications from '@/views/Qualifications.vue'
import Vessels from '@/views/Vessels.vue'
import Dashboard from '@/views/Dashboard.vue'
import Login from '@/views/Login.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      children: [
        {
          path: '/',
          name: 'dashboard',
          component: Dashboard,
        },
        {
          path: '/vessels',
          name: 'vessels',
          component: Vessels,
        },
        {
          path: '/qualifications',
          name: 'qualifications',
          component: Qualifications
        },
      ]
    },
    {
      path: '/login',
      name: 'login',
      component: Login,
    }
  ],
})

export default router

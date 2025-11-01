import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import Qualifications from '@/views/Qualifications.vue'
import VesselDashboard from '@/views/VesselDashboard.vue'
import VesselSearch from '@/views/SearchVessels.vue'
import VesselCreate from '@/views/CreateVessel.vue'
import Dashboard from '@/views/Dashboard.vue'
import Staff from '@/views/Staff.vue'
import Login from '@/views/Login.vue'
import DockSearch from '@/views/SearchDocks.vue'
import DockDashboard from '@/views/DockDashboard.vue'

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
          path: '/vessels/dashboard',
          name: 'vesselDashboard',
          component: VesselDashboard,
        },
        {
          path: '/vessels/search',
          name: 'vessels',
          component: VesselSearch
        },
        {
          path: '/vessels/create',
          name: 'createVessel',
          component: VesselCreate
        },
        {
          path: '/docks/dashboard',
          name: 'docksDashboard',
          component: DockDashboard
        },
        {
          path: '/docks/search',
          name: 'docksSearch',
          component: DockSearch
        },
        {
          path: '/qualifications',
          name: 'qualifications',
          component: Qualifications
        },
        {
          path: '/staff',
          name: 'staff',
          component: Staff
        }
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

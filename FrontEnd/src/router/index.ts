import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import Qualifications from '@/views/Qualifications.vue'
import VesselDashboard from '@/views/VesselDashboard.vue'
import VesselSearch from '@/views/VesselSearch.vue'
import VesselCreate from '@/views/VesselCreate.vue'
import VesselView from '@/views/VesselView.vue'
import Dashboard from '@/views/Dashboard.vue'
import Staff from '@/views/Staff.vue'
import Login from '@/views/Login.vue'
import DockDashboard from '@/views/DockDashboard.vue'
import DockCreate from '@/views/DockCreate.vue'
import DockSearch from '@/views/DockSearch.vue'
import VesselTypeDashboard from '@/views/VesselTypeDashboard.vue'
import VesselTypeSearch from '@/views/VesselTypeSearch.vue'

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
          path: '/vessels/view/:imo',
          name: 'viewVessel',
          component: VesselView
        },
        {
          path: '/vessels/edit/:imo',
          name: 'editVessel',
          component: () => import('@/views/VesselEdit.vue')
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
          path: '/docks/create',
          name: 'createDock',
          component: DockCreate
        },
        {
          path: '/vessel-types/dashboard',
          name: 'vesselTypesDashboard',
          component: VesselTypeDashboard
        },
        {
          path: '/vessel-types/search',
          name: 'vesselTypesSearch',
          component: VesselTypeSearch
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

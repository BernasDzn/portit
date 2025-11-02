import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import Dashboard from '@/views/Dashboard.vue'
import Login from '@/views/Login.vue'
import VesselDashboard from '@/views/Vessels/VesselDashboard.vue'
import VesselSearch from '@/views/Vessels/VesselSearch.vue'
import VesselCreate from '@/views/Vessels/VesselCreate.vue'
import VesselView from '@/views/Vessels/VesselView.vue'
import VesselEdit from '@/views/Vessels/VesselEdit.vue'
import DockDashboard from '@/views/Docks/DockDashboard.vue'
import DockSearch from '@/views/Docks/DockSearch.vue'
import DockCreate from '@/views/Docks/DockCreate.vue'
import DockView from '@/views/Docks/DockView.vue'
import VesselTypeDashboard from '@/views/VesselTypes/VesselTypeDashboard.vue'
import VesselTypeSearch from '@/views/VesselTypes/VesselTypeSearch.vue'
import QualificationDashboard from '@/views/Qualifications/QualificationDashboard.vue'
import QualificationsSearch from '@/views/Qualifications/QualificationsSearch.vue'
import QualificationView from '@/views/Qualifications/QualificationView.vue'
import StaffDashboard from '@/views/Staff/StaffDashboard.vue'
import StaffSearch from '@/views/Staff/StaffSearch.vue'
import StaffView from '@/views/Staff/StaffView.vue'
import Staff from '@/views/Staff.vue'
import QualificationCreate from '@/views/Qualifications/QualificationCreate.vue'

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
          component: VesselEdit
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
          path: '/docks/view/:code',
          name: 'viewDock',
          component: DockView
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
          path: '/qualifications/dashboard',
          name: 'qualifications',
          component: QualificationDashboard
        },
        {
            path: '/qualifications/search',
            name: 'qualificationsSearch',
            component: QualificationsSearch
        },
        {
            path: '/qualifications/view/:id',
            name: 'qualificationView',
            component: QualificationView
        },
        {
          path: '/staff/dashboard',
          name: 'staffDashboard',
          component: StaffDashboard
        },
        {
          path: '/staff/search',
          name: 'staffSearch',
          component: StaffSearch
        },
        {
          path: '/staff/view/:mechanographicNumber',
          name: 'viewStaff',
          component: StaffView
        },
        {
            path: '/qualifications/create',
            name: 'qualificationCreate',
            component: QualificationCreate
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

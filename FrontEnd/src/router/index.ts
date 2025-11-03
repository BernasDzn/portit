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
import VesselTypeView from '@/views/VesselTypes/VesselTypeView.vue'
import VesselTypeCreate from '@/views/VesselTypes/VesselTypeCreate.vue'
import QualificationDashboard from '@/views/Qualifications/QualificationDashboard.vue'
import QualificationsSearch from '@/views/Qualifications/QualificationsSearch.vue'
import QualificationView from '@/views/Qualifications/QualificationView.vue'
import StaffDashboard from '@/views/Staff/StaffDashboard.vue'
import StaffSearch from '@/views/Staff/StaffSearch.vue'
import StaffView from '@/views/Staff/StaffView.vue'
import QualificationCreate from '@/views/Qualifications/QualificationCreate.vue'
import Unauthorized from '@/views/Unauthorized.vue'
import StaffCreate from '@/views/Staff/StaffCreate.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: HomeView,
      children: [
        {
          path: '/',
          name: 'Main Dashboard',
          component: Dashboard,
          meta: {
            icon: "directions_boat"
          }
        },
        {
            path: '/vessels/dashboard',
            name: 'Vessel Dashboard',
            component: VesselDashboard,
            meta: {
                    icon: "directions_boat"
            }
        },
        {
            path: '/vessels/search',
            name: 'Search for vessels',
            component: VesselSearch,
            meta: {
                icon: "search"
            }
        },
        {
            path: '/vessels/create',
            name: 'Create a Vessel',
            component: VesselCreate,
            meta: {
                icon: "add"
            }
        },
        {
          path: '/vessels/view/:imo',
          component: VesselView
        },
        {
          path: '/vessels/edit/:imo',
          component: VesselEdit
        },
        {
            path: '/docks/dashboard',
            name: 'Dock Dashboard',
            component: DockDashboard,
            meta: {
                icon: "anchor"
            }
        },
        {
            path: '/docks/search',
            name: 'Search for Docks',
            component: DockSearch,
            meta: {
                icon: "search"
            }
        },
        {
          path: '/docks/view/:code',
          component: DockView
        },
        {
            path: '/docks/create',
            name: 'Create a Dock',
            component: DockCreate,
            meta: {
                icon: "add"
            }
        },
        {
            path: '/vessel-types/dashboard',
            name: 'Vessel Types Dashboard',
            component: VesselTypeDashboard,
            meta: {
                icon: "sailing"
            }
        },
        {
          path: '/vessel-types/search',
          name: 'Search for vessel types',
          component: VesselTypeSearch,
            meta: {
                icon: "search"
            }
        },
        {
          path: '/vessel-types/view/:name',
          component: VesselTypeView
        },
        {
          path: '/vessel-types/create',
          name: 'Create a Vessel Type',
          component: VesselTypeCreate,
            meta: {
                icon: "add"
            }
        },
        {
          path: '/qualifications/dashboard',
          name: 'Qualifications Dashboard',
          component: QualificationDashboard,
            meta: {
                icon: "school"
            }
        },
        {
            path: '/qualifications/search',
            name: 'Search for qualifications',
            component: QualificationsSearch,
            meta: {
                icon: "search"
            }
        },
        {
            path: '/qualifications/view/:id',
            component: QualificationView
        },
        {
          path: '/staff/dashboard',
          name: 'Staff Dashboard',
          component: StaffDashboard,
            meta: {
                icon: "people"
            }
        },
        {
          path: '/staff/search',
          name: 'Search for staff',
          component: StaffSearch,
            meta: {
                icon: "search"
            }
        },
        {
          path: '/staff/view/:mechanographicNumber',
          component: StaffView
        },
        {
          path: '/staff/create',
          name: 'createStaff',
          component: StaffCreate
        },
        {
            path: '/qualifications/create',
            name: 'Create a Qualification',
            component: QualificationCreate,
            meta: {
                icon: "add"
            }
        }
      ]
    },
    {
        path: '/login',
        name: 'login',
        meta: { hideFromSearch: true },
        component: Login,
    },
    {
        path: '/unauthorized',
        name: 'unauthorized',
        meta: { hideFromSearch: true },
        component: Unauthorized
    }
  ],
})

export default router

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
import DockEdit from '@/views/Docks/DockEdit.vue'
import VesselTypeDashboard from '@/views/VesselTypes/VesselTypeDashboard.vue'
import VesselTypeSearch from '@/views/VesselTypes/VesselTypeSearch.vue'
import VesselTypeView from '@/views/VesselTypes/VesselTypeView.vue'
import VesselTypeCreate from '@/views/VesselTypes/VesselTypeCreate.vue'
import VesselTypeEdit from '@/views/VesselTypes/VesselTypeEdit.vue'
import QualificationDashboard from '@/views/Qualifications/QualificationDashboard.vue'
import QualificationsSearch from '@/views/Qualifications/QualificationsSearch.vue'
import QualificationView from '@/views/Qualifications/QualificationView.vue'
import StaffDashboard from '@/views/Staff/StaffDashboard.vue'
import StaffSearch from '@/views/Staff/StaffSearch.vue'
import StaffView from '@/views/Staff/StaffView.vue'
import QualificationCreate from '@/views/Qualifications/QualificationCreate.vue'
import Unauthorized from '@/views/Unauthorized.vue'
import StaffCreate from '@/views/Staff/StaffCreate.vue'
import QualificationEdit from '@/views/Qualifications/QualificationEdit.vue'
import PhysicalResourceDashboard from '@/views/PhysicalResources/PhysicalResourceDashboard.vue'
import PhysicalResourcesSearch from '@/views/PhysicalResources/PhysicalResourcesSearch.vue'
import PhysicalResourceViewer from '@/views/PhysicalResources/PhysicalResourceViewer.vue'
import PhysicalResourceCreate from '@/views/PhysicalResources/PhysicalResourceCreate.vue'
import StorageAreaDashboard from '@/views/StorageAreas/StorageAreaDashboard.vue'
import StorageAreaSearch from '@/views/StorageAreas/StorageAreaSearch.vue'
import StorageAreaView from '@/views/StorageAreas/StorageAreaView.vue'
import StorageAreaCreate from '@/views/StorageAreas/StorageAreaCreate.vue'
import StorageAreaEdit from '@/views/StorageAreas/StorageAreaEdit.vue'
import AdminDashboard from '@/views/Admin/AdminDashboard.vue'
import UserDashboard from '@/views/Admin/UserDashboard.vue'
import UserSearch from '@/views/Admin/UserSearch.vue'
import UserCreate from '@/views/Admin/UserCreate.vue'
import UserView from '@/views/Admin/UserView.vue'
import Visualizer from '@/views/Visualizer.vue'
import StaffEdit from '@/views/Staff/StaffEdit.vue'
import VVNDashboard from '@/views/VesselVisitNotification/VesselVisitNotificationDashboard.vue'
import VVNSearch from '@/views/VesselVisitNotification/VesselVisitNotificationSearch.vue'
import VVNView from '@/views/VesselVisitNotification/VesselVisitNotificationView.vue'

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
          path: '/docks/edit/:code',
          component: DockEdit
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
          path: '/vessel-types/edit/:name',
          component: VesselTypeEdit
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
          component: StaffCreate
        },
        {
          path: '/staff/edit/:id',
          component: StaffEdit
        },
        {
          path: '/qualifications/create',
          name: 'Create a Qualification',
          component: QualificationCreate,
          meta: {
            icon: "add"
          }
        },
        {
            path: '/qualifications/edit/:id',
            component: QualificationEdit
        },
        {
            path: '/resources/dashboard',
            name: 'Physical Resources Dashboard',
            component: PhysicalResourceDashboard,
            meta: {
                icon: "build"
            }
        },
        {
            path: '/resources/search',
            name: 'Search for physical resources',
            component: PhysicalResourcesSearch,
            meta: {
                icon: "search"
            }
        },
        {
            path: '/resources/view/:code',
            component: PhysicalResourceViewer
        },
        {
            path: '/resources/create',
            name: 'Create a Physical Resource',
            component: PhysicalResourceCreate,
            meta: {
                icon: "add"
            }
        },
        {
          path: '/storage-areas/dashboard',
          name: 'Storage Areas Dashboard',
          component: StorageAreaDashboard,
          meta: {
            icon: "warehouse"
          }
        },
        {
          path: '/storage-areas/search',
          name: 'Search for storage areas',
          component: StorageAreaSearch,
          meta: {
            icon: "search"
          }
        },
        {
          path: '/storage-areas/view/:nameCode',
          component: StorageAreaView
        },
        {
          path: '/storage-areas/create',
          name: 'Create a Storage Area',
          component: StorageAreaCreate,
          meta: {
            icon: "add"
          }
        },
        {
          path: '/storage-areas/edit/:name',
          component: StorageAreaEdit
        },
        {
          path: '/vessel-visit-notifications/dashboard',
          name: 'VVN Dashboard',
          component: VVNDashboard,
          meta: {
            icon: "notifications"
          }
        },
        {
          path: '/vessel-visit-notifications/search',
          name: 'VVN Search',
          component: VVNSearch,
          meta: {
            icon: "search"
          }
        },
        {
          path : '/vessel-visit-notifications/view/:notificationId',
          component: VVNView
        },
        {
          path: '/admin/dashboard',
          name: 'Admin Dashboard',
          component: AdminDashboard,
          meta: {
            icon: "admin_panel_settings"
          }
        },
        {
          path: '/admin/users',
          name: 'User Management',
          component: UserDashboard,
          meta: {
            icon: "manage_accounts"
          }
        },
        {
          path: '/admin/users/search',
          name: 'Search for users',
          component: UserSearch,
          meta: {
            icon: "search"
          }
        },
        {
          path: '/admin/users/create',
          name: 'Create User',
          component: UserCreate,
          meta: {
            icon: "person_add"
          }
        },
        {
          path: '/admin/users/view/:emailAddress',
          name: 'User View',
          component: UserView,
          meta: { 
            icon: "visibility"
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
        path: '/visualization',
        name: 'Port 3D',
        component: Visualizer,
        meta: {
            icon: "view_in_ar"
        }
    },
    {
        path: '/activate',
        name: 'activate',
        meta: { hideFromSearch: true },
        component: () => import('@/views/Activate.vue')
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

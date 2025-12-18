import { createRouter, createWebHistory } from 'vue-router'
import { useSession } from '@/composables/session'
import Activate from '@/views/Activate.vue'

// Only eagerly load critical components (layout and auth)
import HomeView from '../views/HomeView.vue'
import Login from '@/views/Login.vue'
import Unauthorized from '@/views/Unauthorized.vue'

// All other components are lazy-loaded when their route is accessed
// This dramatically reduces initial bundle size and improves load time

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
          component: () => import('@/views/Dashboard.vue'),
          meta: {
            icon: "directions_boat"
          }
        },
        {
          path: '/vessels/dashboard',
          name: 'Vessel Dashboard',
          component: () => import('@/views/Vessels/VesselDashboard.vue'),
          meta: {
            icon: "directions_boat"
          }
        },
        {
          path: '/vessels/search',
          name: 'Search for Vessels',
          component: () => import('@/views/Vessels/VesselSearch.vue'),
          meta: {
            icon: "search"
          }
        },
        {
          path: '/vessels/create',
          name: 'Create a Vessel',
          component: () => import('@/views/Vessels/VesselCreate.vue'),
          meta: {
            icon: "add"
          }
        },
        {
          path: '/vessels/view/:imo',
          component: () => import('@/views/Vessels/VesselView.vue')
        },
        {
          path: '/vessels/edit/:imo',
          component: () => import('@/views/Vessels/VesselEdit.vue')
        },
        {
          path: '/docks/dashboard',
          name: 'Dock Dashboard',
          component: () => import('@/views/Docks/DockDashboard.vue'),
          meta: {
            icon: "anchor"
          }
        },
        {
          path: '/docks/search',
          name: 'Search for Docks',
          component: () => import('@/views/Docks/DockSearch.vue'),
          meta: {
            icon: "search"
          }
        },
        {
          path: '/docks/view/:code',
          component: () => import('@/views/Docks/DockView.vue')
        },
        {
          path: '/docks/create',
          name: 'Create a Dock',
          component: () => import('@/views/Docks/DockCreate.vue'),
          meta: {
            icon: "add"
          }
        },
        {
          path: '/docks/edit/:code',
          component: () => import('@/views/Docks/DockEdit.vue')
        },
        {
          path: '/vessel-types/dashboard',
          name: 'Vessel Types Dashboard',
          component: () => import('@/views/VesselTypes/VesselTypeDashboard.vue'),
          meta: {
            icon: "sailing"
          }
        },
        {
          path: '/vessel-types/search',
          name: 'Search for Vessel Types',
          component: () => import('@/views/VesselTypes/VesselTypeSearch.vue'),
          meta: {
            icon: "search"
          }
        },
        {
          path: '/vessel-types/view/:name',
          component: () => import('@/views/VesselTypes/VesselTypeView.vue')
        },
        {
          path: '/vessel-types/create',
          name: 'Create a Vessel Type',
          component: () => import('@/views/VesselTypes/VesselTypeCreate.vue'),
          meta: {
            icon: "add"
          }
        },
        {
          path: '/vessel-types/edit/:name',
          component: () => import('@/views/VesselTypes/VesselTypeEdit.vue')
        },
        {
          path: '/qualifications/dashboard',
          name: 'Qualifications Dashboard',
          component: () => import('@/views/Qualifications/QualificationDashboard.vue'),
          meta: {
            icon: "school"
          }
        },
        {
          path: '/qualifications/search',
          name: 'Search for Qualifications',
          component: () => import('@/views/Qualifications/QualificationsSearch.vue'),
          meta: {
            icon: "search"
          }
        },
        {
          path: '/qualifications/view/:id',
          component: () => import('@/views/Qualifications/QualificationView.vue')
        },
        {
          path: '/staff/dashboard',
          name: 'Staff Dashboard',
          component: () => import('@/views/Staff/StaffDashboard.vue'),
          meta: {
            icon: "people"
          }
        },
        {
          path: '/staff/search',
          name: 'Search for Staff',
          component: () => import('@/views/Staff/StaffSearch.vue'),
          meta: {
            icon: "search"
          }
        },
        {
          path: '/staff/view/:mechanographicNumber',
          component: () => import('@/views/Staff/StaffView.vue')
        },
        {
          path: '/staff/create',
          component: () => import('@/views/Staff/StaffCreate.vue')
        },
        {
          path: '/staff/edit/:id',
          component: () => import('@/views/Staff/StaffEdit.vue')
        },
        {
          path: '/qualifications/create',
          name: 'Create a Qualification',
          component: () => import('@/views/Qualifications/QualificationCreate.vue'),
          meta: {
            icon: "add"
          }
        },
        {
            path: '/qualifications/edit/:id',
            component: () => import('@/views/Qualifications/QualificationEdit.vue')
        },
        {
            path: '/resources/dashboard',
            name: 'Physical Resources Dashboard',
            component: () => import('@/views/PhysicalResources/PhysicalResourceDashboard.vue'),
            meta: {
                icon: "build"
            }
        },
        {
            path: '/resources/search',
            name: 'Search for Physical Resources',
            component: () => import('@/views/PhysicalResources/PhysicalResourcesSearch.vue'),
            meta: {
                icon: "search"
            }
        },
        {
            path: '/resources/view/:code',
            component: () => import('@/views/PhysicalResources/PhysicalResourceViewer.vue')
        },
        {
            path: '/resources/create',
            name: 'Create a Physical Resource',
            component: () => import('@/views/PhysicalResources/PhysicalResourceCreate.vue'),
            meta: {
                icon: "add"
            }
        },
        {
          path: '/storage-areas/dashboard',
          name: 'Storage Areas Dashboard',
          component: () => import('@/views/StorageAreas/StorageAreaDashboard.vue'),
          meta: {
            icon: "warehouse"
          }
        },
        {
          path: '/storage-areas/search',
          name: 'Search for Storage Areas',
          component: () => import('@/views/StorageAreas/StorageAreaSearch.vue'),
          meta: {
            icon: "search"
          }
        },
        {
          path: '/storage-areas/view/:nameCode',
          component: () => import('@/views/StorageAreas/StorageAreaView.vue')
        },
        {
          path: '/storage-areas/create',
          name: 'Create a Storage Area',
          component: () => import('@/views/StorageAreas/StorageAreaCreate.vue'),
          meta: {
            icon: "add"
          }
        },
        {
          path: '/storage-areas/edit/:name',
          component: () => import('@/views/StorageAreas/StorageAreaEdit.vue')
        },
        {
          path: '/vessel-visit-notifications/dashboard',
          name: 'VVN Dashboard',
          component: () => import('@/views/VesselVisitNotification/VesselVisitNotificationDashboard.vue'),
          meta: {
            icon: "notifications"
          }
        },
        {
          path: '/vessel-visit-notifications/search',
          name: 'Search for VVNs',
          component: () => import('@/views/VesselVisitNotification/VesselVisitNotificationSearch.vue'),
          meta: {
            icon: "search"
          }
        },
        {
          path : '/vessel-visit-notifications/view/:notificationId',
          component: () => import('@/views/VesselVisitNotification/VesselVisitNotificationView.vue')
        },
        {
          path : '/vessel-visit-notifications/pending',
          component: () => import('@/views/VesselVisitNotification/VesselVisitNotificationPendingList.vue')
        },
        {
          path : '/vessel-visit-notifications/review/:notificationId',
          component: () => import('@/views/VesselVisitNotification/VesselVisitNotificationReview.vue')
        },
        {
          path : '/vessel-visit-notifications/edit/:id',
          component: () => import('@/views/VesselVisitNotification/VesselVisitNotificationEdit.vue')
        },
        {
          path: '/admin/dashboard',
          name: 'Admin Dashboard',
          component: () => import('@/views/Admin/AdminDashboard.vue'),
          meta: {
            icon: "admin_panel_settings"
          }
        },
        {
          path: '/admin/users',
          name: 'User Management',
          component: () => import('@/views/Admin/UserDashboard.vue'),
          meta: {
            icon: "manage_accounts"
          }
        },
        {
          path: '/admin/users/search',
          name: 'Search for Users',
          component: () => import('@/views/Admin/UserSearch.vue'),
          meta: {
            icon: "search"
          }
        },
        {
          path: '/admin/users/create',
          name: 'Create a User',
          component: () => import('@/views/Admin/UserCreate.vue'),
          meta: {
            icon: "add"
          }
        },
        {
          path: '/admin/users/view/:emailAddress',
          component: () => import('@/views/Admin/UserView.vue'),
          meta: { 
            icon: "visibility"
          }
        },
        {
            path: '/resources/edit/:code',
            component: () => import('@/views/PhysicalResources/PhysicalResourceEdit.vue')
        },
        {
            path: '/admin/audit-logs',
            name: 'Audit Logs',
            component: () => import('@/views/Admin/AuditLogs.vue'),
            meta: {
              icon: "history"
            }
        },
        {
            path: '/admin/privacy-policy',
            name: 'Privacy Policy',
            component: () => import('@/views/Admin/PrivacyPolicy.vue'),
            meta: {
                icon: "privacy_tip"
            }
        },
        {
            path: '/visualization',
            name: 'Port 3D',
            component: () => import('@/views/Visualizer.vue'),
            meta: {
                icon: "view_in_ar"
            }
        },
        {
            path: '/scheduling-dashboard',
            name: 'Scheduling Dashboard',
            component: () => import('@/views/Scheduling/SchedulingDashboard.vue'),
            meta: {
                icon: "calendar_month"
            }
        },
        {
          path: '/scheduling/schedule',
          name: 'Schedule',
          component: () => import('@/views/Scheduling/SchedulingRequest.vue'),
          meta: {
            icon: "calendar_month"
          }
        },
        {
            path: '/scheduling/results',
            name: 'ScheduleResults',
            component: () => import('@/views/Scheduling/ScheduleResults.vue'),
            meta: {
                hideFromSearch: true
            }
        },        
        {
            path: '/vessel-visit-notifications/create',
            name: 'Create Vessel Visit Notification',
            component: () => import('@/views/VesselVisitNotification/VesselVisitNotificationCreate.vue'),
            meta: {
                icon: "add"
            }
        },
        {
            path: '/vessel-visit-notifications/update/:id',
            name: 'Update Vessel Visit Notification',
            component: () => import('@/views/VesselVisitNotification/VesselVisitNotificationEdit.vue'),
            meta: {
                icon: "edit"
            }
        },
        {
            path: 'admin/notifications',
            name: 'System Notifications',
            component: () => import('@/views/Admin/Notifications.vue'),
            meta: {
                icon: "notifications"
            }
        },
        {
            path: '/scheduling/queue',
            name: 'Scheduling Queue',
            component: () => import('@/views/Scheduling/RequestQueue.vue'),
            meta: {
                icon: "queue"
            }
        },
        {
            path: '/incident-types/dashboard',
            name: 'Incident Types Dashboard',
            component: () => import('@/views/IncidentTypes/IncidentTypeDashboard.vue'),
            meta: {
                icon: "dashboard"
            }
        },
        {
            path: '/incident-types/search',
            name: 'Search Incident Types',
            component: () => import('@/views/IncidentTypes/IncidentTypeSearch.vue'),
            meta: {
                icon: "search"
            }
        },
        {
            path: '/incident-types/view/:id',
            name: 'View Incident Type',
            component: () => import('@/views/IncidentTypes/IncidentTypeView.vue'),
            meta: {
                icon: "eye"
            }
        },
        {
            path: '/incident-types/create',
            name: 'Create Incident Type',
            component: () => import('@/views/IncidentTypes/IncidentTypeCreate.vue'),
            meta: {
                icon: "add"
            }
        },
        {
            path: '/incident-types/edit/:id',
            name: 'Edit Incident Type',
            component: () => import('@/views/IncidentTypes/IncidentTypeEdit.vue'),
            meta: {
                icon: "pencil"
            }
        },
        {
          path: '/task-categories/dashboard',
          name: 'Task Categories Dashboard',
          component: () => import('@/views/TaskCategories/TaskCategoryDashboard.vue'),  
          meta: { 
            icon: "category"
          }
        },
        {
          path: '/task-categories/search',
          name: 'Search Task Categories',
          component: () => import('@/views/TaskCategories/TaskCategorySearch.vue'),
          meta: { 
            icon: "search"
          }
        },
        {path: '/task-categories/view/:id',
          name: 'View Task Category',
          component: () => import('@/views/TaskCategories/TaskCategoryView.vue')
        },
        {
          path: '/about',
          name: 'about',
          component: () => import('@/views/About/About.vue'),
          meta: {
            icon: "info"
          }
        },
        {
          path: '/my-data',
          name: 'My Data & Privacy',
          component: () => import('@/views/DataRightsView.vue'),
          meta: {
            icon: "shield_lock"
          }
        },
        {
            path: '/scheduling/plans-search',
            name: 'Operation Plans Search',
            component: () => import('@/views/Scheduling/OperationPlansSearch.vue'),
            meta: {
                icon: "search"
            }
        },
        {
            path: '/scheduling/plans-view/:id',
            name: 'View Operation Plan',
            component: () => import('@/views/Scheduling/OperationPlanView.vue'),
        },
        {
            path: '/scheduling/plans-edit/:id',
            name: 'Edit Operation Plan',
            component: () => import('@/views/Scheduling/OperationPlanEdit.vue'),
        }
      ]
    },
    {
      path: '/privacy-policy-print',
      name: 'Privacy Policy Print',
      component: () => import('@/views/PrivacyPolicyPrint.vue')
    },
    {
      path: '/login',
      name: 'login',
      meta: { hideFromSearch: true },
      component: Login,
    },
    {
        path: '/activate',
        name: 'activate',
        meta: { hideFromSearch: true },
        component: Activate
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

// Execute before each route change, also called "navigation guard"
// see https://router.vuejs.org/guide/advanced/navigation-guards.html
router.beforeEach((to, from, next) => {
  const session = useSession();
  const role = session.authenticatedUser?.role ?? -1;

  // Public routes that anyone (including unauthenticated users) can access
  const publicPaths = ['/login', '/activate', '/unauthorized', '/'];
  if (publicPaths.includes(to.path)) {
    return next();
  }

  console.log(`Navigating to ${to.path} with role ${role}`);

  // (0=Administrator, 1=PortAuthorityOfficer, 2=SAORepresentative, 3=LogisticsOperator)
  // Map route prefixes to allowed numeric roles, this should probably be put in a config file
  // but we can keep it here for simplicity. Probably not very scalable but OK for this sprint?
  const routeRoleMap: Array<{ prefix: string; roles: number[] }> = [
    { prefix: '/vessels', roles: [0, 1] },
    { prefix: '/vessel-types', roles: [0, 1] },
    { prefix: '/docks', roles: [0, 1] },
    { prefix: '/vessel-visit-notifications', roles: [0, 1, 2] },
    { prefix: '/qualifications', roles: [0, 3] },
    { prefix: '/resources', roles: [0, 3] },
    { prefix: '/staff', roles: [0, 3] },
    { prefix: '/storage-areas', roles: [0, 1] },
    { prefix: '/admin', roles: [0] },
    { prefix: '/scheduling', roles: [0,3] },
    { prefix: '/about', roles: [0, 1, 2, 3] },
    { prefix: '/my-data', roles: [0, 1, 2, 3] }
  ];

  for (const entry of routeRoleMap) {
    if (to.path.startsWith(entry.prefix)) {
      // Here we check the role against the allowed roles for the route
      // if the role is not allowed, redirect to unauthorized.
      // Prolly should make the unauthorized route configurable using a const???
      if (role < 0 || !entry.roles.includes(role)) {

        console.log(`Access denied to ${to.path} for role ${role}`);

        if (to.path === '/unauthorized') return next();
        return next({ path: '/unauthorized' });
      }
      return next();
    }
  }

  // If route is not listed, require admin by default (matches backend AdminOnly fallback)
  if (role !== 0) {
    if (to.path === '/unauthorized') return next();
    return next({ path: '/unauthorized' });
  }
  return next();
});

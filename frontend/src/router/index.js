import { createRouter, createWebHistory } from 'vue-router'

import UsersView from '../views/UsersView.vue'
import BankAccountsView from '../views/BankAccountsView.vue'
import TransactionsView from '../views/TransactionsView.vue'
import TaxSummaryView from '../views/TaxSummaryView.vue'
import TaxCategoriesView from '../views/TaxCategoriesView.vue'
import LoginView from '../views/LoginView.vue'

const router = createRouter({
  history: createWebHistory(),

  routes: [
    {
      path: '/',
      redirect: '/users',
    },

    {
      path: '/users',
      name: 'users',
      component: UsersView,
    },

    {
      path: '/accounts',
      name: 'accounts',
      component: BankAccountsView,
    },

    {
      path: '/transactions',
      name: 'transactions',
      component: TransactionsView,
    },

    {
      path: '/tax-summary',
      name: 'tax-summary',
      component: TaxSummaryView,
    },

    {
      path: '/tax-categories',
      name: 'tax-categories',
      component: TaxCategoriesView,
    },

    {
      path: '/login',
      name: 'login',
      component: LoginView,
    },
  ],
})


// Route Protection
router.beforeEach((to) => {
  const isLoggedIn =
    localStorage.getItem('taxoraAuth') !== null

  // Login page can always be opened
  if (to.path === '/login') {
    return true
  }

  // If not logged in, go to Login page
  if (!isLoggedIn) {
    return '/login'
  }

  // User is logged in
  return true
})


export default router
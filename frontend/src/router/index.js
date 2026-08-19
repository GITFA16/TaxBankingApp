import { createRouter, createWebHistory } from 'vue-router'

import UsersView from '../views/UsersView.vue'
import BankAccountsView from '../views/BankAccountsView.vue'
import TransactionsView from '../views/TransactionsView.vue'
import TaxSummaryView from '../views/TaxSummaryView.vue'
import TaxCategoriesView from '../views/TaxCategoriesView.vue'

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
    
  ],
})

export default router
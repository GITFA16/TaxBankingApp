import { createRouter, createWebHistory } from 'vue-router'

import UsersView from '../views/UsersView.vue'
import BankAccountsView from '../views/BankAccountsView.vue'

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
  ],
})

export default router
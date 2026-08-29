<script setup>
import { ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import taxoraWordmark from './assets/taxora_wordmark.png'
import taxoraIcon from './assets/taxora_icon.png'

const router = useRouter()
const route = useRoute()

// Check if login information exists
const isLoggedIn = ref(
  localStorage.getItem('taxoraAuth') !== null
)

// Check login status whenever the route changes
watch(
  () => route.path,
  () => {
    isLoggedIn.value =
      localStorage.getItem('taxoraAuth') !== null
  }
)

// Logout
function logout() {
  // Remove saved Basic Authentication
  localStorage.removeItem('taxoraAuth')

  // Update login status
  isLoggedIn.value = false

  // Go back to Login page
  router.push('/login')
}
</script>


<template>
  <v-app>

    <!-- LEFT NAVIGATION MENU -->
    <v-navigation-drawer
      v-if="isLoggedIn"
    >
      <v-list>

        <!-- Application name -->
        <v-list-item>
          <v-img
            :src="taxoraWordmark"
            alt="Taxora"
            width="180"
            height="48"
            contain
          />
        </v-list-item>

        <v-divider />

        <!-- Users -->
        <v-list-item
          title="Users"
          to="/users"
        />

        <!-- Bank Accounts -->
        <v-list-item
          title="Bank Accounts"
          to="/accounts"
        />

        <!-- Transactions -->
        <v-list-item
          title="Transactions"
          to="/transactions"
        />

        <!-- Tax Summary -->
        <v-list-item
          title="Tax Summary"
          to="/tax-summary"
        />

        <!-- Tax Categories -->
        <v-list-item
          title="Tax Categories"
          to="/tax-categories"
        />

      </v-list>
    </v-navigation-drawer>


    <!-- TOP BAR -->
    <v-app-bar
      v-if="isLoggedIn"
    >
      <v-app-bar-title>
        <v-img
          :src="taxoraIcon"
          alt="Taxora Icon"
          width="120"
          height="40"
          contain
        />
      </v-app-bar-title>

      <v-spacer />

      <!-- Logout Button -->
      <v-btn
        @click="logout"
      >
        Logout
      </v-btn>

    </v-app-bar>


    <!-- CURRENT PAGE -->
    <v-main>
      <router-view />
    </v-main>

  </v-app>
</template>
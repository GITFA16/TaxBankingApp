<script setup>
import taxoraWordmark from './assets/taxora_wordmark.png'
import taxoraIcon from './assets/taxora_icon.png'
</script>

<template>
  <v-app>

    <!-- LEFT NAVIGATION MENU -->
    <v-navigation-drawer>
      <v-list>

        <!-- Application name -->
        <v-list-item>
          <v-img
            :src="taxoraWordmark"
            alt="Taxora"
            width="120"
            height="40"
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

      </v-list>
    </v-navigation-drawer>


    <!-- TOP BAR -->
    <v-app-bar>
      <v-app-bar-title>
        <v-img
          :src="taxoraIcon"
          alt="Taxora Icon"
          width="120"
          height="40"
          contain
        />
      </v-app-bar-title>
    </v-app-bar>

    <!-- CURRENT PAGE -->
    <v-main>
      <router-view />
    </v-main>

  </v-app>
</template>
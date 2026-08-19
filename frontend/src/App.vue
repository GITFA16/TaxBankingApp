<template>
  <v-app>
    <v-app-bar>
      <v-app-bar-title>
        Taxora
      </v-app-bar-title>
    </v-app-bar>

    <v-main>
       <!-- The active page from Vue Router will be displayed here -->
      <router-view />
    </v-main>
  </v-app>
</template>
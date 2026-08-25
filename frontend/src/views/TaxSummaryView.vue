<script setup>
import { computed, onMounted, ref } from 'vue'

// Stores all users for the dropdown
const users = ref([])

// Stores the selected User ID internally
const selectedUserId = ref(null)

// Stores the tax summary returned by the backend
const taxSummary = ref({})

// Shows whether a request is currently running
const loading = ref(false)


// USER OPTIONS
// Creates a Full Name for every user
// Example:
// id: 1
// fullName: Faizal Alamudi
const userOptions = computed(() => {
  return users.value.map(user => ({
    id: user.id,
    fullName: `${user.firstName} ${user.lastName}`,
  }))
})


// LOAD ALL USERS
async function loadUsers() {
  // Frontend sends a GET request to load all users
  const response = await fetch(
    'http://localhost:5106/api/users',
  )

  // Store users if the request was successful
  if (response.ok) {
    users.value = await response.json()
  }
}


// LOAD TAX SUMMARY
async function loadTaxSummary() {
  // Do nothing if no user was selected
  if (selectedUserId.value === null) {
    return
  }

  // Show loading animation
  loading.value = true

  // Frontend sends a GET request for the selected user
  const response = await fetch(
    `http://localhost:5106/api/users/${selectedUserId.value}/tax-summary`,
  )

  // Store the tax summary returned by the backend
  if (response.ok) {
    taxSummary.value = await response.json()
  }

  // Stop loading animation
  loading.value = false
}


// VUE LIFECYCLE
// Load all users when this page is opened
onMounted(() => {
  loadUsers()
})
</script>


<template>
  <v-container>
    <h1>Tax Summary</h1>


    <!-- USER SELECTION -->
    <v-card class="mb-6">
      <v-card-title>
        Load Tax Summary
      </v-card-title>

      <v-card-text>

        <!-- Select User by Full Name -->
        <v-select
          v-model="selectedUserId"
          :items="userOptions"
          item-title="fullName"
          item-value="id"
          label="User"
        />

      </v-card-text>

      <v-card-actions>

        <!-- Load the tax summary for the selected user -->
        <v-btn
          color="primary"
          :loading="loading"
          @click="loadTaxSummary"
        >
          Load Tax Summary
        </v-btn>

      </v-card-actions>
    </v-card>


    <!-- TAX SUMMARY RESULTS -->
    <v-card>
      <v-card-title>
        Tax Relevant Categories
      </v-card-title>

      <v-card-text>

        <!-- Create one row for every tax category -->
        <div
          v-for="(total, category) in taxSummary"
          :key="category"
          class="mb-3"
        >
          <strong>
            {{ category }}
          </strong>

          <div>
            CHF {{ total }}
          </div>
        </div>

        <!-- Show this message when no tax summary exists -->
        <div v-if="Object.keys(taxSummary).length === 0">
          No tax-relevant transactions found.
        </div>

      </v-card-text>
    </v-card>

  </v-container>
</template>
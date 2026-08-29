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


// BASIC AUTH HEADER
function getAuthHeader() {
  const auth = localStorage.getItem('taxoraAuth')

  return {
    Authorization: `Basic ${auth}`,
  }
}


// USER OPTIONS
// Creates a Full Name for every user
const userOptions = computed(() => {
  return users.value.map(user => ({
    id: user.id,
    fullName: `${user.firstName} ${user.lastName}`,
  }))
})


// LOAD ALL USERS
async function loadUsers() {
  const response = await fetch(
    'https://localhost:7131/api/users',
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    users.value = await response.json()
  }
}


// LOAD TAX SUMMARY
async function loadTaxSummary() {
  if (selectedUserId.value === null) {
    return
  }

  loading.value = true

  const response = await fetch(
    `https://localhost:7131/api/users/${selectedUserId.value}/tax-summary`,
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    taxSummary.value = await response.json()
  }

  loading.value = false
}


// VUE LIFECYCLE
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
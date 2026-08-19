<script setup>
import { ref } from 'vue'

// User ID used to request the tax summary
const userId = ref(null)

// Stores the tax summary returned by the backend
const taxSummary = ref({})

// Shows whether a request is currently running
const loading = ref(false)


// LOAD TAX SUMMARY
async function loadTaxSummary() {
  loading.value = true

  // Frontend sends a GET request to the backend API
  const response = await fetch(
    `http://localhost:5106/api/users/${userId.value}/tax-summary`,
  )

  if (response.ok) {
    taxSummary.value = await response.json()
  }

  loading.value = false
}
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
        <!-- User ID -->
        <v-text-field
          v-model.number="userId"
          label="User ID"
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
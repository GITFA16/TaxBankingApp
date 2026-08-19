<script setup>
import { onMounted, ref } from 'vue'

// Stores all standard tax categories
const categories = ref([])


// READ TAX CATEGORIES
async function loadTaxCategories() {
  // Frontend sends a GET request to the backend API
  const response = await fetch(
    'http://localhost:5106/api/taxcategories',
  )

  // Store the categories if the request was successful
  if (response.ok) {
    categories.value = await response.json()
  }
}


// VUE LIFECYCLE
// Load all tax categories when this component is mounted
onMounted(() => {
  loadTaxCategories()
})
</script>


<template>
  <v-container>
    <h1>Tax Categories</h1>

    <!-- STANDARD TAX CATEGORIES -->
    <v-card
      v-for="category in categories"
      :key="category.id"
      class="mb-4"
    >
      <v-card-title>
        {{ category.name }}
      </v-card-title>

      <v-card-text>
        <div>
          ID: {{ category.id }}
        </div>

        <div>
          {{ category.description }}
        </div>
      </v-card-text>
    </v-card>

  </v-container>
</template>
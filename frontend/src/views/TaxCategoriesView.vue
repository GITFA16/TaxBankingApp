<script setup>
import { onMounted, ref } from 'vue'

// Stores all tax categories
const taxCategories = ref([])

// Create Tax Category form
const name = ref('')
const description = ref('')

// Edit Tax Category form
const editingCategoryId = ref(null)
const editName = ref('')
const editDescription = ref('')

// Basic Authentication header
function getAuthHeader() {
  const auth = localStorage.getItem('taxoraAuth')

  return {
    Authorization: `Basic ${auth}`,
  }
}

// Load all tax categories
async function loadTaxCategories() {
  const response = await fetch(
    'https://localhost:5106/api/taxcategories',
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    taxCategories.value = await response.json()
  }
}

// Create a new tax category
async function createTaxCategory() {
  // Do nothing if Name is empty
  if (name.value === '') {
    return
  }

  const response = await fetch(
    'https://localhost:5106/api/taxcategories',
    {
      method: 'POST',

      headers: {
        ...getAuthHeader(),
        'Content-Type': 'application/json',
      },

      body: JSON.stringify({
        name: name.value,
        description: description.value,
      }),
    },
  )

  if (response.ok) {
    // Clear Create form
    name.value = ''
    description.value = ''

    // Reload all categories
    await loadTaxCategories()
  }
}

// Start Edit mode
function startEdit(category) {
  editingCategoryId.value = category.id

  // Copy current data into Edit form
  editName.value = category.name
  editDescription.value = category.description
}

// Update an existing tax category
async function updateTaxCategory(id) {
  const response = await fetch(
    `https://localhost:5106/api/taxcategories/${id}`,
    {
      method: 'PUT',

      headers: {
        ...getAuthHeader(),
        'Content-Type': 'application/json',
      },

      body: JSON.stringify({
        id: id,
        name: editName.value,
        description: editDescription.value,
      }),
    },
  )

  if (response.ok) {
    // Close Edit mode
    editingCategoryId.value = null

    // Reload categories
    await loadTaxCategories()
  }
}

// Cancel Edit mode
function cancelEdit() {
  editingCategoryId.value = null
}

// Delete a tax category
async function deleteTaxCategory(id) {
  const response = await fetch(
    `https://localhost:5106/api/taxcategories/${id}`,
    {
      method: 'DELETE',
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    // Reload categories
    await loadTaxCategories()
  }
}

// Load all tax categories when the page is opened
onMounted(() => {
  loadTaxCategories()
})
</script>


<template>
  <v-container>
    <h1>Tax Categories</h1>

    <!-- Create Tax Category -->
    <v-card class="mb-6">
      <v-card-title>
        Create Tax Category
      </v-card-title>

      <v-card-text>

        <!-- Category Name -->
        <v-text-field
          v-model="name"
          label="Name"
        />

        <!-- Category Description -->
        <v-text-field
          v-model="description"
          label="Description"
        />

      </v-card-text>

      <v-card-actions>

        <v-btn
          color="primary"
          @click="createTaxCategory"
        >
          Create Tax Category
        </v-btn>

      </v-card-actions>
    </v-card>


    <!-- Display Tax Categories -->
    <v-card
      v-for="category in taxCategories"
      :key="category.id"
      class="mb-4"
    >

      <!-- Normal View -->
      <template v-if="editingCategoryId !== category.id">

        <v-card-title>
          {{ category.name }}
        </v-card-title>

        <v-card-text>

          <div>
            Category ID:
            {{ category.id }}
          </div>

          <div>
            Description:
            {{ category.description }}
          </div>

        </v-card-text>

        <v-card-actions>

          <v-btn
            color="primary"
            @click="startEdit(category)"
          >
            Edit
          </v-btn>

          <v-btn
            color="error"
            @click="deleteTaxCategory(category.id)"
          >
            Delete
          </v-btn>

        </v-card-actions>

      </template>


      <!-- Edit View -->
      <template v-else>

        <v-card-title>
          Edit Tax Category
        </v-card-title>

        <v-card-text>

          <!-- Edit Category Name -->
          <v-text-field
            v-model="editName"
            label="Name"
          />

          <!-- Edit Category Description -->
          <v-text-field
            v-model="editDescription"
            label="Description"
          />

        </v-card-text>

        <v-card-actions>

          <v-btn
            color="primary"
            @click="updateTaxCategory(category.id)"
          >
            Save
          </v-btn>

          <v-btn
            @click="cancelEdit"
          >
            Cancel
          </v-btn>

        </v-card-actions>

      </template>

    </v-card>

  </v-container>
</template>

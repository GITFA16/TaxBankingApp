<script setup>
import { onMounted, ref } from 'vue'

// Reactive variable that stores the list of users
const users = ref([])

// Reactive variables for the Create User form
const firstName = ref('')
const lastName = ref('')
const email = ref('')

// Reactive variables for the Edit User form
const editingUserId = ref(null)
const editFirstName = ref('')
const editLastName = ref('')
const editEmail = ref('')


// READ USERS
async function loadUsers() {
  // Frontend sends a GET request to the backend API
  const response = await fetch(
    'http://localhost:5106/api/users',
    {
      headers: getAuthHeader(),
    },
  ) 

  // Only update the user list if the request was successful
  if (response.ok) {
    users.value = await response.json()
  }
}


// CREATE USER
async function createUser() {
  // Frontend sends a POST request to create a new user
  const response = await fetch('http://localhost:5106/api/users', {
    method: 'POST',

  headers: {
    ...getAuthHeader(),
    'Content-Type': 'application/json',
  },

    // Convert the JavaScript object into JSON before sending it
    body: JSON.stringify({
      firstName: firstName.value,
      lastName: lastName.value,
      email: email.value,
    }),
  })

  // Continue only if the user was created successfully
  if (response.ok) {
    // Clear the Create User form
    firstName.value = ''
    lastName.value = ''
    email.value = ''

    // Load the users again so the new user appears immediately
    await loadUsers()
  }
}


// DELETE USER
async function deleteUser(id) {
  // Frontend sends a DELETE request using the selected user ID
  const response = await fetch(
    `http://localhost:5106/api/users/${id}`,
    {
      method: 'DELETE',
      headers: getAuthHeader(),
    },
  )

  // Reload the users so the deleted user disappears immediately
  if (response.ok) {
    await loadUsers()
  }
}


// START EDIT USER
function startEdit(user) {
  // Store the ID of the user that is currently being edited
  editingUserId.value = user.id

  // Copy the current user data into the Edit User form
  editFirstName.value = user.firstName
  editLastName.value = user.lastName
  editEmail.value = user.email
}


// UPDATE USER
async function updateUser(id) {
  // Frontend sends a PUT request using the selected user ID
  const response = await fetch(
    `http://localhost:5106/api/users/${id}`,
    {
      method: 'PUT',
   
    headers: {
      ...getAuthHeader(),
      'Content-Type': 'application/json',
    },

      // Convert the updated user data into JSON before sending it
      body: JSON.stringify({
        firstName: editFirstName.value,
        lastName: editLastName.value,
        email: editEmail.value,
      }),
    },
  )

  // Continue only if the update was successful
  if (response.ok) {
    // Close Edit mode
    editingUserId.value = null

    // Load the users again so the updated data appears immediately
    await loadUsers()
  }
}


// CANCEL EDIT
function cancelEdit() {
  // Set editingUserId back to null
  // null means that no user is currently being edited
  editingUserId.value = null
}


// VUE LIFECYCLE
// Load all users automatically when this Vue component is mounted
onMounted(() => {
  loadUsers()
})
</script>


<template>
  <v-container>
    <h1>Users</h1>


    <!-- CREATE USER FORM -->
    <v-card class="mb-6">
      <v-card-title>
        Create User
      </v-card-title>

      <v-card-text>
        <!-- First Name input -->
        <v-text-field
          v-model="firstName"
          label="First Name"
        />

        <!-- Last Name input -->
        <v-text-field
          v-model="lastName"
          label="Last Name"
        />

        <!-- Email input -->
        <v-text-field
          v-model="email"
          label="Email"
          type="email"
        />
      </v-card-text>

      <v-card-actions>
        <!-- Calls createUser() when the button is clicked -->
        <v-btn
          color="primary"
          @click="createUser"
        >
          Create User
        </v-btn>
      </v-card-actions>
    </v-card>


    <!-- READ / DISPLAY USERS -->
    <!-- Create one card for every user in the users list -->
    <v-card
      v-for="user in users"
      :key="user.id"
      class="mb-4"
    >

      <!-- NORMAL VIEW -->
      <!-- Show this section when the user is not being edited -->
      <template v-if="editingUserId !== user.id">
        <v-card-title>
          {{ user.firstName }} {{ user.lastName }}
        </v-card-title>

        <v-card-text>
          {{ user.email }}
        </v-card-text>

        <v-card-actions>
          <!-- Start Edit mode for this user -->
          <v-btn
            color="primary"
            @click="startEdit(user)"
          >
            Edit
          </v-btn>

          <!-- Delete this user using user.id -->
          <v-btn
            color="error"
            @click="deleteUser(user.id)"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </template>


      <!-- EDIT VIEW -->
      <!-- Show this section when the selected user is being edited -->
      <template v-else>
        <v-card-title>
          Edit User
        </v-card-title>

        <v-card-text>
          <!-- Edit First Name -->
          <v-text-field
            v-model="editFirstName"
            label="First Name"
          />

          <!-- Edit Last Name -->
          <v-text-field
            v-model="editLastName"
            label="Last Name"
          />

          <!-- Edit Email -->
          <v-text-field
            v-model="editEmail"
            label="Email"
            type="email"
          />
        </v-card-text>

        <v-card-actions>
          <!-- Save the updated user data -->
          <v-btn
            color="primary"
            @click="updateUser(user.id)"
          >
            Save
          </v-btn>

          <!-- Cancel Edit mode without saving -->
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
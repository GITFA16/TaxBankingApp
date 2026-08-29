<script setup>
import { onMounted, ref } from 'vue'

// Stores all users
const users = ref([])

// Create User form
const firstName = ref('')
const lastName = ref('')
const email = ref('')

// Edit User form
const editingUserId = ref(null)
const editFirstName = ref('')
const editLastName = ref('')
const editEmail = ref('')

// Error message
const errorMessage = ref('')

// Basic Authentication header
function getAuthHeader() {
  const auth = localStorage.getItem('taxoraAuth')

  return {
    Authorization: `Basic ${auth}`,
  }
}


// READ USERS
async function loadUsers() {
  errorMessage.value = ''

  try {
    const response = await fetch(
      'https://localhost:7131/api/users',
      {
        headers: getAuthHeader(),
      },
    )

    if (response.ok) {
      users.value = await response.json()
    } else {
      errorMessage.value = 'Users could not be loaded.'
    }
  } catch (error) {
    errorMessage.value = 'Backend connection failed.'
  }
}


// CREATE USER
async function createUser() {
  errorMessage.value = ''

  if (firstName.value.trim() === '') {
    errorMessage.value = 'Please enter a First Name.'
    return
  }

  if (lastName.value.trim() === '') {
    errorMessage.value = 'Please enter a Last Name.'
    return
  }

  if (email.value.trim() === '') {
    errorMessage.value = 'Please enter an Email.'
    return
  }

  const response = await fetch(
    'https://localhost:7131/api/users',
    {
      method: 'POST',

      headers: {
        ...getAuthHeader(),
        'Content-Type': 'application/json',
      },

      body: JSON.stringify({
        firstName: firstName.value,
        lastName: lastName.value,
        email: email.value,
      }),
    },
  )

  if (response.ok) {
    firstName.value = ''
    lastName.value = ''
    email.value = ''

    await loadUsers()
  } else {
    errorMessage.value = 'User could not be created.'
  }
}


// DELETE USER
async function deleteUser(id) {
  errorMessage.value = ''

  const response = await fetch(
    `https://localhost:7131/api/users/${id}`,
    {
      method: 'DELETE',
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    await loadUsers()
  } else {
    errorMessage.value = 'User could not be deleted.'
  }
}


// START EDIT USER
function startEdit(user) {
  editingUserId.value = user.id

  editFirstName.value = user.firstName
  editLastName.value = user.lastName
  editEmail.value = user.email
}


// UPDATE USER
async function updateUser(id) {
  errorMessage.value = ''

  if (editFirstName.value.trim() === '') {
    errorMessage.value = 'Please enter a First Name.'
    return
  }

  if (editLastName.value.trim() === '') {
    errorMessage.value = 'Please enter a Last Name.'
    return
  }

  if (editEmail.value.trim() === '') {
    errorMessage.value = 'Please enter an Email.'
    return
  }

  const response = await fetch(
    `https://localhost:7131/api/users/${id}`,
    {
      method: 'PUT',

      headers: {
        ...getAuthHeader(),
        'Content-Type': 'application/json',
      },

      body: JSON.stringify({
        firstName: editFirstName.value,
        lastName: editLastName.value,
        email: editEmail.value,
      }),
    },
  )

  if (response.ok) {
    editingUserId.value = null

    await loadUsers()
  } else {
    errorMessage.value = 'User could not be updated.'
  }
}


// CANCEL EDIT
function cancelEdit() {
  editingUserId.value = null
}


// Load users when page opens
onMounted(() => {
  loadUsers()
})
</script>


<template>
  <v-container>
    <h1>Users</h1>

    <!-- Error Message -->
    <v-alert
      v-if="errorMessage"
      type="error"
      class="mb-4"
    >
      {{ errorMessage }}
    </v-alert>


    <!-- CREATE USER FORM -->
    <v-card class="mb-6">
      <v-card-title>
        Create User
      </v-card-title>

      <v-card-text>

        <!-- First Name -->
        <v-text-field
          v-model="firstName"
          label="First Name"
        />

        <!-- Last Name -->
        <v-text-field
          v-model="lastName"
          label="Last Name"
        />

        <!-- Email -->
        <v-text-field
          v-model="email"
          label="Email"
          type="email"
        />

      </v-card-text>

      <v-card-actions>
        <v-btn
          color="primary"
          @click="createUser"
        >
          Create User
        </v-btn>
      </v-card-actions>
    </v-card>


    <!-- USER LIST -->
    <v-card
      v-for="user in users"
      :key="user.id"
      class="mb-4"
    >

      <!-- NORMAL VIEW -->
      <template v-if="editingUserId !== user.id">

        <v-card-title>
          {{ user.firstName }} {{ user.lastName }}
        </v-card-title>

        <v-card-text>
          <div>
            User ID:
            {{ user.id }}
          </div>

          <div>
            Email:
            {{ user.email }}
          </div>
        </v-card-text>

        <v-card-actions>

          <v-btn
            color="primary"
            @click="startEdit(user)"
          >
            Edit
          </v-btn>

          <v-btn
            color="error"
            @click="deleteUser(user.id)"
          >
            Delete
          </v-btn>

        </v-card-actions>

      </template>


      <!-- EDIT VIEW -->
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

          <v-btn
            color="primary"
            @click="updateUser(user.id)"
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
<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const username = ref('')
const password = ref('')
const errorMessage = ref('')

async function login() {
  errorMessage.value = ''

  const credentials = btoa(
    `${username.value}:${password.value}`  //YWRtaW46YWRtaW4=
  )

  const response = await fetch(
    'https://localhost:7131/api/users',
    {
      headers: {
        Authorization: `Basic ${credentials}`
      }
    }
  )

  if (response.ok) {
    localStorage.setItem(
      'taxoraAuth',
      credentials
    )

    router.push('/users')
  } else {
    errorMessage.value =
      'Username oder Passwort ist falsch.'
  }
}
</script>

<template>
  <v-container
    class="d-flex justify-center align-center"
    style="min-height: 80vh"
  >
    <v-card
      width="400"
      class="pa-6"
    >
      <v-card-title>
        TaxOra Login
      </v-card-title>

      <v-alert
        v-if="errorMessage"
        type="error"
        class="mb-4"
      >
        {{ errorMessage }}
      </v-alert>

      <v-text-field
        v-model="username"
        label="Username"
      />

      <v-text-field
        v-model="password"
        label="Password"
        type="password"
      />

      <v-btn
        color="primary"
        block
        @click="login"
      >
        Login
      </v-btn>
    </v-card>
  </v-container>
</template>
<script setup>
import { onMounted, ref } from 'vue'

// Reactive variable that stores the list of bank accounts
const accounts = ref([])

// Reactive variables for the Create Bank Account form
const userId = ref('')
const accountName = ref('')
const iban = ref('')
const currency = ref('CHF')
const balance = ref(0)


// READ BANK ACCOUNTS
async function loadBankAccounts() {
  // Frontend sends a GET request to the backend API
  const response = await fetch('http://localhost:5106/api/bankaccounts')

  // Only update the list if the request was successful
  if (response.ok) {
    accounts.value = await response.json()
  }
}


// CREATE BANK ACCOUNT
async function createBankAccount() {
  // Frontend sends a POST request for the selected user
  const response = await fetch(
    `http://localhost:5106/api/users/${userId.value}/accounts`,
    {
      method: 'POST',

      headers: {
        'Content-Type': 'application/json',
      },

      // Convert the JavaScript object into JSON
      body: JSON.stringify({
        accountName: accountName.value,
        iban: iban.value,
        currency: currency.value,
        balance: balance.value,
      }),
    },
  )

  // Continue only if the bank account was created successfully
  if (response.ok) {
    // Clear the Create Bank Account form
    userId.value = ''
    accountName.value = ''
    iban.value = ''
    currency.value = 'CHF'
    balance.value = 0

    // Reload bank accounts so the new account appears immediately
    await loadBankAccounts()
  }
}


// VUE LIFECYCLE
// Load all bank accounts when this component is mounted
onMounted(() => {
  loadBankAccounts()
})
</script>


<template>
  <v-container>
    <h1>Bank Accounts</h1>


    <!-- CREATE BANK ACCOUNT FORM -->
    <v-card class="mb-6">
      <v-card-title>
        Create Bank Account
      </v-card-title>

      <v-card-text>
        <!-- User ID -->
        <v-text-field
          v-model="userId"
          label="User ID"
          type="number"
        />

        <!-- Account Name -->
        <v-text-field
          v-model="accountName"
          label="Account Name"
        />

        <!-- IBAN -->
        <v-text-field
          v-model="iban"
          label="IBAN"
        />

        <!-- Currency -->
        <v-text-field
          v-model="currency"
          label="Currency"
        />

        <!-- Balance -->
        <v-text-field
          v-model="balance"
          label="Balance"
          type="number"
        />
      </v-card-text>

      <v-card-actions>
        <!-- Calls createBankAccount() when clicked -->
        <v-btn
          color="primary"
          @click="createBankAccount"
        >
          Create Bank Account
        </v-btn>
      </v-card-actions>
    </v-card>


    <!-- READ / DISPLAY BANK ACCOUNTS -->
    <!-- Create one card for every bank account -->
    <v-card
      v-for="account in accounts"
      :key="account.id"
      class="mb-4"
    >
      <v-card-title>
        {{ account.accountName }}
      </v-card-title>

      <v-card-text>
        <div>
          User ID: {{ account.userId }}
        </div>

        <div>
          IBAN: {{ account.iban }}
        </div>

        <div>
          Currency: {{ account.currency }}
        </div>

        <div>
          Balance: {{ account.balance }}
        </div>
      </v-card-text>
    </v-card>
  </v-container>
</template>
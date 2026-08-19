<script setup>
import { onMounted, ref } from 'vue'

// Reactive variable that stores the list of bank accounts
const accounts = ref([])

// Reactive variables for the Create Bank Account form
const userId = ref(null)
const accountName = ref('')
const iban = ref('')
const currency = ref('CHF')
const balance = ref(0)

// Reactive variables for the Edit Bank Account form
const editingAccountId = ref(null)
const editAccountName = ref('')
const editIban = ref('')
const editCurrency = ref('')
const editBalance = ref(0)


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
    userId.value = null
    accountName.value = ''
    iban.value = ''
    currency.value = 'CHF'
    balance.value = 0

    // Reload bank accounts so the new account appears immediately
    await loadBankAccounts()
  }
}

// START EDIT BANK ACCOUNT
function startEdit(account) {
  // Store the ID of the bank account that is currently being edited
  editingAccountId.value = account.id

  // Copy the current bank account data into the Edit form
  editAccountName.value = account.accountName
  editIban.value = account.iban
  editCurrency.value = account.currency
  editBalance.value = account.balance
}


// UPDATE BANK ACCOUNT
async function updateBankAccount(id) {
  // Frontend sends a PUT request using the selected bank account ID
  const response = await fetch(
    `http://localhost:5106/api/bankaccounts/${id}`,
    {
      method: 'PUT',

      headers: {
        'Content-Type': 'application/json',
      },

      // Convert the updated bank account data into JSON
      body: JSON.stringify({
        accountName: editAccountName.value,
        iban: editIban.value,
        currency: editCurrency.value,
        balance: editBalance.value,
      }),
    },
  )

  // Continue only if the update was successful
  if (response.ok) {
    // Close Edit mode
    editingAccountId.value = null

    // Reload bank accounts so the updated data appears immediately
    await loadBankAccounts()
  }
}


// CANCEL EDIT
function cancelEdit() {
  // null means that no bank account is currently being edited
  editingAccountId.value = null
}


// DELETE BANK ACCOUNT
async function deleteBankAccount(id) {
  // Frontend sends a DELETE request using the selected bank account ID
  const response = await fetch(
    `http://localhost:5106/api/bankaccounts/${id}`,
    {
      method: 'DELETE',
    },
  )

  // Reload bank accounts so the deleted account disappears immediately
  if (response.ok) {
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
          v-model.number="userId"
          label="User ID"
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
          v-model.number="balance"
          label="Balance"
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

      <!-- NORMAL VIEW -->
      <!-- Show this section when the bank account is not being edited -->
      <template v-if="editingAccountId !== account.id">
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

        <v-card-actions>
          <!-- Start Edit mode for this bank account -->
          <v-btn
            color="primary"
            @click="startEdit(account)"
          >
            Edit
          </v-btn>

          <!-- Delete this bank account using account.id -->
          <v-btn
            color="error"
            @click="deleteBankAccount(account.id)"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </template>


      <!-- EDIT VIEW -->
      <!-- Show this section when the selected bank account is being edited -->
      <template v-else>
        <v-card-title>
          Edit Bank Account
        </v-card-title>

        <v-card-text>
          <!-- Edit Account Name -->
          <v-text-field
            v-model="editAccountName"
            label="Account Name"
          />

          <!-- Edit IBAN -->
          <v-text-field
            v-model="editIban"
            label="IBAN"
          />

          <!-- Edit Currency -->
          <v-text-field
            v-model="editCurrency"
            label="Currency"
          />

          <!-- Edit Balance -->
          <v-text-field
            v-model.number="editBalance"
            label="Balance"
          />
        </v-card-text>

        <v-card-actions>
          <!-- Save the updated bank account -->
          <v-btn
            color="primary"
            @click="updateBankAccount(account.id)"
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
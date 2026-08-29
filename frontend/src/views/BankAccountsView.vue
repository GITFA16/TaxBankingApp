<script setup>
import { computed, onMounted, ref } from 'vue'

// Stores all bank accounts
const accounts = ref([])

// Stores all users for the dropdown
const users = ref([])

// Create Bank Account form
const userId = ref(null)
const accountName = ref('')
const iban = ref('')
const currency = ref('CHF')
const balance = ref(0)

const currencies = [
  'CHF',
  'USD',
  'EUR',
  'IDR',
  'GBP',
]

// Edit Bank Account form
const editingAccountId = ref(null)
const editAccountName = ref('')
const editIban = ref('')
const editCurrency = ref('')
const editBalance = ref(0)

// User used to filter bank accounts
const selectedUserId = ref(null)


// BASIC AUTH HEADER
function getAuthHeader() {
  const auth = localStorage.getItem('taxoraAuth')

  return {
    Authorization: `Basic ${auth}`,
  }
}


// Create a Full Name for every user
const userOptions = computed(() => {
  return users.value.map(user => ({
    id: user.id,
    fullName: `${user.firstName} ${user.lastName}`,
  }))
})


// READ ALL USERS
async function loadUsers() {
  const response = await fetch(
    'https://localhost:5106/api/users',
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    users.value = await response.json()
  }
}


function getUserFullName(userId) {
  const user = users.value.find(user => user.id === userId)

  if (!user) {
    return 'Unknown User'
  }

  return `${user.firstName} ${user.lastName}`
}


// READ ALL BANK ACCOUNTS
async function loadBankAccounts() {
  const response = await fetch(
    'https://localhost:5106/api/bankaccounts',
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    accounts.value = await response.json()
  }
}


// READ BANK ACCOUNTS BY USER
async function loadBankAccountsByUser() {
  if (selectedUserId.value === null) {
    return
  }

  const response = await fetch(
    `https://localhost:5106/api/users/${selectedUserId.value}/accounts`,
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    accounts.value = await response.json()
  }
}


// CREATE BANK ACCOUNT
async function createBankAccount() {
  if (userId.value === null) {
    return
  }

  const response = await fetch(
    `https://localhost:5106/api/users/${userId.value}/accounts`,
    {
      method: 'POST',

      headers: {
        ...getAuthHeader(),
        'Content-Type': 'application/json',
      },

      body: JSON.stringify({
        accountName: accountName.value,
        iban: iban.value,
        currency: currency.value,
        balance: balance.value,
      }),
    },
  )

  if (response.ok) {
    userId.value = null
    accountName.value = ''
    iban.value = ''
    currency.value = 'CHF'
    balance.value = 0

    await loadBankAccounts()
  }
}


// START EDIT BANK ACCOUNT
function startEdit(account) {
  editingAccountId.value = account.id

  editAccountName.value = account.accountName
  editIban.value = account.iban
  editCurrency.value = account.currency
  editBalance.value = account.balance
}


// UPDATE BANK ACCOUNT
async function updateBankAccount(id) {
  const response = await fetch(
    `https://localhost:5106/api/bankaccounts/${id}`,
    {
      method: 'PUT',

      headers: {
        ...getAuthHeader(),
        'Content-Type': 'application/json',
      },

      body: JSON.stringify({
        accountName: editAccountName.value,
        iban: editIban.value,
        currency: editCurrency.value,
        balance: editBalance.value,
      }),
    },
  )

  if (response.ok) {
    editingAccountId.value = null

    if (selectedUserId.value !== null) {
      await loadBankAccountsByUser()
    } else {
      await loadBankAccounts()
    }
  }
}


// CANCEL EDIT
function cancelEdit() {
  editingAccountId.value = null
}


// DELETE BANK ACCOUNT
async function deleteBankAccount(id) {
  const response = await fetch(
    `https://localhost:5106/api/bankaccounts/${id}`,
    {
      method: 'DELETE',
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    if (selectedUserId.value !== null) {
      await loadBankAccountsByUser()
    } else {
      await loadBankAccounts()
    }
  }
}


// SHOW ALL BANK ACCOUNTS
async function showAllBankAccounts() {
  selectedUserId.value = null

  await loadBankAccounts()
}


// Load users and bank accounts when page opens
onMounted(() => {
  loadUsers()
  loadBankAccounts()
})
</script>


<template>
  <v-container>
    <h1>Bank Accounts</h1>

    <!-- CREATE BANK ACCOUNT -->
    <v-card class="mb-6">
      <v-card-title>
        Create Bank Account
      </v-card-title>

      <v-card-text>

        <!-- Select User by Full Name -->
        <v-select
          v-model="userId"
          :items="userOptions"
          item-title="fullName"
          item-value="id"
          label="User"
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
        <v-select
        v-model="currency"
        :items="currencies"
        label="Currency"
        />

        <!-- Balance -->
        <v-text-field
          v-model.number="balance"
          label="Balance"
        />

      </v-card-text>

      <v-card-actions>

        <v-btn
          color="primary"
          @click="createBankAccount"
        >
          Create Bank Account
        </v-btn>

      </v-card-actions>
    </v-card>


    <!-- FILTER BANK ACCOUNTS BY USER -->
    <v-card class="mb-6">
      <v-card-title>
        Bank Accounts by User
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

        <v-btn
          color="primary"
          @click="loadBankAccountsByUser"
        >
          Load Bank Accounts
        </v-btn>

        <v-btn
          @click="showAllBankAccounts"
        >
          Show All
        </v-btn>

      </v-card-actions>
    </v-card>


    <!-- DISPLAY BANK ACCOUNTS -->
    <v-card
      v-for="account in accounts"
      :key="account.id"
      class="mb-4"
    >

      <!-- NORMAL VIEW -->
      <template v-if="editingAccountId !== account.id">

        <v-card-title>
          {{ account.accountName }}
        </v-card-title>

        <v-card-text>

          <div>
            User ID: {{ getUserFullName(account.userId) }}
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

          <v-btn
            color="primary"
            @click="startEdit(account)"
          >
            Edit
          </v-btn>

          <v-btn
            color="error"
            @click="deleteBankAccount(account.id)"
          >
            Delete
          </v-btn>

        </v-card-actions>

      </template>


      <!-- EDIT VIEW -->
      <template v-else>

        <v-card-title>
          Edit Bank Account
        </v-card-title>

        <v-card-text>

          <v-text-field
            v-model="editAccountName"
            label="Account Name"
          />

          <v-text-field
            v-model="editIban"
            label="IBAN"
          />

          <v-select
           v-model="editCurrency"
           :items="currencies"
           label="Currency"
          />

          <v-text-field
            v-model.number="editBalance"
            label="Balance"
          />

        </v-card-text>

        <v-card-actions>

          <v-btn
            color="primary"
            @click="updateBankAccount(account.id)"
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
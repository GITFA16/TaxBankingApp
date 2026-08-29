<script setup>
import { computed, onMounted, ref } from 'vue'

// Data
const transactions = ref([])
const bankAccounts = ref([])
const users = ref([])

// Create Transaction form
const bankAccountId = ref(null)
const bookingDate = ref('')
const description = ref('')
const amount = ref(0)
const currency = ref('CHF')

// Edit Transaction form
const editingTransactionId = ref(null)
const editBankAccountId = ref(null)
const editBookingDate = ref('')
const editDescription = ref('')
const editAmount = ref(0)
const editCurrency = ref('CHF')

// Filter
const selectedBankAccountId = ref(null)

// Validation
const errorMessage = ref('')

// Currency options
const currencies = [
  'CHF',
  'USD',
  'EUR',
  'IDR',
  'GBP',
]

// Basic Authentication header
function getAuthHeader() {
  const auth = localStorage.getItem('taxoraAuth')

  return {
    Authorization: `Basic ${auth}`,
  }
}

// Bank Account dropdown options
// Example: Private Account (Faizal Alamudi)
const bankAccountOptions = computed(() => {
  return bankAccounts.value.map(account => {
    const user = users.value.find(
      user => user.id === account.userId,
    )

    const ownerName = user
      ? `${user.firstName} ${user.lastName}`
      : 'Unknown User'

    return {
      id: account.id,
      displayName: `${account.accountName} (${ownerName})`,
    }
  })
})

// Load all transactions
async function loadTransactions() {
  const response = await fetch(
    'https://localhost:7131/api/transactions',
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    transactions.value = await response.json()
  }
}

// Load all bank accounts
async function loadBankAccounts() {
  const response = await fetch(
    'https://localhost:7131/api/bankaccounts',
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    bankAccounts.value = await response.json()
  }
}

// Load all users
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

// Load transactions for selected bank account
async function loadTransactionsByBankAccount() {
  if (selectedBankAccountId.value === null) {
    return
  }

  const response = await fetch(
    `https://localhost:7131/api/bankaccounts/${selectedBankAccountId.value}/transactions`,
    {
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    transactions.value = await response.json()
  }
}

// Show all transactions
async function showAllTransactions() {
  selectedBankAccountId.value = null

  await loadTransactions()
}

// Create transaction
async function createTransaction() {
  errorMessage.value = ''

  if (bankAccountId.value === null) {
    errorMessage.value = 'Please select a Bank Account.'
    return
  }

  if (bookingDate.value === '') {
    errorMessage.value = 'Please select a Booking Date.'
    return
  }

  if (description.value.trim() === '') {
    errorMessage.value = 'Please enter a Description.'
    return
  }

  const response = await fetch(
    'https://localhost:7131/api/transactions',
    {
      method: 'POST',

      headers: {
        ...getAuthHeader(),
        'Content-Type': 'application/json',
      },

      body: JSON.stringify({
        bankAccountId: bankAccountId.value,
        bookingDate: bookingDate.value,
        description: description.value,
        amount: amount.value,
        currency: currency.value,
      }),
    },
  )

  if (response.ok) {
    bankAccountId.value = null
    bookingDate.value = ''
    description.value = ''
    amount.value = 0
    currency.value = 'CHF'

    errorMessage.value = ''

    if (selectedBankAccountId.value !== null) {
      await loadTransactionsByBankAccount()
    } else {
      await loadTransactions()
    }
  } else {
    errorMessage.value = 'Transaction could not be created.'
  }
}

// Start Edit mode
function startEdit(transaction) {
  editingTransactionId.value = transaction.id

  editBankAccountId.value = transaction.bankAccountId
  editBookingDate.value =
    transaction.bookingDate.substring(0, 10)

  editDescription.value = transaction.description
  editAmount.value = transaction.amount
  editCurrency.value = transaction.currency
}

// Update transaction
async function updateTransaction(id) {
  errorMessage.value = ''

  if (editBankAccountId.value === null) {
    errorMessage.value = 'Please select a Bank Account.'
    return
  }

  if (editBookingDate.value === '') {
    errorMessage.value = 'Please select a Booking Date.'
    return
  }

  if (editDescription.value.trim() === '') {
    errorMessage.value = 'Please enter a Description.'
    return
  }

  const response = await fetch(
    `https://localhost:7131/api/transactions/${id}`,
    {
      method: 'PUT',

      headers: {
        ...getAuthHeader(),
        'Content-Type': 'application/json',
      },

      body: JSON.stringify({
        bankAccountId: editBankAccountId.value,
        bookingDate: editBookingDate.value,
        description: editDescription.value,
        amount: editAmount.value,
        currency: editCurrency.value,
      }),
    },
  )

  if (response.ok) {
    editingTransactionId.value = null

    if (selectedBankAccountId.value !== null) {
      await loadTransactionsByBankAccount()
    } else {
      await loadTransactions()
    }
  } else {
    errorMessage.value = 'Transaction could not be updated.'
  }
}

// Cancel Edit mode
function cancelEdit() {
  editingTransactionId.value = null
}

// Delete transaction
async function deleteTransaction(id) {
  const response = await fetch(
    `https://localhost:7131/api/transactions/${id}`,
    {
      method: 'DELETE',
      headers: getAuthHeader(),
    },
  )

  if (response.ok) {
    if (selectedBankAccountId.value !== null) {
      await loadTransactionsByBankAccount()
    } else {
      await loadTransactions()
    }
  }
}

// Get Bank Account name
function getBankAccountName(bankAccountId) {
  const account = bankAccounts.value.find(
    account => account.id === bankAccountId,
  )

  if (!account) {
    return 'Unknown Bank Account'
  }

  return account.accountName
}

// Get Bank Account owner
function getBankAccountOwner(bankAccountId) {
  const account = bankAccounts.value.find(
    account => account.id === bankAccountId,
  )

  if (!account) {
    return 'Unknown User'
  }

  const user = users.value.find(
    user => user.id === account.userId,
  )

  if (!user) {
    return 'Unknown User'
  }

  return `${user.firstName} ${user.lastName}`
}

// Load data when page opens
onMounted(() => {
  loadTransactions()
  loadBankAccounts()
  loadUsers()
})
</script>

<template>
  <v-container>
    <h1>Transactions</h1>

    <!-- Error Message -->
    <v-alert
      v-if="errorMessage"
      type="error"
      class="mb-4"
    >
      {{ errorMessage }}
    </v-alert>

    <!-- Create Transaction -->
    <v-card class="mb-6">
      <v-card-title>
        Create Transaction
      </v-card-title>

      <v-card-text>

        <!-- Bank Account -->
        <v-select
          v-model="bankAccountId"
          :items="bankAccountOptions"
          item-title="displayName"
          item-value="id"
          label="Bank Account"
        />

        <!-- Booking Date -->
        <v-text-field
          v-model="bookingDate"
          label="Booking Date"
          type="date"
        />

        <!-- Description -->
        <v-text-field
          v-model="description"
          label="Description"
        />

        <!-- Amount -->
        <v-text-field
          v-model.number="amount"
          label="Amount"
          type="number"
        />

        <!-- Currency -->
        <v-select
          v-model="currency"
          :items="currencies"
          label="Currency"
        />

      </v-card-text>

      <v-card-actions>
        <v-btn
          color="primary"
          @click="createTransaction"
        >
          Create Transaction
        </v-btn>
      </v-card-actions>
    </v-card>

    <!-- Filter Transactions -->
    <v-card class="mb-6">
      <v-card-title>
        Transactions by Bank Account
      </v-card-title>

      <v-card-text>
        <v-select
          v-model="selectedBankAccountId"
          :items="bankAccountOptions"
          item-title="displayName"
          item-value="id"
          label="Bank Account"
        />
      </v-card-text>

      <v-card-actions>
        <v-btn
          color="primary"
          @click="loadTransactionsByBankAccount"
        >
          Load Transactions
        </v-btn>

        <v-btn @click="showAllTransactions">
          Show All
        </v-btn>
      </v-card-actions>
    </v-card>

    <!-- Transaction List -->
    <v-card
      v-for="transaction in transactions"
      :key="transaction.id"
      class="mb-4"
    >

      <!-- Normal View -->
      <template
        v-if="editingTransactionId !== transaction.id"
      >
        <v-card-title>
          {{ transaction.description }}
        </v-card-title>

        <v-card-text>

          <div>
            Transaction ID:
            {{ transaction.id }}
          </div>

          <div>
            Owner:
            {{ getBankAccountOwner(transaction.bankAccountId) }}
          </div>

          <div>
            Bank Account:
            {{ getBankAccountName(transaction.bankAccountId) }}
          </div>

          <div>
            Booking Date:
            {{ transaction.bookingDate.substring(0, 10) }}
          </div>

          <div>
            Amount:
            {{ transaction.amount }}
            {{ transaction.currency }}
          </div>

        </v-card-text>

        <v-card-actions>
          <v-btn
            color="primary"
            @click="startEdit(transaction)"
          >
            Edit
          </v-btn>

          <v-btn
            color="error"
            @click="deleteTransaction(transaction.id)"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </template>

      <!-- Edit View -->
      <template v-else>
        <v-card-title>
          Edit Transaction
        </v-card-title>

        <v-card-text>

          <!-- Bank Account -->
          <v-select
            v-model="editBankAccountId"
            :items="bankAccountOptions"
            item-title="displayName"
            item-value="id"
            label="Bank Account"
          />

          <!-- Booking Date -->
          <v-text-field
            v-model="editBookingDate"
            label="Booking Date"
            type="date"
          />

          <!-- Description -->
          <v-text-field
            v-model="editDescription"
            label="Description"
          />

          <!-- Amount -->
          <v-text-field
            v-model.number="editAmount"
            label="Amount"
            type="number"
          />

          <!-- Currency -->
          <v-select
            v-model="editCurrency"
            :items="currencies"
            label="Currency"
          />

        </v-card-text>

        <v-card-actions>
          <v-btn
            color="primary"
            @click="updateTransaction(transaction.id)"
          >
            Save
          </v-btn>

          <v-btn @click="cancelEdit">
            Cancel
          </v-btn>
        </v-card-actions>
      </template>

    </v-card>
  </v-container>
</template>
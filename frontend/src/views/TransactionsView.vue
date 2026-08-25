<script setup>
import { onMounted, ref } from 'vue'

// Stores all transactions that will be displayed
const transactions = ref([])

// Stores all bank accounts for dropdown selection
const bankAccounts = ref([])

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

// Bank Account used to filter transactions
const selectedBankAccountId = ref(null)

// Currency options
const currencies = [
  'CHF',
  'USD',
  'EUR',
  'IDR',
  'GBP',
]

// Load all transactions
async function loadTransactions() {
  const response = await fetch(
    'http://localhost:5106/api/transactions',
  )

  if (response.ok) {
    transactions.value = await response.json()
  }
}

// Load all bank accounts
async function loadBankAccounts() {
  const response = await fetch(
    'http://localhost:5106/api/bankaccounts',
  )

  if (response.ok) {
    bankAccounts.value = await response.json()
  }
}

// Load transactions by selected bank account
async function loadTransactionsByBankAccount() {
  if (selectedBankAccountId.value === null) {
    return
  }

  const response = await fetch(
    `http://localhost:5106/api/bankaccounts/${selectedBankAccountId.value}/transactions`,
  )

  if (response.ok) {
    transactions.value = await response.json()
  }
}

// Show all transactions again
async function showAllTransactions() {
  selectedBankAccountId.value = null

  await loadTransactions()
}

// Create a new transaction
async function createTransaction() {
  if (bankAccountId.value === null) {
    return
  }

  const response = await fetch(
    'http://localhost:5106/api/transactions',
    {
      method: 'POST',

      headers: {
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

    if (selectedBankAccountId.value !== null) {
      await loadTransactionsByBankAccount()
    } else {
      await loadTransactions()
    }
  }
}

// Start Edit mode
function startEdit(transaction) {
  editingTransactionId.value = transaction.id

  editBankAccountId.value = transaction.bankAccountId
  editBookingDate.value = transaction.bookingDate.substring(0, 10)
  editDescription.value = transaction.description
  editAmount.value = transaction.amount
  editCurrency.value = transaction.currency
}

// Update an existing transaction
async function updateTransaction(id) {
  const response = await fetch(
    `http://localhost:5106/api/transactions/${id}`,
    {
      method: 'PUT',

      headers: {
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
  }
}

// Cancel Edit mode
function cancelEdit() {
  editingTransactionId.value = null
}

// Delete a transaction
async function deleteTransaction(id) {
  const response = await fetch(
    `http://localhost:5106/api/transactions/${id}`,
    {
      method: 'DELETE',
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

// Get the Bank Account name from the Bank Account ID
function getBankAccountName(bankAccountId) {
  const account = bankAccounts.value.find(
    account => account.id === bankAccountId,
  )

  if (!account) {
    return 'Unknown Bank Account'
  }

  return account.accountName
}

// Load transactions and bank accounts when the page is opened
onMounted(() => {
  loadTransactions()
  loadBankAccounts()
})
</script>


<template>
  <v-container>
    <h1>Transactions</h1>

    <!-- Create Transaction Form -->
    <v-card class="mb-6">
      <v-card-title>
        Create Transaction
      </v-card-title>

      <v-card-text>

        <!-- Select Bank Account by name -->
        <v-select
          v-model="bankAccountId"
          :items="bankAccounts"
          item-title="accountName"
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


    <!-- Filter Transactions by Bank Account -->
    <v-card class="mb-6">
      <v-card-title>
        Transactions by Bank Account
      </v-card-title>

      <v-card-text>

        <!-- Select Bank Account by name -->
        <v-select
          v-model="selectedBankAccountId"
          :items="bankAccounts"
          item-title="accountName"
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

        <v-btn
          @click="showAllTransactions"
        >
          Show All
        </v-btn>

      </v-card-actions>
    </v-card>


    <!-- Display Transactions -->
    <v-card
      v-for="transaction in transactions"
      :key="transaction.id"
      class="mb-4"
    >

      <!-- Normal View -->
      <template v-if="editingTransactionId !== transaction.id">

        <v-card-title>
          {{ transaction.description }}
        </v-card-title>

        <v-card-text>

          <div>
            Transaction ID: {{ transaction.id }}
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

          <div>
            Tax Category:
            {{ transaction.suggestedTaxCategory }}
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

          <!-- Select Bank Account by name -->
          <v-select
            v-model="editBankAccountId"
            :items="bankAccounts"
            item-title="accountName"
            item-value="id"
            label="Bank Account"
          />

          <!-- Edit Booking Date -->
          <v-text-field
            v-model="editBookingDate"
            label="Booking Date"
            type="date"
          />

          <!-- Edit Description -->
          <v-text-field
            v-model="editDescription"
            label="Description"
          />

          <!-- Edit Amount -->
          <v-text-field
            v-model.number="editAmount"
            label="Amount"
          />

          <!-- Edit Currency -->
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
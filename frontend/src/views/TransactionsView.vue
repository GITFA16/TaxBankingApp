<script setup>
import { onMounted, ref } from 'vue'

// REACTIVE VARIABLES
// Stores all transactions that will be displayed
const transactions = ref([])

// CREATE TRANSACTION FORM
const bankAccountId = ref(null)
const bookingDate = ref('')
const description = ref('')
const amount = ref(0)
const currency = ref('CHF')

// EDIT TRANSACTION FORM
const editingTransactionId = ref(null)

const editBankAccountId = ref(null)
const editBookingDate = ref('')
const editDescription = ref('')
const editAmount = ref(0)
const editCurrency = ref('CHF')


// FILTER TRANSACTIONS BY BANK ACCOUNT
// Stores the Bank Account ID that is used for filtering
const selectedBankAccountId = ref(null)

// READ ALL TRANSACTIONS
async function loadTransactions() {
  // Send GET request to backend
  const response = await fetch(
    'http://localhost:5106/api/transactions',
  )

  // Only update the list if backend responds successfully
  if (response.ok) {
    transactions.value = await response.json()
  }
}

// READ TRANSACTIONS BY BANK ACCOUNT
async function loadTransactionsByBankAccount() {
  // Do nothing if no Bank Account ID was entered
  if (selectedBankAccountId.value === null) {
    return
  }

  // Send GET request for one specific Bank Account
  const response = await fetch(
    `http://localhost:5106/api/bankaccounts/${selectedBankAccountId.value}/transactions`,
  )

  // Store only transactions that belong to this Bank Account
  if (response.ok) {
    transactions.value = await response.json()
  }
}

// SHOW ALL TRANSACTIONS
async function showAllTransactions() {
  // Remove the filter
  selectedBankAccountId.value = null

  // Load all transactions again
  await loadTransactions()
}

// CREATE TRANSACTION
async function createTransaction() {
  // Send POST request to backend
  const response = await fetch(
    'http://localhost:5106/api/transactions',
    {
      method: 'POST',

      headers: {
        'Content-Type': 'application/json',
      },

      // Convert JavaScript object into JSON
      body: JSON.stringify({
        bankAccountId: bankAccountId.value,
        bookingDate: bookingDate.value,
        description: description.value,
        amount: amount.value,
        currency: currency.value,
      }),
    },
  )

  // Continue only if transaction was created successfully
  if (response.ok) {
    // Clear Create Transaction form
    bankAccountId.value = null
    bookingDate.value = ''
    description.value = ''
    amount.value = 0
    currency.value = 'CHF'

    // If a Bank Account filter is active,
    // reload only transactions for that Bank Account
    if (selectedBankAccountId.value !== null) {
      await loadTransactionsByBankAccount()
    } else {
      await loadTransactions()
    }
  }
}

// START EDIT TRANSACTION
function startEdit(transaction) {
  // Store the ID of the transaction that is currently being edited
  editingTransactionId.value = transaction.id

  // Copy current transaction data into the Edit form
  editBankAccountId.value = transaction.bankAccountId

  // Convert date to YYYY-MM-DD for HTML date input
  editBookingDate.value = transaction.bookingDate.substring(0, 10)

  editDescription.value = transaction.description
  editAmount.value = transaction.amount
  editCurrency.value = transaction.currency
}

// UPDATE TRANSACTION
async function updateTransaction(id) {
  // Send PUT request to backend
  const response = await fetch(
    `http://localhost:5106/api/transactions/${id}`,
    {
      method: 'PUT',

      headers: {
        'Content-Type': 'application/json',
      },

      // Send updated transaction data
      body: JSON.stringify({
        bankAccountId: editBankAccountId.value,
        bookingDate: editBookingDate.value,
        description: editDescription.value,
        amount: editAmount.value,
        currency: editCurrency.value,
      }),
    },
  )

  // Continue only if update was successful
  if (response.ok) {
    // Close Edit mode
    editingTransactionId.value = null

    // Keep current filter if one is active
    if (selectedBankAccountId.value !== null) {
      await loadTransactionsByBankAccount()
    } else {
      await loadTransactions()
    }
  }
}

// CANCEL EDIT
function cancelEdit() {
  // null means no transaction is currently being edited
  editingTransactionId.value = null
}

// DELETE TRANSACTION
async function deleteTransaction(id) {
  // Send DELETE request to backend
  const response = await fetch(
    `http://localhost:5106/api/transactions/${id}`,
    {
      method: 'DELETE',
    },
  )

  // Continue only if delete was successful
  if (response.ok) {
    // Keep current filter if one is active
    if (selectedBankAccountId.value !== null) {
      await loadTransactionsByBankAccount()
    } else {
      await loadTransactions()
    }
  }
}

// VUE LIFECYCLE
// Load all transactions when the page is opened
onMounted(() => {
  loadTransactions()
})
</script>


<template>
  <v-container>
    <h1>Transactions</h1>

    <!-- CREATE TRANSACTION FORM -->
  
    <v-card class="mb-6">
      <v-card-title>
        Create Transaction
      </v-card-title>

      <v-card-text>

        <!-- Bank Account ID -->
        <v-text-field
          v-model.number="bankAccountId"
          label="Bank Account ID"
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
        <v-text-field
          v-model="currency"
          label="Currency"
        />

      </v-card-text>

      <v-card-actions>

        <!-- Create new transaction -->
        <v-btn
          color="primary"
          @click="createTransaction"
        >
          Create Transaction
        </v-btn>

      </v-card-actions>
    </v-card>

    <!-- FILTER TRANSACTIONS BY BANK ACCOUNT -->

    <v-card class="mb-6">
      <v-card-title>
        Transactions by Bank Account
      </v-card-title>

      <v-card-text>

        <!-- Bank Account ID used for filtering -->
        <v-text-field
          v-model.number="selectedBankAccountId"
          label="Bank Account ID"
        />

      </v-card-text>

      <v-card-actions>

        <!-- Load transactions for one Bank Account -->
        <v-btn
          color="primary"
          @click="loadTransactionsByBankAccount"
        >
          Load Transactions
        </v-btn>

        <!-- Remove filter and show everything -->
        <v-btn
          @click="showAllTransactions"
        >
          Show All
        </v-btn>

      </v-card-actions>
    </v-card>

    <!-- READ / DISPLAY TRANSACTIONS -->

    <v-card
      v-for="transaction in transactions"
      :key="transaction.id"
      class="mb-4"
    >

      <!-- NORMAL VIEW -->

      <template v-if="editingTransactionId !== transaction.id">

        <v-card-title>
          {{ transaction.description }}
        </v-card-title>

        <v-card-text>

          <div>
            Transaction ID: {{ transaction.id }}
          </div>

          <div>
            Bank Account ID: {{ transaction.bankAccountId }}
          </div>

          <div>
            Booking Date: {{ transaction.bookingDate.substring(0, 10) }}
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

          <!-- Open Edit mode -->
          <v-btn
            color="primary"
            @click="startEdit(transaction)"
          >
            Edit
          </v-btn>

          <!-- Delete transaction -->
          <v-btn
            color="error"
            @click="deleteTransaction(transaction.id)"
          >
            Delete
          </v-btn>

        </v-card-actions>
      </template>

      <!-- EDIT VIEW -->

      <template v-else>

        <v-card-title>
          Edit Transaction
        </v-card-title>

        <v-card-text>

          <!-- Edit Bank Account ID -->
          <v-text-field
            v-model.number="editBankAccountId"
            label="Bank Account ID"
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
          <v-text-field
            v-model="editCurrency"
            label="Currency"
          />

        </v-card-text>

        <v-card-actions>

          <!-- Save updated transaction -->
          <v-btn
            color="primary"
            @click="updateTransaction(transaction.id)"
          >
            Save
          </v-btn>

          <!-- Cancel editing -->
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
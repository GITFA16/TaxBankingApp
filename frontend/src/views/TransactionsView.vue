<script setup>
import { onMounted, ref } from 'vue'

// Reactive variable that stores the list of transactions
const transactions = ref([])

// Reactive variables for the Create Transaction form
const bankAccountId = ref(null)
const bookingDate = ref('')
const description = ref('')
const amount = ref(0)
const currency = ref('CHF')

// Reactive variables for the Edit Transaction form
const editingTransactionId = ref(null)
const editBankAccountId = ref(null)
const editBookingDate = ref('')
const editDescription = ref('')
const editAmount = ref(0)
const editCurrency = ref('CHF')


// READ TRANSACTIONS
async function loadTransactions() {
  // Frontend sends a GET request to the backend API
  const response = await fetch('http://localhost:5106/api/transactions')

  // Only update the transaction list if the request was successful
  if (response.ok) {
    transactions.value = await response.json()
  }
}


// CREATE TRANSACTION
async function createTransaction() {
  // Frontend sends a POST request to create a new transaction
  const response = await fetch(
    'http://localhost:5106/api/transactions',
    {
      method: 'POST',

      headers: {
        'Content-Type': 'application/json',
      },

      // SuggestedTaxCategory is NOT entered manually.
      // The backend TaxCategoryService calculates it automatically.
      body: JSON.stringify({
        bankAccountId: bankAccountId.value,
        bookingDate: bookingDate.value,
        description: description.value,
        amount: amount.value,
        currency: currency.value,
      }),
    },
  )

  // Continue only if the transaction was created successfully
  if (response.ok) {
    // Clear the Create Transaction form
    bankAccountId.value = null
    bookingDate.value = ''
    description.value = ''
    amount.value = 0
    currency.value = 'CHF'

    // Reload transactions so the new transaction appears immediately
    await loadTransactions()
  }
}


// START EDIT TRANSACTION
function startEdit(transaction) {
  // Store the ID of the transaction that is currently being edited
  editingTransactionId.value = transaction.id

  // Copy the current transaction data into the Edit form
  editBankAccountId.value = transaction.bankAccountId
  editBookingDate.value = transaction.bookingDate.substring(0, 10)
  editDescription.value = transaction.description
  editAmount.value = transaction.amount
  editCurrency.value = transaction.currency
}


// UPDATE TRANSACTION
async function updateTransaction(id) {
  // Frontend sends a PUT request using the selected transaction ID
  const response = await fetch(
    `http://localhost:5106/api/transactions/${id}`,
    {
      method: 'PUT',

      headers: {
        'Content-Type': 'application/json',
      },

      // The backend recalculates SuggestedTaxCategory
      // based on the updated description.
      body: JSON.stringify({
        bankAccountId: editBankAccountId.value,
        bookingDate: editBookingDate.value,
        description: editDescription.value,
        amount: editAmount.value,
        currency: editCurrency.value,
      }),
    },
  )

  // Continue only if the update was successful
  if (response.ok) {
    // Close Edit mode
    editingTransactionId.value = null

    // Reload transactions so the updated data appears immediately
    await loadTransactions()
  }
}


// CANCEL EDIT
function cancelEdit() {
  // null means that no transaction is currently being edited
  editingTransactionId.value = null
}


// DELETE TRANSACTION
async function deleteTransaction(id) {
  // Frontend sends a DELETE request using the selected transaction ID
  const response = await fetch(
    `http://localhost:5106/api/transactions/${id}`,
    {
      method: 'DELETE',
    },
  )

  // Reload transactions so the deleted transaction disappears immediately
  if (response.ok) {
    await loadTransactions()
  }
}


// VUE LIFECYCLE
// Load all transactions when this component is mounted
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
        <!-- Calls createTransaction() when clicked -->
        <v-btn
          color="primary"
          @click="createTransaction"
        >
          Create Transaction
        </v-btn>
      </v-card-actions>
    </v-card>


    <!-- READ / DISPLAY TRANSACTIONS -->
    <!-- Create one card for every transaction -->
    <v-card
      v-for="transaction in transactions"
      :key="transaction.id"
      class="mb-4"
    >

      <!-- NORMAL VIEW -->
      <!-- Show this section when the transaction is not being edited -->
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
            Booking Date: {{ transaction.bookingDate }}
          </div>

          <div>
            Amount: {{ transaction.amount }} {{ transaction.currency }}
          </div>

          <div>
            Tax Category: {{ transaction.suggestedTaxCategory }}
          </div>
        </v-card-text>

        <v-card-actions>
          <!-- Start Edit mode -->
          <v-btn
            color="primary"
            @click="startEdit(transaction)"
          >
            Edit
          </v-btn>

          <!-- Delete the selected transaction -->
          <v-btn
            color="error"
            @click="deleteTransaction(transaction.id)"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </template>


      <!-- EDIT VIEW -->
      <!-- Show this section when the selected transaction is being edited -->
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
          <!-- Save the updated transaction -->
          <v-btn
            color="primary"
            @click="updateTransaction(transaction.id)"
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
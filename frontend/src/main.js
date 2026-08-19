import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

// Vuetify styles
import 'vuetify/styles'

// Import Vuetify
import { createVuetify } from 'vuetify'

// Import all Vuetify components
import * as components from 'vuetify/components'

// Import all Vuetify directives
import * as directives from 'vuetify/directives'

// Material Design Icons
import '@mdi/font/css/materialdesignicons.css'


// Create Vuetify instance
const vuetify = createVuetify({
  components,
  directives,
})


const app = createApp(App)

app.use(router)
app.use(vuetify)

app.mount('#app')
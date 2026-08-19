import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

// Vuetify
import 'vuetify/styles'
import { createVuetify } from 'vuetify'

// Material Design Icons
import '@mdi/font/css/materialdesignicons.css'

const vuetify = createVuetify()

const app = createApp(App)

app.use(router)
app.use(vuetify)

app.mount('#app')

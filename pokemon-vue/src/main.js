// src/main.js
import { createApp } from 'vue';
import PrimeVue from 'primevue/config';

// Theme import
import Aura from '@primeuix/themes/aura';  // Aura theme preset

// PrimeVue CSS: only icons needed explicitly
import 'primeicons/primeicons.css';
import 'primeflex/primeflex.css';


import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import Card from 'primevue/card';

import App from './App.vue';
import router from './router';
import { createPinia } from 'pinia';

const app = createApp(App);

app.use(createPinia());
app.use(router);

// Pass the theme preset to PrimeVue
app.use(PrimeVue, {
  theme: { preset: Aura },
  ripple: true  // Optional UX enhancement
});

// Register components globally
app.component('Button', Button);
app.component('InputText', InputText);
app.component('Card', Card);

app.mount('#app');

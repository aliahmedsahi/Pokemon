<template>
 <div class="lg-auth-wrapper">
  <div class="lg-auth-container">
    <!-- Left Panel - Login Form -->
    <div class="lg-login-panel">
      <h1 class="lg-login-title">Sign in to Pokémon</h1>
      <span class="lg-divider">Use your email account:</span>
      <div class="mb-2 w-full">
        <InputText v-model="email" placeholder="Email" class="lg-input-text" />
      </div>
      <div class="p-field w-full relative">
        <InputText
          v-model="password"
          :type="showPassword ? 'text' : 'password'"
          placeholder="Password"
          class="w-full pr-10"
        />
        <i
          :class="['pi', showPassword ? 'pi-eye-slash' : 'pi-eye', 'lg-eye-icon']"
          @click="tooglePasswordVisibility()"
        ></i>
      </div>
      <div v-if="loginError" class="text-warning">
        {{ loginError }}
      </div>
      <Button label="Sign In" class="lg-login-button" @click="login" />
    </div>

    <!-- Right Panel - Sign Up Prompt -->
    <div class="lg-signup-panel">
      <h2 class="mb-2">Hello, Danish!</h2>
      <p>Enter your personal details and <br />start journey with us</p>
      <Button label="Sign Up" class="lg-white-btn" @click="goToSignup" />
    </div>
  </div>
</div>

</template>


<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import api from '@/services/api';

const email = ref('');
const password = ref('');
const showPassword = ref(false);
const loginError = ref('');
const auth = useAuthStore();
const router = useRouter();

async function login() {
  loginError.value = '';

  if (!email.value || !password.value) {
    loginError.value = 'Email and password are required.';
    return;
  }

  try {
    const res = await api.post('/auth/login', {
      email: email.value,
      password: password.value
    });
    auth.setToken(res.data.token);
    router.push('/search');
  } catch (error) {
    if (error.response?.data) {
      loginError.value = error.response.data;
    } else {
      loginError.value = 'Login failed.';
    }
  }
}

function goToSignup() {
  router.push('/sign-up');
}

function tooglePasswordVisibility() {
    showPassword.value = !showPassword.value;
}
</script>

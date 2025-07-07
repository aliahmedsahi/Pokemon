<template>
  <div class="sp-auth-wrapper">
    <div class="sp-auth-container">
      <!-- Left Panel - Welcome Back -->
      <div class="sp-login-panel">
        <h2>Welcome Back!</h2>
        <p>To keep connected with us please<br />login with your personal info</p>
        <Button label="Sign In" class="sp-white-btn" @click="goToLogin" />
      </div>

      <!-- Right Panel - Create Account -->
      <div class="sp-signup-panel">
        <h1 class="sp-h1">Create Account</h1>
        <span class="sp-divider">Use your email for registration:</span>

        <div class="p-field w-full mb-2">
          <InputText v-model="email" placeholder="Email" class="w-full" />
          
        </div>
        <div class="p-field w-full relative">
            <InputText
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="Password"
                class="w-full pr-10"
            />
            <i
                :class="['pi', showPassword ? 'pi-eye-slash' : 'pi-eye', 'sp-eye-icon']"
                @click="tooglePasswordVisibility()"
                ></i>
        </div>

        <p v-if="validationError" class="text-warning">{{ validationError }}</p>
        <Button label="Sign Up" class="sp-login-button mt-3" @click="register" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';

const email = ref('');
const password = ref('');
const showPassword = ref(false);
const router = useRouter();
const validationError = ref('');


async function register() {
  validationError.value = '';

   if (!email.value || !password.value) {
    validationError.value = 'Email and password are required.';
    return;
  }

   if (!validateEmail(email.value)) {
    validationError.value = 'Please enter a valid email address.';
    return;
  }

   const passwordValidationMsg = validatePassword(password.value);
  if (passwordValidationMsg) {
    validationError.value = passwordValidationMsg;
    return;
  }

  try {
    await api.post('/auth/register', {
      email: email.value,
      password: password.value
    });
    router.push('/login');
  } catch(error) {

    if (error.response?.data?.length > 0 && Array.isArray(error.response.data)) {
      validationError.value = error.response.data.map(err => err.description).join(' ');
    } else if (error.response?.data?.description) {
      validationError.value = error.response.data.description;
    } else {
      validationError.value = 'Registration failed.';
    }

  }
}

function goToLogin() {
  router.push('/login');
}

function validateEmail(email) {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!regex.test(email)) return false;
 
}


function validatePassword(password) {
  if (password.length < 6) {
    return 'Password must be at least 6 characters long.';
  }

  if (!/[A-Z]/.test(password)) {
    return 'Password must have at least one uppercase letter (A-Z).';
  }

  if (!/\d/.test(password)) {
    return 'Password must have at least one digit (0-9).';
  }

  if (!/[^a-zA-Z0-9]/.test(password)) {
    return 'Password must have at least one non-alphanumeric character.';
  }

  return '';
}


function tooglePasswordVisibility() {
    showPassword.value = !showPassword.value;
}
</script>



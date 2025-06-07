<template>
  <div class="login-form">
    <form @submit.prevent="handleLogin">
      <div class="form-group">
        <label for="username">Username:</label>
        <input type="text" id="username" v-model="username" placeholder="Enter your username" required />
      </div>
      <div class="form-group">
        <label for="password">Password:</label>
        <input type="password" id="password" v-model="password" placeholder="Enter your password" required />
      </div>
      <button type="submit">Login</button>
      <router-link to="/forgotpassword">Forgot Password?</router-link>
      <p v-if="errorMessage" style="color:red;">{{ errorMessage }}</p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import authService, { handleLoginSuccess } from '../api/authService.js';
import jwtDecode from 'jwt-decode';

const username = ref('');
const password = ref('');
const errorMessage = ref('');
const router = useRouter();

async function handleLogin() {
  try {
    const response = await authService.login({
      username: username.value,
      password: password.value,
    });
    const token = response.data.token;
    if (token) {
      // Sử dụng input username để lưu vào localStorage
      handleLoginSuccess(response.data, username.value);
      
      const decoded = jwtDecode(token);
      console.log('Decoded token:', decoded);
      const role = decoded.role?.toLowerCase() || '';
      
      if (role === 'admin') router.push('/homeview');
      else if (role === 'student') router.push('/homestudent');
      else if (role === 'tutor') router.push('/hometutor');
      else errorMessage.value = 'Unauthorized';
    }
  } catch (error) {
    errorMessage.value =
      error.response?.data?.message ||
      error.message ||
      'An error occurred during login.';
  }
}
</script>

<style scoped>
.login-form {
  max-width: 400px;
  margin: 50px auto;
  padding: 30px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.login-form:hover {
  transform: translateY(-5px);
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #6a1b9a;
}

input[type="text"],
input[type="password"] {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e91e63;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.3s ease, box-shadow 0.3s ease;
  box-sizing: border-box;
}

input[type="text"]:focus,
input[type="password"]:focus {
  border-color: #9c27b0;
  box-shadow: 0 0 8px rgba(233, 30, 99, 0.5);
  outline: none;
}

button {
  width: 100%;
  padding: 0.75rem;
  font-size: 1.1rem;
  font-weight: bold;
  text-transform: uppercase;
  color: #fff;
  border: none;
  border-radius: 4px;
  background: linear-gradient(45deg, #e91e63, #9c27b0);
  cursor: pointer;
  transition: background 0.3s ease, transform 0.2s ease;
  margin-bottom: 1rem;
}

button:hover {
  background: linear-gradient(45deg, #d81b60, #8e24aa);
  transform: scale(1.02);
}

button:active {
  transform: scale(0.98);
}

router-link {
  display: block;
  text-align: center;
  margin-top: 1rem;
  color: #007bff;
  text-decoration: none;
}

router-link:hover {
  text-decoration: underline;
}
</style>
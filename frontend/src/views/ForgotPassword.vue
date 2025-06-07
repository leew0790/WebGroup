<template>
  <div class="forgot-password">
    <h2>Forgot Password</h2>
    <form @submit.prevent="submitForgotPassword">
      <div class="form-group">
        <label for="email">Email</label>
        <input
          v-model="email"
          type="email"
          id="email"
          required
          placeholder="Enter email"
        />
      </div>
      <button type="submit">Send Link Reset Password</button>
      <p v-if="message" class="success">{{ message }}</p>
      <p v-if="error" class="error">{{ error }}</p>
    </form>
  </div>
</template>

<script>
import { sendForgotPasswordEmail } from "../api/authService";

export default {
  data() {
    return {
      email: "",
      message: "",
      error: "",
    };
  },
  methods: {
    async submitForgotPassword() {
      try {
        const response = await sendForgotPasswordEmail(this.email);
        this.message = "Link Reset Password sended to your email.";
        this.error = "";
      } catch (error) {
        this.error =
          error?.message || "Error.";
        this.message = "";
      }
    },
  },
};
</script>

<style scoped>
.forgot-password {
  max-width: 400px;
  margin: 0 auto;
  padding: 20px;
}
form {
  display: flex;
  flex-direction: column;
}
input {
  padding: 10px;
  margin-top: 8px;
}
button {
  margin-top: 15px;
  padding: 10px;
  cursor: pointer;
}
.success {
  color: green;
}
.error {
  color: red;
}
</style>

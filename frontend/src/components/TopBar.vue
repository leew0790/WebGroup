<template>
  <header class="admin-topbar">
    <button class="logout-btn" @click="logout">Logout</button>
  </header>
</template>

<script>
import logoutService from "../api/logoutService";

export default {
  name: "adminTopbar",
  methods: {
    async logout() {
      const confirmed = confirm("Are you sure you want to logout?");
      if (confirmed) {
        await logoutService.performLogout();
        this.$router.push("/login");
      }
    },
  },
};
</script>

<style scoped>
.admin-topbar {
  position: fixed;
  top: 0;
  left: 250px;
  right: 0;
  height: 50px;
  background-color: #b3e5fc;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  padding: 0 20px;
  z-index: 1000;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

@media (max-width: 768px) {
  .admin-topbar {
    left: 0;
  }
}
.logout-btn {
  padding: 6px 12px;
  font-size: 14px;
  background-color: transparent;
  border: 1px solid #0288d1 ;
  border-radius: 4px;
  color: #e91e63;
  cursor: pointer;
  transition: background 0.2s;
}

.logout-btn:hover {
  background-color: #fce4ec;
}
</style>

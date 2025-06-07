<template>
    <div class="admin-layout">
      <!-- Nút toggle hiển thị trên mobile -->
      <button class="sidebar-toggle" @click="toggleSidebar">☰</button>
  
      <!-- Sidebar Admin -->
      <adminSidebar class="sticky-sidebar" :class="{ open: isSidebarOpen }" />
  
      <!-- Nội dung chính -->
      <div class="main-content">
        <adminTopbar @logout="logout" />
        <div class="content">
          <slot></slot>
        </div>
        <footer class="footer">
          <p>&copy; E-Tutoring Admin Panel</p>
        </footer>
      </div>
    </div>
  </template>
  
  <script>
  import adminSidebar from './SideBar.vue'
  import adminTopbar from './TopBar.vue'
  
  export default {
    name: "adminLayout",
    components: {
      adminSidebar,
      adminTopbar
    },
    data() {
      return {
        isSidebarOpen: false
      };
    },
    methods: {
      toggleSidebar() {
        this.isSidebarOpen = !this.isSidebarOpen;
      },
      logout() {
        localStorage.removeItem("token");
        this.$router.push('/login');
      }
    }
  }
  </script>
  
  <style scoped>
  .admin-layout {
    display: flex;
    min-height: 100vh;
    overflow: hidden;
  }
  
  .main-content {
    margin-left: 220px;
    flex: 1;
    display: flex;
    flex-direction: column;
    min-height: 100vh;
    overflow: hidden;
    position: relative;
  }
  
  .content {
    flex: 1;
    padding: 20px;
    overflow-y: auto;
    background-color: #fefefe;
  }
  
  .footer {
    background-color: #b3e5fc;
    text-align: center;
    padding: 10px;
    font-size: 0.9rem;
    color: #01579b;
    border-top: 1px solid #ccc;
  }
  
  /* Sticky Sidebar */
  .sticky-sidebar {
    position: fixed;
    top: 0;
    left: 0;
    width: 220px;
    height: 100vh;
    z-index: 100;
    background-color: #b3e5fc;
    box-shadow: 2px 0 8px rgba(0, 0, 0, 0.05);
    overflow-y: auto;
    transition: left 0.3s ease;
  }
  
  /* Responsive */
  .sidebar-toggle {
  display: none;
  position: fixed;
  top: 16px;
  left: 16px;
  z-index: 1100; /* ✅ Đặt cao hơn topbar */
  background-color: #0288d1;
  color: white;
  font-size: 24px;
  padding: 8px 14px;
  border: none;
  border-radius: 8px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
}

  @media (max-width: 768px) {
    .admin-layout {
      flex-direction: column;
    }
  
    .sidebar-toggle {
      display: block;
    }
  
    .sticky-sidebar {
      position: fixed;
      left: -100%;
      top: 0;
      transition: left 0.3s ease;
    }
  
    .sticky-sidebar.open {
      left: 0;
    }
  
    .main-content {
      margin-left: 0;
    }
  }
  </style>
  
<template>
  <div class="user-layout">
    <button class="sidebar-toggle" @click="toggleSidebar">☰</button>
    <userSidebar class="sticky-sidebar" :class="{ open: isSidebarOpen }" />
    <div class="main-content">
      <userTopbar @logout="logout" />
      <div class="content">
        <slot></slot>
      </div>
      <footer class="footer">
        <p>&copy; Website E-Tutoring</p>
      </footer>
    </div>
  </div>
</template>

<script>
import userSidebar from './userSidebar.vue';
import userTopbar from './userTopbar.vue';

export default {
  name: "userLayout",
  components: {
    userSidebar,
    userTopbar
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
};
</script>

<style scoped>
.user-layout {
  display: flex;
  min-height: 100vh;
  overflow: hidden;
}

.main-content {

  margin-left: 210px;
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
  background-color: #fff;
}

.footer {
  background-color: #eee;
  text-align: center;
  padding: 10px;
  font-size: 0.9rem;
  color: #555;
  border-top: 1px solid #ccc;
}

/* Sticky Sidebar */
.sticky-sidebar {

  position: fixed;         
  top: 0;
  left: 0;
  width: 200px;            
  height: 100vh;
  z-index: 100;
  background-color: #ffe4ec; 
  box-shadow: 2px 0 8px rgba(0, 0, 0, 0.05);
  overflow-y: auto;

}


.sticky-topbar {
  position: sticky;
  top: 0;
  z-index: 200;
  background-color: white;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
}

/* Responsive */
@media (max-width: 768px) {
  .user-layout {
    flex-direction: column;
  }

  .sticky-sidebar {
    position: static;
    height: auto;
  }
}
.sidebar-toggle {
  display: none;
  position: fixed; /* chuyển từ absolute -> fixed */
  top: 16px;
  left: 16px;
  z-index: 999; /* đặt cao hơn sidebar/nav */
  background-color: #d86ba4;
  border: none;
  color: white;
  font-size: 24px;
  padding: 8px 14px;
  border-radius: 8px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
}


@media (max-width: 768px) {
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

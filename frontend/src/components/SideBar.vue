<template>
  <aside class="sidebar" :class="{ open: isOpen }">
    <div class="avatar">
      <img src="../assets/admin_icon.png" alt="Admin Avatar" />
    </div>
    <nav class="nav-links">
      <router-link
        v-for="(item, index) in navItems"
        :key="index"
        :to="item.path"
        class="nav-btn"
        active-class="active-link"
      >
        {{ item.label }}
      </router-link>
    </nav>
  </aside>
</template>

<script>
export default {
  name: 'adminSidebar',
  props: {
    isOpen: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      navItems: [
        { label: 'Home', path: '/homeview' },
        { label: 'Student', path: '/manageStudent' },
        { label: 'Tutor', path: '/manageTutor' },
        { label: 'Class Room', path: '/homeclassroom' },
        { label: 'Blog', path: '/manageblog' },
        { label: 'Class', path: '/homeclass' },
        { label: 'Subject', path: '/homeSubject' },
        { label: 'Schedules', path: '/homeschedule' }
        
      ]
    };
  }
};
</script>

<style scoped>
.sidebar {
  position: fixed;
  top: 0;
  left: 0;
  height: 100vh;
  width: 220px;
  background-color: #b3e5fc; /* pastel blue */
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1.5rem 1rem;
  z-index: 100;
  box-shadow: 2px 0 8px rgba(0, 0, 0, 0.05);
  overflow-y: auto;
  transition: left 0.3s ease;
}

.avatar img {
  width: 100px;
  height: 100px;
  
  margin-bottom: 2rem;
  border-radius: 50%;
  object-fit: cover;
}

.nav-links {
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 1rem;
  
}

.nav-btn {
  display: block;
  text-align: center;
  background-color: #0288d1; /* blue */
  color: white;
  padding: 10px;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 500;
  transition: background 0.3s ease, transform 0.2s;
}

.nav-btn:hover {
  background-color: #0277bd;
  transform: scale(1.03);
}

.active-link {
  background-color: #01579b;
}

/* Responsive */
@media (max-width: 768px) {
  .sidebar {
    left: -100%;
    width: 65%;
    max-width: 320px;
    padding-top: 70px;
  }

  .sidebar.open {
    left: 0;
  }

  .nav-links {
    width: 90%;
    gap: 1rem;
  }

  .nav-btn {
    width: 100%;
    padding: 12px 0;
    font-size: 1rem;
  }

  .avatar {
    margin-bottom: 1rem;
  }
}
</style>

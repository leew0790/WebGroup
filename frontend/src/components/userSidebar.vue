<template>

  <aside class="sidebar">
    <div class="avatar">
      <img :src="roleIcon" alt="user icon" />
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
  name: 'userSidebar',
  computed: {
    role() {

    const role = localStorage.getItem("role");
    return role ? role.toLowerCase() : "student";
  },

    roleIcon() {
      return this.role === 'tutor'
        ? new URL('../assets/tutor_icon.png', import.meta.url).href
        : new URL('../assets/student_icon.jpg', import.meta.url).href;
    },
    navItems() {
      return [
        {
          label: "Home",
          path: this.role === "student" ? "/homestudent" : "/hometutor"
        },
        { label: "Schedule", path: "/schedulerforuser" },
        { label: "Chat", path: "/classlist" },
        { label: "Blog", path: "/manageblog" },
        { label: "Profile", path: "/profile" },
      ];
    }

  }
}
</script>


<style scoped>
.sidebar {
  position: fixed;
  top: 0;
  left: 0;
  height: 100vh;
  width: 200px;
  background-color: #ffe4ec;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1.5rem 1rem;
  z-index: 100;
  box-shadow: 2px 0 8px rgba(0, 0, 0, 0.05);
  overflow-y: auto;
  transition: left 0.3s ease;
}
.main-content {
  margin-left: 200px;
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.avatar img {

  width: 100px;
  height: 100px;
  margin-bottom: 2rem;
  border-radius: 50%;
  object-fit: cover;
  margin-top: 2rem;

}

.nav-links {
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 1rem;
  margin-top: 35px;

}

.nav-btn {
  display: block;
  text-align: center;
  background-color: #d86ba4; /* Hồng tím đậm */
  color: white;
  padding: 10px;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 500;
  transition: background 0.3s ease, transform 0.2s;
}

.nav-btn:hover {
  background-color: #c15894;
  transform: scale(1.03);
}

.active-link {
  background-color: #b24683;
}

@media (max-width: 768px) {
  .sidebar {
    left: -100%;
    width: 65%; /* ✅ Tăng lên từ 55% */
    max-width: 320px; /* ✅ Giới hạn tối đa */
    height: 100vh;
    flex-direction: column;
    align-items: center;
    padding-top: 70px; /* đẩy nội dung xuống dưới toggle */
    box-sizing: border-box;
    overflow-x: hidden;
  }

  .sidebar.open {
    left: 0;
  }

  .nav-links {
    width: 90%; /* hoặc 100% với padding 2 bên */
    padding: 0;
    box-sizing: border-box;
    gap: 1rem;
  }

  .nav-btn {
    width: 100%;
    padding: 12px 0;
    font-size: 1rem;
    text-align: center;
  }

  .avatar {
    display: block;
    margin-bottom: 1rem;
  }
}

</style>
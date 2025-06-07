<template>
  <header class="user-topbar">
    <div class="right-actions">
      <div class="notification-wrapper" @click="toggleNotificationPanel">
        📩
        <span v-if="unreadCount > 0" class="dot">{{ unreadCount }}</span>
      </div>
      <button @click="logout" class="logout-btn">Logout</button>
    </div>

    <div v-if="showNotifications" class="notification-panel">
      <div v-for="(noti, index) in notifications" :key="noti.id" class="notification-item"
        :class="{ unread: !noti.isRead }" @click="markAsRead(noti, index)">
        {{ noti.message }}
        <small>{{ formatDate(noti.createdAt) }}</small>
      </div>
    </div>
  </header>
</template>

<script>
import logoutService from "../api/logoutService";
import notificationService from "../api/notificationService";
import { connectToNotificationHub, stopNotificationHub } from "../api/notificationHub";

export default {
  name: "userTopbar",
  data() {
    return {
      notifications: [],
      showNotifications: false,
    };
  },
  computed: {
    unreadCount() {
      return this.notifications.filter(n => !n.isRead).length;
    }
  },
  mounted() {
    this.fetchNotifications();
    const token = localStorage.getItem("token");
    if (token) {
      connectToNotificationHub(token, this.handleRealtimeNotification);
    }
    window.addEventListener("update-notifications", this.updateNotificationList);
    window.addEventListener("clear-notification-from-user", this.clearNotiFromUser)
  },
  beforeUnmount() {
    stopNotificationHub();
    window.removeEventListener("update-notifications", this.updateNotificationList);
    window.removeEventListener("clear-notification-from-user", this.clearNotiFromUser);
  },
  methods: {

    updateNotificationList(e) {
      this.notifications = e.detail;
    },
    async logout() {
      const confirmed = window.confirm("Are you sure to logout?");
      if (confirmed) {
        await logoutService.performLogout();
        this.$router.push("/login");
      }
    },
    async fetchNotifications() {
      const result = await notificationService.getNotifications();
      if (result) {
        this.notifications = result;
      }
    },
    toggleNotificationPanel() {
      this.showNotifications = !this.showNotifications;
      // Chỉ fetch nếu chưa có dữ liệu
      if (this.showNotifications && this.notifications.length === 0) {
        this.fetchNotifications();
      }
    },
    async markAsRead(noti, index) {
      if (!noti.isRead) {
        await notificationService.markAsRead(noti.id);

        this.notifications[index].isRead = true;

      }
      // ✅ Xử lý redirect theo nội dung
      if (noti.message.includes("bình luận")) {
        this.$router.push("/manageblog");
      } else if (noti.message.includes("tin nhắn mới")) {
        this.$router.push("/classlist");
      } else {
        this.$router.push("/notification");
      }
    },
    handleRealtimeNotification(noti) {

      const newNoti = {
        id: noti.id || Math.random(),
        message: noti.message,
        createdAt: new Date(noti.createdAt || noti.time),
        isRead: false,
        senderId: noti.senderId || null,
      };

      // Gán lại mảng mới để Vue reactivity nhận thay đổi
      this.notifications = [newNoti, ...this.notifications];
    },
    formatDate(date) {
      const d = new Date(date);
      return isNaN(d.getTime())
        ? "N/A"
        : d.toLocaleString("vi-VN", {
          hour: "2-digit",
          minute: "2-digit",
          hour12: false,
          day: "2-digit",
          month: "2-digit",
          year: "numeric",
          timeZone: "Asia/Ho_Chi_Minh",
        });
    },
    async clearNotiFromUser(e) {
      const senderId = e.detail.senderId;

      const toMark = this.notifications.filter(noti =>
        !noti.isRead &&
        noti.senderId === senderId   // ✅ so sánh đúng theo object, không dùng message
      );

      for (const noti of toMark) {
        await notificationService.markAsRead(noti.id);
        noti.isRead = true;
      }

      if (toMark.length > 0) {
        this.notifications = [...this.notifications]; // kích hoạt reactive
      }
    }
  },

};
</script>

<style scoped>
.user-topbar {

  position: fixed;
  top: 0;
  left: 240px;
  right: 0;
  height: 50px;
  /* tăng chiều cao để đẹp hơn */
  background-color: #eec5e0;
  color: rgb(224, 11, 103);
  padding: 0 20px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
  z-index: 1000;
}

.right-actions {
  display: flex;
  align-items: center;
  gap: 20px;
  /* 👈 khoảng cách rõ ràng giữa icon và logout */
}

.notification-wrapper {
  position: relative;
  font-size: 22px;
  cursor: pointer;
}

.logout-btn {
  padding: 6px 12px;
  font-size: 14px;
  background-color: transparent;
  border: 1px solid #e91e63;
  border-radius: 4px;
  color: #e91e63;
  cursor: pointer;
  transition: background 0.2s;
}

.logout-btn:hover {
  background-color: #fce4ec;
}

.dot {
  position: absolute;
  top: -2px;
  right: -5px;
  width: 10px;
  height: 10px;
  background: red;
  border-radius: 50%;
}

.notification-panel {
  position: absolute;
  top: 60px;
  right: 20px;
  width: 300px;
  max-height: 300px;
  overflow-y: auto;
  background: #fff;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  border-radius: 6px;
  z-index: 1200;
  padding: 10px;
}

.right-actions {
  display: flex;
  align-items: center;
  gap: 50px;
}

.notification-item {
  padding: 10px;
  border-bottom: 1px solid #eee;
  cursor: pointer;
  transition: background 0.3s;
}

.notification-item.unread {
  font-weight: bold;
  background-color: #fff0f5;
  color: #ad1457;
}



.notification-wrapper {
  position: relative;
  font-size: 22px;
  cursor: pointer;
}

.dot {
  position: absolute;
  top: -6px;
  right: -10px;
  background: red;
  color: white;
  font-size: 12px;
  font-weight: bold;
  padding: 2px 6px;
  border-radius: 12px;
  line-height: 1;
  min-width: 18px;
  text-align: center;
}

.notification-panel {
  position: absolute;
  top: 60px;
  right: 20px;
  width: 300px;
  max-height: 300px;
  overflow-y: auto;
  background: #fff;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  border-radius: 6px;
  z-index: 1200;
  padding: 10px;
}

.notification-item {
  padding: 10px;
  border-bottom: 1px solid #eee;
  cursor: pointer;
}

.notification-item.unread {
  font-weight: bold;
  background-color: #eec5e0;
  color: rgb(224, 11, 103);
  padding: 10px;
  text-align: right;

}
</style>

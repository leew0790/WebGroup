<template>
  <UserLayout>
    <template #default>
      <div class="main-chat-layout">
        <!-- Danh sách lớp -->
        <div class="class-list">
          <h2>My Classes</h2>
          <div
            v-for="classItem in myClasses"
            :key="classItem.id"
            class="class-item"
          >
            <div @click="toggleDetail(classItem.id)" class="class-header">
              {{ classItem.className }}
            </div>

            <!-- Chi tiết lớp -->
            <div v-if="selectedClassId === classItem.id" class="class-detail">
              <!-- Hiển thị tutor nếu bạn là Student và không phải chính bạn -->
              <p
                v-if="role === 'Student' && classItem.tutorUserId !== currentUserId"
              >
                <strong>Tutor:</strong>
                <button
                  v-if="classItem.tutorId"
                  class="tutor-btn"
                  @click.stop="openChat(classItem.tutorUserId, classItem.tutorName)"

                >
                  {{ classItem.tutorName }}
                </button>
              </p>

              <h3>Students</h3>
                <div class="student-list">
                  <button
                    v-for="(stu, index) in filteredStudents(classItem)"
                    :key="stu.userId"
                    class="student-item"
                    @click.stop="openChat(stu.userId, stu.name)"
                  >
                    {{ stu.name }}
                  </button>
                </div>
            </div>
          </div>
        </div>

        <!-- Hộp chat -->
        <div class="chat-container" v-if="selectedUserId">
          <ChatDialog
            :receiverId="selectedUserId"
            :receiverName="selectedUserName"
          />
        </div>
      </div>
    </template>
  </UserLayout>
</template>

<script>
import classService from "../../api/classService";
import jwtDecode from "jwt-decode";
import UserLayout from "../../components/userLayout.vue";
import ChatDialog from "./ChatDialog.vue";

export default {
  name: "ClassList",
  components: { UserLayout, ChatDialog },
  data() {
    return {
      myClasses: [],
      selectedClassId: null,
      selectedUserId: null,
      selectedUserName: "",
      currentUserId: null,
      role: "", // Student hoặc Tutor
    };
  },
  mounted() {
    const token = localStorage.getItem("token");
    if (!token) return alert("Bạn chưa đăng nhập!");
    const decoded = jwtDecode(token);
    this.currentUserId = decoded.nameid;
    this.role = decoded.role;

    const loadFunc =
      decoded.StudentId !== undefined
        ? classService.loadMyClasses
        : classService.loadMyTutorClasses;

    loadFunc()
      .then((classes) => {
        this.myClasses = classes;
      })
      .catch((err) => {
        console.error("Lỗi tải lớp:", err);
      });
  },
  methods: {
    toggleDetail(classId) {
      this.selectedClassId = this.selectedClassId === classId ? null : classId;
    },
    openChat(userId, name) {
      if (!userId) {
        alert("Không có thông tin để mở chat.");
        return;
      }
      this.selectedUserId = userId;
      this.selectedUserName = name;
    },
    filteredStudents(classItem) {
      return classItem.studentNames
        .map((name, index) => ({
          name,
          userId: classItem.studentUserIds[index],
        }))
        .filter((stu) => {
          // Nếu bạn là Student, ẩn chính mình khỏi danh sách
          if (this.role === 'Student') {
            return stu.userId !== this.currentUserId;
          }
          return true;
        });
    }
  }

};
</script>

<style scoped>
.main-chat-layout {
  display: flex;
  flex-direction: row;
  gap: 24px;
  padding: 20px;
  flex-wrap: wrap;
}

.class-list {
  flex: 0 0 300px;
  font-family: "Segoe UI", sans-serif;
}

.class-item {
  background-color: #fff;
  border: 1px solid #ddd;
  border-radius: 10px;
  margin-bottom: 10px;
  padding: 14px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
}

.class-header {
  font-size: 18px;
  font-weight: 600;
  color: #4b3d73;
  margin-bottom: 10px;
  cursor: pointer;
}

.class-detail {
  margin-top: 12px;
  padding: 16px;
  background-color: #f9f9fb;
  border-left: 4px solid #7a5fac;
  border-radius: 6px;
  display: inline-block;
  max-width: 100%;
  min-width: 150px;
}

.class-detail h3 {
  margin-top: 12px;
  font-size: 15px;
  color: #5a497d;
}

.student-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 8px;
}

.student-item,
.tutor-btn {
  border: none;
  border-radius: 6px;
  padding: 8px 14px;
  font-size: 14px;
  background-color: #e2e2ff;
  cursor: pointer;
  transition: background 0.2s ease;
  white-space: nowrap;
}

.student-item:hover,
.tutor-btn:hover {
  background-color: #c7c7f8;
}

.chat-container {
  flex: 1;
  min-width: 300px;
  padding: 10px;
  box-sizing: border-box;
}

/* Responsive */
@media (max-width: 900px) {
  .main-chat-layout {
    flex-direction: column;
  }

  .class-list,
  .chat-container {
    width: 100%;
  }
}
</style>

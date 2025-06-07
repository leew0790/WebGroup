<template>
  <UserLayout>
    <template #default>
      <div class="profile-container">
        <h2 class="profile-title">👤 Profile</h2>
        <div v-if="profile" class="profile-card">
          <!-- Thông tin chung -->
          <div class="section">
            <p><strong>👤 Full Name:</strong> {{ profile.user?.fullName }}</p>
            <p><strong>👤 UserName:</strong> {{ profile.user?.userName }}</p>
            <p><strong>📧 Email:</strong> {{ profile.user?.email }}</p>
          </div>

          <!-- Nếu là Student -->
          <div class="section" v-if="isStudent && profile.studentCode">
            <h4>🎓 Student </h4>
            <p><strong>StudentCode:</strong> {{ profile.studentCode }}</p>
            <p><strong>Course:</strong> {{ profile.course }}</p>
            <p><strong>Note:</strong> {{ profile.status }}</p>
          </div>

          <!-- Nếu là Tutor -->
          <div class="section" v-else-if="!isStudent && profile.department">
            <h4>📚 Tutor </h4>
            <p><strong>Department:</strong> {{ profile.department }}</p>
            <p><strong>ExperienceYears:</strong> {{ profile.experienceYears }}</p>
            <p><strong>Rate: </strong> {{ profile.rating }}</p>
          </div>
        </div>

        <div v-else class="loading-text">
          Loading profile...
        </div>
      </div>
    </template>
  </UserLayout>
</template>
<script>
import { getUserFromToken } from "../helpers/authHelper";
import { getStudentProfile, getTutorProfile } from "../api/profileService";
import UserLayout from "./userLayout.vue";
export default {
  components: { UserLayout },
  data() {
    return {
      profile: null,
      isStudent: true,
      isAdmin: false,
    };
  },
  async mounted() {
    const token = localStorage.getItem("token"); // Lấy token từ localStorage
    const user = getUserFromToken(token);
    console.log("Token đã giải mã:", user); // Debug token

    if (user) {
      // Sử dụng Object.keys để kiểm tra key trong token
      const keys = Object.keys(user);
      console.log("Danh sách key trong token:", keys);

      // Xử lý theo vai trò
      switch (user.role) {
        case "Student":
          // Xử lý nếu StudentId có chữ hoa
          if (user.StudentId || user.studentId) {
            const studentId = user.StudentId || user.studentId;
            console.log("Đang lấy thông tin student với ID:", studentId);
            this.isStudent = true;
            await this.fetchStudentProfile(studentId, token);
          } else {
            console.error("Không tìm thấy studentId.");
          }
          break;

        case "Tutor":
          if (user.TutorId || user.tutorId) {
            const tutorId = user.TutorId || user.tutorId;
            console.log("Đang lấy thông tin tutor với ID:", tutorId);
            this.isStudent = false;
            await this.fetchTutorProfile(tutorId, token);
          } else {
            console.error("Không tìm thấy tutorId.");
          }
          break;

        case "Admin":
          console.log("Đăng nhập với vai trò admin.");
          this.isAdmin = true;
          this.profile = {
            user: {
              fullName: "Admin",
              userName: "admin",
              email: "admin@example.com",
            },
          };
          break;

        default:
          console.error("Vai trò không hợp lệ.");
      }
    } else {
      console.error("Không tìm thấy thông tin người dùng từ token.");
    }
  },
  methods: {
    // Lấy thông tin Student từ API
    async fetchStudentProfile(studentId, token) {
      try {
        const data = await getStudentProfile(studentId, token);
        console.log("Dữ liệu profile Student:", data);
        this.profile = data;
      } catch (error) {
        console.error("Lỗi khi lấy thông tin học sinh:", error);
      }
    },
    // Lấy thông tin Tutor từ API
    async fetchTutorProfile(tutorId, token) {
      try {
        const data = await getTutorProfile(tutorId, token);
        console.log("Dữ liệu profile Tutor:", data);
        this.profile = data;
      } catch (error) {
        console.error("Lỗi khi lấy thông tin tutor:", error);
      }
    },
  },
};
</script>


<style scoped>
.profile-container {
  max-width: 700px;
  margin: 0 auto;
  padding: 30px 20px;
  font-family: 'Segoe UI', sans-serif;
}

.profile-title {
  text-align: center;
  margin-bottom: 20px;
  color: #4b3d73;
}

.profile-card {
  background: #f9f7fc;
  border: 1px solid #d8c2e0;
  border-radius: 10px;
  padding: 25px 30px;
  box-shadow: 0 4px 10px rgba(80, 67, 130, 0.1);
}

.section {
  margin-bottom: 20px;
}

.section h4 {
  margin-bottom: 10px;
  color: #7c4dff;
}

p {
  margin: 6px 0;
  color: #333;
}

.loading-text {
  text-align: center;
  color: #888;
  font-style: italic;
  margin-top: 40px;
}
</style>
<script setup>
import { ref, watch } from 'vue';
import studentService from '../api/studentService';

const props = defineProps(['editingStudent']);
const emit = defineEmits(['studentUpdated']);

const id = ref(null);
const fullName = ref('');
const studentCode = ref('');
const email = ref('');
const userName = ref('');
const password = ref('');
const course = ref('');
const status = ref('');

watch(() => props.editingStudent, (newVal) => {
  if (newVal) {
    id.value = newVal.id;
    fullName.value = newVal.user?.fullName || '';
    email.value = newVal.user?.email || '';
    userName.value = newVal.user?.userName || '';
    password.value = '';
    studentCode.value = newVal.studentCode || '';
    course.value = newVal.course || '';
    status.value = newVal.status || '';
  }
}, { deep: true });

const submitForm = async () => {
  let updatedStudent = null;

  const studentPayload = {
    FullName: fullName.value,
    StudentCode: studentCode.value,
    Email: email.value,
    UserName: userName.value,
    Password: password.value,
    Course: course.value,
    Status: status.value
  };

  if (id.value) {
    const success = await studentService.updateStudent(id.value, studentPayload);
    if (success) {
      updatedStudent = {
        id: id.value,
        studentCode: studentPayload.StudentCode,
        course: studentPayload.Course,
        status: studentPayload.Status,
        user: {
          fullName: studentPayload.FullName,
          email: studentPayload.Email,
          userName: studentPayload.UserName
        }
      };
    }
  } else {
    const success = await studentService.createStudent(studentPayload);
    if (success) {
      updatedStudent = {
        id: Date.now(), // hoặc ID trả về từ server nếu có
        studentCode: studentPayload.StudentCode,
        course: studentPayload.Course,
        status: studentPayload.Status,
        user: {
          fullName: studentPayload.FullName,
          email: studentPayload.Email,
          userName: studentPayload.UserName
        }
      };
    }
  }

  if (updatedStudent) {
    emit('studentUpdated', updatedStudent);
  }

  // Reset form
  id.value = null;
  fullName.value = '';
  studentCode.value = '';
  email.value = '';
  userName.value = '';
  password.value = '';
  course.value = '';
  status.value = '';
};
</script>

<template>
  <form @submit.prevent="submitForm" class="form-container student-theme">
    <h3>{{ id ? 'Edit Student' : 'Add Student' }}</h3>

    <div class="input-group">
      <label for="fullName" class="input-label">Full Name</label>
      <input id="fullName" v-model="fullName" required placeholder="Full Name" :disabled="id" />
    </div>

    <div class="input-group">
      <label for="studentCode" class="input-label">Student ID</label>
      <input id="studentCode" v-model="studentCode" required placeholder="Student ID" />
    </div>

    <div class="input-group">
      <label for="email" class="input-label">Email</label>
      <input id="email" v-model="email" type="email" required placeholder="Email" :disabled="id" />
    </div>

    <div class="input-group">
      <label for="userName" class="input-label">Username</label>
      <input id="userName" v-model="userName" required placeholder="Username" :disabled="id" />
    </div>

    <div class="input-group">
      <label for="password" class="input-label">Password</label>
      <input id="password" v-model="password" type="password" required placeholder="******" :disabled="id" />
    </div>

    <div class="input-group">
      <label for="course" class="input-label">Course</label>
      <div class="select-group">
        <select id="course" v-model="course" required>
          <option disabled value="">Select Course</option>
          <option value="IT">IT</option>
          <option value="Business">Business</option>
          <option value="Design">Design</option>
        </select>
        <span class="custom-arrow">&#9662;</span>
      </div>
    </div>

    <div class="input-group">
      <label for="status" class="input-label">Status</label>
      <input id="status" v-model="status" required placeholder="Status" />
    </div>

    <button type="submit">{{ id ? 'Update Student' : 'Add Student' }}</button>
  </form>
</template>

<style scoped>
.form-container {
  width: 100%;
  max-width: 600px;
  margin: 40px auto;
  padding: 24px 20px;
  background: #e3f2fd;
  border-radius: 10px;
  box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
}
.input-group {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
}
.input-label {
  flex: 0 0 100px;
  color: #1976d2;
  font-weight: bold;
  text-align: left;
  margin-right: 10px;
  font-size: 14px;
}
.input-group input {
  flex: 1;
  padding: 6px 10px;
  font-size: 14px;
  border: 1px solid #90caf9;
  border-radius: 4px;
  background: #fff;
  outline: none;
  transition: border-color 0.3s ease;
}
.input-group input:focus {
  border-color: #1976d2;
}
.select-group {
  position: relative;
  flex: 1;
}
.select-group select {
  width: 100%;
  padding: 6px 10px;
  font-size: 14px;
  border: 1px solid #90caf9;
  border-radius: 4px;
  background: #fff;
  appearance: none;
  outline: none;
  transition: border-color 0.3s ease;
}
.select-group select:focus {
  border-color: #1976d2;
}
.select-group .custom-arrow {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 12px;
  color: #1976d2;
  pointer-events: none;
}
button {
  margin-top: 10px;
  width: 100%;
  padding: 10px;
  font-size: 14px;
  font-weight: bold;
  background-color: #2196f3;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.3s ease;
}
button:hover {
  background-color: #1976d2;
}
.student-theme {
  border-left: 5px solid #2196f3;
}
</style>

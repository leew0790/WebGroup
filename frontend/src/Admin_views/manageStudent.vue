<script setup>
import { ref, onMounted } from 'vue';
import studentService from '../api/studentService';
import CreateStudentForm from '../components/createStudentForm.vue';
import adminLayout from '../components/adminLayout.vue'; // ✅ layout chuẩn admin

const { deleteStudent } = studentService;
const students = ref([]);
const editingStudent = ref(null);

const loadStudents = async () => {
  try {
    const response = await studentService.getAll();
    students.value = response.data;
  } catch (error) {
    console.error('Error loading students:', error);
    students.value = [];
  }
};

onMounted(() => {
  loadStudents();
});

const removeStudent = async (id) => {
  if (confirm('Are you sure you want to delete this student?')) {
    const success = await deleteStudent(id);
    if (success) {
      students.value = students.value.filter(s => s.id !== id);
    }
  }
};

const editStudent = (student) => {
  editingStudent.value = { ...student };
};

const handleStudentUpdated = (updatedStudent) => {
  const index = students.value.findIndex(s => s.id === updatedStudent.id);
  if (index !== -1) {
    students.value[index] = updatedStudent;
  } else {
    students.value.push(updatedStudent);
  }
};
</script>

<template>
  <adminLayout>
    <div class="student-theme">
      <CreateStudentForm :editingStudent="editingStudent" @studentUpdated="handleStudentUpdated" />

      <table class="data-table">
        <thead>
          <tr>
            <th>FullName</th>
            <th>Student Code</th>
            <th>Email</th>
            <th>Username</th>
            <th>Course</th>
            <th>Status</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="student in students" :key="student.id">
            <td>{{ student.user.fullName }}</td>
            <td>{{ student.studentCode }}</td>
            <td>{{ student.user.email }}</td>
            <td>{{ student.user.userName }}</td>
            <td>{{ student.course }}</td>
            <td>{{ student.status }}</td>
            <td>
              <button class="edit-btn" @click="editStudent(student)">Edit</button>
              <button class="delete-btn" @click="removeStudent(student.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </adminLayout>
</template>

<style scoped>
@import '../assets/tableStyles.css';

.layout-container {
  display: flex;
  height: 100vh;
  overflow-x: hidden;
  background-color: #e3f2fd;
}

.main {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.content {
  flex: 1;
  overflow-y: auto;
  background-color: #e3f2fd; 
  padding: 24px;
  border-radius: 8px;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  background-color: #e3f2fd;
  border: 1px solid #bbdefb;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  margin-top: 30px;
  margin-left: 20px;
}

.data-table th {
  background-color: #bbdefb;
  color: #0d47a1;
  text-align: center;
  padding: 10px;
  font-weight: bold;
}

.data-table td {
  padding: 10px;
  border-bottom: 1px solid #e0e0e0;
  text-align: center;
}

.edit-btn {
  background: #64b5f6;
  color: white;
  border: none;
  padding: 6px 10px;
  border-radius: 4px;
  cursor: pointer;
  margin-right: 4px;
}

.delete-btn {
  background: #ef5350;
  color: white;
  border: none;
  padding: 6px 10px;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  opacity: 0.85;
}
</style>

<template>
  <UserLayout>
    <template #default>
      <h3>📘 My Classes (Student)</h3>
      <div class="search-section">
        <input v-model="searchKeyword" type="text" placeholder="🔍 Search by class name..." class="search-input"
          @keyup.enter="handleSearch" />
        <button class="search-btn" @click="handleSearch">Searching</button>
        <button class="reset-btn" @click="resetSearch">Reload</button>
      </div>
      <table class="class-table">
        <thead>
          <tr>
            <th>#</th>
            <th>Class</th>
            <th>Subject</th>
            <th>Tutor</th>
            <th>Slot</th>
            <th>Start date</th>
            <th>End date</th>
            <th>Description</th>
            <th>Students</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(classItem, index) in myStudentClasses" :key="index">
            <td>{{ index + 1 }}</td>
            <td>{{ classItem.className }}</td>
            <td>{{ classItem.subjectName }}</td>
            <td>{{ classItem.tutorName }}</td>
            <td>{{ classItem.totalSlot }}</td>
            <td>{{ formatDate(classItem.startDate) }}</td>
            <td>{{ formatDate(classItem.endDate) }}</td>
            <td>{{ classItem.description }}</td>
            <td>
              <ul v-if="classItem.studentNames?.length">
                <li v-for="(student, idx) in classItem.studentNames" :key="idx">{{ student }}</li>
              </ul>
              <span v-else>None</span>
            </td>
          </tr>
        </tbody>
      </table>
    </template>
  </UserLayout>
</template>
<script setup>
import { ref, onMounted } from 'vue';
import jwtDecode from 'jwt-decode';
import classService from '../api/classService';
import UserLayout from '../components/userLayout.vue';

const myStudentClasses = ref([]);
const allStudentClasses = ref([]);
const searchKeyword = ref("");

const handleSearch = () => {
  if (!searchKeyword.value.trim()) {
    alert("Enter class name, please!");
    return;
  }

  myStudentClasses.value = allStudentClasses.value.filter(cls =>
    cls.className.toLowerCase().includes(searchKeyword.value.toLowerCase())
  );
};

const resetSearch = () => {
  searchKeyword.value = "";
  myStudentClasses.value = [...allStudentClasses.value];
};

const formatDate = (date) => {
  const d = new Date(date);
  return d.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  });
};

const loadStudentClasses = async () => {
  try {
    const token = localStorage.getItem('token');
    const decoded = jwtDecode(token);
    const studentId = decoded?.StudentId;

    if (!studentId) {
      alert("Not found StudentId in token!");
      return;
    }

    const response = await classService.getAllClasses();
    myStudentClasses.value = response.data.filter((cls) =>
      cls.studentIds.includes(Number(studentId))
    );

    allStudentClasses.value = [...myStudentClasses.value]; // lưu bản sao đầy đủ


  } catch (error) {
    console.error('❌ Error load class:', error);
  }
};

onMounted(loadStudentClasses);
</script>

<style scoped>
.container {
  display: flex;
}

.main-content {
  flex: 1;
  padding: 20px;
}

.class-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  margin-top: 20px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
  border-radius: 10px;
  overflow: hidden;
  font-family: 'Segoe UI', sans-serif;
  background-color: white;
}

.class-table thead th {
  background-color: #D8B2D1;
  /* tím lavender */
  color: #4b3d73;
  /* tím đậm hơn để dễ đọc */
  padding: 14px;
  font-weight: 600;
  text-align: center;
  border-bottom: 1px solid #d6d6f0;
}

/* Dòng dữ liệu */
.class-table tbody td {
  padding: 12px;
  text-align: center;
  vertical-align: middle;
  border-bottom: 1px solid #f0f0f0;
}

/* Zebra stripe */
.class-table tbody tr:nth-child(even) {
  background-color: #f9f7fc;
}

/* Hover */
.class-table tbody tr:hover {
  background-color: #f3e9ff;
  transition: background 0.2s ease;
}

/* Danh sách học sinh */
.class-table ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.table-container {
  overflow-x: auto;
  width: 100%;
}
.search-section {
  display: flex;
  gap: 10px;
  margin: 16px 0;
  justify-content: center;
}

.search-input {
  padding: 8px 12px;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 14px;
  min-width: 200px;
}

.search-btn,
.reset-btn {
  padding: 8px 14px;
  background-color: #7e57c2;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.reset-btn {
  background-color: #9e9e9e;
}

</style>

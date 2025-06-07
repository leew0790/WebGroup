<template>
  <adminLayout>
    <div class="home-class">
      <h2>New Class</h2>
      <form @submit.prevent="handleCreateOrUpdateClass">
        <!-- Thông tin lớp học -->
        <div>
          <label>Class Name:</label>
          <input v-model="classForm.className" type="text" required />
        </div>
        <div>
          <label>Add Tutor:</label>
          <multiselect v-model="selectedTutor" :options="tutors" :multiple="false" label="fullName" track-by="id"
            placeholder="Add Tutor"></multiselect>
        </div>
        <div>
          <label>Add Subject:</label>
          <multiselect v-model="selectedSubject" :options="subjects" :multiple="false" label="subjectName" track-by="id"
            placeholder="Add Subject"></multiselect>
        </div>
        <div>
          <label>Total Slot:</label>
          <input v-model.number="classForm.totalSlot" type="number" required />
        </div>
        <div>
          <label>Start Date:</label>
          <input v-model="classForm.startDate" type="date" required />
        </div>
        <div>
          <label>End Date:</label>
          <input v-model="classForm.endDate" type="date" required />
        </div>
        <div>
          <label>Description:</label>
          <textarea v-model="classForm.description"></textarea>
        </div>

        <!-- Chọn nhiều học sinh từ danh sách load về -->
        <div>
          <label>Add multiple students:</label>
          <multiselect v-model="selectedStudents" :options="students" :multiple="true" label="fullName" track-by="id"
            placeholder="Select student">
            <!-- Hiển thị studentCode + fullName trong dropdown -->
            <template #option="{ option }">
              <div>
                <strong>{{ option.user.fullName }}</strong> -
                <small>{{ option.studentCode }}</small>
              </div>
            </template>

            <!-- Hiển thị khi đã chọn student -->
            <template #tag="{ option, remove }">
              <div class="multiselect__tag">
                {{ option.user.fullName }} ({{ option.studentCode }})
                <span @click.prevent="remove(option)">❌</span>
              </div>
            </template>

            <!-- Khi chỉ chọn 1 item -->
            <template #singleLabel="{ option }">
              <div>
                {{ option.user.fullName }} ({{ option.studentCode }})
              </div>
            </template>
          </multiselect>
        </div>

        <button type="submit" class="btn-save">
          {{ isEditMode ? "💾 Update" : "➕ Add New" }}
        </button>
      </form>
    </div>

    <!-- 🔍 Thanh tìm kiếm lớp học -->
    <div class="search-section">
      <input v-model="searchKeyword" type="text" placeholder="🔍 Search class by name..." class="search-input"
        @keyup.enter="handleSearch" />
      <button @click="handleSearch" class="search-btn">Tìm kiếm</button>
      <button @click="resetSearch" class="reset-btn">Tải lại</button>
    </div>

    <div class="class-list">
      <h3>📚 LIST CLASS</h3>
      <table class="class-table">
        <thead>
          <tr>
            <th>#</th>
            <th>ID</th> <!-- Thêm cột ID -->
            <th>Class Name</th>
            <th>Tutor</th>
            <th>Subject</th>
            <th>Slot</th>
            <th>Start Date</th>
            <th>End Date</th>
            <th>Description</th>
            <th>Students</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          <!-- Vòng lặp để hiển thị danh sách lớp học -->
          <tr v-for="(classItem, index) in classes" :key="index">
            <td>{{ index + 1 }}</td>
            <td>{{ classItem.id || 'N/A' }}</td> <!-- Hiển thị ID -->
            <td>{{ classItem.className || 'Has no class name' }}</td>
            <td>{{ classItem.tutorName || 'Has no tutor' }}</td>
            <td>{{ classItem.subjectName || 'Has no subject' }}</td>
            <td>{{ classItem.totalSlot || 0 }}</td>
            <!-- Hiển thị startDate và endDate từ backend -->
            <td>{{ formatDate(classItem.startDate) }}</td>
            <td>{{ formatDate(classItem.endDate) }}</td>
            <td>{{ classItem.description || 'No description' }}</td>
            <td>
              <ul v-if="classItem.studentNames && classItem.studentNames.length > 0">
                <li v-for="(student, idx) in classItem.studentNames" :key="idx">
                  {{ student }}
                </li>
              </ul>
              <span v-else>Empty student</span>
            </td>
            <!-- ✅ Nút hành động để chỉnh sửa/xóa -->
            <td>
              <button @click="editClass(classItem)" class="btn-edit">✏️ Update</button>
              <button @click="deleteClass(classItem.id)" class="btn-delete">🗑️ Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

  </adminLayout>

</template>

<script setup>
import { ref, onMounted } from 'vue';
import Multiselect from 'vue-multiselect';
import 'vue-multiselect/dist/vue-multiselect.css';
import adminLayout from '../components/adminLayout.vue';
import classService from '../api/classService';

const isEditMode = ref(false);
const editClassId = ref(null);
const students = ref([]);
const tutors = ref([]);
const subjects = ref([]);
const classes = ref([]);
const selectedTutor = ref(null);
const selectedSubject = ref(null);
const selectedStudents = ref([]);
const searchKeyword = ref("");


const classForm = ref({
  className: '',
  totalSlot: 0,
  startDate: '',
  endDate: '',
  description: ''
});

onMounted(() => {
  loadStudents();
  loadTutors();
  loadSubjects();
  loadClasses();
});
// 🔍 Tìm kiếm lớp học theo tên
async function handleSearch() {
  if (!searchKeyword.value.trim()) {
    alert("Vui lòng nhập từ khóa tìm kiếm!");
    return;
  }
  try {
    const all = await classService.getAllClasses();
    classes.value = all.data.filter(cls =>
      cls.className.toLowerCase().includes(searchKeyword.value.toLowerCase())
    );
    console.log("🔍 Kết quả tìm kiếm:", classes.value);
  } catch (err) {
    console.error("❌ Lỗi khi tìm kiếm:", err);
    alert("Không thể tìm kiếm lớp học.");
  }
}

// 🔁 Reset tìm kiếm
async function resetSearch() {
  searchKeyword.value = "";
  await loadClasses();
}

async function loadStudents() {
  try {
    const response = await classService.getAllStudents();
    console.log('Dữ liệu học sinh từ getAllStudents:', response.data);
    students.value = response.data
      .filter(student => student.id)
      .map(student => ({
        id: student.id,
        studentCode: student.studentCode,
        fullName: student.user?.fullName || 'Không xác định',
        user: student.user
      }));
  } catch (error) {
    console.error('Lỗi khi lấy danh sách học sinh:', error);
  }

}

async function loadTutors() {
  try {
    const response = await classService.getAllTutors();
    console.log('Danh sách tutor:', response.data);
    tutors.value = response.data.map(tutor => ({
      id: tutor.id,
      fullName: tutor.user?.fullName || 'Không xác định'
    }));
  } catch (error) {
    console.error('Lỗi khi lấy danh sách tutor:', error);
  }
}

async function loadSubjects() {
  try {
    const response = await classService.getAllSubjects();
    console.log('Danh sách subject:', response.data);
    subjects.value = response.data.map(subject => ({
      id: subject.id,
      subjectName: subject.subjectName
    }));
  } catch (error) {
    console.error('Lỗi khi lấy danh sách subject:', error);
  }
}

async function loadClasses() {
  try {
    const response = await classService.getAllClasses();
    console.log('Danh sách lớp học:', response.data);
    classes.value = response.data;
  } catch (error) {
    console.error('❌ Lỗi khi lấy danh sách lớp học:', error);
  }
}

async function handleCreateOrUpdateClass() {
  try {
    if (!selectedTutor.value || !selectedSubject.value) {
      alert('Vui lòng chọn Tutor và Subject!');
      return;
    }
    const studentNames = [...new Set(selectedStudents.value.map(student => student.user.fullName))];
    const classData = {
      className: classForm.value.className,
      tutorName: selectedTutor.value.fullName,
      subjectName: selectedSubject.value.subjectName,
      totalSlot: Number(classForm.value.totalSlot),
      startDate: formatDateToISOString(classForm.value.startDate),
      endDate: formatDateToISOString(classForm.value.endDate),
      description: classForm.value.description || '',
      studentNames
    };
    console.log('✅ Payload gửi lên:', classData);
    if (isEditMode.value) {
      await classService.updateClass(editClassId.value, classData);
      alert('✅ Lớp học đã được cập nhật thành công!');
    } else {
      await classService.createClass(classData);
      alert('🎉 Lớp học đã được tạo thành công!');
    }
    await loadClasses();
    resetForm();
  } catch (error) {
    if (error.response?.data?.errors) {
      console.error('❌ Lỗi chi tiết từ backend:', error.response.data.errors);
      alert('⚠️ Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!');
    } else {
      console.error('❌ Lỗi khi tạo lớp học:', error.message);
      alert('⚠️ Có lỗi xảy ra khi tạo lớp học!');
    }
  }
}

function editClass(classItem) {
  editClassId.value = classItem.id || classItem.ClassId || classItem.ID;
  isEditMode.value = true;
  classForm.value = {
    className: classItem.className || '',
    totalSlot: classItem.totalSlot || 0,
    startDate: classItem.startDate ? classItem.startDate.split('T')[0] : '',
    endDate: classItem.endDate ? classItem.endDate.split('T')[0] : '',
    description: classItem.description || ''
  };
  selectedTutor.value = tutors.value.find(tutor => tutor.fullName === classItem.tutorName) || null;
  selectedSubject.value = subjects.value.find(subject => subject.subjectName === classItem.subjectName) || null;
  selectedStudents.value = students.value.filter(student => classItem.studentIds.includes(student.id));
  console.log('👉 classItem.studentIds:', classItem.studentIds);
  console.log('✅ selectedStudents:', selectedStudents.value);
  console.log('✏️ Đang chỉnh sửa lớp học với ID:', editClassId.value);
}

function deleteClass(classId) {
  const confirmDelete = confirm('❗ Bạn có chắc chắn muốn xóa lớp học này không?');
  if (!confirmDelete) return;
  classService.deleteClass(classId)
    .then(() => {
      alert('🗑️ Lớp học đã được xóa thành công!');
      loadClasses();
    })
    .catch(error => {
      console.error('❌ Lỗi khi xóa lớp học:', error.response?.data || error.message);
      alert('⚠️ Có lỗi xảy ra khi xóa lớp học!');
    });
}

function formatDate(date) {
  if (!date || date === '0001-01-01T00:00:00') return 'Chưa xác định';
  const formattedDate = new Date(date);
  return formattedDate.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  });
}

function formatDateToISOString(date) {
  if (!date) return null;
  const formattedDate = new Date(date);
  return formattedDate.toISOString();
}

function resetForm() {
  classForm.value = {
    className: '',
    totalSlot: 0,
    startDate: '',
    endDate: '',
    description: ''
  };
  selectedTutor.value = null;
  selectedSubject.value = null;
  selectedStudents.value = [];
  isEditMode.value = false;
  editClassId.value = null;
}
</script>


<style scoped>
.home-class {
  max-width: 800px;
  margin: 20px auto;
  background: #e3f2fd;
  padding: 20px 24px;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(33, 150, 243, 0.1);
  transition: all 0.3s ease;
}


.home-class h2 {
  color: #0d47a1;
  margin-bottom: 24px;
  text-align: center;
}

form>div {
  margin-bottom: 10px;
  display: flex;
  flex-direction: column;
}

label {
  font-weight: bold;
  margin-bottom: 6px;
  color: #1565c0;
}

input,
textarea {
  padding: 10px;
  border: 1px solid #90caf9;
  border-radius: 6px;
  font-size: 14px;
  background: white;
  outline: none;
  transition: border-color 0.3s ease;
  min-height: 10px;
}

input:focus,
textarea:focus {
  border-color: #1976d2;
}

.multiselect {
  border: 1px solid #90caf9 !important;
  border-radius: 6px !important;
  padding: 4px !important;
}

.btn-save {
  width: 100%;
  padding: 12px;
  background-color: #2196f3;
  color: white;
  font-weight: bold;
  font-size: 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  margin-top: 16px;
}

.btn-save:hover {
  background-color: #1976d2;
}

.class-list {
  max-width: 100%;
  padding: 40px;
  background: #e1f5fe;
  border-radius: 12px;
  margin: 40px auto;
  box-shadow: 0 4px 10px rgba(0, 123, 255, 0.05);
}

.class-list h3 {
  color: #0d47a1;
  margin-bottom: 20px;
}

.class-table {
  width: 100%;
  border-collapse: collapse;
  background: #ffffff;
  border: 1px solid #bbdefb;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.class-table th {
  background-color: #bbdefb;
  color: #0d47a1;
  padding: 12px;
  text-align: center;
}

.class-table td {
  padding: 10px;
  border-bottom: 1px solid #e0e0e0;
  text-align: center;
}

.class-table tr:hover {
  background-color: #e3f2fd;
  transition: background-color 0.3s ease;
}

ul {
  list-style-type: disc;
  padding-left: 20px;
  margin: 0;
}

li {
  padding: 4px 0;
  color: #333;
}

.btn-edit {
  background: #64b5f6;
  color: white;
  padding: 6px 10px;
  border: none;
  border-radius: 5px;
  margin-right: 5px;
  cursor: pointer;
  transition: 0.3s ease;
}

.btn-edit:hover {
  background-color: #42a5f5;
}

.btn-delete {
  background: #ef5350;
  color: white;
  padding: 6px 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: 0.3s ease;
}

.btn-delete:hover {
  background-color: #d32f2f;
}
.search-section {
  display: flex;
  gap: 10px;
  margin: 20px 0;
  justify-content: center;
  align-items: center;
}

.search-input {
  flex: 1;
  padding: 10px;
  border: 1px solid #90caf9;
  border-radius: 6px;
  font-size: 14px;
  min-width: 200px;
}

.search-btn,
.reset-btn {
  padding: 10px 16px;
  font-size: 14px;
  background-color: #0288d1;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.reset-btn {
  background-color: #757575;
}

.search-btn:hover {
  background-color: #0277bd;
}
.reset-btn:hover {
  background-color: #616161;
}

</style>

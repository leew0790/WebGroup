<script setup>
import { ref, onMounted } from 'vue';
import tutorService from '../api/tutorService';
import CreateTutorForm from '../components/createTutorForm.vue';
import adminLayout from '../components/adminLayout.vue'; // ✅ sử dụng layout

const { deleteTutor } = tutorService;
const tutors = ref([]);
const editingTutor = ref(null);

const loadTutors = async () => {
  try {
    const response = await tutorService.getAll();
    tutors.value = response.data;
  } catch (error) {
    console.error('Error loading tutors:', error);
    tutors.value = [];
  }
};

onMounted(() => {
  loadTutors();
});

const removeTutor = async (id) => {
  if (confirm('Are you sure you want to delete this tutor?')) {
    const success = await deleteTutor(id);
    if (success) {
      tutors.value = tutors.value.filter(t => t.id !== id);
    }
  }
};

const editTutor = (tutor) => {
  editingTutor.value = { ...tutor };
};

const handleTutorUpdated = (updatedTutor) => {
  const index = tutors.value.findIndex(t => t.id === updatedTutor.id);
  if (index !== -1) {
    tutors.value[index] = updatedTutor;
  } else {
    tutors.value.push(updatedTutor);
  }
};
</script>

<template>
  <adminLayout>
    <div class="tutor-theme">
    

      <CreateTutorForm
        :editingTutor="editingTutor"
        @tutorUpdated="loadTutors"
      />

      <table class="data-table">
        <thead>
          <tr>
            <th>FullName</th>
            <th>Email</th>
            <th>UserName</th>
            <th>Department</th>
            <th>ExperienceYears</th>
            <th>Rating</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tutor in tutors" :key="tutor.id">
            <td>{{ tutor.user.fullName }}</td>
            <td>{{ tutor.user.email }}</td>
            <td>{{ tutor.user.userName }}</td>
            <td>{{ tutor.department }}</td>
            <td>{{ tutor.experienceYears }}</td>
            <td>{{ tutor.rating }}</td>
            <td>
              <button class="edit-btn" @click="editTutor(tutor)">Edit</button>
              <button class="delete-btn" @click="removeTutor(tutor.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </adminLayout>
</template>

<style scoped>
@import '../assets/tableStyles.css';


.tutor-theme {
  background-color: #e3f2fd;
  padding: 1.5rem;
  border-radius: 8px;
  box-sizing: border-box;
  padding-bottom: 10px;
  overflow-x: auto;
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



/* Action Buttons */
.edit-btn {
  background: #ffc107;
  color: black;
  padding: 6px 10px;
  margin-right: 4px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.delete-btn {
  background: #dc3545;
  color: white;
  padding: 6px 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  opacity: 0.85;
}
</style>

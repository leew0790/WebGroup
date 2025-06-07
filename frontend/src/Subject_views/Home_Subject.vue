<template>
  <adminLayout>
    <div class="subject-container">
      <h2>{{ isEditing ? '✏️ Update' : '➕ Add New Subject' }}</h2>

      <form @submit.prevent="handleSubmit" class="subject-form">
        <div class="form-group">
          <label>Subject</label>
          <input v-model="subjectName" required />
        </div>
        <div class="form-group">
          <label>Description</label>
          <textarea v-model="information" rows="3"></textarea>
        </div>

        <div class="button-group">
          <button type="submit" class="btn-primary">
            {{ isEditing ? '💾 Update' : '➕ Add New' }}
          </button>
          <button v-if="isEditing" type="button" @click="cancelEdit" class="btn-cancel">❌ Cancel</button>
        </div>

        <p v-if="message" class="message">{{ message }}</p>
      </form>

      <h3>📚 LIST SUBJECT</h3>
      <table class="data-table">
        <thead>
          <tr>
            <th>Subject</th>
            <th>Description</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="sub in subjects" :key="sub.id">
            <td>{{ sub.subjectName }}</td>
            <td>{{ sub.information }}</td>
            <td>
              <button @click="editSubject(sub)" class="btn-edit">✏️ Update</button>
              <button @click="deleteSubject(sub.id)" class="btn-delete">🗑️ Delete</button>
            </td>
          </tr>
          <tr v-if="subjects.length === 0">
            <td colspan="3" class="no-data">Empty Subject.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </adminLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import subjectService from '../api/subjectService.js'
import adminLayout from '../components/adminLayout.vue'

const subjectName = ref('')
const information = ref('')
const message = ref('')
const subjects = ref([])
const isEditing = ref(false)
const editingId = ref(null)

onMounted(loadSubjects)

async function loadSubjects() {
  try {
    const { data } = await subjectService.getSubjects()
    subjects.value = data
  } catch (err) {
    console.error('Load subjects failed', err)
  }
}

async function handleSubmit() {
  message.value = ''
  try {
    if (isEditing.value) {
      const { data } = await subjectService.updateSubject(editingId.value, {
        subjectName: subjectName.value,
        information: information.value
      })
      const index = subjects.value.findIndex(sub => sub.id === editingId.value)
      if (index !== -1) {
        subjects.value[index].subjectName = subjectName.value
        subjects.value[index].information = information.value
      }
      message.value = data.message
      isEditing.value = false
      editingId.value = null
    } else {
      const { data } = await subjectService.createSubject({
        subjectName: subjectName.value,
        information: information.value
      })
      subjects.value.push(data.subjectDto)
      message.value = data.message
    }

    subjectName.value = ''
    information.value = ''
  } catch (err) {
    message.value = err.response?.data?.message || 'Action failed'
  }
}

function editSubject(subject) {
  isEditing.value = true
  editingId.value = subject.id
  subjectName.value = subject.subjectName
  information.value = subject.information
}

async function deleteSubject(id) {
  if (!confirm('Are you sure to delete?')) return
  try {
    await subjectService.deleteSubject(id)
    subjects.value = subjects.value.filter(sub => sub.id !== id)
    message.value = '✅ Subject deleted.'
  } catch (err) {
    message.value = err.response?.data?.message || 'Delete failed.'
  }
}

function cancelEdit() {
  isEditing.value = false
  editingId.value = null
  subjectName.value = ''
  information.value = ''
  message.value = ''
}
</script>

<style scoped>
.subject-container {
  max-width: 800px;
  margin: 40px auto;
  background: #e3f2fd;
  padding: 28px;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(33, 150, 243, 0.1);
  transition: all 0.3s ease;
}

.subject-container h2,
.subject-container h3 {
  text-align: center;
  color: #0d47a1;
  margin-bottom: 24px;
}

.subject-form .form-group {
  margin-bottom: 16px;
}

label {
  font-weight: 600;
  color: #1565c0;
  margin-bottom: 6px;
  display: block;
}

input,
textarea {
  width: 100%;
  padding: 10px;
  border: 1px solid #90caf9;
  border-radius: 6px;
  font-size: 14px;
  background: white;
  transition: border-color 0.3s ease;
}

input:focus,
textarea:focus {
  border-color: #1976d2;
}

.button-group {
  display: flex;
  gap: 12px;
  margin-top: 12px;
}

.btn-primary {
  background-color: #2196f3;
  color: white;
  padding: 10px 20px;
  border: none;
  font-weight: bold;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.btn-primary:hover {
  background-color: #1976d2;
}

.btn-cancel {
  background-color: #f44336;
  color: white;
  padding: 10px 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.message {
  margin-top: 1rem;
  color: #2e7d32;
  font-weight: bold;
  text-align: center;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  margin-top: 30px;
  border: 1px solid #bbdefb;
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

.data-table tr:hover {
  background-color: #e3f2fd;
  transition: background-color 0.2s ease;
}

.btn-edit {
  background: #64b5f6;
  color: white;
  padding: 6px 10px;
  border: none;
  border-radius: 5px;
  margin-right: 5px;
  cursor: pointer;
}

.btn-delete {
  background: #ef5350;
  color: white;
  padding: 6px 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.btn-edit:hover {
  background-color: #42a5f5;
}

.btn-delete:hover {
  background-color: #d32f2f;
}

.no-data {
  text-align: center;
  font-style: italic;
  color: #777;
}
</style>

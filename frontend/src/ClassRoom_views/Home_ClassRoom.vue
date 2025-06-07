<template>
    <adminLayout>
      <div class="classroom-container">
        <h2>🏫 Add new Classroom</h2>
        <form @submit.prevent="handleCreateClassroom" class="create-form">
          <label for="name">Class Name</label>
          <input v-model="newClassroomName" id="name" type="text" required placeholder="Input name/number of classroom..." />
          <button type="submit">➕ Add new Classroom</button>
        </form>
  
        <h3>📋 List Classroom</h3>
        <ul class="room-list">
          <li v-for="(room, index) in classrooms" :key="room.id" class="room-item">
            <span>{{ index + 1 }}. {{ room.name }}</span>
            <button @click.prevent="handleDeleteRoom(room.id)" class="delete-btn">🗑 Delete</button>
          </li>
        </ul>
      </div>
    </adminLayout>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import classroomService from '../api/classroomService';
  import adminLayout from '../components/adminLayout.vue';
  
  const newClassroomName = ref('');
  const classrooms = ref([]);
  
  const loadClassrooms = async () => {
    try {
      const res = await classroomService.getAllClassrooms();
      classrooms.value = res.data;
    } catch (err) {
      console.error("❌ Lỗi khi tải danh sách phòng học:", err);
    }
  };
  
  const handleCreateClassroom = async () => {
    try {
      const payload = { name: newClassroomName.value };
      const response = await classroomService.createClassroom(payload);
      newClassroomName.value = '';
      await loadClassrooms();
      alert("✅ Created classroom successfully!");
    } catch (err) {
      console.error("❌ Lỗi tạo Classroom:", err);
      alert("❌ Can't create classroom. Check again!");
    }
  };
  
  const handleDeleteRoom = async (roomId) => {
    if (!confirm(`Bạn có chắc chắn muốn xoá phòng học này?`)) return;
    try {
      await classroomService.deleteClassroomById(roomId);
      await loadClassrooms();
      alert("🗑 Deleted classroom!");
    } catch (err) {
      console.error("❌ Lỗi xoá phòng học:", err);
      alert("❌ Can't delete classroom. Check again!");
    }
  };
  
  onMounted(() => {
    loadClassrooms();
  });
  </script>
  
  <style scoped>
  .classroom-container {
  max-width: 700px;
  margin: 40px auto;
  padding: 32px;
  background-color: #e3f2fd;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
}
  .classroom-container h2,
  .classroom-container h3 {
    color: #0d47a1;
    margin-bottom: 16px;
  }
  
  .create-form label {
    font-weight: 600;
    color: #1565c0;
    margin-bottom: 6px;
    display: block;
  }
  
  .create-form input {
    padding: 10px;
    width: 100%;
    border: 1px solid #90caf9;
    border-radius: 6px;
    outline: none;
    font-size: 15px;
    margin-bottom: 16px;
    transition: border-color 0.3s ease;
  }
  
  .create-form input:focus {
    border-color: #1976d2;
  }
  
  .create-form button {
    padding: 10px 16px;
    background-color: #2196f3;
    color: white;
    border: none;
    border-radius: 6px;
    font-weight: bold;
    cursor: pointer;
    transition: background-color 0.3s ease;
    width: 100%;
  }
  
  .create-form button:hover {
    background-color: #1976d2;
  }
  
  .room-list {
    list-style: none;
    padding: 0;
    margin-top: 20px;
  }
  
  .room-item {
    background-color: #ffffff;
    padding: 12px 16px;
    border: 1px solid #bbdefb;
    border-radius: 8px;
    margin-bottom: 10px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    transition: transform 0.2s ease;
  }
  
  .room-item:hover {
    transform: translateY(-2px);
    box-shadow: 0 2px 8px rgba(25, 118, 210, 0.1);
  }
  
  .delete-btn {
    background-color: #ef5350;
    color: white;
    border: none;
    padding: 6px 12px;
    border-radius: 6px;
    font-size: 14px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  
  .delete-btn:hover {
    background-color: #c62828;
  }
  </style>
  
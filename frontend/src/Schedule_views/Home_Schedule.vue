<template>
  <adminLayout>
    <div class="schedule-container">
      <div class="form-section">
        <h2>{{ isEditing ? '📏 Update schedule' : '➕ New Schedule' }}</h2>
        <form @submit.prevent="handleCreate">
          <div class="form-group">
            <label>Select Class:</label>
            <select v-model="form.classId" required>
              <option v-for="cls in classes" :key="cls.id" :value="cls.id">{{ cls.className }}</option>
            </select>
          </div>

          <div class="form-group">
            <label>Day (0 = Sunday -- 6 = Saturday):</label>
            <input type="number" v-model.number="form.day" min="0" max="6" required />
          </div>

          <div class="form-group" v-if="!isRecurring">
            <label>Date:</label>
            <input v-model="form.scheduleDate" type="date" required />
            <p v-if="isDateMismatch" class="warning-text">⚠️ Date does not match order selection!</p>
          </div>

          <div v-if="isRecurring && selectedClassDates" class="info-text">
            📅 The schedule will be counted from <strong>{{ selectedClassDates.start }}</strong> to
            <strong>{{ selectedClassDates.end }}</strong> of the class.
          </div>

          <div v-if="isRecurring && form.scheduleDate" class="form-group">
            <label>Actual start date:</label>
            <input type="date" :value="form.scheduleDate" disabled />
          </div>

          <p v-if="isStartDateMismatch" class="warning-text">
            ⚠️ The class start date does not match the day you selected.
          </p>

          <div class="form-group">
            <label>🕒 Slot:</label>
            <select v-model="form.slot" required>
              <option :value="1">Slot 1</option>
              <option :value="2">Slot 2</option>
              <option :value="3">Slot 3</option>
              <option :value="4">Slot 4</option>
            </select>
            <small><strong>Time:</strong> {{ getSlotTime(form.slot) }}</small>
          </div>

          <div class="form-group">
            <label>Link meetting:</label>
            <input v-model="form.linkMeeting" required />
          </div>

          <div class="form-group">
            <label>Room:</label>
            <select v-model="form.classroomId" required>
              <option v-for="room in classrooms" :key="room.id" :value="room.id">{{ room.name }}</option>
            </select>
          </div>

          <div class="form-group checkbox">
            <label>
              <input type="checkbox" v-model="isRecurring" />
              🔁 Create a weekly repetition
            </label>
          </div>

          <div class="form-actions">
            <button type="submit" class="btn-primary">
              {{ isEditing ? '💾 Update' : '➕ Add New' }}
            </button>
            <button v-if="isEditing" type="button" @click="cancelEdit" class="btn-cancel">❌ Cancel</button>
          </div>
        </form>
      </div>

      <div class="table-section">
        <h2>📋 List of class schedules</h2>
        <table class="schedule-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Date</th>
              <th>Day</th>
              <th>Slot</th>
              <th>Time</th>
              <th>Link</th>
              <th>Class</th>
              <th>Room</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(schedule, index) in schedules" :key="schedule.id">
              <td>{{ index + 1 }}</td>
              <td>{{ formatDate(schedule.scheduleDate) }}</td>
              <td>{{ getDayLabel(schedule.day) }}</td>
              <td>Slot {{ schedule.slot }}</td>
              <td>{{ getSlotTime(schedule.slot) }}</td>
              <td><a :href="schedule.linkMeeting" target="_blank">🔗 Link</a></td>
              <td>{{ getClassName(schedule.classId) }}</td>
              <td>{{ getRoomName(schedule.classroomId) }}</td>
              <td>
                <button @click="handleEdit(schedule)" class="btn-edit">✏️</button>
                <button @click="handleDelete(schedule.id)" class="btn-delete">🗑</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </adminLayout>
</template>


<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import scheduleService from '../api/scheduleService';
import classService from '../api/classService';
import classroomService from '../api/classroomService';
import adminLayout from '../components/adminLayout.vue';

const form = ref({
  day: 1,
  slot: 1,
  linkMeeting: '',
  scheduleDate: '',
  classId: null,
  classroomId: null
});
const isRecurring = ref(false);
const isEditing = ref(false);
const editingId = ref(null);
const schedules = ref([]);
const classes = ref([]);
const classrooms = ref([]);

const getSlotTime = (slot) => {
  const slots = {
    1: '08:00 - 09:30',
    2: '09:30 - 11:00',
    3: '14:00 - 15:30',
    4: '15:30 - 17:00'
  };
  return slots[slot] || 'Unknown Slot';
};

const handleCreate = async () => {
  try {
    const payload = { ...form.value };
    payload.day = parseInt(form.value.day); // ✨ Fix: DayOfWeek backend
    if (isEditing.value && editingId.value) {
      await scheduleService.updateSchedule(editingId.value, payload);
      alert('✅ Update schedule successfully!');
    } else {
      if (isRecurring.value) {
        payload.scheduleDate = new Date(form.value.scheduleDate).toISOString();
        console.log('📤 Payload lặp:', payload);
        await scheduleService.createRecurringSchedule(payload);
        alert('✅ Repeat schedule created!');
      } else {
        payload.scheduleDate = new Date(form.value.scheduleDate).toISOString();
        await scheduleService.createSchedule(payload);
        alert('✅ Created Schedule!');
      }
    }
    await loadSchedules();
    cancelEdit();
  } catch (err) {
    console.error('❌ Error:', err);
    alert('❌ Error!');
  }
};

function getNearestDateByDay(startDateStr, targetDay) {
  if (!startDateStr) return '';

  const [year, month, day] = startDateStr.slice(0, 10).split('-').map(Number);
  const date = new Date(year, month - 1, day);

  const currentDay = date.getDay();
  let diff = (targetDay - currentDay + 7) % 7;

  // ⚡️ Đây là thay đổi QUAN TRỌNG: không được cho diff = 0 là +7 => giữ nguyên nếu không cần trùng tuần sau
  const adjustedDate = new Date(date);
  adjustedDate.setDate(date.getDate() + diff);

  const yyyy = adjustedDate.getFullYear();
  const mm = (adjustedDate.getMonth() + 1).toString().padStart(2, '0');
  const dd = adjustedDate.getDate().toString().padStart(2, '0');

  return `${yyyy}-${mm}-${dd}`;
}


const resetForm = () => {
  form.value = {
    day: 1,
    slot: 1,
    linkMeeting: '',
    scheduleDate: '',
    classId: classes.value[0]?.id ?? null,
    classroomId: null,
  };
  isEditing.value = false;
  editingId.value = null;
  isRecurring.value = false;
};

const cancelEdit = () => {
  form.value = {
    day: 1,
    slot: 1,
    linkMeeting: '',
    scheduleDate: '',
    classId: classes.value[0]?.id ?? null,
    classroomId: null
  };
  isEditing.value = false;
  editingId.value = null;
  isRecurring.value = false;
};

const handleEdit = (schedule) => {
  form.value = {
    day: schedule.day,
    slot: schedule.slot,
    linkMeeting: schedule.linkMeeting,
    scheduleDate: schedule.scheduleDate?.slice(0, 10),
    classId: schedule.classId,
    classroomId: schedule.classroomId
  };
  isEditing.value = true;
  editingId.value = schedule.id;
  isRecurring.value = false;
};

const handleDelete = async (id) => {
  const confirmDelete = confirm('❗Bạn có chắc chắn muốn xoá lịch học này?');
  if (!confirmDelete) return;
  try {
    await scheduleService.deleteSchedule(id);
    alert('🗑️ Đã xoá lịch học thành công!');
    await loadSchedules();
  } catch (err) {
    console.error('❌ Lỗi xoá lịch học:', err);
    alert('❌ Xoá lịch học thất bại!');
  }
};

const isDateMismatch = computed(() => {
  if (!form.value.scheduleDate || isRecurring.value) return false;
  const selectedDate = new Date(form.value.scheduleDate);
  return selectedDate.getDay() !== form.value.day;
});

const isStartDateMismatch = computed(() => {
  if (!form.value.classId || !isRecurring.value) return false;
  const selectedClass = classes.value.find(cls => cls.id === form.value.classId);
  if (!selectedClass) return false;
  const start = new Date(selectedClass.startDate);
  return start.getDay() !== form.value.day;
});

const getDayLabel = (day) => ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'][day] || 'Không rõ';
const getClassName = (id) => classes.value.find(c => c.id === id)?.className || 'Không rõ';
const getRoomName = (id) => classrooms.value.find(r => r.id === id)?.name || 'Không rõ';
const formatDate = (dateStr) => new Date(dateStr).toLocaleDateString('vi-VN');

const loadSchedules = async () => {
  try {
    const res = await scheduleService.getAllSchedules();
    schedules.value = res.data;
  } catch (err) {
    console.error('❌ Lỗi khi tải lịch học:', err);
  }
};

const loadClassesAndRooms = async () => {
  const classRes = await classService.getAllClasses();
  const roomRes = await classroomService.getAllClassrooms();
  classes.value = classRes.data;
  classrooms.value = roomRes.data;
};

const selectedClassDates = computed(() => {
  if (!form.value.classId || !isRecurring.value) return null;
  const selectedClass = classes.value.find(cls => cls.id === form.value.classId);
  if (!selectedClass) return null;

  const start = new Date(selectedClass.startDate).toLocaleDateString("vi-VN");
  const end = new Date(selectedClass.endDate).toLocaleDateString("vi-VN");
  return { start, end };
});


onMounted(async () => {
  await loadSchedules();
  await loadClassesAndRooms();
});

watch([() => form.value.day, () => form.value.classId], ([newDay, newClassId]) => {
  if (isRecurring.value && newClassId !== null) {
    const selectedClass = classes.value.find(cls => cls.id === newClassId);
    if (selectedClass && selectedClass.startDate) {
      const iso = selectedClass.startDate.slice(0, 10);
      const adjustedDate = getNearestDateByDay(iso, newDay);
      form.value.scheduleDate = adjustedDate;
      console.log("📅 Đã tính ngày bắt đầu thực tế:", adjustedDate);
    }
  }
});



</script>

<style scoped>
.schedule-container {
  max-width: 1100px;
  margin: 30px auto;
  padding: 20px;
}

.form-section {
  background-color: #e3f2fd;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(33, 150, 243, 0.1);
  margin-bottom: 30px;
}

.schedule-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border: 1px solid #bbdefb;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.03);
}

.schedule-table th {
  background-color: #bbdefb;
  color: #0d47a1;
  padding: 12px;
  text-align: center;
}

.schedule-table td {
  padding: 10px;
  text-align: center;
  border-bottom: 1px solid #e0e0e0;
}

.schedule-table tr:hover {
  background-color: #f1faff;
  transition: background-color 0.2s ease;
}

.form-group {
  margin-bottom: 14px;
}

input,
select,
textarea {
  width: 100%;
  padding: 10px;
  border: 1px solid #90caf9;
  border-radius: 6px;
  font-size: 14px;
  background: white;
  transition: border 0.3s ease;
}

input:focus,
select:focus,
textarea:focus {
  border-color: #1976d2;
  outline: none;
}

.form-actions {
  display: flex;
  gap: 10px;
  margin-top: 16px;
}

.btn-primary {
  background-color: #2196f3;
  color: white;
  padding: 10px 16px;
  font-weight: bold;
  border: none;
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

.btn-edit {
  background: #64b5f6;
  color: white;
  padding: 6px 8px;
  border: none;
  border-radius: 4px;
  margin-right: 5px;
}

.btn-delete {
  background: #ef5350;
  color: white;
  padding: 6px 8px;
  border: none;
  border-radius: 4px;
}

.warning-text {
  color: orange;
  font-style: italic;
  font-size: 13px;
}

.info-text {
  font-style: italic;
  font-size: 13px;
  margin-bottom: 10px;
  color: #1a237e;
}

.checkbox input {
  margin-right: 8px;
}

.table-section h2 {
  margin-bottom: 16px;
  color: #0d47a1;
}
</style>

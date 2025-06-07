<template>
  <userLayout>
    <div class="student-schedule">
      <h2>📘 My Schedule</h2>

      <table class="schedule-table">
        <thead>
          <tr>
            <th>#</th>
            <th>Date</th>
            <th>Day</th>
            <th>Slot</th>
            <th>Time</th>
            <th>Link Meet</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(sch, index) in mySchedules" :key="index">
            <td>{{ index + 1 }}</td>
            <td>{{ formatDate(sch.scheduleDate) }}</td>
            <td>{{ getDayLabel(sch.day) }}</td>
            <td>Slot {{ sch.slot }}</td>
            <td>{{ getSlotTime(sch.slot) }}</td>
            <td>
              <a :href="sch.linkMeeting" target="_blank" class="join-link">🔗 Join</a>
            </td>
          </tr>
        </tbody>
      </table>

      <p v-if="mySchedules.length === 0" class="no-schedule">You currently do not have any class schedule..</p>
    </div>
  </userLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import jwtDecode from 'jwt-decode';
import userLayout from '../components/userLayout.vue';
import classService from '../api/classService';
import scheduleService from '../api/scheduleService';
import classroomService from '../api/classroomService';

const studentId = ref(null);
const tutorId = ref(null);
const myClasses = ref([]);
const allSchedules = ref([]);
const mySchedules = ref([]);
const classrooms = ref([]);
const classes = ref([]);

const getDayLabel = (day) => ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'][day] || '???';

const getSlotTime = (slot) => ({
  1: '08:00 - 09:30',
  2: '09:30 - 11:00',
  3: '14:00 - 15:30',
  4: '15:30 - 17:00'
})[slot] || '??';

const formatDate = (dateStr) => new Date(dateStr).toLocaleDateString('vi-VN');

onMounted(async () => {
  const token = localStorage.getItem('token');
  if (!token) {
    alert('❗ Bạn chưa đăng nhập!');
    return;
  }

  const decoded = jwtDecode(token);
  studentId.value = decoded['StudentId'];
  tutorId.value = decoded['TutorId'];

  try {
    const classRes = await classService.getAllClasses();
    classes.value = classRes.data;
    const roomRes = await classroomService.getAllClassrooms();
    classrooms.value = roomRes.data;

    if (studentId.value) {
      myClasses.value = classes.value.filter(cls => cls.studentIds.includes(Number(studentId.value)));
    } else if (tutorId.value) {
      myClasses.value = classes.value.filter(cls => cls.tutorId === Number(tutorId.value));
    }

    const scheduleRes = await scheduleService.getAllSchedules();
    allSchedules.value = scheduleRes.data;

    const myClassIds = myClasses.value.map(cls => cls.id);
    mySchedules.value = allSchedules.value.filter(s => myClassIds.includes(s.classId));
  } catch (err) {
    console.error('❌ Lỗi khi tải dữ liệu:', err);
  }
});
</script>


<style scoped>
.student-schedule {
  max-width: 900px;
  margin: 40px auto;
  padding: 24px;
  background-color: #fce4ec; /* hồng tím nền */
  border-radius: 12px;
  box-shadow: 0 6px 12px rgba(156, 39, 176, 0.15);
  transition: all 0.3s ease;
}

.student-schedule h2 {
  text-align: center;
  color: #ad1457;
  margin-bottom: 20px;
}

.schedule-table {
  width: 100%;
  border-collapse: collapse;
  background-color: #fff;
  border: 1px solid #f8bbd0;
  border-radius: 8px;
  overflow: hidden;
}

.schedule-table th {
  background-color: #f8bbd0;
  color: #6a1b9a;
  padding: 12px;
  text-align: center;
  font-weight: bold;
}

.schedule-table td {
  padding: 10px;
  border-bottom: 1px solid #e0e0e0;
  text-align: center;
  font-size: 14px;
  color: #4a148c;
}

.schedule-table tr:hover {
  background-color: #f3e5f5;
  transition: background-color 0.3s ease;
}

.join-link {
  color: #7b1fa2;
  font-weight: 500;
  text-decoration: none;
}
.join-link:hover {
  text-decoration: underline;
}

.no-schedule {
  text-align: center;
  margin-top: 20px;
  color: #9e9e9e;
  font-style: italic;
}
</style>
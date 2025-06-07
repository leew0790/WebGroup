<script setup>
import { ref, onMounted, computed } from 'vue'
import dashboardService from '../api/dashboardService';
import adminLayout from '../components/adminLayout.vue';
import { Bar } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
} from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

const dashboard = ref({
  totalStudents: 0,
  totalTutors: 0,
  totalClasses: 0,
  lessonsToday: 0,
  monthlyStudentStats: [],
  topTutors: []
});

onMounted(async () => {

  try {
    const res = await dashboardService.getDashboardData();
    dashboard.value = res.data;
  } catch (err) {
    console.error('❌ Lỗi dashboard:', err);
  }
});

const chartData = computed(() => {
  return {
    labels: dashboard.value.monthlyStudentStats.map(m => m.label),
    datasets: [
      {
        label: 'Number of students/enrollment',
        data: dashboard.value.monthlyStudentStats.map(m => m.value),
        backgroundColor: '#7fb3e6'
      }
    ]
  };
});

const chartOptions = {
  responsive: true,
  plugins: {
    legend: {
      position: 'top'
    },
    title: {
      display: true,
      text: 'Student statistics by month'
    }
  }
};
</script>

<template>
  <adminLayout>
    <div class="dashboard">
      <h2>📊 Dashboard</h2>

      <!-- Thống kê nhanh -->
      <div class="stats">
        <div class="stat-box">👨‍🎓 Student: {{ dashboard.totalStudents }}</div>
        <div class="stat-box">👩‍🏫 Tutor: {{ dashboard.totalTutors }}</div>
        <div class="stat-box">🏫 Class: {{ dashboard.totalClasses }}</div>
        <div class="stat-box">📅 Today: {{ dashboard.lessonsToday }}</div>
      </div>

      <!-- Biểu đồ -->
      <div class="chart-container">
        <Bar :data="chartData" :options="chartOptions" />
      </div>

      <!-- Top Tutor -->
      <h3>🏆 Top Tutor</h3>
      <table>
        <thead>
          <tr>
            <th>👤 Fullname</th>
            <th>📘 Number of class</th>
            <th>⭐ Rate</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tutor in dashboard.topTutors" :key="tutor.fullName">
            <td>{{ tutor.fullName }}</td>
            <td>{{ tutor.totalClasses }}</td>
            <td>{{ tutor.rating }}/5</td>
          </tr>
        </tbody>
      </table>
    </div>
  </adminLayout>
</template>


<style scoped>
.layout-container {
  display: flex;
  flex-direction: row;
  height: 100vh;
  overflow: hidden;
}

.main {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100vh;
  overflow: hidden;
}

.content {
  flex: 1;
  padding: 1rem;
  margin-top: 60px;
  overflow-y: auto;
  overflow-x: auto;
}

/* Phần dashboard */
.dashboard {
  flex: 1;
  padding: 20px;
  overflow: auto;
  min-width: 100%;
  box-sizing: border-box;
}

.stats {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-box {
  background: #f5f5f5;
  padding: 1rem;
  border-radius: 8px;
  flex: 1 1 200px;
  text-align: center;
  font-weight: bold;
}

.chart-container {
  max-width: 100%;
  max-height: 250px; 
  overflow-x: auto;
  margin-bottom: 2rem;
}
table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

table th,
table td {
  border: 1px solid #ccc;
  padding: 0.5rem;
  text-align: center;
}

table th {
  background-color: #f0f0f0;
}

/* Button group responsive */
.btn-group {
  margin-top: 1rem;
  margin-bottom: 2rem;
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  justify-content: center;
}

.btn-group .btn {
  padding: 0.6rem 1.2rem;
  font-weight: 500;
  background-color: #007bff;
  color: white;
  border-radius: 6px;
  text-decoration: none;
  transition: background 0.2s ease;
}

.btn-group .btn:hover {
  background-color: #0056b3;
}

/* TopBar xử lý nằm cố định và tránh che nội dung */
.top-bar-fixed {
  position: fixed;
  top: 0;
  left: 220px;
  right: 0;
  z-index: 999;
  height: 50px;
}

/* Responsive xử lý thu nhỏ màn hình */
@media (max-width: 768px) {
  .layout-container {
    flex-direction: column;
  }

  .main {
    margin-left: 0;
  }

  .top-bar-fixed {
    left: 0;
  }

  .stat-box {
    flex: 1 1 100%;
  }

  .chart-container {
    overflow-x: auto;
  }

  table {
    display: block;
    overflow-x: auto;
    white-space: nowrap;
  }
}

</style>

<template>
    <div class="dashboard">
      <h2>📊 Dashboard Thống kê</h2>
  
      <!-- Thông kê nhanh -->
      <div class="stats">
        <div class="stat-box">👨‍🎓 Học sinh: {{ dashboard.totalStudents }}</div>
        <div class="stat-box">👩‍🏫 Gia sư: {{ dashboard.totalTutors }}</div>
        <div class="stat-box">🏫 Lớp học: {{ dashboard.totalClasses }}</div>
        <div class="stat-box">📅 Hôm nay: {{ dashboard.lessonsToday }}</div>
      </div>
  
      <!-- Biểu đồ học sinh theo tháng -->
      <div class="chart-container">
        <Bar :data="chartData" :options="chartOptions" />
      </div>
  
      <!-- Top tutor -->
      <h3>🏆 Top Gia sư</h3>
      <table>
        <thead>
          <tr>
            <th>👤 Họ tên</th>
            <th>📘 Số lớp</th>
            <th>⭐ Đánh giá</th>
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
  </template>
  
  <script setup>
  import { ref, onMounted, computed } from 'vue';
  import dashboardService from '../api/dashboardService';
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
          label: 'Số học sinh/đăng ký',
          data: dashboard.value.monthlyStudentStats.map(m => m.value),
          backgroundColor: '#4caf50'
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
        text: 'Thống kê học sinh theo tháng'
      }
    }
  };
  </script>
  
  <style scoped>
  .dashboard {
    padding: 20px;
  }
  .stats {
    display: flex;
    gap: 1rem;
    margin-bottom: 2rem;
  }
  .stat-box {
    background: #f5f5f5;
    padding: 1rem;
    border-radius: 8px;
    flex: 1;
    text-align: center;
    font-weight: bold;
  }
  .chart-container {
    max-width: 600px;
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
  </style>
  
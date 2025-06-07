// 📁 src/api/scheduleService.js
import axios from 'axios';

const BASE_URL = 'https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api';
const token = () => localStorage.getItem('token');



export default {
  // 🔄 Tạo 1 lịch học đơn
  createSchedule(payload) {
    return axios.post(`${BASE_URL}/Schedule/create-schedule`, payload, {
      headers: { Authorization: `Bearer ${token()}` }
    });
  },

  // 🔁 Tạo lịch học lặp lại mỗi tuần
  createRecurringSchedule(payload) {
    return axios.post(`${BASE_URL}/Schedule/create-recurring-schedules`, payload, {
      headers: { Authorization: `Bearer ${token()}` }
    });
  },

  // (Dùng sau) – Xem tất cả lịch
  getAllSchedules() {
    return axios.get(`${BASE_URL}/Schedule/get-all-schedules`, {
      headers: { Authorization: `Bearer ${token()}` }
    });
  },

  // (Dùng sau) – Xoá lịch học
  deleteSchedule(id) {
    return axios.delete(`${BASE_URL}/Schedule/delete-schedule/${id}`, {
      headers: { Authorization: `Bearer ${token()}` }
    });
  },

  // (Dùng sau) – Cập nhật lịch học
  updateSchedule(id, payload) {
    return axios.put(`${BASE_URL}/Schedule/update-schedule/${id}`, payload, {
      headers: { Authorization: `Bearer ${token()}` }
    });
  }
};

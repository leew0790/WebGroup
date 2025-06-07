// 📁 src/api/dashboardService.js
import axios from 'axios';

const BASE_URL = 'https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api';
const token = () => localStorage.getItem('token');

export default {
  getDashboardData() {
    return axios.get(`${BASE_URL}/Dashboard`, {
      headers: { Authorization: `Bearer ${token()}` }
    });
  }
};

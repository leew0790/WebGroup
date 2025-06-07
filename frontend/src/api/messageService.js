import axios from 'axios';
const BASE_URL = 'https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api';



export default {
  sendMessage(message) {
    const token = localStorage.getItem('token');
    return axios.post(`${BASE_URL}/Messages/send`, message, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      }
    });
  },
  getConversation(receiverId) {
    const token = localStorage.getItem('token');
    return axios.get(`${BASE_URL}/Messages/conversation/${receiverId}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  }
};

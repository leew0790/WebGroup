import axios from 'axios';


const BASE_URL = 'https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/Subjects';
const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    headers: {
      Authorization: `Bearer ${token}`
    }
  };
};
``
export default {
  createSubject(payload) {
    return axios.post(`https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/Subjects/create-subject`, payload, getAuthHeaders());
  },
  getSubjects() { return axios.get(`https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/Subjects/get-all-subjects`, getAuthHeaders()) },

  updateSubject(id, subject) {

    return axios.put(`https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/Subjects/update-subject/${id}`, subject, getAuthHeaders())

  },
  
  deleteSubject(id) {
    return axios.delete(`https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/Subjects/delete-subject/${id}`,getAuthHeaders())

  }
};

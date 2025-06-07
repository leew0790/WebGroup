import axios from 'axios';

const BASE_URL = 'https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api';

const token = () => localStorage.getItem("token");

export default {
  createClassroom(payload) {
    return axios.post(`${BASE_URL}/Classroom/create-classroom`, payload, {
      headers: { Authorization: `Bearer ${token()}` }
    });
  },

  getAllClassrooms() {
    return axios.get(`${BASE_URL}/Classroom/get-all-classrooms`,{
      headers: { Authorization: `Bearer ${token()}` }
    });
  },

  deleteClassroomById(id) {
    return axios.delete(`${BASE_URL}/Classroom/delete-classroom/${id}`, {
      headers: { Authorization: `Bearer ${token()}` }
    });
  }
};

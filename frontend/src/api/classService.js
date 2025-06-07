import axios from 'axios';
import jwtDecode from "jwt-decode";

const BASE_URL = 'https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api';

export default {
  // Lấy toàn bộ danh sách student
  getAllStudents() {
    const token = localStorage.getItem('token');
    return axios.get(`${BASE_URL}/profile/students`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  },

  // Gửi dữ liệu tạo lớp học mới
  createClass(classData) {
    const token = localStorage.getItem('token');
    return axios.post(`${BASE_URL}/Class/create-class`, classData, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  },

  getClassById(classId) {
    const token = localStorage.getItem('token');
    return axios.get(`${BASE_URL}/Class/get-class/${classId}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  },
  // Lấy danh sách tutor (đính kèm token)
  getAllTutors() {
    const token = localStorage.getItem('token');
    return axios.get(`${BASE_URL}/profile/tutors`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  },

  // Lấy danh sách subject
  getAllSubjects() {
    const token = localStorage.getItem('token');
    return axios.get(`${BASE_URL}/Subjects/get-all-subjects`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  },

  getAllClasses() {
    const token = localStorage.getItem('token');
    return axios.get(`${BASE_URL}/Class/get-all-classes`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  },
  
  updateClass(id, classData) {
    const token = localStorage.getItem('token');
    return axios.put(`${BASE_URL}/Class/update-class/${id}`, classData, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  },
  deleteClass(id) {
    return axios.delete(`${BASE_URL}/Class/delete-class/${id}`);
  },

  loadMyClasses() {
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error("Bạn chưa đăng nhập!");
    }
  
    const decoded = jwtDecode(token);
    const studentId = decoded["StudentId"];
  
    if (!studentId) {
      throw new Error("Không tìm thấy StudentId trong token!");
    }
  
    return axios.get(`${BASE_URL}/Class/get-all-classes`, {
      headers: { Authorization: `Bearer ${token}` }
    }).then(res => {
      const allClasses = res.data;
      const myClasses = allClasses.filter(classItem =>
        classItem.studentIds.includes(Number(studentId))
      );
      return myClasses;
    });
  },
  loadMyTutorClasses() {
    const token = localStorage.getItem("token");
    if (!token) {
      alert("Bạn chưa đăng nhập!");
      return [];
    }
  
    const decoded = jwtDecode(token);
    const tutorId = decoded["TutorId"];
  
    if (!tutorId) {
      alert("Không tìm thấy TutorId trong token!");
      return [];
    }
  
    return axios.get(`${BASE_URL}/Class/get-all-classes`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }).then((res) => {
      const allClasses = res.data;
      const myClasses = allClasses.filter(c => 
        c.tutorId === Number(tutorId)
      );
      return myClasses;
    }).catch((err) => {
      console.error("❌ Lỗi khi tải lớp của tutor:", err);
      return [];
    });
  }
};






import axios from "axios";
const BASE_URL = "https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/profile";

export async function getStudentProfile(studentId, token) {
  try {
    const response = await axios.get(`${BASE_URL}/students/${studentId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data;
  } catch (error) {
    console.error("Lỗi khi lấy thông tin student:", error);
    throw error;
  }
}
export async function getTutorProfile(tutorId, token) {
  try {
    const response = await axios.get(`${BASE_URL}/tutors/${tutorId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data;
  } catch (error) {
    console.error("Lỗi khi lấy thông tin tutor:", error);
    throw error;
  }
}



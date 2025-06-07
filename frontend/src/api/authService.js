import axios from "axios";
import jwtDecode from 'jwt-decode';

// ✅ Đặt lại API_URL gốc để tránh lỗi
const BASE_API_URL = "https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api";

export default {
  login(credentials) {
    return axios.post(`${BASE_API_URL}/account/login`, {
      username: credentials.username,
      password: credentials.password,
    });
  },
};
export const getToken = () => {
  return localStorage.getItem("token");
};

export const handleLoginSuccess = (data, inputUsername) => {
  const token = data.token;
  if (token) {
    // Giải mã token nếu cần thiết cho các thông tin khác
    const decoded = jwtDecode(token);
    console.log("Decoded Token:", decoded);

    // Nếu không cần thiết lấy userName từ token, bạn có thể dùng inputUsername hoặc data.userName
    const username = inputUsername; // hoặc data.userName nếu muốn
    const userId = decoded.nameid;
    const role = decoded.role;
    const email = decoded.email;
    const studentId = decoded.StudentId || null;

    localStorage.setItem("token", token);
    localStorage.setItem("userId", userId);
    localStorage.setItem("role", role);
    localStorage.setItem("email", email);
    localStorage.setItem("username", username); // lưu username vào localStorage
    if (studentId) {
      localStorage.setItem("studentId", studentId);
    }
  } else {
    console.error("⚠️ Token is missing or invalid!");
  }
};
export const sendForgotPasswordEmail = async (email) => {
  try {
    const response = await axios.post(
      `${BASE_API_URL}/SendEmail/forgot-password`,
      email,
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    return response.data;
  } catch (error) {
    throw error.response?.data || "Lỗi khi gửi email.";
  }
};

export const resetPassword = async (email, newPassword, token) => {
  try {
    const response = await axios.post(
      `${BASE_API_URL}/ResetPassword/reset-password`,
      {
        email,
        newPassword,
        token,
      },
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );

    if (typeof response.data === "string") {
      return { message: response.data };
    } else {
      return response.data;
    }
  } catch (error) {
    throw error.response?.data || "Lỗi khi đặt lại mật khẩu.";
  }
};

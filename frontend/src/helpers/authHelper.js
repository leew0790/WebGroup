// helpers/authHelper.js
import jwtDecode from 'jwt-decode';

// Giải mã token để lấy thông tin người dùng
export const getUserFromToken = (token) => {
  if (!token) return null;
  try {
    const decoded = jwtDecode(token); // Giải mã đúng
    return decoded;
  } catch (error) {
    console.error("Lỗi giải mã token:", error);
    return null;
  }
};

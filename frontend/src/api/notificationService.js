import axios from "axios";

const BASE_API_URL = "https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api";
const getToken = () => localStorage.getItem("token");

export const getNotifications = async () => {
  try {
    const token = getToken();
    const res = await axios.get(`${BASE_API_URL}/Notification`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return res.data;
  } catch (err) {
    console.error("❌ Lỗi lấy thông báo:", err);
    return null;
  }
};

export const markAsRead = async (id) => {
  try {
    const token = getToken();
    await axios.post(`${BASE_API_URL}/Notification/read/${id}`, {}, {
      headers: { Authorization: `Bearer ${token}` },
    });
  } catch (err) {
    console.error(`❌ Lỗi markAsRead (id ${id}):`, err);
  }
};

export default {
  getNotifications,
  markAsRead,
};

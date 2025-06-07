// src/api/notificationHub.js
import * as signalR from '@microsoft/signalr';

let notificationConnection = null;

export function connectToNotificationHub(token, onReceiveNotification) {
  // Nếu đã có kết nối thì chỉ cập nhật lại listener
  if (notificationConnection) {
    notificationConnection.off("ReceiveNotification");
    notificationConnection.on("ReceiveNotification", (data) => {
      console.log("🔔 ReceiveNotification", data);
      onReceiveNotification({
        message: data.message,
        createdAt: new Date(data.time), // Chuyển sang dạng Date object
        isRead: false,
        id: data.id || Math.random(), // fallback nếu BE chưa gửi ID
      });
    });
    return notificationConnection;
  }

  // Tạo mới kết nối nếu chưa có
  notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl('https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/notificationHub', {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .build();

  // Đăng ký listener khi nhận thông báo mới
  notificationConnection.on("ReceiveNotification", (data) => {
    console.log("🔔 ReceiveNotification", data);
    onReceiveNotification({
      message: data.message,
      createdAt: new Date(data.time),
      isRead: false,
      id: data.id || Math.random(),
    });
  });

  // Sự kiện kết nối lại
  notificationConnection.onreconnecting(() => {
    console.warn('🔄 Đang reconnect NotificationHub...');
  });

  notificationConnection.onreconnected(() => {
    console.log('✅ Đã reconnect NotificationHub');
  });

  // Bắt đầu kết nối
  notificationConnection.start()
    .then(() => console.log('✅ Kết nối NotificationHub thành công'))
    .catch((err) => console.error('❌ Lỗi kết nối NotificationHub:', err));

  return notificationConnection;
}

export function stopNotificationHub() {
  if (notificationConnection) {
    notificationConnection.off("ReceiveNotification");
    notificationConnection.stop();
    notificationConnection = null;
    console.log("🛑 Đã ngắt kết nối NotificationHub");
  }
}

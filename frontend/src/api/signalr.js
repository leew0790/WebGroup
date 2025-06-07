// src/api/signalr.js
import * as signalR from '@microsoft/signalr';

let connection = null;

export function connectToMessageHub(token, onReceiveMessage) {
  // Nếu đã có kết nối thì chỉ cập nhật lại listener
  if (connection) {
    connection.off("ReceiveMessage"); // Xóa listener cũ
    connection.on("ReceiveMessage", (senderId, content,timestamp) => {
      console.log("📩 ReceiveMessage", senderId, content);
      onReceiveMessage({ senderId, content,timestamp: new Date(timestamp) });
    });
    return connection;
  }

  // Tạo mới kết nối nếu chưa có
  connection = new signalR.HubConnectionBuilder()
    .withUrl('https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/MessageHub', {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .build();

  // Đăng ký lắng nghe sự kiện nhận tin nhắn
  connection.on("ReceiveMessage", (senderId, content,timestamp) => {
    console.log("📩 ReceiveMessage", senderId, content);
    onReceiveMessage({ senderId, content, timestamp: new Date(timestamp) });
  });

  // Các sự kiện kết nối lại
  connection.onreconnecting(() => {
    console.warn('🔄 Đang kết nối lại với SignalR...');
  });

  connection.onreconnected(() => {
    console.log('✅ Đã kết nối lại thành công với SignalR');
  });

  // Bắt đầu kết nối
  connection.start()
    .then(() => console.log('✅ Đã kết nối SignalR'))
    .catch((err) => console.error('❌ Lỗi kết nối SignalR:', err));

  return connection;
}

export function stopSignalR() {
  if (connection) {
    connection.off("ReceiveMessage"); // 💡 Đảm bảo hủy listener khi ngắt kết nối
    connection.stop();
    connection = null;
  }
}
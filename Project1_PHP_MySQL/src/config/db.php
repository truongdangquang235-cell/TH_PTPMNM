<?php
date_default_timezone_set('Asia/Ho_Chi_Minh');

$servername = "truongdangquang.infinityfree.me";
$username = "if0_3456789"; // Thay đổi nếu bạn có username khác
$password = "0399708261";     // Thay đổi nếu bạn có password khác
$dbname = "if0_3456789_db_coffee";

// Tạo kết nối bằng MySQLi
$conn = new mysqli($servername, $username, $password, $dbname);

// Kiểm tra kết nối
if ($conn->connect_error) {
    die("Kết nối thất bại: " . $conn->connect_error);
}

// Thiết lập mã hóa ký tự (UTF-8) để hỗ trợ tiếng Việt
$conn->set_charset("utf8mb4");
// echo "Kết nối thành công!"; // Bạn có thể bỏ dòng này sau khi test
?>
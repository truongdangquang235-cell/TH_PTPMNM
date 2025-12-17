<?php
session_start();
if (isset($_SESSION['user_id'])) {
    header('Location: index.php');
    exit;
}
include 'config/db.php';

$message = '';
$error = '';

if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $username = $conn->real_escape_string($_POST['username']);
    $password = $_POST['password']; 
    $confirm_password = $_POST['confirm_password']; // Thêm xác nhận mật khẩu
    $email = $conn->real_escape_string($_POST['email']);
    $role = 'user';

    if ($password !== $confirm_password) {
        $error = "Mật khẩu xác nhận không khớp.";
    } else {
        // Kiểm tra user tồn tại
        $check = $conn->query("SELECT id FROM users WHERE username = '$username'");
        if ($check->num_rows > 0) {
            $error = "Tên đăng nhập đã tồn tại.";
        } else {
            $hashed_password = password_hash($password, PASSWORD_DEFAULT);
            $sql = "INSERT INTO users (username, password, email, role) VALUES ('$username', '$hashed_password', '$email', '$role')";
            
            if ($conn->query($sql) === TRUE) {
                header('Location: login.php?registered=true');
                exit;
            } else {
                $error = "Lỗi hệ thống: " . $conn->error;
            }
        }
    }
}
$conn->close();
?>

<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <title>Đăng Ký - CTusCoffee</title>
    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@700&family=Poppins:wght@400;500;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <link rel="stylesheet" href="Project1_PHP_MySQL/src/assets/css/style.css">
</head>
<body>

    <div class="auth-wrapper">
        <a href="Project1_PHP_MySQL/src/index.php" class="btn-home-back" title="Về trang chủ"><i class="fas fa-home"></i></a>

        <div class="auth-box">
            <h2>Tạo Tài Khoản</h2>

            <?php if (!empty($error)): ?>
                <p style="color: #d9534f; background: #f9d6d5; padding: 10px; border-radius: 5px; font-size: 0.9em; margin-bottom: 15px;">
                    <i class="fas fa-exclamation-circle"></i> <?php echo $error; ?>
                </p>
            <?php endif; ?>

            <form method="POST" action="register.php" class="auth-form">
                <label for="username">Tên đăng nhập</label>
                <input type="text" id="username" name="username" required placeholder="Chọn tên đăng nhập...">

                <label for="email">Email</label>
                <input type="email" id="email" name="email" required placeholder="Địa chỉ email của bạn...">

                <label for="password">Mật khẩu</label>
                <input type="password" id="password" name="password" required placeholder="Nhập mật khẩu...">
                
                <label for="confirm_password">Nhập lại Mật khẩu</label>
                <input type="password" id="confirm_password" name="confirm_password" required placeholder="Xác nhận mật khẩu...">

                <button type="submit" class="btn-auth">ĐĂNG KÝ</button>
            </form>

            <div class="auth-link">
                <p>Đã có tài khoản? <a href="Project1_PHP_MySQL/src/login.php">Đăng nhập</a></p>
            </div>
        </div>
    </div>

</body>
</html>
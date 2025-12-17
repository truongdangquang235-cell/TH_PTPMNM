<?php
session_start();
// Nếu đã đăng nhập, chuyển hướng về trang chủ
if (isset($_SESSION['user_id'])) {
    header('Location: index.php');
    exit;
}
include 'config/db.php';

$message = '';
// Hiển thị thông báo đăng ký thành công
if (isset($_GET['registered']) && $_GET['registered'] == 'true') {
    $message = "🎉 Đăng ký thành công! Vui lòng đăng nhập.";
}

if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $username = $conn->real_escape_string($_POST['username']);
    $password = $_POST['password'];

    $sql = "SELECT id, username, password, role FROM users WHERE username = '$username'";
    $result = $conn->query($sql);

    if ($result->num_rows == 1) {
        $user = $result->fetch_assoc();
        if (password_verify($password, $user['password'])) {
            $_SESSION['user_id'] = $user['id'];
            $_SESSION['username'] = $user['username'];
            $_SESSION['role'] = $user['role'];

            // Điều hướng dựa trên role
            if ($user['role'] == 'admin') header('Location: admin/index.php');
            elseif ($user['role'] == 'kitchen') header('Location: kitchen/index.php');
            else {
                // Kiểm tra nếu có trang đích cần quay lại (ví dụ từ checkout)
                if (isset($_SESSION['redirect_url'])) {
                    $url = $_SESSION['redirect_url'];
                    unset($_SESSION['redirect_url']);
                    header('Location: ' . $url);
                } else {
                    header('Location: index.php');
                }
            }
            exit;
        } else {
            $message = "Mật khẩu không đúng.";
        }
    } else {
        $message = "Tên đăng nhập không tồn tại.";
    }
}
$conn->close();
?>

<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Đăng Nhập - CTusCoffee</title>
    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@700&family=Poppins:wght@400;500;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <link rel="stylesheet" href="Project1_PHP_MySQL/src/assets/css/style.css">
</head>
<body>

    <div class="auth-wrapper">
        <a href="Project1_PHP_MySQL/src/index.php" class="btn-home-back" title="Về trang chủ"><i class="fas fa-home"></i></a>

        <div class="auth-box">
            <h2>Đăng Nhập</h2>
            
            <?php if (!empty($message)): ?>
                <p style="color: #d9534f; background: #f9d6d5; padding: 10px; border-radius: 5px; font-size: 0.9em;">
                    <?php echo $message; ?>
                </p>
            <?php endif; ?>

            <form method="POST" action="login.php" class="auth-form">
                <label for="username">Tên đăng nhập</label>
                <input type="text" id="username" name="username" placeholder="Nhập tên đăng nhập..." required>

                <label for="password">Mật khẩu</label>
                <input type="password" id="password" name="password" placeholder="Nhập mật khẩu..." required>

                <button type="submit" class="btn-auth">ĐĂNG NHẬP</button>
            </form>

            <div class="auth-link">
                <p>Chưa là thành viên? <a href="Project1_PHP_MySQL/src/register.php">Đăng ký ngay</a></p>
            </div>
        </div>
    </div>

</body>
</html>
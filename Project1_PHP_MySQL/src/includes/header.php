<?php
session_start();
include 'config/db.php'; 

// --- 1. LOGIC PHP (Giữ nguyên) ---
$is_logged_in = isset($_SESSION['user_id']);
$username = isset($_SESSION['username']) ? $_SESSION['username'] : ''; 
$user_role = isset($_SESSION['role']) ? $_SESSION['role'] : '';

if (!isset($_SESSION['cart'])) {
    $_SESSION['cart'] = [];
}
$cart_count = 0;
if (!empty($_SESSION['cart'])) {
    $cart_count = array_sum(array_column($_SESSION['cart'], 'quantity'));
}
?>

<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>CTusCoffee - Hương vị Đam mê</title>
    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@700&family=Poppins:wght@400;600&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Project1_PHP_MySQL/src/assets/css/style.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    
    <style>
        /* Đảm bảo navigation bar luôn ở trên cùng và có màu nền nâu đậm */
        .nav-bar {
            background-color: #3C2A21; /* Màu nâu đậm, như trong hình bạn gửi */
            color: white;
            padding: 15px 0;
            position: sticky; 
            top: 0; 
            z-index: 100;
        }
        .nav-container {
            margin: 0 auto; 
            display: flex; 
            justify-content: space-between; 
            align-items: center; 
            width: 90%; 
            max-width: 1200px;
        }
        /* Style cho Logo CTusCoffee */
        .logo-text {
            color: white; 
            text-decoration: none; 
            font-size: 2.2em; /* To hơn đáng kể */
            font-family: 'Playfair Display', serif; /* Phông chữ đặc biệt */
            font-weight: 700;
            letter-spacing: 1.5px;
        }
        /* Style cho các liên kết Menu */
        .nav-links a {
            color: white;
            text-decoration: none;
            font-family: 'Poppins', sans-serif; /* Phông chữ đồng nhất, hiện đại */
            font-weight: 600; /* Dày hơn để dễ đọc */
            margin-left: 20px;
            transition: color 0.2s;
            font-size: 0.95em;
        }
        .nav-links a:hover {
            color: #FFCC00; /* Màu vàng cam khi hover */
        }
        /* Style cho tên người dùng đăng nhập */
        .nav-links span {
            color: #FFCC00; 
            font-weight: 500;
            font-family: 'Poppins', sans-serif;
            margin-left: 20px;
            font-size: 0.95em;
        }
    </style>
</head>
<body>
    <header class="nav-bar">
        <div class="nav-container">
            
            <a href="/Project1_PHP_MySQL/src/index.php" class="logo-text">CTusCoffee</a>
            
            <nav class="nav-links">
                <a href="/Project1_PHP_MySQL/src/index.php">Trang Chủ</a>
                
                <a href="/Project1_PHP_MySQL/src/cart.php" style="font-weight: bold;">Giỏ Hàng (<?php echo $cart_count; ?>)</a>
    
                <?php if ($is_logged_in): ?>
                    <span>Chào, **<?php echo htmlspecialchars($username); ?>**</span>
                    <a href="/Project1_PHP_MySQL/src/my_orders.php">Đơn Hàng</a>
                    
                    <?php if ($user_role == 'admin'): ?> 
                        <a href="/Project1_PHP_MySQL/src/admin/index.php">Admin</a>
                    <?php elseif ($user_role == 'kitchen'): ?> 
                        <a href="/Project1_PHP_MySQL/src/kitchen/index.php">Bếp</a>
                    <?php endif; ?>
                    
                    <a href="/Project1_PHP_MySQL/src/logout.php">Đăng Xuất</a>
                <?php else: ?>
                    <a href="/Project1_PHP_MySQL/src/login.php">Đăng Nhập</a>
                    <a href="/Project1_PHP_MySQL/src/register.php">Đăng Ký</a>
                <?php endif; ?>
                
    <a href="labs.php" class="nav-link">Các bài Lab</a>

            </nav>
        </div>
    </header>
    <main>
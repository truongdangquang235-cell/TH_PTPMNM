<?php
session_start();
include '../config/db.php'; // Lưu ý dấu ../

// --- BẢO MẬT: CHẶN TRUY CẬP NẾU KHÔNG PHẢI ADMIN ---
if (!isset($_SESSION['user_id']) || $_SESSION['role'] !== 'admin') {
    // Nếu chưa đăng nhập hoặc không phải admin, đá về trang login
    header('Location: ../login.php');
    exit;
}

// Lấy tên trang hiện tại để highlight menu
$current_page = basename($_SERVER['PHP_SELF']);
?>

<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Admin Dashboard - CTusCoffee</title>
    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@700&family=Poppins:wght@400;500;600&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">
</head>
<body>
    <div class="admin-wrapper">
        

<nav class="admin-sidebar">
    <div class="admin-sidebar-header">
        <h3><i class="fas fa-coffee"></i> CTus Admin</h3>
        <p style="font-size: 0.8em; color: #aaa;">Xin chào, <?php echo htmlspecialchars($_SESSION['username']); ?></p>
    </div>
    
    <ul class="admin-menu">
        <li>
            <a href="index.php" class="<?php echo ($current_page == 'index.php') ? 'active' : ''; ?>">
                <i class="fas fa-tachometer-alt"></i> Dashboard
            </a>
        </li>
        <li>
            <a href="products.php" class="<?php echo ($current_page == 'products.php') ? 'active' : ''; ?>">
                <i class="fas fa-box"></i> Quản lý Sản phẩm
            </a>
        </li>
        <li>
            <a href="categories.php" class="<?php echo ($current_page == 'categories.php') ? 'active' : ''; ?>">
                <i class="fas fa-tags"></i> Quản lý Danh mục
            </a>
        </li>
        <li>
            <a href="orders.php" class="<?php echo ($current_page == 'orders.php') ? 'active' : ''; ?>">
                <i class="fas fa-receipt"></i> Quản lý Đơn hàng
            </a>
        </li>
        <li>
    <a href="stats.php" class="<?php echo ($current_page == 'stats.php') ? 'active' : ''; ?>">
        <i class="fas fa-chart-pie"></i> Báo cáo Thống kê
    </a>
</li>
<li>
            <a href="coupons.php" class="<?php echo ($current_page == 'coupons.php') ? 'active' : ''; ?>">
                <i class="fas fa-ticket-alt"></i> Mã Giảm Giá
            </a>
        </li>
        <li>
            <a href="users.php" class="<?php echo ($current_page == 'users.php') ? 'active' : ''; ?>">
                <i class="fas fa-users"></i> Khách hàng
            </a>
        </li>
        
        <li style="margin-top: 20px; border-top: 1px solid #4a3b32;">
            <a href="../index.php" target="_blank">
                <i class="fas fa-globe"></i> Xem Website
            </a>
        </li>
        <li>
            <a href="../logout.php" style="color: #ff6b6b;">
                <i class="fas fa-sign-out-alt"></i> Đăng xuất
            </a>
        </li>
    </ul>
</nav>
<?php
session_start();
include '../config/db.php';

// --- BẢO MẬT: CHỈ CHO PHÉP KITCHEN HOẶC ADMIN ---
if (!isset($_SESSION['user_id']) || ($_SESSION['role'] !== 'kitchen' && $_SESSION['role'] !== 'admin')) {
    header('Location: ../login.php');
    exit;
}
?>
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="refresh" content="30"> 
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>KDS - Màn Hình Bếp</title>
    <link href="https://fonts.googleapis.com/css2?family=Courier+Prime:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">
</head>
<body style="background-color: #222;"> <div class="kds-wrapper">
        <div class="kds-header-bar">
            <div>
                <h2 style="margin: 0; color: #FFCC00;"><i class="fas fa-utensils"></i> CTus BẾP</h2>
            </div>
            <div>
                <span style="margin-right: 20px;">Xin chào, <?php echo htmlspecialchars($_SESSION['username']); ?></span>
                <a href="../logout.php" style="color: #ff6b6b; text-decoration: none; border: 1px solid #ff6b6b; padding: 5px 10px; border-radius: 4px;">Đăng xuất</a>
            </div>
        </div>
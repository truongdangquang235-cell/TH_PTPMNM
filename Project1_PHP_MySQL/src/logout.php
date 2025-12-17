<?php
session_start();

// Hủy tất cả các biến session
$_SESSION = array();

// Hủy session trên máy chủ
session_destroy();

// Chuyển hướng về trang chủ
header('Location: Project1_PHP_MySQL/src/index.php');
exit;
?>
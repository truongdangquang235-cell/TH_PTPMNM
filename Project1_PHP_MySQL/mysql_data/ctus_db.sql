-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Máy chủ: 127.0.0.1:3306
-- Thời gian đã tạo: Th12 13, 2025 lúc 10:55 AM
-- Phiên bản máy phục vụ: 9.1.0
-- Phiên bản PHP: 8.3.14

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Cơ sở dữ liệu: `ctus_db`
--

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `categories`
--

DROP TABLE IF EXISTS `categories`;
CREATE TABLE IF NOT EXISTS `categories` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=MyISAM AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Đang đổ dữ liệu cho bảng `categories`
--

INSERT INTO `categories` (`id`, `name`) VALUES
(1, 'Coffee Việt'),
(2, 'Trà Trái Cây'),
(3, 'Thức Uống Đá Xay'),
(4, 'Bánh Ngọt'),
(5, 'Coffee Hiện Đại');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `coupons`
--

DROP TABLE IF EXISTS `coupons`;
CREATE TABLE IF NOT EXISTS `coupons` (
  `id` int NOT NULL AUTO_INCREMENT,
  `code` varchar(50) NOT NULL,
  `discount_percent` int NOT NULL,
  `expiry_date` date NOT NULL,
  `status` enum('active','expired') DEFAULT 'active',
  PRIMARY KEY (`id`),
  UNIQUE KEY `code` (`code`)
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Đang đổ dữ liệu cho bảng `coupons`
--

INSERT INTO `coupons` (`id`, `code`, `discount_percent`, `expiry_date`, `status`) VALUES
(1, 'WELCOME', 20, '9999-12-05', 'active'),
(2, 'CHUDEPTRAI', 10, '9999-02-03', 'active');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `orders`
--

DROP TABLE IF EXISTS `orders`;
CREATE TABLE IF NOT EXISTS `orders` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `order_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `total_amount` decimal(10,0) NOT NULL,
  `shipping_name` varchar(100) NOT NULL,
  `shipping_phone` varchar(20) NOT NULL,
  `shipping_address` varchar(255) NOT NULL,
  `status` enum('Pending','Processing','Shipped','Delivered','Cancelled') NOT NULL DEFAULT 'Pending',
  `discount_amount` decimal(15,2) DEFAULT '0.00',
  `final_amount` decimal(15,2) DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`)
) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Đang đổ dữ liệu cho bảng `orders`
--

INSERT INTO `orders` (`id`, `user_id`, `order_date`, `total_amount`, `shipping_name`, `shipping_phone`, `shipping_address`, `status`, `discount_amount`, `final_amount`) VALUES
(1, 3, '2025-12-09 14:09:04', 122000, 'Nguyễn Văn A', '0905556666', 'Số 10, Đường Khách Hàng, TP.HCM', 'Delivered', 0.00, 0.00),
(2, 4, '2025-12-09 12:09:04', 74000, 'Trần Thị B', '0907778888', 'Số 20, Đường Nguyễn Huệ, Hà Nội', 'Delivered', 0.00, 0.00),
(3, 3, '2025-12-08 14:09:04', 108000, 'Nguyễn Văn A', '0905556666', 'Số 10, Đường Khách Hàng, TP.HCM', 'Delivered', 0.00, 0.00),
(4, 5, '2025-12-13 13:45:33', 114000, 'dangquangtruong', '012345678', 'sàasdasda', 'Delivered', 0.00, 0.00),
(5, 5, '2025-12-13 14:36:13', 57000, 'dangquangtruong', '1646545', '56465', 'Delivered', 0.00, 0.00),
(6, 7, '2025-12-13 14:44:47', 178000, 'khachhangdeptrai', '12341234', '12341234', 'Delivered', 0.00, 0.00),
(7, 7, '2025-12-13 14:55:39', 32000, 'khachhangdeptrai', '12341234', '123412341', 'Delivered', 0.00, 32000.00),
(8, 7, '2025-12-13 14:56:08', 32000, 'khachhangdeptrai', '12341234', '12341234', 'Delivered', 6400.00, 25600.00),
(9, 5, '2025-12-13 15:14:55', 49000, 'dangquangtruong', 'qưerqwe', 'qưerqwe', 'Delivered', 4900.00, 44100.00),
(10, 7, '2025-12-13 16:44:46', 35000, 'khachhangdeptrai', '12341234', '12341234', 'Delivered', 0.00, 35000.00),
(11, 5, '2025-12-13 17:44:40', 32000, 'dangquangtruong', 'q12341234', '123412', 'Delivered', 0.00, 32000.00),
(12, 7, '2025-12-13 17:49:56', 45000, 'khachhangdeptrai', '12341', '1234123', 'Pending', 9000.00, 36000.00);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `order_details`
--

DROP TABLE IF EXISTS `order_details`;
CREATE TABLE IF NOT EXISTS `order_details` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `product_id` int NOT NULL,
  `quantity` int NOT NULL,
  `price` decimal(10,0) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`),
  KEY `product_id` (`product_id`)
) ENGINE=MyISAM AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Đang đổ dữ liệu cho bảng `order_details`
--

INSERT INTO `order_details` (`id`, `order_id`, `product_id`, `quantity`, `price`) VALUES
(1, 1, 1, 2, 25000),
(2, 1, 3, 1, 35000),
(3, 1, 6, 1, 37000),
(4, 2, 2, 1, 29000),
(5, 2, 4, 1, 39000),
(6, 2, 5, 1, 45000),
(7, 3, 5, 2, 45000),
(8, 3, 1, 1, 25000),
(9, 4, 1, 2, 25000),
(10, 4, 2, 1, 29000),
(11, 4, 3, 1, 35000),
(12, 5, 7, 1, 32000),
(13, 5, 1, 1, 25000),
(14, 6, 9, 1, 45000),
(15, 6, 6, 1, 49000),
(16, 6, 14, 1, 39000),
(17, 6, 13, 1, 45000),
(18, 7, 7, 1, 32000),
(19, 8, 7, 1, 32000),
(20, 9, 6, 1, 49000),
(21, 10, 3, 1, 35000),
(22, 11, 7, 1, 32000),
(23, 12, 13, 1, 45000);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `products`
--

DROP TABLE IF EXISTS `products`;
CREATE TABLE IF NOT EXISTS `products` (
  `id` int NOT NULL AUTO_INCREMENT,
  `category_id` int NOT NULL,
  `name` varchar(255) NOT NULL,
  `price` decimal(10,0) NOT NULL,
  `description` text,
  `image` varchar(255) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `category_id` (`category_id`)
) ENGINE=MyISAM AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Đang đổ dữ liệu cho bảng `products`
--

INSERT INTO `products` (`id`, `category_id`, `name`, `price`, `description`, `image`, `created_at`) VALUES
(1, 1, 'Cà Phê Đen Đá', 25000, 'Đậm đà hương vị truyền thống', 'cf_den_da.jpg', '2025-12-09 07:08:49'),
(2, 1, 'Cà Phê Sữa Đá', 29000, 'Ngọt ngào và mạnh mẽ', 'cf_sua_da.jpg', '2025-12-09 07:08:49'),
(3, 2, 'Trà Vải', 35000, 'Thơm mát vị vải tươi', 'tra_vai.jpg', '2025-12-09 07:08:49'),
(4, 2, 'Trà Đào Cam Sả', 39000, 'Sự kết hợp hoàn hảo', 'tra_dao_cam_sa.jpg', '2025-12-09 07:08:49'),
(5, 3, 'Chocolate Đá Xay', 45000, 'Mát lạnh, béo ngậy vị chocolate', 'choco_freeze.jpg', '2025-12-09 07:08:49'),
(6, 4, 'Tiramisu', 49000, 'Bánh ngọt Ý cổ điển', 'tiramisu.jpg', '2025-12-09 07:08:49'),
(7, 1, 'Bạc Xỉu', 32000, 'Hương vị béo ngậy của sữa hòa quyện cà phê', 'bac_xiu.jpg', '2025-12-13 07:11:49'),
(8, 5, 'Espresso', 25000, 'Cà phê nguyên chất đậm đà phong cách Ý', 'espresso.jpg', '2025-12-13 07:11:49'),
(9, 5, 'Cappuccino', 45000, 'Cà phê kết hợp bọt sữa bồng bềnh', 'cappuccino.jpg', '2025-12-13 07:11:49'),
(10, 5, 'Latte', 45000, 'Vị sữa dịu nhẹ, cà phê thơm lừng', 'latte.jpg', '2025-12-13 07:11:49'),
(11, 3, 'Matcha Đá Xay', 55000, 'Trà xanh Nhật Bản xay nhuyễn mát lạnh', 'matcha_freeze.jpg', '2025-12-13 07:11:49'),
(12, 3, 'Cookie Đá Xay', 55000, 'Bánh quy Oreo xay cùng đá tuyết', 'cookie_freeze.jpg', '2025-12-13 07:11:49'),
(13, 4, 'Bánh Red Velvet', 45000, 'Chiếc bánh nhung đỏ ngọt ngào', 'red_velvet.jpg', '2025-12-13 07:11:49'),
(14, 4, 'Mousse Chanh Dây', 39000, 'Chua ngọt thanh mát, mềm tan trong miệng', 'mousse_chanh.jpg', '2025-12-13 07:11:49');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `users`
--

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `email` varchar(100) DEFAULT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `address` varchar(255) DEFAULT NULL,
  `role` enum('admin','kitchen','user') NOT NULL DEFAULT 'user',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `total_spent` decimal(15,2) DEFAULT '0.00',
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`),
  UNIQUE KEY `email` (`email`)
) ENGINE=MyISAM AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Đang đổ dữ liệu cho bảng `users`
--

INSERT INTO `users` (`id`, `username`, `password`, `email`, `phone`, `address`, `role`, `created_at`, `total_spent`) VALUES
(1, 'admin_ct', '$2y$10$t3Xy/w.H.uX9Y0o4gO0W9.P/C1zXjT7yB0P8uR/A2jJ8yK9', 'admin@ctcoffee.com', '0901112222', 'Số 1, Đường Admin, Quận 1', 'admin', '2025-12-09 07:08:14', 0.00),
(2, 'kitchen_01', '$2y$10$t3Xy/w.H.uX9Y0o4gO0W9.P/C1zXjT7yB0P8uR/A2jJ8yK9', 'kitchen@ctcoffee.com', '0903334444', 'Số 5, Đường Bếp, Quận 5', 'kitchen', '2025-12-09 07:08:14', 0.00),
(3, 'khach_hang_A', '$2y$10$t3Xy/w.H.uX9Y0o4gO0W9.P/C1zXjT7yB0P8uR/A2jJ8yK9', 'khachhangA@gmail.com', '0905556666', 'Số 10, Đường Khách Hàng, TP.HCM', 'user', '2025-12-09 07:08:14', 0.00),
(4, 'khach_hang_B', '$2y$10$t3Xy/w.H.uX9Y0o4gO0W9.P/C1zXjT7yB0P8uR/A2jJ8yK9', 'khachhangB@gmail.com', '0907778888', 'Số 20, Đường Nguyễn Huệ, Hà Nội', 'user', '2025-12-09 07:08:14', 0.00),
(5, 'dangquangtruong', '$2y$10$yLeoTWvBJoV90QcNOgo0ZOTUSCmcOfYIxI/WJhWsFWXgosG.tD3La', 'truongdangquang235@gmail.com', NULL, NULL, 'admin', '2025-12-13 06:41:40', 0.00),
(6, 'taoladaubep', '$2y$10$s7HfA2nncf5NpOXIry/RI.8dl3JXz9ItXrDue13Q8BAwtGNQtVbmO', 'beptruongdeptrai@gmail.com', NULL, NULL, 'kitchen', '2025-12-13 07:35:00', 0.00),
(7, 'khachhangdeptrai', '$2y$10$MqAKuldTkVbtyUi3j9N5U.oPA.yWJOt7Mh5nygf0bNnkywrjHSGKu', 'deptrai@gmail.com', NULL, NULL, 'user', '2025-12-13 07:43:56', 0.00);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

<?php
include 'includes/header.php';
include 'includes/sidebar.php';

// --- THỐNG KÊ DỮ LIỆU ---

// 1. Tổng doanh thu (Chỉ tính đơn đã giao 'Delivered')
$revenue_sql = "SELECT SUM(total_amount) AS total FROM orders WHERE status = 'Delivered'";
$revenue_result = $conn->query($revenue_sql);
$revenue = $revenue_result->fetch_assoc()['total'] ?? 0;

// 2. Tổng số đơn hàng
$order_count_sql = "SELECT COUNT(*) AS total FROM orders";
$order_count = $conn->query($order_count_sql)->fetch_assoc()['total'];

// 3. Đơn hàng mới (Pending)
$pending_count_sql = "SELECT COUNT(*) AS total FROM orders WHERE status = 'Pending'";
$pending_count = $conn->query($pending_count_sql)->fetch_assoc()['total'];

// 4. Tổng số sản phẩm
$product_count_sql = "SELECT COUNT(*) AS total FROM products";
$product_count = $conn->query($product_count_sql)->fetch_assoc()['total'];
?>

<main class="admin-content">
    <h1 class="admin-page-title">Tổng Quan Kinh Doanh</h1>

    <div class="stats-grid">
        
        <div class="stat-card" style="border-left-color: #28a745;">
            <div class="stat-info">
                <h3><?php echo number_format($revenue, 0, ',', '.'); ?>đ</h3>
                <p>Doanh thu thực tế</p>
            </div>
            <div class="stat-icon" style="color: #28a745;"><i class="fas fa-money-bill-wave"></i></div>
        </div>

        <div class="stat-card" style="border-left-color: #17a2b8;">
            <div class="stat-info">
                <h3><?php echo $order_count; ?></h3>
                <p>Tổng đơn hàng</p>
            </div>
            <div class="stat-icon" style="color: #17a2b8;"><i class="fas fa-shopping-cart"></i></div>
        </div>

        <div class="stat-card" style="border-left-color: #ffc107;">
            <div class="stat-info">
                <h3 style="color: #d39e00;"><?php echo $pending_count; ?></h3>
                <p>Đơn chờ xử lý</p>
            </div>
            <div class="stat-icon" style="color: #ffc107;"><i class="fas fa-clock"></i></div>
        </div>

        <div class="stat-card" style="border-left-color: #6c757d;">
            <div class="stat-info">
                <h3><?php echo $product_count; ?></h3>
                <p>Món trong Menu</p>
            </div>
            <div class="stat-icon" style="color: #6c757d;"><i class="fas fa-coffee"></i></div>
        </div>

    </div>

    <div style="background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05);">
        <h3 style="font-family: 'Playfair Display', serif; margin-bottom: 20px;">Lời chào từ hệ thống</h3>
        <p>Chào mừng Admin quay trở lại. Hãy chọn một chức năng bên thanh menu trái để bắt đầu quản lý.</p>
    </div>

</main>

<?php include 'includes/footer.php'; ?>
<?php
include 'includes/header.php';

// Yêu cầu đăng nhập
if (!$is_logged_in) {
    header('Location: login.php');
    exit;
}

$user_id = $_SESSION['user_id'];

// Lấy danh sách đơn hàng
$sql = "SELECT id, order_date, total_amount, status FROM orders WHERE user_id = $user_id ORDER BY order_date DESC";
$result = $conn->query($sql);

// Đếm tổng số đơn hàng để hiển thị thống kê
$total_orders = $result->num_rows;
?>

<div class="checkout-section">
    <div class="container">
        
        <div class="checkout-card">
            
            <div class="checkout-form-col">
                <h2 class="checkout-title">Lịch Sử Đơn Hàng</h2>

                <?php if ($total_orders > 0): ?>
                    <div style="overflow-x: auto;"> <table class="order-table">
                            <thead>
                                <tr>
                                    <th>Mã Đơn</th>
                                    <th>Ngày Đặt</th>
                                    <th>Tổng Tiền</th>
                                    <th>Trạng Thái</th>
                                    <th>Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                <?php 
                                // Reset pointer về đầu vì chúng ta đã dùng num_rows ở trên (hoặc không cần thiết nếu num_rows không di chuyển pointer)
                                // Nhưng an toàn nhất là cứ loop bình thường
                                while ($order = $result->fetch_assoc()): 
                                ?>
                                <tr>
                                    <td><strong>#<?php echo $order['id']; ?></strong></td>
                                    <td><?php echo date('d/m/Y H:i', strtotime($order['order_date'])); ?></td>
                                    <td style="font-weight: bold; color: #3C2A21;"><?php echo number_format($order['total_amount'], 0, ',', '.'); ?>đ</td>
                                    <td>
                                        <span class="status-badge status-<?php echo $order['status']; ?>">
                                            <?php echo $order['status']; ?>
                                        </span>
                                    </td>
                                    <td>
                                        <a href="Project1_PHP_MySQL/src/my_order_detail.php?order_id=<?php echo $order['id']; ?>" class="btn-view-detail">
                                            Xem <i class="fas fa-arrow-right"></i>
                                        </a>
                                    </td>
                                </tr>
                                <?php endwhile; ?>
                            </tbody>
                        </table>
                    </div>
                <?php else: ?>
                    <div style="text-align: center; padding: 40px; color: #777;">
                        <i class="fas fa-mug-hot" style="font-size: 3em; margin-bottom: 20px; color: #ddd;"></i>
                        <p>Bạn chưa có đơn hàng nào tại CTusCoffee.</p>
                        <p>Hãy thử gọi món đồ uống yêu thích ngay nhé!</p>
                    </div>
                <?php endif; ?>
            </div>

            <div class="checkout-summary-col">
                
                <div class="user-profile-summary">
                    <div class="user-avatar-placeholder">
                        <i class="fas fa-user"></i>
                    </div>
                    <h3 style="font-family: 'Playfair Display', serif; margin: 10px 0;">
                        <?php echo htmlspecialchars($_SESSION['username']); ?>
                    </h3>
                    <p style="color: #777; font-size: 0.9em;">Thành viên thân thiết</p>
                </div>

                <div class="bill-box" style="border: none; background: transparent; padding: 0;">
                    <div class="bill-row">
                        <span>Tổng đơn hàng</span>
                        <strong><?php echo $total_orders; ?></strong>
                    </div>
                    <div class="bill-row">
                        <span>Hạng thành viên</span>
                        <strong>Vàng</strong>
                    </div>
                </div>

                <div style="margin-top: 30px;">
                    <a href="Project1_PHP_MySQL/src/index.php" class="btn-finish-order" style="text-align: center; text-decoration: none;">
                        ĐẶT MÓN MỚI NGAY
                    </a>
                    
                    <a href="Project1_PHP_MySQL/src/logout.php" class="back-cart-link" style="margin-top: 20px; color: #dc3545;">
                        <i class="fas fa-sign-out-alt"></i> Đăng xuất
                    </a>
                </div>

            </div>

        </div> </div>
</div>

<?php include 'includes/footer.php'; ?>
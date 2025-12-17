<?php
include 'includes/header.php';

// 1. KIỂM TRA ĐĂNG NHẬP
if (!$is_logged_in) {
    header('Location: login.php');
    exit;
}

$user_id = $_SESSION['user_id'];
$order_id = isset($_GET['order_id']) ? intval($_GET['order_id']) : 0;

// 2. LẤY THÔNG TIN ĐƠN HÀNG
// Chỉ lấy đơn hàng thuộc về user đang đăng nhập (AND user_id = $user_id) để bảo mật
$sql = "SELECT * FROM orders WHERE id = $order_id AND user_id = $user_id";
$result = $conn->query($sql);

if ($result->num_rows == 0) {
    // Nếu không tìm thấy hoặc đơn không phải của user này
    echo "<div class='container' style='padding: 50px; text-align: center;'>
            <h3>Không tìm thấy đơn hàng hoặc bạn không có quyền truy cập.</h3>
            <a href='index.php' class='btn-shop'>Về trang chủ</a>
          </div>";
    include 'includes/footer.php';
    exit;
}

$order = $result->fetch_assoc();

// 3. LẤY CHI TIẾT SẢN PHẨM TRONG ĐƠN
$detail_sql = "SELECT od.*, p.name, p.image 
               FROM order_details od 
               JOIN products p ON od.product_id = p.id 
               WHERE od.order_id = $order_id";
$details_result = $conn->query($detail_sql);

// Thông báo thành công nếu vừa đặt hàng xong
$success_msg = '';
if (isset($_GET['success']) && $_GET['success'] == 'true') {
    $success_msg = "🎉 Đặt hàng thành công! Cảm ơn bạn đã ủng hộ.";
}
?>

<div class="checkout-section">
    <div class="container">
        
        <div class="checkout-card">
            
            <div class="checkout-form-col">
                
                <?php if (!empty($success_msg)): ?>
                    <div style="background-color: #d4edda; color: #155724; padding: 15px; border-radius: 5px; margin-bottom: 20px; text-align: center; border: 1px solid #c3e6cb;">
                        <i class="fas fa-check-circle"></i> <strong><?php echo $success_msg; ?></strong>
                    </div>
                <?php endif; ?>

                <h2 class="checkout-title">Đơn Hàng #<?php echo $order['id']; ?></h2>
                
                <div class="info-row">
                    <strong><i class="far fa-calendar-alt"></i> Ngày đặt:</strong> 
                    <?php echo date('d/m/Y H:i', strtotime($order['order_date'])); ?>
                </div>
                
                <div class="info-row">
                    <strong><i class="fas fa-info-circle"></i> Trạng thái:</strong> 
                    <span class="status-badge status-<?php echo $order['status']; ?>">
                        <?php 
                        switch($order['status']) {
                            case 'Pending': echo 'Chờ xử lý'; break;
                            case 'Processing': echo 'Đang chuẩn bị'; break;
                            case 'Delivered': echo 'Đã giao hàng'; break;
                            case 'Cancelled': echo 'Đã hủy'; break;
                            default: echo $order['status'];
                        }
                        ?>
                    </span>
                </div>

                <hr class="divider-coffee">

                <h3 style="font-family: 'Playfair Display', serif; color: #3C2A21; margin-bottom: 20px;">
                    Thông Tin Giao Hàng
                </h3>

                <div class="info-row">
                    <strong><i class="fas fa-user"></i> Người nhận:</strong> 
                    <?php echo htmlspecialchars($order['shipping_name']); ?>
                </div>

                <div class="info-row">
                    <strong><i class="fas fa-phone"></i> Điện thoại:</strong> 
                    <?php echo htmlspecialchars($order['shipping_phone']); ?>
                </div>

                <div class="info-row">
                    <strong><i class="fas fa-map-marker-alt"></i> Địa chỉ:</strong> 
                    <?php echo nl2br(htmlspecialchars($order['shipping_address'])); ?>
                </div>
                
                <div style="margin-top: 40px;">
                    <a href="Project1_PHP_MySQL/src/my_orders.php" style="color: #A0522D; text-decoration: none; font-weight: bold;">
                        <i class="fas fa-arrow-left"></i> Quay lại Lịch sử Đơn hàng
                    </a>
                </div>

            </div>

            <div class="checkout-summary-col">
                <h2 class="checkout-title" style="font-size: 1.5em;">Chi Tiết Hóa Đơn</h2>
                
                <div class="bill-box">
                    <?php 
                    $subtotal_calc = 0; // Biến tính lại tổng tiền hàng để hiển thị
                    while ($item = $details_result->fetch_assoc()): 
                        $item_total = $item['price'] * $item['quantity'];
                        $subtotal_calc += $item_total;
                    ?>
                    <div class="bill-row">
                        <span style="display: flex; align-items: center; gap: 10px;">
                            <img src="assets/images/<?php echo htmlspecialchars($item['image']); ?>" 
                                 style="width: 40px; height: 40px; border-radius: 4px; object-fit: cover; border: 1px solid #ddd;"
                                 onerror="this.src='https://via.placeholder.com/40'">
                            <div>
                                <strong><?php echo htmlspecialchars($item['name']); ?></strong> 
                                <span style="font-size: 0.9em; color: #777;">x<?php echo $item['quantity']; ?></span>
                            </div>
                        </span>
                        <span><?php echo number_format($item_total, 0, ',', '.'); ?></span>
                    </div>
                    <?php endwhile; ?>
                    
                    <div style="border-top: 1px dashed #ccc; margin: 15px 0;"></div>
                    
                    <div class="bill-row">
                        <span>Tạm tính</span>
                        <span><?php echo number_format($subtotal_calc, 0, ',', '.'); ?>đ</span>
                    </div>

                    <?php if ($order['discount_amount'] > 0): ?>
                    <div class="bill-row" style="color: #28a745;">
                        <span>Giảm giá (Voucher/TV)</span>
                        <span>-<?php echo number_format($order['discount_amount'], 0, ',', '.'); ?>đ</span>
                    </div>
                    <?php endif; ?>
                    
                    <div class="bill-row">
                        <span>Phí vận chuyển</span>
                        <span>0 đ</span>
                    </div>

                    <div class="bill-total">
                        <span>TỔNG THANH TOÁN</span>
                        <span>
                            <?php 
                            // Nếu DB chưa có cột final_amount (do dữ liệu cũ), ta fallback về total_amount
                            $final = isset($order['final_amount']) && $order['final_amount'] > 0 
                                     ? $order['final_amount'] 
                                     : $order['total_amount'];
                            echo number_format($final, 0, ',', '.'); 
                            ?> đ
                        </span>
                    </div>
                </div>

                <div style="text-align: center; margin-top: 30px;">
                    <p style="font-style: italic; color: #666; font-size: 0.9em;">Cảm ơn bạn đã lựa chọn CTusCoffee!</p>
                    
                    <a href="Project1_PHP_MySQL/src/index.php" class="btn-finish-order" style="text-align: center; text-decoration: none;">
                        ĐẶT THÊM MÓN MỚI <i class="fas fa-mug-hot"></i>
                    </a>
                </div>
                
            </div>

        </div> </div>
</div>

<?php include 'includes/footer.php'; ?>
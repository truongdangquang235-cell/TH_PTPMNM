<?php
include 'includes/header.php';
include 'includes/sidebar.php';

$order_id = isset($_GET['id']) ? intval($_GET['id']) : 0;

// 1. LẤY THÔNG TIN ĐƠN HÀNG
$order_sql = "SELECT * FROM orders WHERE id = $order_id";
$order_result = $conn->query($order_sql);

if ($order_result->num_rows == 0) {
    echo "<div class='admin-content'><h3>Không tìm thấy đơn hàng.</h3><a href='orders.php' class='btn-admin'>Quay lại</a></div>";
    include 'includes/footer.php';
    exit;
}
$order = $order_result->fetch_assoc();

// --- XỬ LÝ CẬP NHẬT TRẠNG THÁI & TÍCH ĐIỂM ---
if ($_SERVER['REQUEST_METHOD'] == 'POST' && isset($_POST['update_status'])) {
    $new_status = $_POST['status'];
    $old_status = $order['status'];

    // Cập nhật trạng thái mới
    $conn->query("UPDATE orders SET status = '$new_status' WHERE id = $order_id");

    // LOGIC TÍCH ĐIỂM THÀNH VIÊN
    // Chỉ cộng điểm khi chuyển từ trạng thái KHÁC sang 'Delivered'
    // (Tránh trường hợp bấm Delivered nhiều lần bị cộng dồn nhiều lần)
    if ($new_status == 'Delivered' && $old_status != 'Delivered') {
        $user_id = $order['user_id'];
        
        // Lấy số tiền thực trả (final_amount) để cộng tích lũy
        // Nếu database cũ chưa có final_amount thì dùng total_amount
        $amount_to_add = ($order['final_amount'] > 0) ? $order['final_amount'] : $order['total_amount'];

        if ($user_id > 0 && $amount_to_add > 0) {
            $conn->query("UPDATE users SET total_spent = total_spent + $amount_to_add WHERE id = $user_id");
        }
    }

    echo "<script>alert('Đã cập nhật trạng thái đơn hàng!'); window.location.href='order_detail.php?id=$order_id';</script>";
}

// 2. LẤY CHI TIẾT SẢN PHẨM
$detail_sql = "SELECT od.*, p.name, p.image 
               FROM order_details od 
               JOIN products p ON od.product_id = p.id 
               WHERE od.order_id = $order_id";
$details_result = $conn->query($detail_sql);
?>

<main class="admin-content">
    
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
        <h1 class="admin-page-title" style="margin: 0; border: none;">Chi Tiết Đơn #<?php echo $order_id; ?></h1>
        <a href="orders.php" class="btn-admin" style="background: #6c757d; color: white;">&larr; Quay lại danh sách</a>
    </div>

    <div class="admin-form-card" style="margin-bottom: 30px; background-color: #fff3cd; border: 1px solid #ffeeba; border-left: 5px solid #ffc107;">
        <form action="" method="POST" style="display: flex; align-items: center; gap: 20px;">
            <label style="margin: 0; font-weight: bold; color: #856404;">Cập nhật trạng thái:</label>
            
            <select name="status" class="form-control" style="width: auto; display: inline-block;">
                <option value="Pending" <?php echo ($order['status'] == 'Pending') ? 'selected' : ''; ?>>Pending (Chờ xử lý)</option>
                <option value="Processing" <?php echo ($order['status'] == 'Processing') ? 'selected' : ''; ?>>Processing (Đang làm)</option>
                <option value="Delivered" <?php echo ($order['status'] == 'Delivered') ? 'selected' : ''; ?>>Delivered (Đã giao)</option>
                <option value="Cancelled" <?php echo ($order['status'] == 'Cancelled') ? 'selected' : ''; ?>>Cancelled (Đã hủy)</option>
            </select>

            <button type="submit" name="update_status" class="btn-admin btn-add-new" style="margin: 0;">
                <i class="fas fa-sync-alt"></i> Cập nhật
            </button>
        </form>
        <p style="margin: 5px 0 0; font-size: 0.85em; color: #666;">
            * Lưu ý: Khi chuyển sang <strong>Delivered</strong>, hệ thống sẽ tự động cộng điểm tích lũy cho khách hàng.
        </p>
    </div>

    <div style="display: flex; gap: 30px; flex-wrap: wrap;">
        
        <div class="admin-form-card" style="flex: 1; min-width: 300px; margin: 0; height: fit-content;">
            <h3 style="border-bottom: 1px solid #eee; padding-bottom: 10px; margin-top: 0; color: #3C2A21;">Thông Tin Khách Hàng</h3>
            
            <p><strong>Người nhận:</strong> <br><?php echo htmlspecialchars($order['shipping_name']); ?></p>
            <p><strong>Số điện thoại:</strong> <br><?php echo htmlspecialchars($order['shipping_phone']); ?></p>
            <p><strong>Địa chỉ:</strong> <br><?php echo nl2br(htmlspecialchars($order['shipping_address'])); ?></p>
            <hr style="border: 0; border-top: 1px dashed #ddd; margin: 15px 0;">
            <p><strong>Ngày đặt:</strong> <?php echo date('d/m/Y H:i', strtotime($order['order_date'])); ?></p>
            <p><strong>User ID:</strong> #<?php echo $order['user_id']; ?></p>
        </div>

        <div class="admin-form-card" style="flex: 1.5; min-width: 400px; margin: 0;">
            <h3 style="border-bottom: 1px solid #eee; padding-bottom: 10px; margin-top: 0; color: #3C2A21;">Danh Sách Món</h3>
            
            <table class="admin-table" style="box-shadow: none;">
                <thead>
                    <tr>
                        <th>Món</th>
                        <th style="text-align: right;">Giá</th>
                        <th style="text-align: center;">SL</th>
                        <th style="text-align: right;">Thành tiền</th>
                    </tr>
                </thead>
                <tbody>
                    <?php 
                    $calc_subtotal = 0;
                    while ($item = $details_result->fetch_assoc()): 
                        $row_total = $item['price'] * $item['quantity'];
                        $calc_subtotal += $row_total;
                    ?>
                    <tr>
                        <td style="display: flex; align-items: center; gap: 10px; border: none;">
                            <img src="../assets/images/<?php echo htmlspecialchars($item['image']); ?>" 
                                 width="40" height="40" style="border-radius: 4px; border: 1px solid #ddd; object-fit: cover;"
                                 onerror="this.src='https://via.placeholder.com/40'">
                            <div>
                                <strong><?php echo htmlspecialchars($item['name']); ?></strong>
                            </div>
                        </td>
                        <td style="text-align: right;"><?php echo number_format($item['price'], 0, ',', '.'); ?></td>
                        <td style="text-align: center;">x<?php echo $item['quantity']; ?></td>
                        <td style="text-align: right; font-weight: bold;"><?php echo number_format($row_total, 0, ',', '.'); ?></td>
                    </tr>
                    <?php endwhile; ?>
                    
                    <tr style="border-top: 2px solid #3C2A21;">
                        <td colspan="3" style="text-align: right;">Tạm tính:</td>
                        <td style="text-align: right;"><?php echo number_format($calc_subtotal, 0, ',', '.'); ?>đ</td>
                    </tr>

                    <?php if ($order['discount_amount'] > 0): ?>
                    <tr style="color: #28a745;">
                        <td colspan="3" style="text-align: right;">Giảm giá (Coupon/Thành viên):</td>
                        <td style="text-align: right;">-<?php echo number_format($order['discount_amount'], 0, ',', '.'); ?>đ</td>
                    </tr>
                    <?php endif; ?>

                    <tr style="background-color: #fffbf2; font-size: 1.1em;">
                        <td colspan="3" style="text-align: right; font-weight: bold; color: #3C2A21;">THỰC THU:</td>
                        <td style="text-align: right; font-weight: bold; color: #A0522D; font-size: 1.2em;">
                            <?php 
                            $final_show = ($order['final_amount'] > 0) ? $order['final_amount'] : $order['total_amount'];
                            echo number_format($final_show, 0, ',', '.'); 
                            ?>đ
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

    </div>
</main>

<?php include 'includes/footer.php'; ?>
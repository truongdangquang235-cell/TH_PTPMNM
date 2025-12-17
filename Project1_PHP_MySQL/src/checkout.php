<?php
include 'includes/header.php';

if (!$is_logged_in || empty($_SESSION['cart'])) {
    header('Location: index.php'); exit;
}

// 1. TÍNH TOÁN GIỎ HÀNG 
$cart_items = $_SESSION['cart'];
$subtotal = 0;
foreach ($cart_items as $item) $subtotal += $item['price'] * $item['quantity'];

// 2. TÍNH GIẢM GIÁ THÀNH VIÊN 
$user_id = $_SESSION['user_id'];
$user_query = $conn->query("SELECT total_spent FROM users WHERE id = $user_id");
$user_spent = $user_query->fetch_assoc()['total_spent'];

$member_rank = 'Thành viên mới';
$member_discount_percent = 0;

if ($user_spent >= 10000000) { $member_rank = '💎 KIM CƯƠNG'; $member_discount_percent = 15; }
elseif ($user_spent >= 5000000) { $member_rank = '🥇 VÀNG'; $member_discount_percent = 10; }
elseif ($user_spent >= 1000000) { $member_rank = '🥈 BẠC'; $member_discount_percent = 5; }

$member_discount_amount = ($subtotal * $member_discount_percent) / 100;

// 3. XỬ LÝ COUPON 
$coupon_discount_amount = 0;
$coupon_code = '';
$coupon_msg = '';

if (isset($_POST['apply_coupon'])) {
    $code_input = strtoupper($conn->real_escape_string($_POST['coupon_code']));
    $today = date('Y-m-d');
    
    $cp_sql = "SELECT * FROM coupons WHERE code = '$code_input' AND expiry_date >= '$today' AND status = 'active'";
    $cp_res = $conn->query($cp_sql);
    
    if ($cp_res->num_rows > 0) {
        $coupon = $cp_res->fetch_assoc();
        $_SESSION['applied_coupon'] = $coupon; 
        $coupon_msg = "Áp dụng mã giảm " . $coupon['discount_percent'] . "% thành công!";
    } else {
        $coupon_msg = "Mã không hợp lệ hoặc đã hết hạn.";
        unset($_SESSION['applied_coupon']);
    }
}


if (isset($_SESSION['applied_coupon'])) {
    $coupon = $_SESSION['applied_coupon'];
    $coupon_code = $coupon['code'];
    $coupon_discount_amount = ($subtotal * $coupon['discount_percent']) / 100;
}


$total_discount = $member_discount_amount + $coupon_discount_amount;
$final_amount = $subtotal - $total_discount;
if ($final_amount < 0) $final_amount = 0;

if (isset($_POST['place_order'])) {
    $ship_name = $_POST['shipping_name'];
    $ship_phone = $_POST['shipping_phone'];
    $ship_address = $_POST['shipping_address'];
    
    $conn->begin_transaction();
    try {
       
        $order_sql = "INSERT INTO orders (user_id, total_amount, discount_amount, final_amount, shipping_name, shipping_phone, shipping_address, status) 
                      VALUES ('$user_id', '$subtotal', '$total_discount', '$final_amount', '$ship_name', '$ship_phone', '$ship_address', 'Pending')";
        $conn->query($order_sql);
        $order_id = $conn->insert_id;


        foreach ($cart_items as $item) {
            $conn->query("INSERT INTO order_details (order_id, product_id, quantity, price) VALUES ('$order_id', '{$item['product_id']}', '{$item['quantity']}', '{$item['price']}')");
        }

        $conn->commit();
        unset($_SESSION['cart']);
        unset($_SESSION['applied_coupon']); 
        echo "<script>window.location.href='my_order_detail.php?order_id=$order_id&success=true';</script>";
        exit;
    } catch (Exception $e) {
        $conn->rollback();
        echo "Lỗi: " . $e->getMessage();
    }
}

// Lấy thông tin user điền sẵn
$u_info = $conn->query("SELECT phone, address FROM users WHERE id=$user_id")->fetch_assoc();
?>

<div class="checkout-section">
    <div class="container">
        <form method="POST" class="checkout-card">
            
            <div class="checkout-form-col">
                <h2 class="checkout-title">Thanh Toán</h2>
                
                <div style="background: #fff3cd; padding: 15px; border-radius: 5px; margin-bottom: 20px; border: 1px solid #ffeeba;">
                    <i class="fas fa-crown" style="color: #d39e00;"></i> Hạng của bạn: <strong><?php echo $member_rank; ?></strong>
                    <br>
                    <small>Chi tiêu tích lũy: <?php echo number_format($user_spent, 0, ',', '.'); ?>đ (Giảm <?php echo $member_discount_percent; ?>% trên đơn này)</small>
                </div>

                <div class="coffee-input-group">
                    <label>Người nhận</label>
                    <input type="text" name="shipping_name" class="coffee-input" value="<?php echo htmlspecialchars($_SESSION['username']); ?>" required>
                </div>
                <div class="coffee-input-group">
                    <label>Điện thoại</label>
                    <input type="text" name="shipping_phone" class="coffee-input" value="<?php echo htmlspecialchars($u_info['phone'] ?? ''); ?>" required>
                </div>
                <div class="coffee-input-group">
                    <label>Địa chỉ</label>
                    <textarea name="shipping_address" class="coffee-input" rows="2" required><?php echo htmlspecialchars($u_info['address'] ?? ''); ?></textarea>
                </div>
            </div>

            <div class="checkout-summary-col">
                <h2 class="checkout-title">Đơn Hàng</h2>
                
                <div class="bill-box">
                    <?php foreach ($cart_items as $item): ?>
                    <div class="bill-row">
                        <span><?php echo $item['name']; ?> x<?php echo $item['quantity']; ?></span>
                        <span><?php echo number_format($item['price'] * $item['quantity'], 0, ',', '.'); ?></span>
                    </div>
                    <?php endforeach; ?>
                    
                    <div style="border-top: 1px dashed #ccc; margin: 10px 0;"></div>

                    <div class="bill-row">
                        <span>Tạm tính</span>
                        <span><?php echo number_format($subtotal, 0, ',', '.'); ?>đ</span>
                    </div>

                    <?php if($member_discount_amount > 0): ?>
                    <div class="bill-row" style="color: #28a745;">
                        <span>Thành viên (<?php echo $member_rank; ?>)</span>
                        <span>-<?php echo number_format($member_discount_amount, 0, ',', '.'); ?>đ</span>
                    </div>
                    <?php endif; ?>

                    <?php if($coupon_discount_amount > 0): ?>
                    <div class="bill-row" style="color: #28a745;">
                        <span>Mã <?php echo $coupon_code; ?></span>
                        <span>-<?php echo number_format($coupon_discount_amount, 0, ',', '.'); ?>đ</span>
                    </div>
                    <?php endif; ?>

                    <div class="bill-total">
                        <span>THANH TOÁN</span>
                        <span><?php echo number_format($final_amount, 0, ',', '.'); ?>đ</span>
                    </div>
                </div>

                <div style="margin-top: 20px; display: flex; gap: 5px;">
                    <input type="text" name="coupon_code" class="coffee-input" placeholder="Nhập mã giảm giá..." value="<?php echo $coupon_code; ?>" style="margin: 0;">
                    <button type="submit" name="apply_coupon" class="btn-admin" style="background: #3C2A21; color: white; white-space: nowrap;">Áp dụng</button>
                </div>
                <?php if($coupon_msg): ?>
                    <p style="font-size: 0.9em; color: <?php echo strpos($coupon_msg, 'thành công') ? 'green' : 'red'; ?>; margin-top: 5px;">
                        <?php echo $coupon_msg; ?>
                    </p>
                <?php endif; ?>

                <button type="submit" name="place_order" class="btn-finish-order">ĐẶT HÀNG NGAY</button>
            </div>
        </form>
    </div>
</div>
<?php include 'includes/footer.php'; ?>
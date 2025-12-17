<?php
include 'includes/header.php'; // Bắt đầu session và kết nối DB

// --- 1. XỬ LÝ LOGIC PHP ---

// Khởi tạo giỏ hàng nếu chưa có
if (!isset($_SESSION['cart'])) {
    $_SESSION['cart'] = [];
}

$message = '';

// A. XỬ LÝ YÊU CẦU POST (Thêm vào giỏ, Cập nhật số lượng)
if ($_SERVER['REQUEST_METHOD'] == 'POST' && isset($_POST['action'])) {
    $action = $_POST['action'];
    $product_id = isset($_POST['product_id']) ? intval($_POST['product_id']) : 0;
    
    // --- THÊM VÀO GIỎ (ADD) ---
    if ($action == 'add' && $product_id > 0) {
        $quantity = isset($_POST['quantity']) ? intval($_POST['quantity']) : 1;
        
        $sql = "SELECT id, name, price, image FROM products WHERE id = $product_id";
        $result = $conn->query($sql);
        
        if ($result->num_rows > 0) {
            $product = $result->fetch_assoc();
            
            if (isset($_SESSION['cart'][$product_id])) {
                $_SESSION['cart'][$product_id]['quantity'] += $quantity;
            } else {
                $_SESSION['cart'][$product_id] = [
                    'product_id' => $product_id,
                    'name' => $product['name'],
                    'price' => $product['price'],
                    'quantity' => $quantity
                ];
            }
            // Chuyển hướng về Menu
            header('Location: index.php?added=' . urlencode($product['name']) . '#menu-section');
            exit;
        }
    }
    
    // --- CẬP NHẬT SỐ LƯỢNG (UPDATE) ---
    elseif ($action == 'update') {
        foreach ($_POST as $key => $val) {
            if (strpos($key, 'product_id_') === 0) {
                $pid = intval(str_replace('product_id_', '', $key));
                $qty = intval($val);
                
                if ($qty > 0 && isset($_SESSION['cart'][$pid])) {
                    $_SESSION['cart'][$pid]['quantity'] = $qty;
                } elseif ($qty <= 0 && isset($_SESSION['cart'][$pid])) {
                    unset($_SESSION['cart'][$pid]);
                }
            }
        }
        $message = "Đã cập nhật giỏ hàng.";
        header('Location: cart.php?msg=' . urlencode($message));
        exit;
    }
}

// B. XỬ LÝ YÊU CẦU GET (Xóa món - Quan trọng: Đã sửa phần này)
if (isset($_GET['action']) && $_GET['action'] == 'remove' && isset($_GET['product_id'])) {
    $product_id = intval($_GET['product_id']);
    
    if ($product_id > 0 && isset($_SESSION['cart'][$product_id])) {
        unset($_SESSION['cart'][$product_id]);
        $message = "Đã xóa món khỏi giỏ hàng.";
        header('Location: cart.php?msg=' . urlencode($message));
        exit;
    }
}

// Lấy thông báo từ URL (nếu có)
if (isset($_GET['msg'])) {
    $message = htmlspecialchars($_GET['msg']);
}

$cart_items = $_SESSION['cart'];
$total_cart_amount = 0;
?>

<div class="cart-wrapper">
    
    <div class="receipt-box">
        
        <div class="receipt-header">
            <h2>CTusCoffee</h2>
            <p>180 Cao Lỗ, Q.8, TP.HCM</p>
            <p>Hotline: 0399 708 261</p>
            <br>
            <p><strong>PHIẾU THANH TOÁN (BILL)</strong></p>
            <p>Ngày: <?php echo date('d/m/Y H:i'); ?></p>
            <p>Khách: <?php echo $is_logged_in ? htmlspecialchars($username) : 'Khách lẻ'; ?></p>
        </div>

        <div class="dashed-line"></div>

        <?php if (!empty($message)): ?>
            <p style="text-align: center; color: green; margin-bottom: 10px; font-weight: bold;">
                <i class="fas fa-check"></i> <?php echo $message; ?>
            </p>
            <div class="dashed-line"></div>
        <?php endif; ?>

        <?php if (empty($cart_items)): ?>
            <div style="text-align: center; padding: 20px;">
                <p>(Giỏ hàng trống)</p>
                <br>
                <a href="Project1_PHP_MySQL/src/index.php" class="btn-back-receipt">← Gọi món ngay</a>
            </div>
        <?php else: ?>
            
            <form action="cart.php" method="POST">
                <input type="hidden" name="action" value="update">
                
                <table class="receipt-table">
                    <thead>
                        <tr>
                            <th style="width: 45%;">Món</th>
                            <th style="width: 20%;">SL</th>
                            <th style="width: 35%; text-align: right;">Tiền</th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php foreach ($cart_items as $item): 
                            $subtotal = $item['price'] * $item['quantity'];
                            $total_cart_amount += $subtotal;
                        ?>
                        <tr>
                            <td>
                                <?php echo htmlspecialchars($item['name']); ?><br>
                                <span style="font-size: 0.8em; color: #777;">@<?php echo number_format($item['price']/1000, 0); ?>k</span>
                            </td>
                            <td>
                                x <input type="number" name="product_id_<?php echo $item['product_id']; ?>" 
                                       value="<?php echo $item['quantity']; ?>" 
                                       min="1" class="receipt-qty">
                            </td>
                            <td style="text-align: right;">
                                <?php echo number_format($subtotal, 0, ',', '.'); ?>
                                <a href="cart.php?action=remove&product_id=<?php echo $item['product_id']; ?>" 
                                   class="btn-remove-small" onclick="return confirm('Xóa món này?');">×</a>
                            </td>
                        </tr>
                        <?php endforeach; ?>
                    </tbody>
                </table>
                
                <button type="submit" class="btn-update-receipt">Cập nhật số lượng</button>

                <div class="dashed-line"></div>

                <div class="receipt-total">
                    <span>TỔNG CỘNG:</span>
                    <span><?php echo number_format($total_cart_amount, 0, ',', '.'); ?>đ</span>
                </div>
                
                <div class="dashed-line"></div>
                
                <div style="text-align: center; margin-top: 20px; font-size: 0.9em; font-style: italic;">
                    <p>Cảm ơn & Hẹn gặp lại!</p>
                    <p>Wifi: CTusCoffee / Pass: 240206</p>
                    <p style="font-family: 'Libre Barcode 39 Text', cursive; font-size: 2.5em; margin: 10px 0;">CODE-12345</p>
                </div>

                <div class="receipt-actions">
                    <a href="Project1_PHP_MySQL/src/checkout.php" class="btn-checkout-receipt">THANH TOÁN NGAY</a>
                    <a href="Project1_PHP_MySQL/src/index.php" class="btn-back-receipt">← Gọi thêm món</a>
                </div>

            </form>
        <?php endif; ?>
    </div>
</div>

<?php include 'Project1_PHP_MySQL/src/includes/footer.php'; ?>
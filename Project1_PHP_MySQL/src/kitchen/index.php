<?php
include 'includes/header.php';

if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $order_id = intval($_POST['order_id']);
    $action = $_POST['action'];

    if ($action == 'start_cooking') {
        $conn->query("UPDATE orders SET status = 'Processing' WHERE id = $order_id");
    } elseif ($action == 'finish_cooking') {
        $conn->query("UPDATE orders SET status = 'Delivered' WHERE id = $order_id");
    }
    
    header("Location: index.php");
    exit;
}

$sql = "SELECT * FROM orders WHERE status IN ('Pending', 'Processing') ORDER BY order_date ASC";
$result = $conn->query($sql);
?>

<div class="kds-grid">
    
    <?php if ($result->num_rows > 0): ?>
        <?php while ($order = $result->fetch_assoc()): 
            // Xác định class màu sắc dựa trên trạng thái
            $ticket_class = ($order['status'] == 'Pending') ? 'ticket-pending' : 'ticket-processing';
            
            // Tính thời gian chờ (Phút)
            $order_time = strtotime($order['order_date']);
            $now = time();
            $waiting_minutes = round(($now - $order_time) / 60);
        ?>
        
        <div class="order-ticket <?php echo $ticket_class; ?>">
            
            <div class="ticket-header">
                <span style="font-size: 1.2em;">#<?php echo $order['id']; ?></span>
                <div class="ticket-timer">
                    <i class="far fa-clock"></i> <?php echo date('H:i', $order_time); ?>
                    <br>
                    <span style="<?php echo ($waiting_minutes > 15) ? 'color:red; font-weight:bold;' : ''; ?>">
                        (<?php echo $waiting_minutes; ?> phút trước)
                    </span>
                </div>
            </div>

            <div class="ticket-body">
                <?php
                // Lấy chi tiết món cho đơn hàng này
                $oid = $order['id'];
                $detail_sql = "SELECT od.quantity, p.name 
                               FROM order_details od 
                               JOIN products p ON od.product_id = p.id 
                               WHERE od.order_id = $oid";
                $details = $conn->query($detail_sql);
                
                while ($item = $details->fetch_assoc()):
                ?>
                    <div class="ticket-item">
                        <span>
                            <span class="ticket-qty"><?php echo $item['quantity']; ?></span> 
                            <?php echo htmlspecialchars($item['name']); ?>
                        </span>
                    </div>
                <?php endwhile; ?>
                
                </div>

            <div class="ticket-footer">
                <form method="POST">
                    <input type="hidden" name="order_id" value="<?php echo $order['id']; ?>">
                    
                    <?php if ($order['status'] == 'Pending'): ?>
                        <button type="submit" name="action" value="start_cooking" class="btn-kds btn-start">
                            <i class="fas fa-fire"></i> NHẬN ĐƠN
                        </button>
                    <?php else: ?>
                        <button type="submit" name="action" value="finish_cooking" class="btn-kds btn-done">
                            <i class="fas fa-check-circle"></i> HOÀN THÀNH
                        </button>
                    <?php endif; ?>
                </form>
            </div>

        </div> 
        <?php endwhile; ?>
    <?php else: ?>
        <div style="grid-column: 1 / -1; text-align: center; color: #777; margin-top: 50px;">
            <i class="fas fa-mug-hot" style="font-size: 4em; margin-bottom: 20px;"></i>
            <h2>Hiện tại không có đơn nào cần làm.</h2>
            <p>Hãy nghỉ ngơi một chút!</p>
        </div>
    <?php endif; ?>

</div>

<?php include 'includes/footer.php'; ?>
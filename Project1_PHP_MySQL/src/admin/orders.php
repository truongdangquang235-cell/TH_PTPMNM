<?php
include 'includes/header.php';
include 'includes/sidebar.php';

// Lấy danh sách đơn hàng (Mới nhất lên đầu)
$sql = "SELECT * FROM orders ORDER BY order_date DESC";
$result = $conn->query($sql);
?>

<main class="admin-content">
    <h1 class="admin-page-title">Quản lý Đơn hàng</h1>

    <div class="admin-form-card" style="max-width: 100%; padding: 0; box-shadow: none; background: transparent;">
        <table class="admin-table">
            <thead>
                <tr>
                    <th width="10%">Mã Đơn</th>
                    <th width="20%">Khách hàng</th>
                    <th width="20%">Ngày đặt</th>
                    <th width="15%">Tổng tiền</th>
                    <th width="15%">Trạng thái</th>
                    <th width="10%">Hành động</th>
                </tr>
            </thead>
            <tbody>
                <?php if ($result->num_rows > 0): ?>
                    <?php while ($order = $result->fetch_assoc()): ?>
                    <tr>
                        <td><strong>#<?php echo $order['id']; ?></strong></td>
                        <td>
                            <?php echo htmlspecialchars($order['shipping_name']); ?><br>
                            <span style="font-size: 0.85em; color: #888;"><?php echo htmlspecialchars($order['shipping_phone']); ?></span>
                        </td>
                        <td><?php echo date('d/m/Y H:i', strtotime($order['order_date'])); ?></td>
                        <td style="font-weight: bold; color: #A0522D;">
                            <?php echo number_format($order['total_amount'], 0, ',', '.'); ?>đ
                        </td>
                        <td>
                            <span class="status-badge status-<?php echo $order['status']; ?>">
                                <?php echo $order['status']; ?>
                            </span>
                        </td>
                        <td>
                            <a href="order_detail.php?id=<?php echo $order['id']; ?>" 
                               class="btn-admin" style="background-color: #17a2b8; color: white;">
                               <i class="fas fa-eye"></i> Xem
                            </a>
                        </td>
                    </tr>
                    <?php endwhile; ?>
                <?php else: ?>
                    <tr><td colspan="6" style="text-align:center; padding: 30px;">Chưa có đơn hàng nào.</td></tr>
                <?php endif; ?>
            </tbody>
        </table>
    </div>
</main>

<?php include 'includes/footer.php'; ?>
<?php
include 'includes/header.php';
include 'includes/sidebar.php';

// Xử lý thêm mã
if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $code = strtoupper($_POST['code']); // Chuyển thành chữ hoa
    $percent = intval($_POST['discount_percent']);
    $expiry = $_POST['expiry_date'];
    
    $conn->query("INSERT INTO coupons (code, discount_percent, expiry_date) VALUES ('$code', '$percent', '$expiry')");
    echo "<script>alert('Đã tạo mã giảm giá!'); window.location.href='coupons.php';</script>";
}

// Xử lý xóa mã
if (isset($_GET['delete_id'])) {
    $id = intval($_GET['delete_id']);
    $conn->query("DELETE FROM coupons WHERE id=$id");
    header("Location: coupons.php");
}

$coupons = $conn->query("SELECT * FROM coupons ORDER BY expiry_date DESC");
?>

<main class="admin-content">
    <h1 class="admin-page-title">Quản Lý Mã Giảm Giá</h1>
    
    <div class="admin-form-card" style="margin-bottom: 30px;">
        <form method="POST" style="display: flex; gap: 15px; align-items: end;">
            <div style="flex: 1;">
                <label>Mã Code (VD: SALE10)</label>
                <input type="text" name="code" class="form-control" required>
            </div>
            <div style="width: 150px;">
                <label>Giảm (%)</label>
                <input type="number" name="discount_percent" class="form-control" min="1" max="100" required>
            </div>
            <div style="width: 200px;">
                <label>Hết hạn</label>
                <input type="date" name="expiry_date" class="form-control" required>
            </div>
            <button type="submit" class="btn-admin btn-add-new" style="margin: 0;">Tạo Mã</button>
        </form>
    </div>

    <table class="admin-table">
        <thead>
            <tr>
                <th>Mã Code</th>
                <th>Giảm giá</th>
                <th>Hạn dùng</th>
                <th>Trạng thái</th>
                <th>Xóa</th>
            </tr>
        </thead>
        <tbody>
            <?php while($row = $coupons->fetch_assoc()): 
                $is_expired = strtotime($row['expiry_date']) < time();
            ?>
            <tr>
                <td style="font-weight: bold; color: #A0522D;"><?php echo $row['code']; ?></td>
                <td><?php echo $row['discount_percent']; ?>%</td>
                <td><?php echo date('d/m/Y', strtotime($row['expiry_date'])); ?></td>
                <td>
                    <?php if($is_expired): ?>
                        <span style="color: red;">Đã hết hạn</span>
                    <?php else: ?>
                        <span style="color: green;">Đang hoạt động</span>
                    <?php endif; ?>
                </td>
                <td><a href="coupons.php?delete_id=<?php echo $row['id']; ?>" class="btn-admin btn-delete">Xóa</a></td>
            </tr>
            <?php endwhile; ?>
        </tbody>
    </table>
</main>
<?php include 'includes/footer.php'; ?>
<?php
include 'includes/header.php';
include 'includes/sidebar.php';

// --- XỬ LÝ XÓA TÀI KHOẢN ---
if (isset($_GET['delete_id'])) {
    $delete_id = intval($_GET['delete_id']);
    
    // 1. Chặn không cho xóa chính mình (Admin đang đăng nhập)
    if ($delete_id == $_SESSION['user_id']) {
        echo "<script>alert('Không thể xóa tài khoản bạn đang sử dụng!'); window.location.href='users.php';</script>";
        exit;
    }

    // 2. Kiểm tra xem user này có đơn hàng không
    // Nếu có đơn hàng, ta không nên xóa user vì sẽ làm mất dữ liệu lịch sử đơn hàng
    // Thay vào đó, bạn có thể thêm cột 'status' (Active/Banned) để khóa tài khoản (Nâng cao)
    // Ở đây ta làm đơn giản: Nếu có đơn hàng -> Không cho xóa.
    $check_orders = $conn->query("SELECT COUNT(*) as count FROM orders WHERE user_id = $delete_id");
    $has_orders = $check_orders->fetch_assoc()['count'];

    if ($has_orders > 0) {
        echo "<script>alert('Không thể xóa! Khách hàng này đã có lịch sử mua hàng ($has_orders đơn).'); window.location.href='users.php';</script>";
    } else {
        // Thực hiện xóa
        $conn->query("DELETE FROM users WHERE id = $delete_id");
        echo "<script>alert('Đã xóa tài khoản thành công!'); window.location.href='users.php';</script>";
    }
}

// Lấy danh sách Users (Trừ Admin ra cho đỡ rối, hoặc lấy hết tùy bạn)
// Ở đây mình lấy hết để bạn dễ quản lý
$sql = "SELECT * FROM users ORDER BY id DESC";
$result = $conn->query($sql);
?>

<main class="admin-content">
    <h1 class="admin-page-title">Quản lý Khách hàng</h1>

    <div class="admin-form-card" style="max-width: 100%; padding: 0; box-shadow: none; background: transparent;">
        <table class="admin-table">
            <thead>
                <tr>
                    <th width="5%">ID</th>
                    <th width="20%">Tên đăng nhập</th>
                    <th width="25%">Email</th>
                    <th width="15%">Vai trò</th>
                    <th width="20%">Ngày tạo</th>
                    <th width="15%">Hành động</th>
                </tr>
            </thead>
            <tbody>
                <?php if ($result->num_rows > 0): ?>
                    <?php while ($row = $result->fetch_assoc()): ?>
                    <tr>
                        <td>#<?php echo $row['id']; ?></td>
                        <td>
                            <div style="display: flex; align-items: center; gap: 10px;">
                                <div style="width: 30px; height: 30px; background: #3C2A21; color: white; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 0.8em;">
                                    <i class="fas fa-user"></i>
                                </div>
                                <strong><?php echo htmlspecialchars($row['username']); ?></strong>
                            </div>
                        </td>
                        <td><?php echo htmlspecialchars($row['email']); ?></td>
                        <td>
                            <?php if ($row['role'] == 'admin'): ?>
                                <span class="status-badge status-Delivered" style="background:#dc3545; color:white; border:none;">ADMIN</span>
                            <?php elseif ($row['role'] == 'kitchen'): ?>
                                <span class="status-badge status-Processing" style="background:#ffc107; color:#333; border:none;">BẾP</span>
                            <?php else: ?>
                                <span class="status-badge status-Pending" style="background:#e2e6ea; color:#333; border:none;">Khách hàng</span>
                            <?php endif; ?>
                        </td>
                        <td>
                            <?php 
                            // Nếu DB có cột created_at thì hiển thị, nếu không thì để trống hoặc cập nhật DB sau
                            echo isset($row['created_at']) ? date('d/m/Y', strtotime($row['created_at'])) : '---'; 
                            ?>
                        </td>
                        <td>
                            <?php if ($row['role'] != 'admin'): // Không hiện nút xóa cho Admin ?>
                                <a href="users.php?delete_id=<?php echo $row['id']; ?>" 
                                   class="btn-admin btn-delete"
                                   onclick="return confirm('CẢNH BÁO: Bạn có chắc muốn xóa tài khoản này không?');" 
                                   title="Xóa tài khoản">
                                   <i class="fas fa-trash-alt"></i> Xóa
                                </a>
                            <?php endif; ?>
                        </td>
                    </tr>
                    <?php endwhile; ?>
                <?php else: ?>
                    <tr><td colspan="6" style="text-align:center; padding: 30px;">Chưa có tài khoản nào.</td></tr>
                <?php endif; ?>
            </tbody>
        </table>
    </div>
</main>

<?php include 'includes/footer.php'; ?>
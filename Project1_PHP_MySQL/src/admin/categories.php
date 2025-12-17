<?php
include 'includes/header.php';
include 'includes/sidebar.php';

// --- XỬ LÝ XÓA DANH MỤC ---
if (isset($_GET['delete_id'])) {
    $delete_id = intval($_GET['delete_id']);

    // Kiểm tra xem danh mục này có đang chứa sản phẩm không
    $check_products = $conn->query("SELECT COUNT(*) as count FROM products WHERE category_id = $delete_id");
    $product_count = $check_products->fetch_assoc()['count'];

    if ($product_count > 0) {
        echo "<script>alert('Không thể xóa! Danh mục này đang chứa $product_count sản phẩm. Hãy xóa hoặc chuyển sản phẩm sang danh mục khác trước.'); window.location.href='categories.php';</script>";
    } else {
        $conn->query("DELETE FROM categories WHERE id = $delete_id");
        echo "<script>alert('Đã xóa danh mục thành công!'); window.location.href='categories.php';</script>";
    }
}

// Lấy danh sách danh mục + Đếm số sản phẩm trong mỗi danh mục
// Sử dụng LEFT JOIN và GROUP BY để đếm
$sql = "SELECT c.id, c.name, COUNT(p.id) as product_count 
        FROM categories c 
        LEFT JOIN products p ON c.id = p.category_id 
        GROUP BY c.id 
        ORDER BY c.id ASC";
$result = $conn->query($sql);
?>

<main class="admin-content">
    <div style="display: flex; justify-content: space-between; align-items: center;">
        <h1 class="admin-page-title">Quản lý Danh mục</h1>
        <a href="category_form.php" class="btn-admin btn-add-new"><i class="fas fa-plus"></i> Thêm Danh Mục</a>
    </div>

    <table class="admin-table">
        <thead>
            <tr>
                <th width="10%">ID</th>
                <th width="40%">Tên danh mục</th>
                <th width="30%">Số lượng món</th>
                <th width="20%">Hành động</th>
            </tr>
        </thead>
        <tbody>
            <?php if ($result->num_rows > 0): ?>
                <?php while ($row = $result->fetch_assoc()): ?>
                <tr>
                    <td>#<?php echo $row['id']; ?></td>
                    <td><strong><?php echo htmlspecialchars($row['name']); ?></strong></td>
                    <td>
                        <span class="status-badge status-Processing" style="background:#e2e6ea; color:#333; border:none;">
                            <?php echo $row['product_count']; ?> món
                        </span>
                    </td>
                    <td>
                        <a href="category_form.php?id=<?php echo $row['id']; ?>" class="btn-admin btn-edit" title="Sửa"><i class="fas fa-edit"></i></a>
                        
                        <a href="categories.php?delete_id=<?php echo $row['id']; ?>" 
                           class="btn-admin btn-delete" 
                           onclick="return confirm('Bạn chắc chắn muốn xóa danh mục này?');" title="Xóa"><i class="fas fa-trash"></i></a>
                    </td>
                </tr>
                <?php endwhile; ?>
            <?php else: ?>
                <tr><td colspan="4" style="text-align:center;">Chưa có danh mục nào.</td></tr>
            <?php endif; ?>
        </tbody>
    </table>
</main>

<?php include 'includes/footer.php'; ?>
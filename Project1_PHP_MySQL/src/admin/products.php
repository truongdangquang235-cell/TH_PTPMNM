<?php
include 'includes/header.php';
include 'includes/sidebar.php';

// --- XỬ LÝ XÓA SẢN PHẨM ---
if (isset($_GET['delete_id'])) {
    $delete_id = intval($_GET['delete_id']);
    
    // Xóa trong Database
    $conn->query("DELETE FROM products WHERE id = $delete_id");
    
    // (Tùy chọn: Xóa file ảnh vật lý nếu muốn, nhưng để an toàn ta chỉ xóa DB trước)
    
    echo "<script>alert('Đã xóa sản phẩm thành công!'); window.location.href='products.php';</script>";
}

// Lấy danh sách sản phẩm kèm tên danh mục
$sql = "SELECT p.*, c.name as category_name 
        FROM products p 
        LEFT JOIN categories c ON p.category_id = c.id 
        ORDER BY p.id DESC";
$result = $conn->query($sql);
?>

<main class="admin-content">
    <div style="display: flex; justify-content: space-between; align-items: center;">
        <h1 class="admin-page-title">Quản lý Sản phẩm</h1>
        <a href="product_form.php" class="btn-admin btn-add-new"><i class="fas fa-plus"></i> Thêm Món Mới</a>
    </div>

    <table class="admin-table">
        <thead>
            <tr>
                <th width="5%">ID</th>
                <th width="10%">Hình ảnh</th>
                <th width="25%">Tên món</th>
                <th width="15%">Danh mục</th>
                <th width="15%">Giá</th>
                <th width="20%">Mô tả</th>
                <th width="10%">Hành động</th>
            </tr>
        </thead>
        <tbody>
            <?php if ($result->num_rows > 0): ?>
                <?php while ($row = $result->fetch_assoc()): ?>
                <tr>
                    <td>#<?php echo $row['id']; ?></td>
                    <td>
                        <img src="../assets/images/<?php echo htmlspecialchars($row['image']); ?>" 
                             alt="Img" onerror="this.src='https://via.placeholder.com/60'">
                    </td>
                    <td><strong><?php echo htmlspecialchars($row['name']); ?></strong></td>
                    <td><span class="status-badge status-Processing" style="background:#e2e6ea; color:#333; border:none;"><?php echo htmlspecialchars($row['category_name']); ?></span></td>
                    <td style="color: #A0522D; font-weight: bold;"><?php echo number_format($row['price'], 0, ',', '.'); ?>đ</td>
                    <td style="font-size: 0.9em; color: #666;">
                        <?php echo mb_strimwidth(htmlspecialchars($row['description']), 0, 50, "..."); ?>
                    </td>
                    <td>
                        <a href="product_form.php?id=<?php echo $row['id']; ?>" class="btn-admin btn-edit" title="Sửa"><i class="fas fa-edit"></i></a>
                        <a href="products.php?delete_id=<?php echo $row['id']; ?>" 
                           class="btn-admin btn-delete" 
                           onclick="return confirm('Bạn chắc chắn muốn xóa món này?');" title="Xóa"><i class="fas fa-trash"></i></a>
                    </td>
                </tr>
                <?php endwhile; ?>
            <?php else: ?>
                <tr><td colspan="7" style="text-align:center;">Chưa có sản phẩm nào.</td></tr>
            <?php endif; ?>
        </tbody>
    </table>
</main>

<?php include 'includes/footer.php'; ?>
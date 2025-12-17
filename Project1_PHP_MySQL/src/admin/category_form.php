<?php
include 'includes/header.php';
include 'includes/sidebar.php';

$id = 0;
$name = '';
$is_edit = false;

// Kiểm tra chế độ SỬA
if (isset($_GET['id'])) {
    $id = intval($_GET['id']);
    $is_edit = true;
    $result = $conn->query("SELECT * FROM categories WHERE id = $id");
    if ($result->num_rows > 0) {
        $row = $result->fetch_assoc();
        $name = $row['name'];
    }
}

// XỬ LÝ SUBMIT
if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $name = $conn->real_escape_string($_POST['name']);

    if ($is_edit) {
        // Cập nhật
        $sql = "UPDATE categories SET name = '$name' WHERE id = $id";
    } else {
        // Thêm mới
        $sql = "INSERT INTO categories (name) VALUES ('$name')";
    }

    if ($conn->query($sql)) {
        echo "<script>alert('Lưu thành công!'); window.location.href='categories.php';</script>";
    } else {
        echo "<script>alert('Lỗi: " . $conn->error . "');</script>";
    }
}
?>

<main class="admin-content">
    <h1 class="admin-page-title"><?php echo $is_edit ? 'Chỉnh Sửa Danh Mục' : 'Thêm Danh Mục Mới'; ?></h1>
    
    <div class="admin-form-card" style="max-width: 600px;"> <form action="" method="POST">
            
            <div class="form-group">
                <label>Tên danh mục:</label>
                <input type="text" name="name" class="form-control" value="<?php echo htmlspecialchars($name); ?>" required placeholder="Ví dụ: Cà Phê, Trà Sữa, Bánh Ngọt...">
            </div>

            <div style="margin-top: 30px;">
                <button type="submit" class="btn-admin btn-add-new" style="border: none; cursor: pointer;">
                    <i class="fas fa-save"></i> Lưu Dữ Liệu
                </button>
                <a href="categories.php" class="btn-admin" style="background: #6c757d; color: white;">Hủy bỏ</a>
            </div>

        </form>
    </div>
</main>

<?php include 'includes/footer.php'; ?>
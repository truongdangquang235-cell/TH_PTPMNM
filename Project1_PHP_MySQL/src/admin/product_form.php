<?php
include 'includes/header.php';
include 'includes/sidebar.php';

// Khởi tạo biến
$id = 0;
$name = '';
$category_id = '';
$price = '';
$description = '';
$image = '';
$is_edit = false;

// Nếu có ID trên URL => Chế độ SỬA (EDIT)
if (isset($_GET['id'])) {
    $id = intval($_GET['id']);
    $is_edit = true;
    $result = $conn->query("SELECT * FROM products WHERE id = $id");
    if ($result->num_rows > 0) {
        $row = $result->fetch_assoc();
        $name = $row['name'];
        $category_id = $row['category_id'];
        $price = $row['price'];
        $description = $row['description'];
        $image = $row['image'];
    }
}

// XỬ LÝ KHI SUBMIT FORM
if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $name = $conn->real_escape_string($_POST['name']);
    $category_id = intval($_POST['category_id']);
    $price = intval($_POST['price']);
    $description = $conn->real_escape_string($_POST['description']);
    
    // Xử lý Upload Ảnh
    $image_update_sql = ""; // Chuỗi SQL cập nhật ảnh
    if (isset($_FILES['image']) && $_FILES['image']['error'] == 0) {
        $target_dir = "../assets/images/"; // Thư mục lưu ảnh
        $file_name = time() . "_" . basename($_FILES["image"]["name"]); // Đổi tên file để tránh trùng
        $target_file = $target_dir . $file_name;
        
        // Di chuyển file
        if (move_uploaded_file($_FILES["image"]["tmp_name"], $target_file)) {
            $image = $file_name; // Cập nhật tên ảnh mới
            $image_update_sql = ", image = '$image'";
        }
    }

    if ($is_edit) {
        // --- CẬP NHẬT (UPDATE) ---
        $sql = "UPDATE products SET 
                name = '$name', 
                category_id = '$category_id', 
                price = '$price', 
                description = '$description' 
                $image_update_sql 
                WHERE id = $id";
    } else {
        // --- THÊM MỚI (INSERT) ---
        // Nếu thêm mới mà không chọn ảnh, gán ảnh mặc định
        if (empty($image)) $image = 'default.jpg'; 
        
        $sql = "INSERT INTO products (name, category_id, price, description, image) 
                VALUES ('$name', '$category_id', '$price', '$description', '$image')";
    }

    if ($conn->query($sql)) {
        echo "<script>alert('Lưu thành công!'); window.location.href='products.php';</script>";
    } else {
        echo "<script>alert('Lỗi: " . $conn->error . "');</script>";
    }
}

// Lấy danh sách danh mục để hiển thị trong <select>
$categories = $conn->query("SELECT * FROM categories");
?>

<main class="admin-content">
    <h1 class="admin-page-title"><?php echo $is_edit ? 'Chỉnh Sửa Món' : 'Thêm Món Mới'; ?></h1>
    
    <div class="admin-form-card">
        <form action="" method="POST" enctype="multipart/form-data"> <div class="form-group">
                <label>Tên món:</label>
                <input type="text" name="name" class="form-control" value="<?php echo htmlspecialchars($name); ?>" required>
            </div>

            <div class="form-group">
                <label>Danh mục:</label>
                <select name="category_id" class="form-control">
                    <?php while ($cat = $categories->fetch_assoc()): ?>
                        <option value="<?php echo $cat['id']; ?>" <?php echo ($cat['id'] == $category_id) ? 'selected' : ''; ?>>
                            <?php echo $cat['name']; ?>
                        </option>
                    <?php endwhile; ?>
                </select>
            </div>

            <div class="form-group">
                <label>Giá bán (VNĐ):</label>
                <input type="number" name="price" class="form-control" value="<?php echo $price; ?>" required>
            </div>

            <div class="form-group">
                <label>Mô tả ngắn:</label>
                <textarea name="description" class="form-control" rows="4"><?php echo htmlspecialchars($description); ?></textarea>
            </div>

            <div class="form-group">
                <label>Hình ảnh:</label>
                <?php if ($is_edit && !empty($image)): ?>
                    <div style="margin-bottom: 10px;">
                        <img src="../assets/images/<?php echo $image; ?>" width="100" style="border-radius: 5px; border: 1px solid #ccc;">
                        <p style="font-size: 0.8em; color: #666;">Ảnh hiện tại</p>
                    </div>
                <?php endif; ?>
                <input type="file" name="image" class="form-control" accept="image/*">
                <p style="font-size: 0.8em; color: #888; margin-top: 5px;">*Để trống nếu không muốn thay đổi ảnh (khi sửa).</p>
            </div>

            <div style="margin-top: 30px;">
                <button type="submit" class="btn-admin btn-add-new" style="border: none; cursor: pointer;">
                    <i class="fas fa-save"></i> Lưu Dữ Liệu
                </button>
                <a href="products.php" class="btn-admin" style="background: #6c757d; color: white;">Hủy bỏ</a>
            </div>

        </form>
    </div>
</main>

<?php include 'includes/footer.php'; ?>
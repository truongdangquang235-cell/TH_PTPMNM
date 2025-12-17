<?php
include 'includes/header.php'; 

// Truy vấn Danh mục và Sản phẩm
$categories_sql = "SELECT id, name FROM categories ORDER BY id";
$categories_result = $conn->query($categories_sql);

$products_sql = "SELECT p.id, p.name, p.price, p.image, c.name AS category_name 
                 FROM products p JOIN categories c ON p.category_id = c.id 
                 ORDER BY c.id, p.name";
$products_result = $conn->query($products_sql);

$products_by_category = [];
if ($products_result && $products_result->num_rows > 0) {
    while ($product = $products_result->fetch_assoc()) {
        $products_by_category[$product['category_name']][] = $product;
    }
}
?>

<div class="main-header">
    <h1>Hương Vị Của Đam Mê</h1>
    <p>Khởi đầu ngày mới với một tách cà phê hoàn hảo từ CTusCoffee</p>
    <a href="#menu-section" class="btn-shop">XEM MENU NGAY <i class="fas fa-arrow-down"></i></a>
</div>

<div class="container" id="menu-section">
    
    <div style="text-align: center; margin-bottom: 50px;">
        <h2 class="section-title">Thực Đơn Của Chúng Tôi</h2>
        <p style="color: #777; font-style: italic;">Tuyển chọn những hạt cà phê thượng hạng nhất</p>
    </div>

    <?php if (isset($_GET['added'])): ?>
        <div style="
            position: fixed; 
            top: 100px; 
            right: 20px; 
            background-color: #3C2A21; 
            color: white; 
            padding: 15px 25px; 
            border-radius: 8px; 
            box-shadow: 0 5px 15px rgba(0,0,0,0.3); 
            z-index: 9999; 
            display: flex; 
            align-items: center; 
            animation: slideIn 0.5s ease-out;">
            <i class="fas fa-check-circle" style="color: #FFCC00; font-size: 1.5em; margin-right: 15px;"></i>
            <div>
                <strong>Tuyệt vời!</strong><br>
                Đã thêm "<?php echo htmlspecialchars($_GET['added']); ?>" vào giỏ.
            </div>
        </div>
        <script>
            setTimeout(function() {
                const alertBox = document.querySelector('div[style*="position: fixed"]');
                if(alertBox) {
                    alertBox.style.opacity = '0';
                    alertBox.style.transition = 'opacity 0.5s';
                    setTimeout(() => alertBox.remove(), 500);
                }
            }, 3000);
        </script>
        <style>
            @keyframes slideIn {
                from { transform: translateX(100%); opacity: 0; }
                to { transform: translateX(0); opacity: 1; }
            }
        </style>
    <?php endif; ?>

    <?php
    if ($categories_result && $categories_result->num_rows > 0) {
        while ($category = $categories_result->fetch_assoc()) {
            $cat_name = htmlspecialchars($category['name']);
            
            // Chỉ hiển thị danh mục nếu có sản phẩm
            if (isset($products_by_category[$cat_name])) {
                // Tiêu đề danh mục với icon
                echo "<div class='category-title'><i class='fas fa-mug-hot'></i> {$cat_name}</div>";
                
                echo "<div class='product-grid'>"; 
                foreach ($products_by_category[$cat_name] as $product) {
                    ?>
                    <div class="product-card">
                        
                        <div class="product-image-box">
                            <img src="assets/images/<?php echo htmlspecialchars($product['image'] ?? 'default.jpg'); ?>" 
                                 alt="<?php echo htmlspecialchars($product['name']); ?>"
                                 onerror="this.src='https://via.placeholder.com/300x200?text=CTusCoffee'"> </div>
                        
                        <div class="product-info">
                            <h3><?php echo htmlspecialchars($product['name']); ?></h3>
                            <div class="price"><?php echo number_format($product['price'], 0, ',', '.'); ?> đ</div>
                            
                            <form action="cart.php" method="POST">
                                <input type="hidden" name="action" value="add">
                                <input type="hidden" name="product_id" value="<?php echo $product['id']; ?>">
                                <input type="hidden" name="quantity" value="1">
                                <button type="submit" class="btn-add-cart">
                                    Thêm vào giỏ <i class="fas fa-plus-circle"></i>
                                </button>
                            </form>
                        </div>

                    </div>
                    <?php
                }
                echo "</div>"; // End grid
            }
        }
    } else {
        echo "<p style='text-align:center;'>Chưa có sản phẩm nào.</p>";
    }
    ?>
</div>

<?php include 'includes/footer.php'; ?>
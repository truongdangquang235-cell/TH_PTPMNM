<?php
include 'includes/header.php';
include 'includes/sidebar.php';

// --- 1. TRUY VẤN DOANH THU THEO THÁNG (6 tháng gần nhất) ---
// Chỉ tính đơn hàng đã giao thành công ('Delivered')
$sql_revenue = "SELECT DATE_FORMAT(order_date, '%m/%Y') as month_year, SUM(total_amount) as total 
                FROM orders 
                WHERE status = 'Delivered' 
                GROUP BY month_year 
                ORDER BY order_date ASC 
                LIMIT 6";
$result_revenue = $conn->query($sql_revenue);

$months = [];
$totals = [];

while ($row = $result_revenue->fetch_assoc()) {
    $months[] = $row['month_year'];
    $totals[] = $row['total'];
}

// --- 2. TRUY VẤN TOP 5 MÓN BÁN CHẠY NHẤT ---
$sql_top_products = "SELECT p.name, SUM(od.quantity) as total_sold 
                     FROM order_details od 
                     JOIN products p ON od.product_id = p.id 
                     JOIN orders o ON od.order_id = o.id 
                     WHERE o.status = 'Delivered' 
                     GROUP BY p.name 
                     ORDER BY total_sold DESC 
                     LIMIT 5";
$result_top = $conn->query($sql_top_products);

$product_names = [];
$product_sold = [];

while ($row = $result_top->fetch_assoc()) {
    $product_names[] = $row['name'];
    $product_sold[] = $row['total_sold'];
}
?>

<main class="admin-content">
    <h1 class="admin-page-title">Thống Kê & Báo Cáo</h1>

    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(400px, 1fr)); gap: 30px;">
        
        <div class="admin-form-card" style="margin: 0;">
            <h3 style="text-align: center; color: #3C2A21; margin-bottom: 20px;">Biểu Đồ Doanh Thu (VNĐ)</h3>
            <canvas id="revenueChart"></canvas>
        </div>

        <div class="admin-form-card" style="margin: 0;">
            <h3 style="text-align: center; color: #3C2A21; margin-bottom: 20px;">Top 5 Món Bán Chạy</h3>
            <canvas id="topProductsChart"></canvas>
        </div>

    </div>
</main>

<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

<script>
    // --- CẤU HÌNH BIỂU ĐỒ DOANH THU (BAR CHART) ---
    const ctxRevenue = document.getElementById('revenueChart').getContext('2d');
    new Chart(ctxRevenue, {
        type: 'bar', // Dạng cột
        data: {
            labels: <?php echo json_encode($months); ?>, // Dữ liệu tháng từ PHP
            datasets: [{
                label: 'Doanh thu',
                data: <?php echo json_encode($totals); ?>, // Dữ liệu tiền từ PHP
                backgroundColor: '#A0522D', // Màu nâu cà phê
                borderColor: '#3C2A21',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: { beginAtZero: true }
            }
        }
    });

    // --- CẤU HÌNH BIỂU ĐỒ TOP SẢN PHẨM (DOUGHNUT CHART) ---
    const ctxTop = document.getElementById('topProductsChart').getContext('2d');
    new Chart(ctxTop, {
        type: 'doughnut', // Dạng bánh Donut
        data: {
            labels: <?php echo json_encode($product_names); ?>, // Tên món
            datasets: [{
                label: 'Số lượng bán',
                data: <?php echo json_encode($product_sold); ?>, // Số lượng
                backgroundColor: [
                    '#3C2A21', // Nâu đậm
                    '#A0522D', // Nâu đất
                    '#D2691E', // Chocolate
                    '#FFCC00', // Vàng
                    '#F5DEB3'  // Kem
                ],
                hoverOffset: 4
            }]
        }
    });
</script>

<?php include 'includes/footer.php'; ?>
<footer style="background-color: #2C1E18; color: #ecf0f1; padding: 50px 0 20px; font-family: 'Poppins', sans-serif; margin-top: auto;">
    <div class="container" style="display: flex; flex-wrap: wrap; justify-content: space-between; gap: 30px;">
        
        <div style="flex: 1; min-width: 250px;">
            <h3 style="font-family: 'Playfair Display', serif; color: #FFCC00; margin-bottom: 20px;">CTusCoffee</h3>
            <p style="color: #ccc; line-height: 1.6; font-size: 0.95em;">
                Nơi hương vị cà phê đánh thức mọi giác quan. Chúng tôi cam kết mang đến những ly cà phê chất lượng nhất trong không gian ấm cúng.
            </p>
            <div style="margin-top: 20px;">
                <p><i class="fas fa-map-marker-alt" style="width: 20px; color: #A0522D;"></i> 180 Cao Lỗ, P.4, Q.8, TP.HCM</p>
                <p><i class="fas fa-phone" style="width: 20px; color: #A0522D;"></i> 0399 708 261</p>
                <p><i class="fas fa-envelope" style="width: 20px; color: #A0522D;"></i> contact@ctuscoffee.com</p>
            </div>
        </div>

        <div style="flex: 1; min-width: 200px;">
            <h4 style="color: #fff; margin-bottom: 20px; border-bottom: 2px solid #A0522D; display: inline-block; padding-bottom: 5px;">Liên Kết</h4>
            <ul style="list-style: none; padding: 0;">
                <li style="margin-bottom: 10px;"><a href="index.php" style="color: #ccc; text-decoration: none; transition: color 0.3s;">Trang Chủ</a></li>
                <li style="margin-bottom: 10px;"><a href="index.php#menu-section" style="color: #ccc; text-decoration: none; transition: color 0.3s;">Thực Đơn</a></li>
                <li style="margin-bottom: 10px;"><a href="cart.php" style="color: #ccc; text-decoration: none; transition: color 0.3s;">Giỏ Hàng</a></li>
                <li style="margin-bottom: 10px;"><a href="my_orders.php" style="color: #ccc; text-decoration: none; transition: color 0.3s;">Tra Cứu Đơn Hàng</a></li>
            </ul>
        </div>

        <div style="flex: 1; min-width: 300px;">
            <h4 style="color: #fff; margin-bottom: 20px; border-bottom: 2px solid #A0522D; display: inline-block; padding-bottom: 5px;">Vị Trí Cửa Hàng</h4>
            <div style="width: 100%; height: 200px; border-radius: 8px; overflow: hidden; border: 2px solid #4a3b32;">
                <iframe 
                    width="100%" 
                    height="100%" 
                    frameborder="0" 
                    scrolling="no" 
                    marginheight="0" 
                    marginwidth="0" 
                    src="https://maps.google.com/maps?q=180%20Cao%20L%E1%BB%97%2C%20Ph%C6%B0%E1%BB%9Dng%204%2C%20Qu%E1%BA%ADn%208%2C%20H%E1%BB%93%20Ch%C3%AD%20Minh&t=&z=15&ie=UTF8&iwloc=&output=embed">
                </iframe>
            </div>
        </div>

    </div>

    <div style="text-align: center; margin-top: 40px; padding-top: 20px; border-top: 1px solid #3a2e28; color: #777; font-size: 0.9em;">
        &copy; <?php echo date('Y'); ?> CTusCoffee. All rights reserved. Design by <strong>DangQuangTruong</strong>.
    </div>
</footer>

<?php
// Đóng kết nối CSDL nếu có
if (isset($conn)) {
    $conn->close();
}
?>
</body>
</html>
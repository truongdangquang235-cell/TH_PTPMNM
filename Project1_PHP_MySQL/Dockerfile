# Sử dụng image PHP kèm Apache
FROM php:8.1-apache

# Cài đặt các extension cần thiết để kết nối MySQL
RUN docker-php-ext-install mysqli pdo pdo_mysql

# Copy code từ thư mục src vào thư mục web của container
COPY ./src /var/www/html/

# Mở cổng 80
EXPOSE 80
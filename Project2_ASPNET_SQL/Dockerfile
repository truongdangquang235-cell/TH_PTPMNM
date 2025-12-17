# --- Giai đoạn 1: Build Code (Dùng bản 8.0) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy file project và restore thư viện
COPY ./src/*.csproj ./
RUN dotnet restore

# Copy toàn bộ code vào
COPY ./src/ ./

# --- SỬA DÒNG NÀY ---
# Thêm tên file "WebApplication1.csproj" vào lệnh để nó không bị nhầm sang file .sln
RUN dotnet publish "WebApplication1.csproj" -c Release -o out

# --- Giai đoạn 2: Chạy ứng dụng (Dùng bản 8.0) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Tên file chạy (giữ nguyên như cũ)
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
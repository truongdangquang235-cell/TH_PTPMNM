# --- Giai đoạn 1: Build Code (Sử dụng SDK 8.0) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy file project cụ thể (tránh nhầm lẫn)
COPY ./src/GoCoffeeTea.csproj ./
RUN dotnet restore

# Copy toàn bộ source code
COPY ./src/ ./

# Build ra file dll (Chỉ đích danh file csproj để không bị lỗi tìm nhầm file .sln)
RUN dotnet publish "GoCoffeeTea.csproj" -c Release -o out

# --- Giai đoạn 2: Chạy ứng dụng ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Chạy file dll (Tên file này luôn giống tên file .csproj)
ENTRYPOINT ["dotnet", "GoCoffeeTea.dll"]
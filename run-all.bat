@echo off
chcp 65001 > nul
echo ===================================================
echo     DANG KHOI CHAY TOAN BO HE THONG SHOPMINI
echo ===================================================

echo 1. Khoi chay Gateway.Api (Port 5258)...
start "Gateway.Api" cmd /k "cd gateway\Gateway.Api && dotnet run"

echo 2. Khoi chay UserService (Port 5104)...
start "UserService" cmd /k "cd services\UserService && dotnet run"

echo 3. Khoi chay ProductService (Port 5071)...
start "ProductService" cmd /k "cd services\ProductService && dotnet run"

echo 4. Khoi chay CartService (Port 5290)...
start "CartService" cmd /k "cd services\CartService && dotnet run"

echo 5. Khoi chay OrderService (Port 5218)...
start "OrderService" cmd /k "cd services\OrderService && dotnet run"

echo 6. Khoi chay PaymentService (Port 5151)...
start "PaymentService" cmd /k "cd services\PaymentService && dotnet run"

echo ===================================================
echo  Da bat xong 6 cua so terminal cho 6 service!
echo  Dang mo trang chu ShopMini tren trinh duyet...
echo ===================================================
start "" "frontend\index.html"
pause

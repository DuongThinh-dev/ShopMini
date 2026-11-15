let mergedCartItems = [];
let userSavedAddresses = [];
let pendingQrSession = null;

document.addEventListener('DOMContentLoaded', async () => {
    if (!document.getElementById('cart-items-body')) return;
    const user = auth.requireAuth();
    if (!user) return;

    await Promise.all([
        loadUserCartAndProducts(user.userId),
        loadUserSavedAddresses(user)
    ]);
});

async function loadUserCartAndProducts(userId) {
    const tbody = document.getElementById('cart-items-body');
    try {
        const [cart, products] = await Promise.all([
            api.getCart(userId),
            api.getProducts()
        ]);

        const rawItems = cart?.items || cart?.cartItems || [];
        const productMap = new Map((products || []).map(p => [p.productId, p]));

        mergedCartItems = rawItems.map(item => {
            const prod = productMap.get(item.productId) || {};
            return {
                cartItemId: item.cartItemId,
                productId: item.productId,
                quantity: item.quantity,
                productName: prod.productName || prod.name || 'Sản phẩm ShopMini',
                price: prod.price || 0,
                stock: prod.stock ?? 99,
                imageUrl: prod.imageUrl || null
            };
        });

        renderCartTable();
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="5" class="text-danger py-4 text-center">Lỗi tải giỏ hàng: ${err.message}</td></tr>`;
    }
}

function renderCartTable() {
    const tbody = document.getElementById('cart-items-body');
    const subtotalEl = document.getElementById('summary-subtotal');
    const totalEl = document.getElementById('summary-total');
    const checkoutBtn = document.getElementById('btn-place-order');

    if (mergedCartItems.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center py-5">
                    <i class="fa-solid fa-cart-arrow-down fa-3x text-muted mb-3"></i>
                    <h5 class="fw-bold">Giỏ hàng của bạn đang trống</h5>
                    <p class="text-muted small mb-3">Hãy chọn thêm sản phẩm công nghệ hoặc streetwear yêu thích nhé!</p>
                    <a href="products.html" class="btn btn-primary-modern px-4">Tiếp tục mua sắm</a>
                </td>
            </tr>
        `;
        if (subtotalEl) subtotalEl.textContent = formatCurrency(0);
        if (totalEl) totalEl.textContent = formatCurrency(0);
        if (checkoutBtn) checkoutBtn.disabled = true;
        return;
    }

    if (checkoutBtn) checkoutBtn.disabled = false;
    let totalAmount = 0;

    tbody.innerHTML = mergedCartItems.map(item => {
        const lineTotal = item.price * item.quantity;
        totalAmount += lineTotal;
        const imgSrc = resolveProductImage(item.imageUrl, item.productName);
        const fallbackSrc = getProductFallbackImage(item.productName);

        return `
            <tr>
                <td>
                    <div class="d-flex align-items-center gap-3">
                        <img src="${imgSrc}" alt="${item.productName}" class="rounded-3 border" style="width:64px;height:64px;object-fit:cover;" onerror="this.onerror=null;this.src='${fallbackSrc}'">
                        <div>
                            <a href="product-detail.html?id=${item.productId}" class="fw-bold text-dark text-decoration-none d-block">${item.productName}</a>
                            <small class="text-muted">Kho còn: ${item.stock}</small>
                        </div>
                    </div>
                </td>
                <td class="fw-semibold">${formatCurrency(item.price)}</td>
                <td>
                    <div class="input-group input-group-sm" style="width: 115px;">
                        <button class="btn btn-outline-secondary" onclick="changeItemQty('${item.productId}', ${item.quantity - 1})">-</button>
                        <input type="text" class="form-control text-center fw-bold bg-white" value="${item.quantity}" readonly>
                        <button class="btn btn-outline-secondary" onclick="changeItemQty('${item.productId}', ${item.quantity + 1})">+</button>
                    </div>
                </td>
                <td class="fw-bold text-primary">${formatCurrency(lineTotal)}</td>
                <td class="text-end">
                    <button class="btn btn-sm btn-outline-danger rounded-3" title="Xóa sản phẩm" onclick="removeItemFromCart('${item.productId}')">
                        <i class="fa-solid fa-trash-can"></i>
                    </button>
                </td>
            </tr>
        `;
    }).join('');

    if (subtotalEl) subtotalEl.textContent = formatCurrency(totalAmount);
    if (totalEl) totalEl.textContent = formatCurrency(totalAmount);
}

async function changeItemQty(productId, newQty) {
    const user = auth.getCurrentUser();
    if (!user) return;
    if (newQty <= 0) {
        await removeItemFromCart(productId);
        return;
    }
    try {
        await api.updateCartItem(user.userId, productId, newQty);
        await loadUserCartAndProducts(user.userId);
        await updateCartBadge();
    } catch (err) {
        showToast(err.message || 'Không thể cập nhật số lượng', 'danger');
    }
}

async function removeItemFromCart(productId) {
    const user = auth.getCurrentUser();
    if (!user) return;
    try {
        await api.removeCartItem(user.userId, productId);
        await loadUserCartAndProducts(user.userId);
        await updateCartBadge();
        showToast('Đã xóa sản phẩm khỏi giỏ hàng', 'info');
    } catch (err) {
        showToast(err.message || 'Lỗi khi xóa sản phẩm', 'danger');
    }
}

async function handleClearCart() {
    const user = auth.getCurrentUser();
    if (!user || mergedCartItems.length === 0) return;
    try {
        await api.clearCart(user.userId);
        mergedCartItems = [];
        renderCartTable();
        await updateCartBadge();
        showToast('Đã làm trống giỏ hàng', 'info');
    } catch (err) {
        showToast(err.message || 'Lỗi làm trống giỏ hàng', 'danger');
    }
}

async function loadUserSavedAddresses(user) {
    const selectEl = document.getElementById('saved-address-select');
    const receiverInput = document.getElementById('checkout-receiver');
    if (receiverInput && user.fullName) receiverInput.value = user.fullName;

    try {
        const addresses = await api.getAddressesByUser(user.userId);
        userSavedAddresses = addresses || [];

        if (!selectEl) return;
        if (userSavedAddresses.length === 0) {
            selectEl.innerHTML = `<option value="">-- Nhập địa chỉ giao hàng mới bên dưới --</option>`;
            return;
        }

        selectEl.innerHTML = userSavedAddresses.map((addr, idx) => {
            const fullAddr = `${addr.detailAddress}, ${addr.ward}, ${addr.district}, ${addr.province}`;
            return `<option value="${idx}" ${addr.isDefault ? 'selected' : ''}>${addr.isDefault ? '[Mặc định] ' : ''}${addr.receiverName} - ${fullAddr}</option>`;
        }).join('') + `<option value="custom">+ Nhập địa chỉ giao hàng khác...</option>`;

        applySelectedAddress();
    } catch {
        // User mới chưa có địa chỉ đã lưu
    }
}

function applySelectedAddress() {
    const selectEl = document.getElementById('saved-address-select');
    if (!selectEl) return;
    const val = selectEl.value;
    if (val === '' || val === 'custom') return;

    const addr = userSavedAddresses[parseInt(val, 10)];
    if (!addr) return;

    document.getElementById('checkout-receiver').value = addr.receiverName || '';
    document.getElementById('checkout-phone').value = addr.phone || '';
    document.getElementById('checkout-address').value = `${addr.detailAddress}, ${addr.ward}, ${addr.district}, ${addr.province}`;
}

async function handleCheckoutSubmit(e) {
    e.preventDefault();
    const user = auth.requireAuth();
    if (!user || mergedCartItems.length === 0) return;

    const receiverName = document.getElementById('checkout-receiver').value.trim();
    const phone = document.getElementById('checkout-phone').value.trim();
    const shippingAddress = document.getElementById('checkout-address').value.trim();
    const paymentMethod = document.querySelector('input[name="paymentMethod"]:checked')?.value || 'Simulation';

    const btn = document.getElementById('btn-place-order');
    btn.disabled = true;
    btn.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>Đang tạo đơn hàng...`;

    try {
        const totalAmount = mergedCartItems.reduce((sum, i) => sum + i.price * i.quantity, 0);

        const createdOrder = await api.createOrder({
            userId: user.userId,
            receiverName,
            phone,
            shippingAddress,
            orderItems: mergedCartItems.map(item => ({
                productId: item.productId,
                productName: item.productName,
                unitPrice: item.price,
                quantity: item.quantity
            }))
        });

        const orderId = createdOrder.orderId || createdOrder.id;
        const createdPayment = await api.createPayment({
            orderId,
            amount: totalAmount,
            paymentMethod
        });

        await api.clearCart(user.userId);
        mergedCartItems = [];
        renderCartTable();
        await updateCartBadge();

        if (paymentMethod === 'Simulation') {
            showToast('Tạo đơn hàng thành công! Đang chuyển sang trang Đơn hàng để quét mã QR...', 'success');
            setTimeout(() => {
                window.location.href = `orders.html?payNow=${orderId}`;
            }, 700);
            return;
        }

        // Đơn hàng COD: chuyển ngay từ PendingPayment sang PendingConfirmation (Chờ người bán xác nhận)
        try {
            await api.updateOrderStatus(orderId, 'PendingConfirmation');
        } catch (statusErr) {
            console.warn('Cập nhật trạng thái COD:', statusErr);
        }

        showToast('Đặt hàng COD thành công! Đang chuyển sang trang Đơn hàng...', 'success');
        setTimeout(() => {
            window.location.href = 'orders.html';
        }, 700);
    } catch (err) {
        showToast(err.message || 'Đặt hàng thất bại, vui lòng thử lại.', 'danger');
        btn.disabled = false;
        btn.innerHTML = `<i class="fa-solid fa-check-circle me-2"></i>Thanh toán & Đặt hàng`;
    }
}

async function confirmQrDemoPaid() {
    if (!pendingQrSession) return;
    const btn = document.getElementById('btn-confirm-qr-paid');
    btn.disabled = true;
    btn.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>Đang xác nhận giao dịch...`;

    try {
        const txnCode = `QR-DEMO-${Date.now()}`;
        await api.updatePaymentStatus(pendingQrSession.paymentId, 'Paid', txnCode);
        if (pendingQrSession.orderStatus === 'PendingPayment') {
            await api.updateOrderStatus(pendingQrSession.orderId, 'PendingConfirmation');
        }
        showToast(`Thanh toán QR thành công! Mã GD: ${txnCode}`, 'success');
        setTimeout(() => {
            window.location.href = 'orders.html';
        }, 900);
    } catch (err) {
        showToast(err.message || 'Không thể cập nhật trạng thái thanh toán', 'danger');
        btn.disabled = false;
        btn.innerHTML = `<i class="fa-solid fa-circle-check me-2"></i>Tôi đã chuyển khoản (Xác nhận thanh toán)`;
    }
}

window.changeItemQty = changeItemQty;
window.removeItemFromCart = removeItemFromCart;
window.handleClearCart = handleClearCart;
window.applySelectedAddress = applySelectedAddress;
window.handleCheckoutSubmit = handleCheckoutSubmit;
window.confirmQrDemoPaid = confirmQrDemoPaid;
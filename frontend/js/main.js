document.addEventListener('DOMContentLoaded', () => {
    renderAuthSlot();
    updateCartBadge();
});

function renderAuthSlot() {
    const slot = document.getElementById('auth-nav-slot');
    if (!slot) return;

    const user = auth.getCurrentUser();
    const isSubPage = window.location.pathname.includes('/pages/');
    const loginPath = isSubPage ? 'login.html' : 'pages/login.html';
    const ordersPath = isSubPage ? 'orders.html' : 'pages/orders.html';
    const adminPath = isSubPage ? 'admin.html' : 'pages/admin.html';

    if (!user) {
        slot.innerHTML = `
            <a class="btn btn-primary-modern rounded-pill px-4" href="${loginPath}">
                <i class="fa-regular fa-user me-1"></i> Đăng nhập
            </a>
        `;
        return;
    }

    const displayName = user.fullName || user.email;
    const isAdmin = auth.isAdmin();

    slot.innerHTML = `
        <div class="d-flex align-items-center gap-2">
            ${isAdmin ? `
                <a href="${adminPath}" class="btn btn-warning btn-sm rounded-pill px-3 fw-bold text-dark">
                    <i class="fa-solid fa-chart-line me-1"></i> Trang Admin
                </a>
            ` : ''}
            <div class="dropdown">
                <button class="btn btn-outline-primary rounded-pill px-3 fw-semibold dropdown-toggle" type="button" data-bs-toggle="dropdown">
                    <i class="fa-solid fa-circle-user me-1"></i> ${displayName}
                </button>
                <ul class="dropdown-menu dropdown-menu-end shadow border-0 rounded-3 mt-2">
                    <li class="px-3 py-2 border-bottom">
                        <div class="fw-bold small">${displayName} <span class="badge ${isAdmin ? 'bg-warning text-dark' : 'bg-primary-subtle text-primary'} ms-1">${isAdmin ? 'Admin' : 'Customer'}</span></div>
                        <div class="text-muted" style="font-size: 0.75rem;">${user.email}</div>
                    </li>
                    ${isAdmin ? `<li><a class="dropdown-item py-2 fw-semibold text-primary" href="${adminPath}"><i class="fa-solid fa-gauge-high me-2"></i>Bảng điều khiển Admin</a></li>` : ''}
                    <li><a class="dropdown-item py-2" href="${ordersPath}"><i class="fa-solid fa-box me-2 text-primary"></i>Đơn hàng của tôi</a></li>
                    <li><hr class="dropdown-divider"></li>
                    <li><button class="dropdown-item py-2 text-danger" onclick="auth.logout()"><i class="fa-solid fa-right-from-bracket me-2"></i>Đăng xuất</button></li>
                </ul>
            </div>
        </div>
    `;
}

async function updateCartBadge() {
    const badge = document.getElementById('cart-badge');
    if (!badge) return;

    const user = auth.getCurrentUser();
    if (!user) {
        badge.textContent = '0';
        return;
    }

    try {
        const cart = await api.getCart(user.userId);
        const items = cart?.items || cart?.cartItems || [];
        const totalQty = items.reduce((sum, item) => sum + (item.quantity || 0), 0);
        badge.textContent = totalQty;
    } catch {
        badge.textContent = '0';
    }
}

window.updateCartBadge = updateCartBadge;
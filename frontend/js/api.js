const GATEWAY_BASE_URL = 'http://localhost:5258/api';

const api = {
    async request(endpoint, options = {}) {
        const url = `${GATEWAY_BASE_URL}${endpoint}`;
        const config = {
            headers: {
                'Content-Type': 'application/json',
                ...(options.headers || {})
            },
            ...options
        };

        const response = await fetch(url, config);
        const text = await response.text();
        let data = null;
        try {
            data = text ? JSON.parse(text) : null;
        } catch {
            data = text;
        }

        if (!response.ok) {
            let detailMsg = '';
            if (data && typeof data === 'object') {
                if (data.errors) {
                    detailMsg = Object.values(data.errors).flat().join('; ');
                } else {
                    detailMsg = data.detail || data.message || data.title || JSON.stringify(data);
                }
            } else if (typeof data === 'string' && data.trim()) {
                detailMsg = data.split('\n')[0].slice(0, 180);
            }
            throw new Error(detailMsg || `Lỗi hệ thống (${response.status})`);
        }
        return data;
    },

    // --- ProductService (/api/products, /api/categories) ---
    getProducts: () => api.request('/products'),
    getProductById: (id) => api.request(`/products/${id}`),
    createProduct: (payload) => api.request('/products', { method: 'POST', body: JSON.stringify(payload) }),
    updateProduct: (id, payload) => api.request(`/products/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
    deleteProduct: (id) => api.request(`/products/${id}`, { method: 'DELETE' }),
    getCategories: () => api.request('/categories'),

    // --- UserService (/api/users, /api/roles, /api/addresses) ---
    getUsers: () => api.request('/users'),
    getUserById: (id) => api.request(`/users/${id}`),
    getUserByEmail: (email) => api.request(`/users/by-email?email=${encodeURIComponent(email)}`),
    createUser: (payload) => api.request('/users', { method: 'POST', body: JSON.stringify(payload) }),
    getRoles: () => api.request('/roles'),
    getAddressesByUser: (userId) => api.request(`/addresses/user/${userId}`),
    createAddress: (payload) => api.request('/addresses', { method: 'POST', body: JSON.stringify(payload) }),

    // --- CartService (/api/carts) ---
    getCart: (userId) => api.request(`/carts/${userId}`),
    addToCart: (userId, productId, quantity = 1) =>
        api.request(`/carts/${userId}/items`, {
            method: 'POST',
            body: JSON.stringify({ productId, quantity })
        }),
    updateCartItem: (userId, productId, quantity) =>
        api.request(`/carts/${userId}/items/${productId}`, {
            method: 'PUT',
            body: JSON.stringify({ quantity })
        }),
    removeCartItem: (userId, productId) =>
        api.request(`/carts/${userId}/items/${productId}`, { method: 'DELETE' }),
    clearCart: (userId) =>
        api.request(`/carts/${userId}`, { method: 'DELETE' }),

    // --- OrderService (/api/orders) ---
    getAllOrders: () => api.request('/orders'),
    getOrdersByUser: (userId) => api.request(`/orders/user/${userId}`),
    getOrderById: (id) => api.request(`/orders/${id}`),
    createOrder: (payload) => api.request('/orders', { method: 'POST', body: JSON.stringify(payload) }),
    updateOrderStatus: (orderId, status) =>
        api.request(`/orders/${orderId}/status`, {
            method: 'PATCH',
            body: JSON.stringify({ status })
        }),
    cancelOrder: (orderId, userId) =>
        api.request(`/orders/${orderId}/cancel?userId=${userId}`, { method: 'PATCH' }),

    // --- PaymentService (/api/payments) ---
    getAllPayments: () => api.request('/payments'),
    getPaymentByOrder: (orderId) => api.request(`/payments/by-order/${orderId}`),
    createPayment: (payload) => api.request('/payments', { method: 'POST', body: JSON.stringify(payload) }),
    updatePaymentStatus: (paymentId, status, transactionCode = null) =>
        api.request(`/payments/${paymentId}/status`, {
            method: 'PUT',
            body: JSON.stringify({ status, transactionCode })
        })
};

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(Number(amount || 0));
}

function getProductFallbackImage(name = 'ShopMini') {
    const safeText = String(name)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .slice(0, 22);

    const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="400" height="300" viewBox="0 0 400 300">
        <defs>
            <linearGradient id="g" x1="0" y1="0" x2="1" y2="1">
                <stop offset="0%" stop-color="#eff6ff"/>
                <stop offset="100%" stop-color="#dbeafe"/>
            </linearGradient>
        </defs>
        <rect width="400" height="300" fill="url(#g)"/>
        <circle cx="200" cy="120" r="36" fill="#2563eb" opacity="0.15"/>
        <text x="50%" y="58%" dominant-baseline="middle" text-anchor="middle" font-family="sans-serif" font-weight="700" font-size="16" fill="#1e3a8a">${safeText}</text>
    </svg>`;

    return `data:image/svg+xml;base64,${btoa(unescape(encodeURIComponent(svg)))}`;
}

function resolveProductImage(imageUrl, name) {
    if (!imageUrl) return getProductFallbackImage(name);
    if (imageUrl.startsWith('http') || imageUrl.startsWith('data:')) return imageUrl;
    const isSubPage = window.location.pathname.includes('/pages/');
    const cleanPath = imageUrl.replace(/^\/+/, '');
    return isSubPage ? `../${cleanPath}` : cleanPath;
}

function generateDemoQrDataUrl(orderCode, amount) {
    const label = String(orderCode || 'SHOPMINI').slice(0, 18);
    const amountText = formatCurrency(amount);
    let cells = '';
    let seed = 0;
    for (let i = 0; i < label.length; i++) seed += label.charCodeAt(i) * (i + 7);

    for (let r = 0; r < 15; r++) {
        for (let c = 0; c < 15; c++) {
            const inFinderTL = r < 4 && c < 4;
            const inFinderTR = r < 4 && c > 10;
            const inFinderBL = r > 10 && c < 4;
            if (inFinderTL || inFinderTR || inFinderBL) continue;
            const bit = ((r * 31 + c * 17 + seed) % 5 === 0) || ((r * c + seed) % 3 === 0);
            if (bit) {
                cells += `<rect x="${25 + c * 10}" y="${25 + r * 10}" width="8" height="8" rx="1.5" fill="#0f172a"/>`;
            }
        }
    }

    const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="220" height="245" viewBox="0 0 220 245">
        <rect width="220" height="245" rx="16" fill="#ffffff" stroke="#e2e8f0" stroke-width="2"/>
        <rect x="25" y="25" width="36" height="36" rx="6" fill="none" stroke="#2563eb" stroke-width="5"/>
        <rect x="34" y="34" width="18" height="18" rx="3" fill="#0f172a"/>
        <rect x="139" y="25" width="36" height="36" rx="6" fill="none" stroke="#2563eb" stroke-width="5"/>
        <rect x="148" y="34" width="18" height="18" rx="3" fill="#0f172a"/>
        <rect x="25" y="139" width="36" height="36" rx="6" fill="none" stroke="#2563eb" stroke-width="5"/>
        <rect x="34" y="148" width="18" height="18" rx="3" fill="#0f172a"/>
        ${cells}
        <rect x="78" y="83" width="44" height="34" rx="8" fill="#2563eb"/>
        <text x="100" y="104" text-anchor="middle" font-family="sans-serif" font-weight="800" font-size="11" fill="#ffffff">QR</text>
        <text x="110" y="205" text-anchor="middle" font-family="sans-serif" font-weight="800" font-size="13" fill="#2563eb">${amountText}</text>
        <text x="110" y="224" text-anchor="middle" font-family="sans-serif" font-weight="600" font-size="10" fill="#64748b">DEMO PAY • ${label}</text>
    </svg>`;

    return `data:image/svg+xml;base64,${btoa(unescape(encodeURIComponent(svg)))}`;
}

function showToast(message, type = 'success') {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        container.style.zIndex = '1090';
        document.body.appendChild(container);
    }

    const bgMap = {
        success: 'bg-success text-white',
        danger: 'bg-danger text-white',
        warning: 'bg-warning text-dark',
        info: 'bg-primary text-white'
    };
    const iconMap = {
        success: 'fa-circle-check',
        danger: 'fa-circle-exclamation',
        warning: 'fa-triangle-exclamation',
        info: 'fa-circle-info'
    };

    const toastEl = document.createElement('div');
    toastEl.className = `toast align-items-center border-0 shadow-lg ${bgMap[type] || bgMap.info}`;
    toastEl.setAttribute('role', 'alert');
    toastEl.innerHTML = `
        <div class="d-flex">
            <div class="toast-body fw-semibold">
                <i class="fa-solid ${iconMap[type] || iconMap.info} me-2"></i>${message}
            </div>
            <button type="button" class="btn-close ${type === 'warning' ? '' : 'btn-close-white'} me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    container.appendChild(toastEl);
    const bsToast = new bootstrap.Toast(toastEl, { delay: 3200 });
    bsToast.show();
    toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
}

window.api = api;
window.formatCurrency = formatCurrency;
window.getProductFallbackImage = getProductFallbackImage;
window.resolveProductImage = resolveProductImage;
window.generateDemoQrDataUrl = generateDemoQrDataUrl;
window.showToast = showToast;
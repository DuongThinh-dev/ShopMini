let adminProducts = [];
let adminCategories = [];
let adminOrders = [];
let adminPayments = [];

// Quy tắc chuyển trạng thái chuẩn theo OrderService State Machine
const NEXT_STATUS_TRANSITIONS = {
    PendingPayment: ['PendingConfirmation', 'Cancelled'],
    PendingConfirmation: ['Confirmed', 'Cancelled'],
    Confirmed: ['Shipping', 'Cancelled'],
    Shipping: ['Completed'],
    Completed: [],
    Cancelled: [],
    PaymentFailed: ['Cancelled']
};

const STATUS_VI_LABELS = {
    PendingPayment: { label: 'Chờ thanh toán', badge: 'bg-warning text-dark' },
    PendingConfirmation: { label: 'Chờ xác nhận', badge: 'bg-info text-dark' },
    Confirmed: { label: 'Đã xác nhận', badge: 'bg-primary' },
    Shipping: { label: 'Đang giao hàng', badge: 'bg-indigo text-white bg-primary' },
    Completed: { label: 'Hoàn thành', badge: 'bg-success' },
    Cancelled: { label: 'Đã hủy', badge: 'bg-danger' },
    PaymentFailed: { label: 'Thanh toán lỗi', badge: 'bg-danger' }
};

document.addEventListener('DOMContentLoaded', async () => {
    if (!auth.requireAdmin()) return;
    await loadAdminDashboard();
});

async function loadAdminDashboard() {
    try {
        const [products, categories, orders, payments] = await Promise.all([
            api.getProducts(),
            api.getCategories(),
            api.getAllOrders(),
            api.getAllPayments().catch(() => [])
        ]);

        adminProducts = products || [];
        adminCategories = categories || [];
        adminOrders = orders || [];
        adminPayments = payments || [];

        populateCategorySelect();
        renderAdminKPIs();
        renderAdminProductsTable();
        renderAdminOrdersTable();
        renderAdminSoldProductsTable();
    } catch (err) {
        showToast(`Lỗi tải dữ liệu Admin: ${err.message}`, 'danger');
    }
}

function renderAdminKPIs() {
    const validOrders = adminOrders.filter(o => o.status !== 'Cancelled' && o.status !== 'PaymentFailed');
    const totalRevenue = validOrders.reduce((sum, o) => sum + Number(o.totalAmount || 0), 0);

    const paidRevenue = adminPayments
        .filter(p => p.status === 'Paid')
        .reduce((sum, p) => sum + Number(p.amount || 0), 0);

    const totalSoldQty = validOrders.reduce((sum, o) => {
        const items = o.orderItems || o.items || [];
        return sum + items.reduce((s, i) => s + Number(i.quantity || 0), 0);
    }, 0);

    const pendingCount = adminOrders.filter(o =>
        o.status === 'PendingPayment' || o.status === 'PendingConfirmation'
    ).length;

    const totalStock = adminProducts.reduce((sum, p) => sum + Number(p.stock || 0), 0);

    document.getElementById('kpi-revenue').textContent = formatCurrency(totalRevenue);
    document.getElementById('kpi-paid-revenue').textContent = `Đã thu tiền (Paid): ${formatCurrency(paidRevenue)}`;
    document.getElementById('kpi-sold-qty').textContent = `${totalSoldQty} sản phẩm`;
    document.getElementById('kpi-total-orders').textContent = `${adminOrders.length} đơn`;
    document.getElementById('kpi-pending-orders').textContent = `Chờ xử lý: ${pendingCount} đơn`;
    document.getElementById('kpi-total-products').textContent = `${adminProducts.length} mẫu`;
    document.getElementById('kpi-total-stock').textContent = `Tổng tồn kho: ${totalStock}`;
}

function populateCategorySelect() {
    const select = document.getElementById('prod-category');
    if (!select) return;
    select.innerHTML = adminCategories.map(c =>
        `<option value="${c.categoryId}">${c.categoryName}</option>`
    ).join('');
}

function renderAdminProductsTable() {
    const tbody = document.getElementById('admin-products-body');
    const countEl = document.getElementById('admin-product-count');
    const keyword = (document.getElementById('admin-product-search')?.value || '').trim().toLowerCase();

    const filtered = adminProducts.filter(p =>
        (p.productName || p.name || '').toLowerCase().includes(keyword)
    );

    if (countEl) countEl.textContent = `${filtered.length} sản phẩm`;

    if (filtered.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center py-4 text-muted">Không có sản phẩm nào phù hợp.</td></tr>`;
        return;
    }

    tbody.innerHTML = filtered.map(p => {
        const name = p.productName || p.name;
        const cat = adminCategories.find(c => c.categoryId === p.categoryId);
        const imgSrc = resolveProductImage(p.imageUrl, name);
        const fallback = getProductFallbackImage(name);

        return `
            <tr>
                <td>
                    <div class="d-flex align-items-center gap-3">
                        <img src="${imgSrc}" alt="${name}" class="rounded-3 border" style="width:48px;height:48px;object-fit:cover;" onerror="this.onerror=null;this.src='${fallback}'">
                        <div>
                            <div class="fw-bold text-dark">${name}</div>
                            <small class="text-muted">${(p.description || '').slice(0, 48)}</small>
                        </div>
                    </div>
                </td>
                <td><span class="badge bg-light text-dark border">${cat ? cat.categoryName : 'Chưa phân loại'}</span></td>
                <td class="fw-bold text-primary">${formatCurrency(p.price)}</td>
                <td><span class="badge ${p.stock > 5 ? 'bg-success-subtle text-success' : 'bg-danger-subtle text-danger'}">${p.stock}</span></td>
                <td>
                    <span class="badge ${p.isActive !== false ? 'bg-primary-subtle text-primary' : 'bg-secondary-subtle text-secondary'}">
                        ${p.isActive !== false ? 'Đang bán' : 'Ngừng bán'}
                    </span>
                </td>
                <td class="text-end">
                    <button class="btn btn-sm btn-outline-primary rounded-3 me-1" onclick="openProductModal('${p.productId}')">
                        <i class="fa-solid fa-pen-to-square"></i> Sửa
                    </button>
                    <button class="btn btn-sm btn-outline-danger rounded-3" onclick="handleDeleteProduct('${p.productId}', '${name.replace(/'/g, "\\'")}')">
                        <i class="fa-solid fa-trash"></i> Xóa
                    </button>
                </td>
            </tr>
        `;
    }).join('');
}

function openProductModal(productId = null) {
    const titleEl = document.getElementById('product-modal-title');
    document.getElementById('prod-id').value = productId || '';

    if (productId) {
        const p = adminProducts.find(item => item.productId === productId);
        if (!p) return;
        titleEl.textContent = 'Chỉnh sửa sản phẩm';
        document.getElementById('prod-name').value = p.productName || p.name || '';
        document.getElementById('prod-category').value = p.categoryId;
        document.getElementById('prod-price').value = p.price;
        document.getElementById('prod-stock').value = p.stock;
        document.getElementById('prod-image').value = p.imageUrl || '';
        document.getElementById('prod-desc').value = p.description || '';
        document.getElementById('prod-active').checked = p.isActive !== false;
    } else {
        titleEl.textContent = 'Thêm sản phẩm mới';
        document.getElementById('prod-name').value = '';
        if (adminCategories[0]) document.getElementById('prod-category').value = adminCategories[0].categoryId;
        document.getElementById('prod-price').value = '';
        document.getElementById('prod-stock').value = '20';
        document.getElementById('prod-image').value = '';
        document.getElementById('prod-desc').value = '';
        document.getElementById('prod-active').checked = true;
    }

    new bootstrap.Modal(document.getElementById('productFormModal')).show();
}

async function handleSaveProduct(e) {
    e.preventDefault();
    const id = document.getElementById('prod-id').value;
    const payload = {
        categoryId: document.getElementById('prod-category').value,
        productName: document.getElementById('prod-name').value.trim(),
        price: Number(document.getElementById('prod-price').value),
        stock: Number(document.getElementById('prod-stock').value),
        imageUrl: document.getElementById('prod-image').value.trim() || null,
        description: document.getElementById('prod-desc').value.trim() || null,
        isActive: document.getElementById('prod-active').checked
    };

    const btn = document.getElementById('btn-save-product');
    btn.disabled = true;

    try {
        if (id) {
            await api.updateProduct(id, payload);
            showToast('Cập nhật sản phẩm thành công!', 'success');
        } else {
            await api.createProduct(payload);
            showToast('Thêm sản phẩm mới thành công!', 'success');
        }
        bootstrap.Modal.getInstance(document.getElementById('productFormModal'))?.hide();
        await loadAdminDashboard();
    } catch (err) {
        showToast(err.message || 'Lỗi lưu sản phẩm', 'danger');
    } finally {
        btn.disabled = false;
    }
}

async function handleDeleteProduct(productId, name) {
    if (!confirm(`Bạn có chắc chắn muốn xóa sản phẩm "${name}" không?`)) return;
    try {
        await api.deleteProduct(productId);
        showToast('Đã xóa sản phẩm khỏi hệ thống!', 'info');
        await loadAdminDashboard();
    } catch (err) {
        showToast(err.message || 'Không thể xóa sản phẩm (có thể đã phát sinh đơn hàng)', 'danger');
    }
}

function renderAdminOrdersTable() {
    const tbody = document.getElementById('admin-orders-body');
    const paymentMap = new Map(adminPayments.map(p => [p.orderId, p]));

    if (adminOrders.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center py-4 text-muted">Chưa có đơn hàng nào trong hệ thống.</td></tr>`;
        return;
    }

    tbody.innerHTML = adminOrders.map(o => {
        const pay = paymentMap.get(o.orderId);
        const items = o.orderItems || o.items || [];
        const nextStates = NEXT_STATUS_TRANSITIONS[o.status] || [];
        const statusMeta = STATUS_VI_LABELS[o.status] || { label: o.status, badge: 'bg-secondary' };

        return `
            <tr>
                <td>
                    <button class="btn btn-link p-0 text-start text-decoration-none fw-bold" onclick="openAdminOrderDetail('${o.orderId}')">
                        <code class="small text-primary d-block">${String(o.orderId).slice(0, 13)}... <i class="fa-solid fa-arrow-up-right-from-square" style="font-size:0.7rem;"></i></code>
                    </button>
                    <div class="fw-bold small">${o.receiverName} (${o.phone})</div>
                    <div class="text-muted text-truncate" style="font-size:0.75rem; max-width: 220px;" title="${o.shippingAddress}">${o.shippingAddress}</div>
                </td>
                <td class="small">
                    ${items.map(i => `<div>• ${i.productName} <strong>x${i.quantity}</strong></div>`).join('')}
                </td>
                <td class="fw-bold text-primary">${formatCurrency(o.totalAmount)}</td>
                <td class="small">
                    <div>${pay ? (pay.method === 'Simulation' ? 'Quét QR Demo' : pay.method) : 'COD'}</div>
                    <span class="badge ${pay?.status === 'Paid' ? 'bg-success' : 'bg-warning text-dark'}">${pay?.status || 'Unpaid'}</span>
                </td>
                <td>
                    <span class="badge ${statusMeta.badge} d-block py-1 text-center mb-1">${statusMeta.label}</span>
                    <small class="text-muted d-block text-center" style="font-size:0.7rem;">${o.status}</small>
                </td>
                <td class="text-end">
                    <div class="d-inline-flex gap-1 flex-wrap justify-content-end mb-1">
                        <button class="btn btn-sm btn-outline-info rounded-pill px-2" onclick="openAdminOrderDetail('${o.orderId}')">
                            <i class="fa-solid fa-eye me-1"></i>Chi tiết
                        </button>
                        ${nextStates.map(st => {
                            const stLabel = STATUS_VI_LABELS[st]?.label || st;
                            return `
                                <button class="btn btn-sm ${st === 'Cancelled' ? 'btn-outline-danger' : 'btn-outline-primary'} rounded-pill"
                                    onclick="handleAdminChangeOrderStatus('${o.orderId}', '${st}')">
                                    → ${stLabel}
                                </button>
                            `;
                        }).join('')}
                    </div>
                </td>
            </tr>
        `;
    }).join('');
}

function openAdminOrderDetail(orderId) {
    const o = adminOrders.find(item => item.orderId === orderId);
    if (!o) return;
    const pay = adminPayments.find(p => p.orderId === orderId);
    const items = o.orderItems || o.items || [];
    const nextStates = NEXT_STATUS_TRANSITIONS[o.status] || [];

    document.getElementById('modal-order-id-label').textContent = `Mã đơn hàng: ${o.orderId}`;
    document.getElementById('modal-receiver-name').textContent = o.receiverName;
    document.getElementById('modal-receiver-phone').textContent = `SĐT: ${o.phone}`;
    document.getElementById('modal-shipping-address').textContent = `Địa chỉ: ${o.shippingAddress}`;

    const payMethodName = pay ? (pay.method === 'Simulation' ? 'Quét QR Demo' : pay.method) : 'COD';
    document.getElementById('modal-payment-method').textContent = payMethodName;

    const payStatusEl = document.getElementById('modal-payment-status');
    const isPaid = pay?.status === 'Paid';
    payStatusEl.innerHTML = `<span class="badge ${isPaid ? 'bg-success' : 'bg-warning text-dark'}">${pay?.status || 'Unpaid'}</span>`;
    document.getElementById('modal-payment-txn').textContent = pay?.transactionCode || '(Chưa có)';

    // Stepper tiến trình trực quan
    const STEPS = [
        { key: 'PendingPayment', label: '1. Chờ thanh toán' },
        { key: 'PendingConfirmation', label: '2. Chờ xác nhận' },
        { key: 'Confirmed', label: '3. Đã xác nhận' },
        { key: 'Shipping', label: '4. Đang giao hàng' },
        { key: 'Completed', label: '5. Hoàn thành' }
    ];
    const isCancelled = o.status === 'Cancelled' || o.status === 'PaymentFailed';
    const activeIdx = STEPS.findIndex(s => s.key === o.status);

    const stepperEl = document.getElementById('modal-order-stepper');
    if (isCancelled) {
        stepperEl.innerHTML = `
            <div class="alert alert-danger py-2 px-3 rounded-3 small fw-bold">
                <i class="fa-solid fa-ban me-2"></i>Tình trạng hiện tại: ĐÃ HỦY ĐƠN HÀNG (${o.status})
            </div>
        `;
    } else {
        stepperEl.innerHTML = `
            <div class="d-flex justify-content-between align-items-center position-relative my-3 p-3 bg-light rounded-3">
                ${STEPS.map((step, idx) => {
                    const isCompleted = idx < activeIdx;
                    const isCurrent = idx === activeIdx;
                    const dotClass = isCurrent ? 'bg-primary text-white border-primary shadow' : (isCompleted ? 'bg-success text-white border-success' : 'bg-white text-muted border');
                    const textClass = isCurrent ? 'fw-bold text-primary' : (isCompleted ? 'fw-semibold text-success' : 'text-muted');
                    return `
                        <div class="text-center" style="flex: 1; z-index: 2;">
                            <div class="rounded-circle d-inline-flex align-items-center justify-content-center border ${dotClass}" style="width: 32px; height: 32px; font-size: 0.85rem;">
                                ${isCompleted ? '<i class="fa-solid fa-check"></i>' : (idx + 1)}
                            </div>
                            <div class="small mt-1 ${textClass}" style="font-size: 0.72rem;">${step.label}</div>
                            ${isCurrent ? '<span class="badge bg-primary text-white" style="font-size:0.62rem;">Đang ở bước này</span>' : ''}
                        </div>
                    `;
                }).join('')}
            </div>
        `;
    }

    // Items
    const itemsTbody = document.getElementById('modal-order-items-body');
    itemsTbody.innerHTML = items.map(item => `
        <tr>
            <td>
                <div class="fw-bold">${item.productName}</div>
                <code class="text-muted" style="font-size:0.7rem;">ID: ${item.productId}</code>
            </td>
            <td class="text-center fw-bold">x${item.quantity}</td>
            <td class="text-end">${formatCurrency(item.unitPrice)}</td>
            <td class="text-end fw-semibold text-primary">${formatCurrency(item.subtotal || (item.unitPrice * item.quantity))}</td>
        </tr>
    `).join('');

    document.getElementById('modal-order-total').textContent = formatCurrency(o.totalAmount);

    // Status Actions inside modal
    const actionsEl = document.getElementById('modal-status-actions');
    if (nextStates.length > 0) {
        actionsEl.innerHTML = nextStates.map(st => {
            const stMeta = STATUS_VI_LABELS[st] || { label: st };
            return `
                <button class="btn btn-sm ${st === 'Cancelled' ? 'btn-outline-danger' : 'btn-primary'} rounded-pill px-3"
                    onclick="handleAdminChangeOrderStatus('${o.orderId}', '${st}')">
                    Chuyển sang: ${stMeta.label} (${st})
                </button>
            `;
        }).join('');
    } else {
        actionsEl.innerHTML = `<span class="text-muted small">Đơn hàng đã kết thúc quy trình (${o.status}). Không có trạng thái kế tiếp.</span>`;
    }

    const modal = new bootstrap.Modal(document.getElementById('orderDetailModal'));
    modal.show();
}

async function handleAdminChangeOrderStatus(orderId, newStatus) {
    try {
        await api.updateOrderStatus(orderId, newStatus);
        showToast(`Đã chuyển trạng thái đơn sang ${STATUS_VI_LABELS[newStatus]?.label || newStatus}`, 'success');
        await loadAdminDashboard();
        const modalEl = document.getElementById('orderDetailModal');
        if (modalEl && modalEl.classList.contains('show')) {
            openAdminOrderDetail(orderId);
        }
    } catch (err) {
        showToast(err.message || 'Không thể cập nhật trạng thái đơn hàng', 'danger');
    }
}

function renderAdminSoldProductsTable() {
    const tbody = document.getElementById('admin-sales-body');
    const validOrders = adminOrders.filter(o => o.status !== 'Cancelled' && o.status !== 'PaymentFailed');
    const summaryMap = new Map();

    validOrders.forEach(o => {
        const items = o.orderItems || o.items || [];
        items.forEach(item => {
            const key = item.productId || item.productName;
            const prev = summaryMap.get(key) || {
                productName: item.productName,
                soldQty: 0,
                revenue: 0
            };
            const qty = Number(item.quantity || 0);
            const lineTotal = Number(item.subtotal || (item.unitPrice * qty) || 0);
            prev.soldQty += qty;
            prev.revenue += lineTotal;
            summaryMap.set(key, prev);
        });
    });

    const ranked = Array.from(summaryMap.values()).sort((a, b) => b.soldQty - a.soldQty);

    if (ranked.length === 0) {
        tbody.innerHTML = `<tr><td colspan="4" class="text-center py-4 text-muted">Chưa có sản phẩm nào được bán.</td></tr>`;
        return;
    }

    tbody.innerHTML = ranked.map((item, idx) => `
        <tr>
            <td><span class="badge ${idx === 0 ? 'bg-warning text-dark' : 'bg-light text-dark border'}">#${idx + 1}</span></td>
            <td class="fw-bold">${item.productName}</td>
            <td class="text-center fw-bold text-primary">${item.soldQty}</td>
            <td class="text-end fw-bold text-success">${formatCurrency(item.revenue)}</td>
        </tr>
    `).join('');
}

window.loadAdminDashboard = loadAdminDashboard;
window.renderAdminProductsTable = renderAdminProductsTable;
window.openProductModal = openProductModal;
window.handleSaveProduct = handleSaveProduct;
window.handleDeleteProduct = handleDeleteProduct;
window.handleAdminChangeOrderStatus = handleAdminChangeOrderStatus;
window.openAdminOrderDetail = openAdminOrderDetail;
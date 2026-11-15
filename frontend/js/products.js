let allProductsCache = [];
let allCategoriesCache = [];

document.addEventListener('DOMContentLoaded', async () => {
    const featuredContainer = document.getElementById('featured-products');
    const catalogContainer = document.getElementById('catalog-products');
    const detailContainer = document.getElementById('product-detail-container');

    if (featuredContainer || catalogContainer) {
        await initProductListing(Boolean(featuredContainer));
    }

    if (detailContainer) {
        await initProductDetail();
    }
});

async function initProductListing(isHomePage) {
    try {
        const [categories, products] = await Promise.all([
            api.getCategories(),
            api.getProducts()
        ]);

        allCategoriesCache = (categories || []).filter(c => c.isActive !== false);
        allProductsCache = (products || []).filter(p => p.isActive !== false);

        renderCategoryControls(isHomePage);

        if (isHomePage) {
            renderProductCards(allProductsCache.slice(0, 8), 'featured-products');
        } else {
            applyCatalogFilters();
        }
    } catch (err) {
        const targetId = isHomePage ? 'featured-products' : 'catalog-products';
        const target = document.getElementById(targetId);
        if (target) {
            target.innerHTML = `
                <div class="col-12">
                    <div class="alert alert-danger rounded-4">
                        <i class="fa-solid fa-triangle-exclamation me-2"></i>
                        Không thể tải danh sách sản phẩm từ Gateway (<code>http://localhost:5258</code>): ${err.message}
                    </div>
                </div>
            `;
        }
    }
}

function renderCategoryControls(isHomePage) {
    if (isHomePage) {
        const pillWrap = document.getElementById('category-pills');
        if (!pillWrap) return;

        allCategoriesCache.forEach(cat => {
            const btn = document.createElement('button');
            btn.className = 'category-pill';
            btn.dataset.category = cat.categoryId;
            btn.textContent = cat.categoryName;
            btn.onclick = () => {
                document.querySelectorAll('#category-pills .category-pill').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                const filtered = allProductsCache.filter(p => p.categoryId === cat.categoryId);
                renderProductCards(filtered, 'featured-products');
            };
            pillWrap.appendChild(btn);
        });

        const allBtn = pillWrap.querySelector('[data-category="all"]');
        if (allBtn) {
            allBtn.onclick = () => {
                document.querySelectorAll('#category-pills .category-pill').forEach(b => b.classList.remove('active'));
                allBtn.classList.add('active');
                renderProductCards(allProductsCache.slice(0, 8), 'featured-products');
            };
        }
        return;
    }

    const sidebarList = document.getElementById('sidebar-categories');
    if (sidebarList) {
        sidebarList.innerHTML = `
            <button class="list-group-item list-group-item-action active d-flex justify-content-between align-items-center" data-cat="all">
                <span>Tất cả danh mục</span>
                <span class="badge bg-primary rounded-pill">${allProductsCache.length}</span>
            </button>
        `;

        allCategoriesCache.forEach(cat => {
            const count = allProductsCache.filter(p => p.categoryId === cat.categoryId).length;
            const item = document.createElement('button');
            item.className = 'list-group-item list-group-item-action d-flex justify-content-between align-items-center';
            item.dataset.cat = cat.categoryId;
            item.innerHTML = `<span>${cat.categoryName}</span><span class="badge bg-light text-dark border rounded-pill">${count}</span>`;
            sidebarList.appendChild(item);
        });

        sidebarList.querySelectorAll('button').forEach(btn => {
            btn.onclick = () => {
                sidebarList.querySelectorAll('button').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                applyCatalogFilters();
            };
        });
    }

    const params = new URLSearchParams(window.location.search);
    const searchInput = document.getElementById('catalog-search');
    if (searchInput && params.get('search')) {
        searchInput.value = params.get('search');
    }
}

function applyCatalogFilters() {
    const activeCatBtn = document.querySelector('#sidebar-categories button.active');
    const selectedCat = activeCatBtn ? activeCatBtn.dataset.cat : 'all';
    const keyword = (document.getElementById('catalog-search')?.value || '').trim().toLowerCase();
    const sortValue = document.getElementById('catalog-sort')?.value || 'default';
    const onlyInStock = document.getElementById('filter-instock')?.checked || false;

    let result = [...allProductsCache];

    if (selectedCat !== 'all') {
        result = result.filter(p => p.categoryId === selectedCat);
    }
    if (keyword) {
        result = result.filter(p =>
            (p.productName || p.name || '').toLowerCase().includes(keyword) ||
            (p.description || '').toLowerCase().includes(keyword)
        );
    }
    if (onlyInStock) {
        result = result.filter(p => (p.stock || 0) > 0);
    }

    if (sortValue === 'price-asc') {
        result.sort((a, b) => a.price - b.price);
    } else if (sortValue === 'price-desc') {
        result.sort((a, b) => b.price - a.price);
    } else if (sortValue === 'name-asc') {
        result.sort((a, b) => (a.productName || a.name).localeCompare(b.productName || b.name));
    }

    const countEl = document.getElementById('catalog-result-count');
    if (countEl) countEl.textContent = `Hiển thị ${result.length} sản phẩm`;

    renderProductCards(result, 'catalog-products');
}

function renderProductCards(products, containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;

    if (!products || products.length === 0) {
        container.innerHTML = `
            <div class="col-12 text-center py-5">
                <i class="fa-solid fa-box-open fa-3x text-muted mb-3"></i>
                <h5 class="fw-bold">Không tìm thấy sản phẩm phù hợp</h5>
                <p class="text-muted small">Hãy thử thay đổi bộ lọc hoặc từ khóa tìm kiếm khác.</p>
            </div>
        `;
        return;
    }

    const isSubPage = window.location.pathname.includes('/pages/');
    const detailBase = isSubPage ? 'product-detail.html' : 'pages/product-detail.html';

    container.innerHTML = products.map(p => {
        const name = p.productName || p.name || 'Sản phẩm';
        const cat = allCategoriesCache.find(c => c.categoryId === p.categoryId);
        const catName = cat ? cat.categoryName : 'Sản phẩm';
        const imgSrc = resolveProductImage(p.imageUrl, name);
        const fallbackSrc = getProductFallbackImage(name);
        const outOfStock = (p.stock || 0) <= 0;

        return `
            <div class="col-xl-3 col-lg-4 col-sm-6">
                <div class="product-card">
                    <a href="${detailBase}?id=${p.productId}" class="product-img-wrap text-decoration-none">
                        <span class="stock-badge">${outOfStock ? 'Hết hàng' : `Còn ${p.stock} sp`}</span>
                        <img src="${imgSrc}" alt="${name}" onerror="this.onerror=null;this.src='${fallbackSrc}'">
                    </a>
                    <div class="product-body">
                        <div class="product-category-tag">${catName}</div>
                        <a href="${detailBase}?id=${p.productId}" class="product-title">${name}</a>
                        <p class="text-muted small mb-3 flex-grow-1" style="display:-webkit-box;-webkit-line-clamp:2;-webkit-box-orient:vertical;overflow:hidden;">
                            ${p.description || 'Sản phẩm chính hãng phân phối bởi ShopMini.'}
                        </p>
                        <div class="d-flex justify-content-between align-items-center pt-2 border-top">
                            <span class="product-price">${formatCurrency(p.price)}</span>
                            <button class="btn btn-primary-modern btn-sm px-3" ${outOfStock ? 'disabled' : ''} onclick="quickAddToCart('${p.productId}')">
                                <i class="fa-solid fa-cart-plus me-1"></i> Thêm
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }).join('');
}

async function quickAddToCart(productId, quantity = 1) {
    const user = auth.getCurrentUser();
    if (!user) {
        showToast('Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!', 'warning');
        setTimeout(() => auth.requireAuth(), 900);
        return;
    }

    try {
        await api.addToCart(user.userId, productId, quantity);
        await updateCartBadge();
        showToast('Đã thêm sản phẩm vào giỏ hàng!', 'success');
    } catch (err) {
        showToast(err.message || 'Không thể thêm vào giỏ hàng', 'danger');
    }
}

async function initProductDetail() {
    const container = document.getElementById('product-detail-container');
    const params = new URLSearchParams(window.location.search);
    const productId = params.get('id');

    if (!productId) {
        container.innerHTML = `<div class="alert alert-warning rounded-4">Thiếu mã sản phẩm trên đường dẫn.</div>`;
        return;
    }

    try {
        const [product, categories] = await Promise.all([
            api.getProductById(productId),
            api.getCategories()
        ]);

        const name = product.productName || product.name;
        const cat = (categories || []).find(c => c.categoryId === product.categoryId);
        const imgSrc = resolveProductImage(product.imageUrl, name);
        const fallbackSrc = getProductFallbackImage(name);
        const maxStock = product.stock || 0;

        document.title = `${name} — ShopMini`;
        const breadcrumbName = document.getElementById('breadcrumb-product-name');
        if (breadcrumbName) breadcrumbName.textContent = name;

        container.innerHTML = `
            <div class="card border-0 shadow-sm rounded-4 p-4">
                <div class="row g-4 align-items-center">
                    <div class="col-lg-5">
                        <div class="rounded-4 overflow-hidden bg-light border d-flex align-items-center justify-content-center" style="aspect-ratio:4/3;">
                            <img src="${imgSrc}" alt="${name}" class="w-100 h-100 object-fit-cover" onerror="this.onerror=null;this.src='${fallbackSrc}'">
                        </div>
                    </div>
                    <div class="col-lg-7">
                        <span class="badge bg-primary-subtle text-primary px-3 py-2 rounded-pill fw-bold mb-2">
                            ${cat ? cat.categoryName : 'Danh mục sản phẩm'}
                        </span>
                        <h2 class="fw-bold mb-2">${name}</h2>
                        <div class="d-flex align-items-center gap-3 mb-3">
                            <span class="fs-2 fw-extrabold text-warning" style="color: var(--accent) !important;">${formatCurrency(product.price)}</span>
                            <span class="badge ${maxStock > 0 ? 'bg-success-subtle text-success' : 'bg-danger-subtle text-danger'} px-3 py-2">
                                ${maxStock > 0 ? `Sẵn hàng (${maxStock} sản phẩm)` : 'Tạm hết hàng'}
                            </span>
                        </div>
                        <p class="text-muted mb-4">${product.description || 'Sản phẩm chính hãng chất lượng cao tại ShopMini.'}</p>

                        <div class="d-flex align-items-center gap-3 mb-4">
                            <label class="fw-semibold">Số lượng:</label>
                            <div class="input-group" style="width: 140px;">
                                <button class="btn btn-outline-secondary" type="button" onclick="adjustDetailQty(-1, ${maxStock})">-</button>
                                <input type="number" id="detail-qty" class="form-control text-center fw-bold" value="1" min="1" max="${maxStock}">
                                <button class="btn btn-outline-secondary" type="button" onclick="adjustDetailQty(1, ${maxStock})">+</button>
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-3">
                            <button class="btn btn-primary-modern btn-lg px-4" ${maxStock <= 0 ? 'disabled' : ''} onclick="handleDetailAddCart('${product.productId}', false)">
                                <i class="fa-solid fa-cart-plus me-2"></i>Thêm vào giỏ hàng
                            </button>
                            <button class="btn btn-dark btn-lg px-4 rounded-3" ${maxStock <= 0 ? 'disabled' : ''} onclick="handleDetailAddCart('${product.productId}', true)">
                                Mua ngay
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    } catch (err) {
        container.innerHTML = `<div class="alert alert-danger rounded-4">Lỗi tải chi tiết sản phẩm: ${err.message}</div>`;
    }
}

function adjustDetailQty(delta, maxStock) {
    const input = document.getElementById('detail-qty');
    if (!input) return;
    let val = parseInt(input.value || '1', 10) + delta;
    if (val < 1) val = 1;
    if (maxStock > 0 && val > maxStock) val = maxStock;
    input.value = val;
}

async function handleDetailAddCart(productId, redirectCheckout) {
    const qty = parseInt(document.getElementById('detail-qty')?.value || '1', 10);
    await quickAddToCart(productId, qty);
    if (redirectCheckout && auth.getCurrentUser()) {
        window.location.href = 'cart.html';
    }
}

window.applyCatalogFilters = applyCatalogFilters;
window.quickAddToCart = quickAddToCart;
window.adjustDetailQty = adjustDetailQty;
window.handleDetailAddCart = handleDetailAddCart;
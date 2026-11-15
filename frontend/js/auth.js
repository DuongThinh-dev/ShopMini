const AUTH_STORAGE_KEY = 'shopmini_current_user';

const auth = {
    getCurrentUser() {
        try {
            const raw = localStorage.getItem(AUTH_STORAGE_KEY);
            return raw ? JSON.parse(raw) : null;
        } catch {
            return null;
        }
    },

    setCurrentUser(user) {
        localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(user));
    },

    isAdmin() {
        const user = this.getCurrentUser();
        if (!user) return false;
        const role = (user.roleName || user.role?.roleName || '').toLowerCase();
        const email = (user.email || '').toLowerCase();
        return role === 'admin' || email === 'admin@shopmini.com' || email === 'admin@gmail.com';
    },

    logout() {
        localStorage.removeItem(AUTH_STORAGE_KEY);
        const isSubPage = window.location.pathname.includes('/pages/');
        window.location.href = isSubPage ? 'login.html' : 'pages/login.html';
    },

    requireAuth() {
        const user = this.getCurrentUser();
        if (!user) {
            const isSubPage = window.location.pathname.includes('/pages/');
            window.location.href = isSubPage ? 'login.html' : 'pages/login.html';
            return null;
        }
        return user;
    },

    requireAdmin() {
        const user = this.requireAuth();
        if (!user) return null;
        if (!this.isAdmin()) {
            const isSubPage = window.location.pathname.includes('/pages/');
            window.location.href = isSubPage ? '../index.html' : 'index.html';
            return null;
        }
        return user;
    },

    async login(email) {
        const [user, roles] = await Promise.all([
            api.getUserByEmail(email.trim()),
            api.getRoles().catch(() => [])
        ]);

        if (!user || user.isActive === false) {
            throw new Error('Tài khoản không tồn tại hoặc đã bị vô hiệu hóa.');
        }

        const userRoleId = String(user.roleId || user.RoleId || '').toLowerCase();
        const matchedRole = (roles || []).find(r => String(r.roleId || r.RoleId || '').toLowerCase() === userRoleId);
        const roleTitle = matchedRole?.roleName || matchedRole?.RoleName;

        const enrichedUser = {
            ...user,
            userId: user.userId || user.UserId,
            fullName: user.fullName || user.FullName,
            email: user.email || user.Email,
            roleId: user.roleId || user.RoleId,
            roleName: user.roleName || roleTitle || (email.toLowerCase().includes('admin') ? 'Admin' : 'Customer')
        };

        this.setCurrentUser(enrichedUser);
        return enrichedUser;
    },

    async register(fullName, email, password) {
        const roles = await api.getRoles();
        const customerRole = (roles || []).find(r => String(r.roleName || r.RoleName || '').toLowerCase() === 'customer') || roles?.[0];
        if (!customerRole) {
            throw new Error('Không tìm thấy quyền Customer trong hệ thống.');
        }

        const roleId = customerRole.roleId || customerRole.RoleId;
        const created = await api.createUser({
            fullName: fullName.trim(),
            email: email.trim(),
            password,
            roleId: roleId
        });

        const enrichedUser = {
            ...created,
            userId: created.userId || created.UserId,
            fullName: created.fullName || created.FullName,
            email: created.email || created.Email,
            roleId: roleId,
            roleName: 'Customer'
        };
        this.setCurrentUser(enrichedUser);
        return enrichedUser;
    }
};

window.auth = auth;
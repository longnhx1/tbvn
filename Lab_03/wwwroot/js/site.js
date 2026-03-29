// ===== Site Scripts - Murasaki Store =====

document.addEventListener('DOMContentLoaded', function () {
    initBackToTop();
    initActiveNavLink();
    initSmoothScroll();
});

// Back to Top Button
function initBackToTop() {
    const backToTopBtn = document.getElementById('backToTop');

    if (backToTopBtn) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 300) {
                backToTopBtn.classList.add('show');
            } else {
                backToTopBtn.classList.remove('show');
            }
        });

        backToTopBtn.addEventListener('click', function () {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }
}

// Active Navigation Link
function initActiveNavLink() {
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href) {
            const linkPath = href.toLowerCase();

            // Check if current path matches link
            if (currentPath === linkPath ||
                (currentPath === '/' && linkPath.includes('/home')) ||
                (currentPath.includes('/ticket') && linkPath.includes('/ticket')) ||
                (currentPath.includes('/categories') && linkPath.includes('/categories'))) {
                link.classList.add('active');
            }
        }
    });
}

// Smooth Scroll for Anchor Links
function initSmoothScroll() {
    const anchors = document.querySelectorAll('a[href^="#"]');

    anchors.forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;

            const target = document.querySelector(targetId);
            if (target) {
                e.preventDefault();
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// Utility: Show Notification
function showNotification(message, type = 'info') {
    const existing = document.querySelector('.site-notification');
    if (existing) existing.remove();

    const notification = document.createElement('div');
    notification.className = `site-notification notification-${type}`;
    notification.innerHTML = `
        <span class="notification-message">${message}</span>
        <button class="notification-close">&times;</button>
    `;

    const colors = {
        success: '#22c55e',
        error: '#ef4444',
        info: '#6366f1',
        warning: '#f59e0b'
    };

    Object.assign(notification.style, {
        position: 'fixed',
        top: '20px',
        right: '20px',
        padding: '14px 20px',
        background: colors[type],
        color: '#ffffff',
        borderRadius: '8px',
        display: 'flex',
        alignItems: 'center',
        gap: '15px',
        boxShadow: '0 4px 15px rgba(0,0,0,0.2)',
        zIndex: '10000',
        animation: 'slideInRight 0.3s ease'
    });

    // Add animation
    if (!document.getElementById('notificationStyle')) {
        const style = document.createElement('style');
        style.id = 'notificationStyle';
        style.textContent = `
            @keyframes slideInRight {
                from { transform: translateX(100%); opacity: 0; }
                to { transform: translateX(0); opacity: 1; }
            }
            .notification-close {
                background: none;
                border: none;
                color: white;
                font-size: 20px;
                cursor: pointer;
                padding: 0;
                line-height: 1;
            }
        `;
        document.head.appendChild(style);
    }

    document.body.appendChild(notification);

    // Close button
    notification.querySelector('.notification-close').addEventListener('click', () => {
        notification.remove();
    });

    // Auto remove
    setTimeout(() => {
        if (notification.parentNode) {
            notification.style.animation = 'slideInRight 0.3s ease reverse';
            setTimeout(() => notification.remove(), 300);
        }
    }, 4000);
}

// Export for global use
window.showNotification = showNotification;
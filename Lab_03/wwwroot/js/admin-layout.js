// ============================================================================
// ADMIN LAYOUT INTERACTIONS
// Sidebar toggle, submenu collapse/expand, and responsive behavior
// ============================================================================

(function () {
    'use strict';

    // Elements
    var sidebarToggleBtn = document.getElementById('sidebarToggleBtn');
    var adminSidebar = document.getElementById('adminSidebar');
    var parentMenuBtns = document.querySelectorAll('.admin-nav__parent');

    // ===== SIDEBAR TOGGLE (Mobile) =====
    if (sidebarToggleBtn && adminSidebar) {
        sidebarToggleBtn.addEventListener('click', function () {
            adminSidebar.classList.toggle('open');
            var isOpen = adminSidebar.classList.contains('open');
            sidebarToggleBtn.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
        });

        // Close sidebar when clicking a link
        var sidebarLinks = adminSidebar.querySelectorAll('a');
        sidebarLinks.forEach(function (link) {
            link.addEventListener('click', function () {
                // Only close on mobile
                if (window.innerWidth < 768) {
                    adminSidebar.classList.remove('open');
                    sidebarToggleBtn.setAttribute('aria-expanded', 'false');
                }
            });
        });
    }

    // ===== SUBMENU TOGGLE =====
    parentMenuBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var submenuId = this.getAttribute('data-submenu');
            var submenu = document.getElementById('submenu-' + submenuId);
            var chevron = this.querySelector('.admin-nav__chevron');

            if (submenu) {
                submenu.classList.toggle('open');
                submenu.setAttribute('aria-hidden', submenu.classList.contains('open') ? 'false' : 'true');

                if (chevron) {
                    chevron.classList.toggle('open');
                }
            }
        });
    });

    // ===== CLOSE SIDEBAR ON OUTSIDE CLICK (Mobile) =====
    if (window.innerWidth < 768) {
        document.addEventListener('click', function (e) {
            var isClickInsideSidebar = adminSidebar.contains(e.target);
            var isClickOnToggle = sidebarToggleBtn && sidebarToggleBtn.contains(e.target);

            if (!isClickInsideSidebar && !isClickOnToggle && adminSidebar.classList.contains('open')) {
                adminSidebar.classList.remove('open');
                if (sidebarToggleBtn) {
                    sidebarToggleBtn.setAttribute('aria-expanded', 'false');
                }
            }
        });
    }

    // ===== HANDLE WINDOW RESIZE =====
    var resizeTimer;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            if (window.innerWidth >= 768) {
                // Desktop: ensure sidebar is not hidden
                adminSidebar.classList.remove('open');
            }
        }, 250);
    });

})();

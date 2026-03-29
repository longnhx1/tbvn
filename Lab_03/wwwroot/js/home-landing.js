/**
 * home-landing.js — trang chủ khách hàng
 * - Navbar sticky shadow on scroll
 * - Hamburger mobile menu toggle (nếu có)
 * - Smooth reveal on scroll
 */

(function () {
    'use strict';

    const navbar = document.getElementById('mainNavbar');

    function handleScroll() {
        if (!navbar) return;
        if (window.scrollY > 8) {
            navbar.classList.add('navbar--scrolled');
        } else {
            navbar.classList.remove('navbar--scrolled');
        }
    }

    window.addEventListener('scroll', handleScroll, { passive: true });
    handleScroll();

    const hamburgerBtn = document.getElementById('hamburgerBtn');
    const mobileMenu = document.getElementById('mobileMenu');

    if (hamburgerBtn && mobileMenu) {
        hamburgerBtn.addEventListener('click', function () {
            const isOpen = mobileMenu.classList.toggle('is-open');
            hamburgerBtn.setAttribute('aria-expanded', isOpen.toString());
            mobileMenu.setAttribute('aria-hidden', (!isOpen).toString());
        });

        mobileMenu.querySelectorAll('a').forEach(function (link) {
            link.addEventListener('click', function () {
                mobileMenu.classList.remove('is-open');
                hamburgerBtn.setAttribute('aria-expanded', 'false');
                mobileMenu.setAttribute('aria-hidden', 'true');
            });
        });

        document.addEventListener('click', function (e) {
            if (
                mobileMenu.classList.contains('is-open') &&
                !navbar.contains(e.target)
            ) {
                mobileMenu.classList.remove('is-open');
                hamburgerBtn.setAttribute('aria-expanded', 'false');
                mobileMenu.setAttribute('aria-hidden', 'true');
            }
        });
    }

    const revealEls = document.querySelectorAll(
        '.ticket-card, .category-card, .dest-card, .hero__content'
    );

    if ('IntersectionObserver' in window && revealEls.length > 0) {
        const observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });

        revealEls.forEach(function (el) {
            el.style.opacity = '0';
            el.style.transform = 'translateY(20px)';
            el.style.transition = 'opacity 0.45s ease, transform 0.45s ease';
            observer.observe(el);
        });
    }
})();

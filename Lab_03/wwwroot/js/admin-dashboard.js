/**
 * Admin Dashboard — Travel Booking Việt Nam
 * Handles: counter animations, live clock, activity refresh
 */

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        initCounterAnimations();
        initTimeUpdater();
        initActivityRefresh();
        initCardHover();
    });

    // ===== Animated Counter =====
    function initCounterAnimations() {
        var counters = document.querySelectorAll('[data-counter]');

        counters.forEach(function (counter) {
            var raw = counter.getAttribute('data-counter');
            var target = parseInt(raw, 10) || 0;
            var isCurrency = counter.classList.contains('stat-card__value--currency');
            var duration = 1400;
            var startTime = null;

            function easeOutQuart(t) {
                return 1 - Math.pow(1 - t, 4);
            }

            function formatValue(val) {
                if (isCurrency) {
                    // Format as VND shorthand e.g. 125.400.000
                    return val.toLocaleString('vi-VN');
                }
                return val.toLocaleString('vi-VN');
            }

            function animate(currentTime) {
                if (!startTime) startTime = currentTime;
                var elapsed = currentTime - startTime;
                var progress = Math.min(elapsed / duration, 1);
                var eased = easeOutQuart(progress);
                var current = Math.floor(target * eased);

                counter.textContent = formatValue(current);

                if (progress < 1) {
                    requestAnimationFrame(animate);
                } else {
                    counter.textContent = formatValue(target);
                }
            }

            // Only animate when card enters viewport
            var observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        // Small delay so animation starts after card fade-in
                        setTimeout(function () {
                            requestAnimationFrame(animate);
                        }, 100);
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.25 });

            observer.observe(counter);
        });
    }

    // ===== Live Time (vi-VN locale) =====
    function initTimeUpdater() {
        var el = document.getElementById('currentTime');
        if (!el) return;

        function update() {
            var now = new Date();
            el.textContent = now.toLocaleDateString('vi-VN', {
                weekday: 'short',
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            });
        }

        update();
        setInterval(update, 30000);
    }

    // ===== Activity Refresh =====
    function initActivityRefresh() {
        var btn = document.getElementById('refreshActivity');
        var list = document.querySelector('.activity-list');
        if (!btn || !list) return;

        btn.addEventListener('click', function () {
            btn.classList.add('is-loading');
            btn.disabled = true;

            // In production: replace with fetch('/admin/api/activity')
            setTimeout(function () {
                var items = list.querySelectorAll('.activity-item');
                items.forEach(function (item, i) {
                    item.style.animation = 'none';
                    void item.offsetHeight; // force reflow
                    item.style.animation = 'fadeSlideUp 0.35s ease backwards';
                    item.style.animationDelay = (i * 0.055) + 's';
                });

                btn.classList.remove('is-loading');
                btn.disabled = false;
            }, 750);
        });
    }

    // ===== Stat Card Hover — will-change optimization =====
    function initCardHover() {
        document.querySelectorAll('.stat-card, .quick-action').forEach(function (el) {
            el.addEventListener('mouseenter', function () {
                this.style.willChange = 'transform, box-shadow';
            });
            el.addEventListener('mouseleave', function () {
                this.style.willChange = 'auto';
            });
        });
    }

})();

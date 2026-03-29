/**
 * Admin Ticket Display Page JavaScript
 * Handles image lightbox and copy link functionality
 */

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        initLightbox();
        initCopyLink();
    }

    // ===== Image Lightbox =====
    function initLightbox() {
        var zoomBtn = document.getElementById('zoomImageBtn');
        var lightbox = document.getElementById('imageLightbox');
        var closeBtn = document.getElementById('closeLightbox');

        if (!lightbox) return;

        // Open lightbox
        if (zoomBtn) {
            zoomBtn.addEventListener('click', function () {
                lightbox.classList.add('active');
                document.body.style.overflow = 'hidden';
            });
        }

        // Close lightbox
        if (closeBtn) {
            closeBtn.addEventListener('click', closeLightbox);
        }

        // Close on background click
        lightbox.addEventListener('click', function (e) {
            if (e.target === lightbox) {
                closeLightbox();
            }
        });

        // Close on Escape key
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && lightbox.classList.contains('active')) {
                closeLightbox();
            }
        });

        function closeLightbox() {
            lightbox.classList.remove('active');
            document.body.style.overflow = '';
        }
    }

    // ===== Copy Link =====
    function initCopyLink() {
        var copyBtn = document.getElementById('copyLinkBtn');
        if (!copyBtn) return;

        copyBtn.addEventListener('click', function () {
            var url = window.location.origin + copyBtn.dataset.url;

            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(url).then(function () {
                    showToast('Da sao chep lien ket!');
                }).catch(function () {
                    fallbackCopy(url);
                });
            } else {
                fallbackCopy(url);
            }
        });

        function fallbackCopy(text) {
            var textarea = document.createElement('textarea');
            textarea.value = text;
            textarea.style.position = 'fixed';
            textarea.style.opacity = '0';
            document.body.appendChild(textarea);
            textarea.select();
            try {
                document.execCommand('copy');
                showToast('Da sao chep lien ket!');
            } catch (err) {
                showToast('Khong the sao chep!');
            }
            document.body.removeChild(textarea);
        }

        function showToast(message) {
            // Remove existing toast
            var existingToast = document.querySelector('.copy-toast');
            if (existingToast) {
                existingToast.remove();
            }

            // Create toast
            var toast = document.createElement('div');
            toast.className = 'copy-toast';
            toast.innerHTML = '<i class="fa-solid fa-check"></i> ' + message;
            toast.style.cssText = [
                'position: fixed',
                'bottom: 2rem',
                'left: 50%',
                'transform: translateX(-50%) translateY(20px)',
                'background: #1d1d1f',
                'color: #ffffff',
                'padding: 0.75rem 1.5rem',
                'border-radius: 8px',
                'font-size: 0.9375rem',
                'font-weight: 500',
                'z-index: 10000',
                'opacity: 0',
                'transition: all 0.3s ease',
                'display: flex',
                'align-items: center',
                'gap: 0.5rem'
            ].join(';');

            document.body.appendChild(toast);

            // Animate in
            requestAnimationFrame(function () {
                toast.style.opacity = '1';
                toast.style.transform = 'translateX(-50%) translateY(0)';
            });

            // Remove after delay
            setTimeout(function () {
                toast.style.opacity = '0';
                toast.style.transform = 'translateX(-50%) translateY(20px)';
                setTimeout(function () {
                    toast.remove();
                }, 300);
            }, 2500);
        }
    }

})();

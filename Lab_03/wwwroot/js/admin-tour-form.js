/**
 * Admin Ticket Form JavaScript
 * Handles image upload preview, drag & drop, and form reset
 */

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        initImageUpload();
        initFormReset();
    });

    /**
     * Initialize image upload functionality
     */
    function initImageUpload() {
        var imageInput = document.getElementById('imageInput');
        var imageUpload = document.getElementById('imageUpload');
        var uploadPlaceholder = document.getElementById('uploadPlaceholder');
        var previewContainer = document.getElementById('imagePreviewContainer');
        var previewImage = document.getElementById('imagePreview');
        var removeBtn = document.getElementById('removeImage');

        if (!imageInput || !imageUpload) return;

        // Handle file input change
        imageInput.addEventListener('change', function (e) {
            handleFileSelect(e.target.files[0]);
        });

        // Handle drag & drop
        imageUpload.addEventListener('dragover', function (e) {
            e.preventDefault();
            e.stopPropagation();
            imageUpload.classList.add('dragover');
        });

        imageUpload.addEventListener('dragleave', function (e) {
            e.preventDefault();
            e.stopPropagation();
            imageUpload.classList.remove('dragover');
        });

        imageUpload.addEventListener('drop', function (e) {
            e.preventDefault();
            e.stopPropagation();
            imageUpload.classList.remove('dragover');

            var files = e.dataTransfer.files;
            if (files.length > 0 && files[0].type.startsWith('image/')) {
                imageInput.files = files;
                handleFileSelect(files[0]);
            }
        });

        // Handle remove image
        if (removeBtn) {
            removeBtn.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                resetImageUpload();
            });
        }

        /**
         * Handle file selection and show preview
         */
        function handleFileSelect(file) {
            if (!file) {
                resetImageUpload();
                return;
            }

            // Validate file type
            if (!file.type.startsWith('image/')) {
                alert('Vui lòng chọn file hình ảnh (PNG, JPG, WEBP)');
                resetImageUpload();
                return;
            }

            // Validate file size (5MB max)
            var maxSize = 5 * 1024 * 1024;
            if (file.size > maxSize) {
                alert('File quá lớn. Vui lòng chọn file nhỏ hơn 5MB');
                resetImageUpload();
                return;
            }

            // Show preview
            var reader = new FileReader();
            reader.onload = function (e) {
                previewImage.src = e.target.result;
                uploadPlaceholder.classList.add('d-none');
                previewContainer.classList.remove('d-none');
            };
            reader.readAsDataURL(file);
        }

        /**
         * Reset image upload to initial state
         */
        function resetImageUpload() {
            imageInput.value = '';
            previewImage.src = '#';
            uploadPlaceholder.classList.remove('d-none');
            previewContainer.classList.add('d-none');
        }

        // Expose reset function globally
        window.resetImageUpload = resetImageUpload;
    }

    /**
     * Initialize form reset functionality
     */
    function initFormReset() {
        var resetBtn = document.getElementById('btnReset');
        var form = document.getElementById('productForm');

        if (!resetBtn || !form) return;

        resetBtn.addEventListener('click', function () {
            // Reset form fields
            form.reset();

            // Reset image upload
            if (typeof window.resetImageUpload === 'function') {
                window.resetImageUpload();
            }

            // Clear validation errors
            var errors = form.querySelectorAll('.form-error');
            errors.forEach(function (el) {
                el.textContent = '';
            });

            // Remove validation classes
            var inputs = form.querySelectorAll('.form-input, .form-select, .form-textarea');
            inputs.forEach(function (el) {
                el.classList.remove('input-validation-error');
            });

            // Focus first input
            var firstInput = form.querySelector('.form-input');
            if (firstInput) {
                firstInput.focus();
            }
        });
    }

})();

/**
 * Category Form Handler
 * Handles form validation, styling, and interactions
 */

class CategoryForm {
    constructor() {
        this.form = document.querySelector('.category-form');
        this.inputs = document.querySelectorAll('.form-input');
        this.submitBtn = document.querySelector('.btn-primary');

        if (!this.form) return;

        this.init();
    }

    init() {
        // Add event listeners to inputs
        this.inputs.forEach(input => {
            input.addEventListener('input', (e) => this.handleInput(e));
            input.addEventListener('blur', (e) => this.handleBlur(e));
            input.addEventListener('focus', (e) => this.handleFocus(e));
        });

        // Add form submit listener
        this.form.addEventListener('submit', (e) => this.handleSubmit(e));

        // Initialize validation for pre-filled values
        this.validateAllInputs();
    }

    handleInput(event) {
        const input = event.target;

        // Remove error styling on input
        if (input.classList.contains('error')) {
            input.classList.remove('error');
            const errorElement = input.parentElement.querySelector('.form-error');
            if (errorElement) {
                errorElement.classList.remove('show');
            }
        }

        // Validate input as user types
        this.validateInput(input);
    }

    handleBlur(event) {
        const input = event.target;
        this.validateInput(input);
    }

    handleFocus(event) {
        const input = event.target;
        // Clear error message on focus
        const errorElement = input.parentElement.querySelector('.form-error');
        if (errorElement && errorElement.classList.contains('show')) {
            errorElement.classList.remove('show');
        }
    }

    validateInput(input) {
        const value = input.value.trim();
        const errorElement = input.parentElement.querySelector('.form-error');

        // Check if input is required (Name field)
        if (input.hasAttribute('required') || input.name === 'Name') {
            if (!value) {
                this.showError(input, errorElement, 'Category name is required');
                return false;
            }

            // Check minimum length
            if (value.length < 2) {
                this.showError(input, errorElement, 'Category name must be at least 2 characters');
                return false;
            }

            // Check maximum length
            if (value.length > 100) {
                this.showError(input, errorElement, 'Category name must not exceed 100 characters');
                return false;
            }

            // Check for invalid characters
            if (!/^[a-zA-Z0-9\s\-&().,]+$/.test(value)) {
                this.showError(input, errorElement, 'Category name contains invalid characters');
                return false;
            }

            // Valid input
            input.classList.remove('error');
            if (errorElement) {
                errorElement.classList.remove('show');
            }
            input.classList.add('success');
            return true;
        }

        return true;
    }

    showError(input, errorElement, message) {
        input.classList.add('error');
        input.classList.remove('success');

        if (errorElement) {
            errorElement.textContent = message;
            errorElement.classList.add('show');
        }
    }

    validateAllInputs() {
        let isValid = true;

        this.inputs.forEach(input => {
            if (!this.validateInput(input)) {
                isValid = false;
            }
        });

        return isValid;
    }

    handleSubmit(event) {
        // Validate all inputs before submission
        if (!this.validateAllInputs()) {
            event.preventDefault();
            this.showSubmitError('Please fix the errors above');
            return;
        }

        // Disable form during submission
        this.form.classList.add('form-loading');
        this.submitBtn.disabled = true;
    }

    showSubmitError(message) {
        // Create or update error message container
        let errorContainer = this.form.querySelector('.form-submit-error');

        if (!errorContainer) {
            errorContainer = document.createElement('div');
            errorContainer.className = 'form-submit-error';
            this.form.insertBefore(errorContainer, this.form.firstChild);
        }

        errorContainer.textContent = message;
        errorContainer.style.display = 'block';
    }

    clearSubmitError() {
        const errorContainer = this.form.querySelector('.form-submit-error');
        if (errorContainer) {
            errorContainer.style.display = 'none';
        }
    }
}

// Initialize form when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new CategoryForm();
});

/**
 * Additional utility functions
 */

// Clear validation error when input value is cleared
function clearValidation(input) {
    if (input.value === '') {
        input.classList.remove('error', 'success');
        const errorElement = input.parentElement.querySelector('.form-error');
        if (errorElement) {
            errorElement.classList.remove('show');
        }
    }
}

// Format input value (trim whitespace)
function formatInputValue(input) {
    input.value = input.value.trim();
}

// Show loading state on button
function setButtonLoading(button, isLoading = true) {
    if (isLoading) {
        button.disabled = true;
        button.innerHTML = '<span class="spinner"></span> Loading...';
    } else {
        button.disabled = false;
        button.textContent = button.getAttribute('data-original-text') || 'Submit';
    }
}

// Display form success message
function showSuccessMessage(message, duration = 3000) {
    const successDiv = document.createElement('div');
    successDiv.className = 'form-success-message';
    successDiv.textContent = message;
    document.body.appendChild(successDiv);

    setTimeout(() => {
        successDiv.remove();
    }, duration);
}

/**
 * Prevent accidental form submission with Enter key on input fields
 * (only allow submit via button click)
 */
document.addEventListener('keypress', (e) => {
    if (e.key === 'Enter' && e.target.classList.contains('form-input')) {
        e.preventDefault();
        const submitBtn = document.querySelector('.btn-primary');
        if (submitBtn) {
            submitBtn.click();
        }
    }
});

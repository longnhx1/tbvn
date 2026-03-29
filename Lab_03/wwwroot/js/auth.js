/* ===== Authentication Pages Scripts ===== */

document.addEventListener('DOMContentLoaded', function () {
    initPasswordStrength();
    initFormValidation();
});

// Toggle Password Visibility
function togglePassword(inputId, button) {
    const input = document.getElementById(inputId);
    const icon = button.querySelector('.eye-icon');

    if (input.type === 'password') {
        input.type = 'text';
        icon.innerHTML = '&#128064;'; // Eye open
    } else {
        input.type = 'password';
        icon.innerHTML = '&#128065;'; // Eye closed
    }
}

// Password Strength Indicator
function initPasswordStrength() {
    const passwordInput = document.getElementById('passwordInput');

    if (passwordInput && document.getElementById('registerForm')) {
        const strengthBar = document.createElement('div');
        strengthBar.className = 'password-strength';
        strengthBar.innerHTML = `
            <div class="strength-bar">
                <div class="strength-fill"></div>
            </div>
            <span class="strength-text"></span>
        `;
        passwordInput.closest('.form-group').appendChild(strengthBar);

        passwordInput.addEventListener('input', function () {
            const password = this.value;
            const strength = calculateStrength(password);
            const fill = strengthBar.querySelector('.strength-fill');
            const text = strengthBar.querySelector('.strength-text');

            fill.className = 'strength-fill';

            if (password.length === 0) {
                fill.style.width = '0%';
                text.textContent = '';
            } else if (strength < 30) {
                fill.style.width = '33%';
                fill.classList.add('strength-weak');
                text.textContent = 'Yếu';
                text.style.color = '#ef4444';
            } else if (strength < 60) {
                fill.style.width = '66%';
                fill.classList.add('strength-medium');
                text.textContent = 'Trung bình';
                text.style.color = '#f59e0b';
            } else {
                fill.style.width = '100%';
                fill.classList.add('strength-strong');
                text.textContent = 'Mạnh';
                text.style.color = '#22c55e';
            }
        });
    }
}

function calculateStrength(password) {
    let strength = 0;
    if (password.length >= 8) strength += 25;
    if (password.match(/[a-z]/)) strength += 15;
    if (password.match(/[A-Z]/)) strength += 20;
    if (password.match(/[0-9]/)) strength += 20;
    if (password.match(/[^a-zA-Z0-9]/)) strength += 20;
    return strength;
}

// Form Validation Enhancement
function initFormValidation() {
    const forms = document.querySelectorAll('.auth-form');

    forms.forEach(function (form) {
        const inputs = form.querySelectorAll('.form-control');

        inputs.forEach(function (input) {
            input.addEventListener('blur', function () {
                validateInput(this);
            });

            input.addEventListener('input', function () {
                if (this.classList.contains('is-invalid')) {
                    validateInput(this);
                }
            });
        });
    });
}

function validateInput(input) {
    const wrapper = input.closest('.input-wrapper');
    if (!wrapper) return;

    if (input.validity.valid && input.value.trim() !== '') {
        wrapper.classList.remove('is-invalid');
        wrapper.classList.add('is-valid');
    } else if (input.value.trim() === '') {
        wrapper.classList.remove('is-valid', 'is-invalid');
    } else {
        wrapper.classList.remove('is-valid');
        wrapper.classList.add('is-invalid');
    }
}

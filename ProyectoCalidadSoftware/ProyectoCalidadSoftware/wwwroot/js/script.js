// =======================
// auth.js
// =======================

// Variables globales
let captchaText = '';
let loginAttempts = 3;
let isBlocked = false;
let blockTimer = null;

// DOM listo
document.addEventListener('DOMContentLoaded', function () {
    initializeForm();
    generateVisualCaptcha();
});

// Inicializar componentes
function initializeForm() {
    // 1. Tabs (solo si existen)
    const tabButtons = document.querySelectorAll('.tab-btn');
    if (tabButtons.length > 0) {
        tabButtons.forEach(button => {
            button.addEventListener('click', switchForm);
        });
    }

    // 2. Toggle de contraseña
    document.querySelectorAll('.toggle-password').forEach(button => {
        button.addEventListener('click', togglePasswordVisibility);
    });

    // 3. Modal de privacidad
    document.querySelectorAll('.privacy-link').forEach(link => {
        link.addEventListener('click', openPrivacyModal);
    });

    const closeModal = document.querySelector('.close-modal');
    if (closeModal) {
        closeModal.addEventListener('click', closePrivacyModal);
    }

    // 4. Botón "volver" del forgot (si estás en la vista de login unificada)
    const backBtn = document.querySelector('.back-btn');
    if (backBtn) {
        backBtn.addEventListener('click', () => {
            const loginTab = document.querySelector('[data-form="login"]');
            if (loginTab) {
                switchForm({ target: loginTab });
            } else {
                // en páginas separadas, puedes redirigir
                window.location.href = '/Login/Index';
            }
        });
    }

    // 5. Toggle de credenciales demo
    const demoToggle = document.querySelector('.demo-toggle');
    if (demoToggle) {
        demoToggle.addEventListener('click', toggleDemoCredentials);
    }

    // 6. Formularios
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLoginSubmit);
    }

    const registerForm = document.getElementById('register-form');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegisterSubmit);

        // validación de password en registro
        const passwordInput = document.getElementById('register-password');
        if (passwordInput) {
            passwordInput.addEventListener('input', checkPasswordStrength);
        }

        const confirmPasswordInput = document.getElementById('confirm-password');
        if (confirmPasswordInput) {
            confirmPasswordInput.addEventListener('input', validatePasswordMatch);
        }
    }

    const forgotForm = document.getElementById('forgot-form');
    if (forgotForm) {
        forgotForm.addEventListener('submit', handleForgotSubmit);
    }
}

// Cambiar entre formularios (solo si estás usando pestañas en una sola vista)
function switchForm(event) {
    const targetForm = event.target.getAttribute('data-form');
    if (!targetForm) return;

    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');

    document.querySelectorAll('.auth-form').forEach(form => form.classList.remove('active'));
    const selectedForm = document.getElementById(targetForm + '-form');
    if (selectedForm) {
        selectedForm.classList.add('active');
    }

    clearMessages();
}

// Toggle de contraseña
function togglePasswordVisibility(event) {
    const button = event.target.closest('.toggle-password');
    if (!button) return;
    const targetId = button.getAttribute('data-target');
    const input = document.getElementById(targetId);
    if (!input) return;

    if (input.type === 'password') {
        input.type = 'text';
        button.innerHTML = '<i class="fas fa-eye-slash"></i>';
    } else {
        input.type = 'password';
        button.innerHTML = '<i class="fas fa-eye"></i>';
    }
}

// =======================
// CAPTCHA
// =======================
function generateVisualCaptcha() {
    const canvas = document.getElementById('captcha-canvas');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    const width = canvas.width;
    const height = canvas.height;

    ctx.clearRect(0, 0, width, height);

    const gradient = ctx.createLinearGradient(0, 0, width, height);
    gradient.addColorStop(0, 'rgba(147, 197, 253, 0.3)');
    gradient.addColorStop(1, 'rgba(219, 234, 254, 0.3)');
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, width, height);

    captchaText = generateRandomText(6);

    ctx.font = 'bold 24px Arial';
    ctx.fillStyle = '#1e40af';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';

    for (let i = 0; i < captchaText.length; i++) {
        const x = (width / captchaText.length) * (i + 0.5);
        const y = height / 2 + Math.sin(i * 0.5) * 5;
        ctx.save();
        ctx.translate(x, y);
        ctx.rotate((Math.random() - 0.5) * 0.3);
        ctx.fillText(captchaText[i], 0, 0);
        ctx.restore();
    }

    for (let i = 0; i < 5; i++) {
        ctx.strokeStyle = `rgba(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255}, 0.3)`;
        ctx.beginPath();
        ctx.moveTo(Math.random() * width, Math.random() * height);
        ctx.lineTo(Math.random() * width, Math.random() * height);
        ctx.stroke();
    }

    for (let i = 0; i < 50; i++) {
        ctx.fillStyle = `rgba(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255}, 0.5)`;
        ctx.fillRect(Math.random() * width, Math.random() * height, 1, 1);
    }
}

function generateRandomText(length) {
    const chars = 'ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return result;
}

function validateCaptcha() {
    const input = document.getElementById('captcha-answer');
    if (!input) return true; // en registro no hay captcha
    return input.value.toUpperCase() === captchaText.toUpperCase();
}

// =======================
// Credenciales demo
// =======================
function toggleDemoCredentials() {
    const demoList = document.getElementById('demo-credentials');
    const toggleIcon = document.querySelector('.demo-toggle i');

    if (!demoList) return;

    if (demoList.style.display === 'none' || demoList.style.display === '') {
        demoList.style.display = 'block';
        if (toggleIcon) toggleIcon.className = 'fas fa-eye-slash';
    } else {
        demoList.style.display = 'none';
        if (toggleIcon) toggleIcon.className = 'fas fa-eye';
    }
}

// =======================
// LOGIN
// =======================
async function handleLoginSubmit(event) {
    event.preventDefault();

    if (isBlocked) {
        showMessage('Cuenta bloqueada temporalmente. Intente más tarde.', 'error');
        return;
    }

    const formData = new FormData(event.target);
    const username = formData.get('username');
    const password = formData.get('password');
    const privacyAccepted = formData.get('privacy-login') === 'on';

    if (!username || !password) {
        showMessage('Por favor complete todos los campos requeridos.', 'error');
        return;
    }

    if (!privacyAccepted) {
        showMessage('Debe aceptar la Declaratoria de Privacidad.', 'error');
        return;
    }

    if (!validateCaptcha()) {
        showMessage('Captcha incorrecto. Intente nuevamente.', 'error');
        generateVisualCaptcha();
        return;
    }

    const submitBtn = event.target.querySelector('.submit-btn');
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Iniciando...';

    try {
        // aquí llamas a tu acción real
        const response = await fetch('/Portal/LoginAjax', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: new URLSearchParams({
                NombreUsuario: username,
                Clave: password
            })
        });

        const result = await response.json();

        if (response.ok && result.success) {
            showMessage('Inicio de sesión exitoso. Redirigiendo...', 'success');
            setTimeout(() => {
                window.location.href = '/Home/Index';
            }, 1500);
        } else {
            loginAttempts--;
            updateAttemptsDisplay();

            if (loginAttempts <= 0) {
                blockAccount();
            } else {
                showMessage(result.message || 'Usuario o contraseña incorrectos.', 'error');
            }
        }
    } catch (error) {
        console.error('Error:', error);
        showMessage('Error de conexión. Intente nuevamente.', 'error');
    } finally {
        submitBtn.disabled = false;
        submitBtn.innerHTML = '<i class="fas fa-sign-in-alt"></i> Iniciar Sesión';
    }
}

function updateAttemptsDisplay() {
    const attemptsElement = document.getElementById('login-attempts');
    if (attemptsElement) {
        attemptsElement.textContent = `Intentos restantes: ${loginAttempts}`;
    }
}

function blockAccount() {
    isBlocked = true;
    let timeLeft = 60;
    const timerElement = document.getElementById('timer-countdown');
    const timerDisplay = document.getElementById('timer-display');

    if (timerDisplay) {
        timerDisplay.classList.remove('hidden');
    }

    blockTimer = setInterval(() => {
        const minutes = Math.floor(timeLeft / 60);
        const seconds = timeLeft % 60;
        if (timerElement) {
            timerElement.textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;
        }

        timeLeft--;

        if (timeLeft < 0) {
            clearInterval(blockTimer);
            isBlocked = false;
            loginAttempts = 3;
            updateAttemptsDisplay();
            if (timerDisplay) {
                timerDisplay.classList.add('hidden');
            }
            showMessage('Puede intentar iniciar sesión nuevamente.', 'success');
        }
    }, 1000);

    showMessage('Demasiados intentos fallidos. Cuenta bloqueada por 1 minuto.', 'error');
}

// =======================
// REGISTRO
// =======================
async function handleRegisterSubmit(event) {
    event.preventDefault();

    const formData = new FormData(event.target);
    const username = formData.get('username');
    const email = formData.get('email');
    const password = formData.get('password');
    const confirmPassword = formData.get('confirmPassword');
    const recaptchaAccepted = formData.get('recaptcha-check') === 'on';
    const privacyAccepted = formData.get('privacy-register') === 'on';

    if (!username || !email || !password || !confirmPassword) {
        showMessage('Por favor complete todos los campos.', 'error');
        return;
    }

    if (password !== confirmPassword) {
        showMessage('Las contraseñas no coinciden.', 'error');
        return;
    }

    if (!recaptchaAccepted) {
        showMessage('Por favor complete el reCAPTCHA.', 'error');
        return;
    }

    if (!privacyAccepted) {
        showMessage('Debe aceptar la Declaratoria de Privacidad.', 'error');
        return;
    }

    // Aquí iría tu POST real
    showMessage('Registro exitoso. Redirigiendo al inicio de sesión...', 'success');
    setTimeout(() => {
        window.location.href = '/Login/Index';
    }, 2000);
}

// =======================
// Forgot password (solo si existe ese form)
// =======================
async function handleForgotSubmit(event) {
    event.preventDefault();

    const email = event.target.email.value;

    if (!email) {
        showMessage('Por favor ingrese su correo electrónico.', 'error');
        return;
    }

    showMessage('Se ha enviado un enlace de recuperación a su correo.', 'success');
    setTimeout(() => {
        // si hay tabs
        const loginTab = document.querySelector('[data-form="login"]');
        if (loginTab) {
            switchForm({ target: loginTab });
        } else {
            window.location.href = '/Login/Index';
        }
    }, 2000);
}

// =======================
// Mensajes y modal
// =======================
function showMessage(message, type) {
    clearMessages();

    const messageContainer = document.getElementById('message-container');
    if (!messageContainer) return;

    const messageElement = document.createElement('div');
    messageElement.className = `message ${type}`;
    messageElement.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'}"></i>
        ${message}
    `;

    messageContainer.appendChild(messageElement);

    setTimeout(() => {
        if (messageElement.parentNode) {
            messageElement.remove();
        }
    }, 5000);
}

function clearMessages() {
    const messageContainer = document.getElementById('message-container');
    if (messageContainer) {
        messageContainer.innerHTML = '';
    }
}

function openPrivacyModal() {
    const modal = document.getElementById('privacy-modal');
    if (modal) {
        modal.classList.add('show');
    }
}

function closePrivacyModal() {
    const modal = document.getElementById('privacy-modal');
    if (modal) {
        modal.classList.remove('show');
    }
}

document.addEventListener('click', function (event) {
    const modal = document.getElementById('privacy-modal');
    if (modal && event.target === modal) {
        closePrivacyModal();
    }
});

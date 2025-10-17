// Variables globales
let captchaText = '';
let loginAttempts = 3;
let isBlocked = false;
let blockTimer = null;

// Inicialización cuando el DOM está listo
document.addEventListener('DOMContentLoaded', function() {
    initializeForm();
    generateVisualCaptcha();
});

// Inicializar componentes del formulario
function initializeForm() {
    // Tabs de formulario
    const tabButtons = document.querySelectorAll('.tab-btn');
    tabButtons.forEach(button => {
        button.addEventListener('click', switchForm);
    });

    // Toggle de contraseña
    const toggleButtons = document.querySelectorAll('.toggle-password');
    toggleButtons.forEach(button => {
        button.addEventListener('click', togglePasswordVisibility);
    });

    // Modal de privacidad
    const privacyLinks = document.querySelectorAll('.privacy-link');
    privacyLinks.forEach(link => {
        link.addEventListener('click', openPrivacyModal);
    });

    const closeModal = document.querySelector('.close-modal');
    if (closeModal) {
        closeModal.addEventListener('click', closePrivacyModal);
    }

    // Botón de volver en forgot password
    const backBtn = document.querySelector('.back-btn');
    if (backBtn) {
        backBtn.addEventListener('click', () => switchForm({ target: document.querySelector('[data-form="login"]') }));
    }

    // Toggle de credenciales demo
    const demoToggle = document.querySelector('.demo-toggle');
    if (demoToggle) {
        demoToggle.addEventListener('click', toggleDemoCredentials);
    }

    // Envío de formularios
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLoginSubmit);
    }

    const registerForm = document.getElementById('register-form');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegisterSubmit);
    }

    const forgotForm = document.getElementById('forgot-form');
    if (forgotForm) {
        forgotForm.addEventListener('submit', handleForgotSubmit);
    }

    // Validación de contraseña en registro
    const passwordInput = document.getElementById('register-password');
    if (passwordInput) {
        passwordInput.addEventListener('input', checkPasswordStrength);
    }

    // Validación de confirmación de contraseña
    const confirmPasswordInput = document.getElementById('confirm-password');
    if (confirmPasswordInput) {
        confirmPasswordInput.addEventListener('input', validatePasswordMatch);
    }
}

// Cambiar entre formularios
function switchForm(event) {
    const targetForm = event.target.getAttribute('data-form');

    // Remover clase active de todos los tabs
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
    // Agregar clase active al tab clickeado
    event.target.classList.add('active');

    // Ocultar todos los formularios
    document.querySelectorAll('.auth-form').forEach(form => form.classList.remove('active'));
    // Mostrar formulario seleccionado
    const selectedForm = document.getElementById(targetForm + '-form');
    if (selectedForm) {
        selectedForm.classList.add('active');
    }

    // Limpiar mensajes
    clearMessages();
}

// Toggle de visibilidad de contraseña
function togglePasswordVisibility(event) {
    const button = event.target.closest('.toggle-password');
    const targetId = button.getAttribute('data-target');
    const input = document.getElementById(targetId);

    if (input.type === 'password') {
        input.type = 'text';
        button.innerHTML = '<i class="fas fa-eye-slash"></i>';
    } else {
        input.type = 'password';
        button.innerHTML = '<i class="fas fa-eye"></i>';
    }
}

// Generar captcha visual
function generateVisualCaptcha() {
    const canvas = document.getElementById('captcha-canvas');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    const width = canvas.width;
    const height = canvas.height;

    // Limpiar canvas
    ctx.clearRect(0, 0, width, height);

    // Fondo con gradiente
    const gradient = ctx.createLinearGradient(0, 0, width, height);
    gradient.addColorStop(0, 'rgba(147, 197, 253, 0.3)');
    gradient.addColorStop(1, 'rgba(219, 234, 254, 0.3)');
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, width, height);

    // Generar texto aleatorio
    captchaText = generateRandomText(6);

    // Dibujar texto con distorsión
    ctx.font = 'bold 24px Arial';
    ctx.fillStyle = '#1e40af';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';

    // Aplicar distorsión
    for (let i = 0; i < captchaText.length; i++) {
        const x = (width / captchaText.length) * (i + 0.5);
        const y = height / 2 + Math.sin(i * 0.5) * 5;
        ctx.save();
        ctx.translate(x, y);
        ctx.rotate((Math.random() - 0.5) * 0.3);
        ctx.fillText(captchaText[i], 0, 0);
        ctx.restore();
    }

    // Agregar líneas de ruido
    for (let i = 0; i < 5; i++) {
        ctx.strokeStyle = `rgba(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255}, 0.3)`;
        ctx.beginPath();
        ctx.moveTo(Math.random() * width, Math.random() * height);
        ctx.lineTo(Math.random() * width, Math.random() * height);
        ctx.stroke();
    }

    // Agregar puntos de ruido
    for (let i = 0; i < 50; i++) {
        ctx.fillStyle = `rgba(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255}, 0.5)`;
        ctx.fillRect(Math.random() * width, Math.random() * height, 1, 1);
    }
}

// Generar texto aleatorio para captcha
function generateRandomText(length) {
    const chars = 'ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return result;
}

// Validar captcha
function validateCaptcha() {
    const userInput = document.getElementById('captcha-answer').value.toUpperCase();
    return userInput === captchaText.toUpperCase();
}

// Toggle de credenciales demo
function toggleDemoCredentials() {
    const demoList = document.getElementById('demo-credentials');
    const toggleIcon = document.querySelector('.demo-toggle i');

    if (demoList.style.display === 'none' || demoList.style.display === '') {
        demoList.style.display = 'block';
        toggleIcon.className = 'fas fa-eye-slash';
    } else {
        demoList.style.display = 'none';
        toggleIcon.className = 'fas fa-eye';
    }
}

// Verificar fortaleza de contraseña
function checkPasswordStrength() {
    const password = document.getElementById('register-password').value;
    const strengthIndicator = document.getElementById('password-strength');

    if (!strengthIndicator) return;

    let strength = 0;
    let feedback = [];

    if (password.length >= 8) strength++;
    else feedback.push('Al menos 8 caracteres');

    if (/[a-z]/.test(password)) strength++;
    else feedback.push('Una letra minúscula');

    if (/[A-Z]/.test(password)) strength++;
    else feedback.push('Una letra mayúscula');

    if (/[0-9]/.test(password)) strength++;
    else feedback.push('Un número');

    if (/[^A-Za-z0-9]/.test(password)) strength++;
    else feedback.push('Un carácter especial');

    strengthIndicator.className = 'password-strength';

    if (strength < 3) {
        strengthIndicator.classList.add('weak');
        strengthIndicator.textContent = 'Débil: ' + feedback.join(', ');
    } else if (strength < 4) {
        strengthIndicator.classList.add('medium');
        strengthIndicator.textContent = 'Media: Agrega más complejidad';
    } else {
        strengthIndicator.classList.add('strong');
        strengthIndicator.textContent = 'Fuerte: ¡Excelente!';
    }
}

// Validar coincidencia de contraseñas
function validatePasswordMatch() {
    const password = document.getElementById('register-password').value;
    const confirmPassword = document.getElementById('confirm-password').value;
    const confirmInput = document.getElementById('confirm-password');

    if (password !== confirmPassword) {
        confirmInput.setCustomValidity('Las contraseñas no coinciden');
    } else {
        confirmInput.setCustomValidity('');
    }
}

// Manejar envío del formulario de login
async function handleLoginSubmit(event) {
    event.preventDefault();

    if (isBlocked) {
        showMessage('Cuenta bloqueada temporalmente. Intente más tarde.', 'error');
        return;
    }

    const formData = new FormData(event.target);
    const username = formData.get('username');
    const password = formData.get('password');
    const captcha = formData.get('captcha');
    const rememberMe = formData.get('remember-me') === 'on';
    const privacyAccepted = formData.get('privacy-login') === 'on';

    // Validaciones
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

    // Deshabilitar botón
    const submitBtn = event.target.querySelector('.submit-btn');
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Iniciando...';

    try {
        const response = await fetch('/Login/Index', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
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

// Actualizar display de intentos
function updateAttemptsDisplay() {
    const attemptsElement = document.getElementById('login-attempts');
    if (attemptsElement) {
        attemptsElement.textContent = `Intentos restantes: ${loginAttempts}`;
    }
}

// Bloquear cuenta temporalmente
function blockAccount() {
    isBlocked = true;
    let timeLeft = 60; // 1 minuto

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

// Manejar envío del formulario de registro
async function handleRegisterSubmit(event) {
    event.preventDefault();

    const formData = new FormData(event.target);
    const username = formData.get('username');
    const email = formData.get('email');
    const password = formData.get('password');
    const confirmPassword = formData.get('confirmPassword');
    const recaptchaAccepted = formData.get('recaptcha-check') === 'on';
    const privacyAccepted = formData.get('privacy-register') === 'on';

    // Validaciones
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

    // Aquí iría la lógica de registro
    showMessage('Registro exitoso. Puede iniciar sesión ahora.', 'success');
    setTimeout(() => {
        switchForm({ target: document.querySelector('[data-form="login"]') });
    }, 2000);
}

// Manejar envío del formulario de recuperación
async function handleForgotSubmit(event) {
    event.preventDefault();

    const email = event.target.email.value;

    if (!email) {
        showMessage('Por favor ingrese su correo electrónico.', 'error');
        return;
    }

    // Aquí iría la lógica de recuperación
    showMessage('Se ha enviado un enlace de recuperación a su correo.', 'success');
    setTimeout(() => {
        switchForm({ target: document.querySelector('[data-form="login"]') });
    }, 2000);
}

// Mostrar mensajes
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

    // Auto-remover después de 5 segundos
    setTimeout(() => {
        if (messageElement.parentNode) {
            messageElement.remove();
        }
    }, 5000);
}

// Limpiar mensajes
function clearMessages() {
    const messageContainer = document.getElementById('message-container');
    if (messageContainer) {
        messageContainer.innerHTML = '';
    }
}

// Abrir modal de privacidad
function openPrivacyModal() {
    const modal = document.getElementById('privacy-modal');
    if (modal) {
        modal.classList.add('show');
    }
}

// Cerrar modal de privacidad
function closePrivacyModal() {
    const modal = document.getElementById('privacy-modal');
    if (modal) {
        modal.classList.remove('show');
    }
}

// Cerrar modal al hacer click fuera
document.addEventListener('click', function(event) {
    const modal = document.getElementById('privacy-modal');
    if (modal && event.target === modal) {
        closePrivacyModal();
    }
});
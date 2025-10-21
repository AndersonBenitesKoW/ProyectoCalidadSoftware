class StaffAuthSystem {
  constructor() {
    this.currentForm = "staff-login"
    this.loginAttempts = 3
    this.isBlocked = false
    this.blockTimer = null
    this.captchaText = ""

    this.init()
  }

  init() {
    this.generateStaffCaptcha()
    this.bindEvents()
    this.loadRememberedSession()
  }

  bindEvents() {
    // Form submissions
    document.getElementById("staff-login-form")?.addEventListener("submit", (e) => this.handleStaffLogin(e))
    document.getElementById("support-form")?.addEventListener("submit", (e) => this.handleSupportRequest(e))

    // Password toggle buttons
    document.querySelectorAll(".toggle-password").forEach((btn) => {
      btn.addEventListener("click", (e) => this.togglePassword(e))
    })

    // Support buttons
    document.querySelector(".forgot-password-btn")?.addEventListener("click", () => this.showSupportForm())
    document.querySelector(".contact-admin-btn")?.addEventListener("click", () => this.showSupportForm())
    document.querySelector(".back-btn")?.addEventListener("click", () => this.showStaffLogin())

    // Privacy policy
    document.querySelectorAll(".privacy-link").forEach((link) => {
      link.addEventListener("click", (e) => {
        e.preventDefault()
        this.showPrivacyModal()
      })
    })

    // Modal close
    document.querySelector(".close-modal")?.addEventListener("click", () => this.closeModal())
    document.getElementById("staff-privacy-modal")?.addEventListener("click", (e) => {
      if (e.target.id === "staff-privacy-modal") {
        this.closeModal()
      }
    })

    // Department selection enhancement
    document.getElementById("department")?.addEventListener("change", (e) => {
      this.validateDepartmentAccess(e.target.value)
    })
  }

  generateStaffCaptcha() {
    const canvas = document.getElementById("staff-captcha-canvas")
    if (!canvas) return

    const ctx = canvas.getContext("2d")
    const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    this.captchaText = ""

    // Generate random text
    for (let i = 0; i < 6; i++) {
      this.captchaText += chars.charAt(Math.floor(Math.random() * chars.length))
    }

    // Clear canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height)

    // Background with medical theme
    const gradient = ctx.createLinearGradient(0, 0, canvas.width, canvas.height)
    gradient.addColorStop(0, "#4c5570")
    gradient.addColorStop(0.5, "#738cb6")
    gradient.addColorStop(1, "#93c5fd")
    ctx.fillStyle = gradient
    ctx.fillRect(0, 0, canvas.width, canvas.height)

    // Add noise
    for (let i = 0; i < 100; i++) {
      ctx.fillStyle = `rgba(255, 255, 255, ${Math.random() * 0.3})`
      ctx.fillRect(Math.random() * canvas.width, Math.random() * canvas.height, 2, 2)
    }

    // Draw text with distortion
    ctx.font = "bold 32px Arial"
    ctx.textAlign = "center"
    ctx.textBaseline = "middle"

    for (let i = 0; i < this.captchaText.length; i++) {
      ctx.save()
      ctx.translate(40 + i * 35, 40)
      ctx.rotate((Math.random() - 0.5) * 0.5)
      ctx.fillStyle = "#ffffff"
      ctx.strokeStyle = "#2563eb"
      ctx.lineWidth = 2
      ctx.fillText(this.captchaText[i], 0, 0)
      ctx.strokeText(this.captchaText[i], 0, 0)
      ctx.restore()
    }

    // Add lines
    for (let i = 0; i < 5; i++) {
      ctx.strokeStyle = `rgba(255, 255, 255, ${Math.random() * 0.5 + 0.2})`
      ctx.lineWidth = Math.random() * 3 + 1
      ctx.beginPath()
      ctx.moveTo(Math.random() * canvas.width, Math.random() * canvas.height)
      ctx.lineTo(Math.random() * canvas.width, Math.random() * canvas.height)
      ctx.stroke()
    }
  }

  async handleStaffLogin(e) {
    e.preventDefault()

    if (this.isBlocked) {
      this.showMessage("Sistema bloqueado. Espera a que termine el tiempo.", "error")
      return
    }

    const formData = new FormData(e.target)
    const staffId = formData.get("staffId")
    const password = formData.get("password")
    const department = formData.get("department")
    const captcha = formData.get("captcha")
    const rememberSession = formData.get("remember-session") === "on"

    // Validate captcha
    if (captcha.toUpperCase() !== this.captchaText) {
      this.showMessage("Captcha incorrecto. Inténtalo de nuevo.", "error")
      this.generateStaffCaptcha()
      document.getElementById("staff-captcha-answer").value = ""
      return
    }

    // Validate required fields
    if (!staffId || !password || !department) {
      this.showMessage("Por favor completa todos los campos requeridos.", "error")
      return
    }

    // Simulate staff authentication
    try {
      const isValid = await this.validateStaffCredentials(staffId, password, department)

      if (isValid) {
        if (rememberSession) {
          localStorage.setItem(
            "staffSession",
            JSON.stringify({
              staffId,
              department,
              timestamp: Date.now(),
            }),
          )
        }

        this.showMessage("Acceso autorizado. Redirigiendo al sistema...", "success")

        // Simulate redirect to staff dashboard
        setTimeout(() => {
          window.location.href = `staff-dashboard.html?dept=${department}&id=${staffId}`
        }, 2000)
      } else {
        this.loginAttempts--
        this.updateAttemptsDisplay()

        if (this.loginAttempts <= 0) {
          this.blockAccess()
        } else {
          this.showMessage(`Credenciales incorrectas. Intentos restantes: ${this.loginAttempts}`, "error")
        }

        this.generateStaffCaptcha()
        document.getElementById("staff-captcha-answer").value = ""
      }
    } catch (error) {
      console.error("Error en autenticación:", error)
      this.showMessage("Error del sistema. Contacta al administrador.", "error")
    }
  }

  async validateStaffCredentials(staffId, password, department) {
    // Simulate API call to validate staff credentials
    return new Promise((resolve) => {
      setTimeout(() => {
        // Demo credentials for testing
        const validCredentials = [
          { id: "EMP001", password: "admin123", department: "administracion" },
          { id: "DOC001", password: "doctor123", department: "obstetricia" },
          { id: "ENF001", password: "nurse123", department: "enfermeria" },
          { id: "LAB001", password: "lab123", department: "laboratorio" },
        ]

        const isValid = validCredentials.some(
          (cred) => cred.id === staffId && cred.password === password && cred.department === department,
        )

        resolve(isValid)
      }, 1000)
    })
  }

  validateDepartmentAccess(department) {
    const restrictedDepartments = ["administracion", "sistemas"]

    if (restrictedDepartments.includes(department)) {
      this.showMessage("Departamento con acceso restringido. Se requiere autorización especial.", "warning")
    }
  }

  async handleSupportRequest(e) {
    e.preventDefault()

    const formData = new FormData(e.target)
    const staffId = formData.get("staffId")
    const email = formData.get("email")
    const issue = formData.get("issue")

    if (!staffId || !email || !issue) {
      this.showMessage("Por favor completa todos los campos.", "error")
      return
    }

    try {
      // Simulate sending support request
      await this.sendSupportRequest({ staffId, email, issue })

      this.showMessage("Solicitud enviada correctamente. El administrador se pondrá en contacto contigo.", "success")

      // Reset form and go back to login
      e.target.reset()
      setTimeout(() => {
        this.showStaffLogin()
      }, 2000)
    } catch (error) {
      console.error("Error enviando solicitud:", error)
      this.showMessage("Error al enviar la solicitud. Inténtalo más tarde.", "error")
    }
  }

  async sendSupportRequest(data) {
    // Simulate API call
    return new Promise((resolve) => {
      setTimeout(() => {
        console.log("Solicitud de soporte enviada:", data)
        resolve(true)
      }, 1000)
    })
  }

  showStaffLogin() {
    document.getElementById("staff-login-form").classList.add("active")
    document.getElementById("support-form").classList.remove("active")
  }

  showSupportForm() {
    document.getElementById("staff-login-form").classList.remove("active")
    document.getElementById("support-form").classList.add("active")
  }

  togglePassword(e) {
    const targetId = e.currentTarget.dataset.target
    const input = document.getElementById(targetId)
    const icon = e.currentTarget.querySelector("i")

    if (input.type === "password") {
      input.type = "text"
      icon.classList.remove("fa-eye")
      icon.classList.add("fa-eye-slash")
    } else {
      input.type = "password"
      icon.classList.remove("fa-eye-slash")
      icon.classList.add("fa-eye")
    }
  }

  blockAccess() {
    this.isBlocked = true
    const blockTime = 300 // 5 minutes
    let timeLeft = blockTime

    const timerDisplay = document.getElementById("timer-display")
    const countdown = document.getElementById("timer-countdown")

    timerDisplay.classList.remove("hidden")

    this.blockTimer = setInterval(() => {
      const minutes = Math.floor(timeLeft / 60)
      const seconds = timeLeft % 60
      countdown.textContent = `${minutes.toString().padStart(2, "0")}:${seconds.toString().padStart(2, "0")}`

      timeLeft--

      if (timeLeft < 0) {
        clearInterval(this.blockTimer)
        this.isBlocked = false
        this.loginAttempts = 3
        timerDisplay.classList.add("hidden")
        this.updateAttemptsDisplay()
        this.generateStaffCaptcha()
        this.showMessage("Puedes intentar iniciar sesión nuevamente.", "success")
      }
    }, 1000)
  }

  updateAttemptsDisplay() {
    const attemptsSpan = document.getElementById("staff-attempts")
    if (attemptsSpan) {
      attemptsSpan.textContent = `Intentos restantes: ${this.loginAttempts}`
    }
  }

  loadRememberedSession() {
    const savedSession = localStorage.getItem("staffSession")
    if (savedSession) {
      try {
        const session = JSON.parse(savedSession)
        const hoursPassed = (Date.now() - session.timestamp) / (1000 * 60 * 60)

        if (hoursPassed < 24) {
          // Session valid for 24 hours
          document.getElementById("staff-id").value = session.staffId
          document.getElementById("department").value = session.department
          document.getElementById("remember-session").checked = true
        }
      } catch (error) {
        console.error("Error loading saved session:", error)
        localStorage.removeItem("staffSession")
      }
    }
  }

  showPrivacyModal() {
    const modal = document.getElementById("staff-privacy-modal")
    if (modal) {
      modal.classList.add("active")
    }
  }

  closeModal() {
    const modal = document.getElementById("staff-privacy-modal")
    if (modal) {
      modal.classList.remove("active")
    }
  }

  showMessage(text, type = "info") {
    const container = document.getElementById("message-container")
    if (!container) return

    const message = document.createElement("div")
    message.className = `message ${type}`

    const icon =
      type === "success"
        ? "fa-check-circle"
        : type === "error"
          ? "fa-exclamation-circle"
          : type === "warning"
            ? "fa-exclamation-triangle"
            : "fa-info-circle"

    message.innerHTML = `
            <i class="fas ${icon}"></i>
            <span>${text}</span>
        `

    container.appendChild(message)

    setTimeout(() => {
      message.remove()
    }, 5000)
  }
}

// Global functions for HTML onclick events
function generateStaffCaptcha() {
  if (window.staffAuth) {
    window.staffAuth.generateStaffCaptcha()
  }
}

// Initialize when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
  window.staffAuth = new StaffAuthSystem()
})

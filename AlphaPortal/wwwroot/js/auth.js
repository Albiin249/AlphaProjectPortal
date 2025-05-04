document.addEventListener('DOMContentLoaded', () => {



    // Password toggles
    const passwordInput = document.getElementById("passwordInput")
    const togglePassword = document.getElementById("togglePassword")

    const confirmPasswordInput = document.getElementById("confirmPasswordInput")
    const toggleConfirmPassword = document.getElementById("toggleConfirmPassword")

    togglePassword.addEventListener("click", () => {
        const isShown = togglePassword.getAttribute("data-show") === "true"

        passwordInput.type = isShown ? "password" : "text"
        togglePassword.setAttribute("data-show", !isShown)
        togglePassword.classList.toggle("rotated", !isShown)
    });

    toggleConfirmPassword.addEventListener("click", () => {
        const isShown = toggleConfirmPassword.getAttribute("data-show") === "true"

        confirmPasswordInput.type = isShown ? "password" : "text"
        toggleConfirmPassword.setAttribute("data-show", !isShown)

        toggleConfirmPassword.classList.toggle("rotated", !isShown)

    });
})

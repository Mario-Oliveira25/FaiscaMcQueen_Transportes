using FaiscaMcQueen_Transportes.Models;
using FaiscaMcQueen_Transportes.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FaiscaMcQueen_Transportes.Controllers
{
    public class ContaController : Controller
    {
        // O mordomo do Identity que gere os utilizadores
        private readonly UserManager<ApplicationUser> _userManager;

        public ContaController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // 1. Método GET: Dispara quando o utilizador clica no link do email
        [HttpGet]
        public IActionResult DefinirPassword(string email, string token)
        {
            // Se o link vier incompleto, mandamos para a página inicial
            if (email == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Colocamos o email e a chave na "bandeja" e mandamos para a mesa (View)
            var model = new DefinirPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        // 2. Método POST: Dispara quando o utilizador clica no botão "Guardar"
        [HttpPost]
        public async Task<IActionResult> DefinirPassword(DefinirPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); // Se as passwords não forem iguais, volta a mostrar o formulário

            // Vai à base de dados procurar o dono do email
            var utilizador = await _userManager.FindByEmailAsync(model.Email);
            if (utilizador == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // O ResetPasswordAsync faz a magia toda: valida a chave (Token) e aplica a nova password
            var resultado = await _userManager.ResetPasswordAsync(utilizador, model.Token, model.Password);

            if (resultado.Succeeded)
            {
                // Se correu bem, podes redirecionar para a página de Login (substitui pelo nome do teu controller de login)
                return RedirectToAction("Index", "Home");
            }

            // Se a password for fraca (ex: não tem números), o Identity diz-nos os erros aqui
            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, erro.Description);
            }

            return View(model);
        }
    }
}

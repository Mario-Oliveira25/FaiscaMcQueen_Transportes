using FaiscaMcQueen_Transportes.Data;
using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using FaiscaMcQueen_Transportes.Models;
using FaiscaMcQueen_Transportes.Services;
using FaiscaMcQueen_Transportes.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaiscaMcQueen_Transportes.Controllers
{
    public class TecnicosController : Controller
    {
        private readonly FaiscaMcQueenContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IEmailService _emailService;

        public TecnicosController(FaiscaMcQueenContext context, UserManager<ApplicationUser> usermanager, IEmailService emailService)
        {
            _context = context;
            _usermanager = usermanager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var tecnicos = await _context.Tecnicos.ToListAsync();

            return View(tecnicos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new NovoTecnicoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(NovoTecnicoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var novoTecnico = new Tecnico
                {
                    Id = Guid.NewGuid(),
                    Nome = viewModel.Nome,
                    Nif = viewModel.Nif,
                    DataNascimento = viewModel.DataNascimento,
                    Especialidade = viewModel.Especialidade
                };

                _context.Tecnicos.Add(novoTecnico);
                await _context.SaveChangesAsync();

                var user = new ApplicationUser
                {
                    UserName = viewModel.Nome.Replace(" ", "").ToLower(),
                    Email = viewModel.Email,
                    TecnicoId = novoTecnico.Id,
                    EmailConfirmed = true
                };
                var resultado = await _usermanager.CreateAsync(user);

                if (resultado.Succeeded)
                {
                    await _usermanager.AddToRoleAsync(user, "Utilizador");

                    var token = await _usermanager.GeneratePasswordResetTokenAsync(user);

                    // 2. Construir o link exato para a tua página de DefinirPassword
                    var linkParaEmail = Url.Action(
                        action: "DefinirPassword",
                        controller: "Conta",
                        values: new { email = user.Email, token },
                        protocol: Request.Scheme);

                    // 3. Preparar a carta
                    string assunto = "Bem-vindo! Defina a sua palavra-passe";
                    string mensagem = $"<h2>Olá!</h2>" +
                                      $"<p>O seu perfil de técnico foi criado. Clique no link abaixo para criar a sua palavra-passe e aceder ao sistema:</p>" +
                                      $"<a href='{linkParaEmail}' style='padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>Definir Palavra-passe</a>";

                    // 4. Chamar o teu Estafeta!
                    await _emailService.EnviarEmailAsync(user.Email, assunto, mensagem);

                }
                else if (!resultado.Succeeded)
                {
                    _context.Tecnicos.Remove(novoTecnico);
                    await _context.SaveChangesAsync();
                    return Json(new { 
                        success = false,
                        errors = resultado.Errors.Select(e => e.Description) });
                }

                return RedirectToAction("Details", new { id = novoTecnico.Id });
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null) return RedirectToAction("Index");

            var tecnico = await _context.Tecnicos
                                      .Include(t => t.Intervencoes)
                                      .FirstOrDefaultAsync(t => t.Id == id);

            if (tecnico == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new TecnicoViewModel
            {
                Id = tecnico.Id,
                Nome = tecnico.Nome,
                Nif = tecnico.Nif,
                DataNascimento = tecnico.DataNascimento,
                Especialidade = tecnico.Especialidade,

                ListaIntervencoes = tecnico.Intervencoes
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null) return RedirectToAction("Index");

            var tecnico = await _context.Tecnicos.FirstOrDefaultAsync(t => t.Id == id);

            if (tecnico == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new TecnicoViewModel
            {
                Id = tecnico.Id,
                Nome = tecnico.Nome,
                Nif = tecnico.Nif,
                DataNascimento = tecnico.DataNascimento,
                Especialidade = tecnico.Especialidade
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TecnicoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var tecnico = await _context.Tecnicos.FirstOrDefaultAsync(t => t.Id == viewModel.Id);

                if (tecnico == null) return RedirectToAction("Index");

                tecnico.Nome = viewModel.Nome;
                tecnico.Nif = viewModel.Nif;
                tecnico.DataNascimento = viewModel.DataNascimento;
                tecnico.Especialidade = viewModel.Especialidade;

                _context.Tecnicos.Update(tecnico);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = tecnico.Id });
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null) return RedirectToAction("Index");

            var tecnico = await _context.Tecnicos.FirstOrDefaultAsync(t => t.Id == id);

            if (tecnico == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new TecnicoViewModel
            {
                Id = tecnico.Id,
                Nome = tecnico.Nome,
                Nif = tecnico.Nif
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == null) return RedirectToAction("Index");

            var tecnico = await _context.Tecnicos.FirstOrDefaultAsync(t => t.Id == id);

            if (tecnico != null)
            {
                _context.Tecnicos.Remove(tecnico);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}

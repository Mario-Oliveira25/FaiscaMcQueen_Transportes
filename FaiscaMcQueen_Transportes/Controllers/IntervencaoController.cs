using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using FaiscaMcQueen_Transportes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FaiscaMcQueen_Transportes.Controllers
{
    public class IntervencaoController : Controller
    {

        private readonly Data.FaiscaMcQueenContext _context;

        public IntervencaoController(Data.FaiscaMcQueenContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var intervencoes = await _context.Intervencoes.ToListAsync();
            return View(intervencoes);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intervencao = await _context.Intervencoes.FirstOrDefaultAsync(a => a.Id == id);

            if (intervencao == null)
            {
                return NotFound();
            }

            return View(intervencao);
        }
        [Authorize(Roles = "Chefe de Equipa")]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new RegistoIntervencaoViewModel();

            model.Ativos = _context.Ativos
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Matricula })
                .ToList();

            model.Tecnicos = _context.Tecnicos
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToList();

            return View(model);
        }

        [Authorize(Roles = "Chefe de Equipa")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistoIntervencaoViewModel model)
        {
            // Verificar se o técnico tem alguma intervenção na data pedida
            var intervencaoExistente = await _context.Intervencoes
                .FirstOrDefaultAsync(i => i.TecnicoId == model.TecnicoId &&
                                           i.Data.Date == model.DataIntervencao.Date);

            if (intervencaoExistente != null)
            {
                ModelState.AddModelError("DataIntervencao", "Este técnico já tem uma intervenção agendada para esta data.");

                // Recarregar as listas de seleção
                model.Ativos = _context.Ativos
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Matricula })
                    .ToList();

                model.Tecnicos = _context.Tecnicos
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                    .ToList();

                return View(model);
            }

            if (ModelState.IsValid)
            {
                var intervencao = new Intervencao
                {
                    Id = Guid.NewGuid(),
                    AtivoId = model.AtivoId,
                    TecnicoId = model.TecnicoId,
                    Data = model.DataIntervencao,
                    Descricao = model.Descricao,
                    Estado = model.Estado
                };

                _context.Intervencoes.Add(intervencao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Authorize(Roles = "Chefe de Equipa")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var intervencao = await _context.Intervencoes.FindAsync(id);
            if (intervencao == null)
            {
                return NotFound();
            }
            return View(intervencao);
        }

        [Authorize(Roles = "Chefe de Equipa")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            var intervencao = await _context.Intervencoes.FindAsync(id);
            if (intervencao == null)
            {
                return NotFound();
            }
            _context.Intervencoes.Remove(intervencao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Chefe de Equipa")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var intervencao = await _context.Intervencoes.FindAsync(id);
            if (intervencao == null)
            {
                return NotFound();
            }
            return View(intervencao);
        }
        [Authorize(Roles = "Chefe de Equipa")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Intervencao intervencao)
        {
            if (id != intervencao.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(intervencao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IntervencaoExists(intervencao.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(intervencao);
        }
        public bool IntervencaoExists(Guid id)
        {
            return _context.Intervencoes.Any(e => e.Id == id);

        }
    }
}

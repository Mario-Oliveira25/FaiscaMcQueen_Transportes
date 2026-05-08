using FaiscaMcQueen_Transportes.Data;
using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using FaiscaMcQueen_Transportes.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FaiscaMcQueen_Transportes.Controllers
{
    public class TecnicosController : Controller
    {
        private readonly FaiscaMcQueenContext _context;

        public TecnicosController(FaiscaMcQueenContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tecnicos = await _context.Tecnicos.ToList();

            return View(tecnicos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TecnicoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(TecnicoViewModel viewModel)
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

                return RedirectToAction("Details", new {id = novoTecnico.Id});
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null) return NotFound();

            var tecnico =  await _context.Tecnicos
                                      .Include(t => t.Intervencoes)
                                      .FirstOrDefault(t => t.Id == id);

            if (tecnico == null)
            {
                return NotFound();
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
            if (id == null) return NotFound();

            var tecnico = await _context.Tecnicos.FirstOrDefault(t => t.Id == id);

            if (tecnico == null)
            {
                return NotFound();
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
                var tecnico = await _context.Tecnicos.FirstOrDefault(t => t.Id == viewModel.Id);

                if (tecnico == null) return NotFound();

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
            if (id == null) return NotFound();

            var tecnico = await _context.Tecnicos.FindAsync(id);

            if (tecnico == null)
            {
                return NotFound();
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
            if (id == null) return NotFound();

            var tecnico = await _context.Tecnicos.FindAsync(id);

            if (tecnico != null)
            {
                _context.Tecnicos.Remove(tecnico);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}

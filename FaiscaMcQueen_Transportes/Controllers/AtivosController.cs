using FaiscaMcQueen_Transportes.Data;
using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using FaiscaMcQueen_Transportes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaiscaMcQueen_Transportes.Controllers
{
    public class AtivosController : Controller
    {
        private readonly FaiscaMcQueenContext _context;

        public AtivosController(FaiscaMcQueenContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var ativos = await _context.Ativos.ToListAsync();

            var viewModel = ativos.Select(a => new AtivoViewModel
            {
                Id = a.Id,
                Matricula = a.Matricula,
                Tipo = a.Tipo,
                Marca = a.Marca,
                Modelo = a.Modelo
            }).ToList();



            return View(viewModel);
        }


        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var ativo = await _context.Ativos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ativo == null)
                return NotFound();

            var viewModel = new AtivoViewModel
            {
                Id = ativo.Id,
                Matricula = ativo.Matricula,
                Tipo = ativo.Tipo,
                Marca = ativo.Marca,
                Modelo = ativo.Modelo
            };

            return View(viewModel);
        }


        public IActionResult Create()
        {
            return View(new AtivoViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AtivoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var ativo = new Ativo
                {
                    Id = Guid.NewGuid(),
                    Matricula = viewModel.Matricula,
                    Tipo = viewModel.Tipo,
                    Marca = viewModel.Marca,
                    Modelo = viewModel.Modelo
                };

                _context.Update(ativo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }


        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var ativo = await _context.Ativos.FindAsync(id);

            if (ativo == null)
                return NotFound();

            var viewModel = new AtivoViewModel
            {
                Id = ativo.Id,
                Matricula = ativo.Matricula,
                Tipo = ativo.Tipo,
                Marca = ativo.Marca,
                Modelo = ativo.Modelo
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AtivoViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var ativo = await _context.Ativos.FindAsync(id);
                if (ativo == null)
                    return NotFound();

                ativo.Matricula = viewModel.Matricula;
                ativo.Tipo = viewModel.Tipo;
                ativo.Marca = viewModel.Marca;
                ativo.Modelo = viewModel.Modelo;

                _context.Update(ativo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }


        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var ativo = await _context.Ativos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ativo == null)
                return NotFound();

            var viewModel = new AtivoViewModel
            {
                Id = ativo.Id,
                Matricula = ativo.Matricula,
                Tipo = ativo.Tipo,
                Marca = ativo.Marca,
                Modelo = ativo.Modelo
            };

            return View(viewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ativo = await _context.Ativos.FindAsync(id);

            if (ativo != null)
            {
                _context.Ativos.Remove(ativo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

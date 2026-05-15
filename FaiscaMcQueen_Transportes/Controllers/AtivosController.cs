using FaiscaMcQueen_Transportes.Data;
using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using FaiscaMcQueen_Transportes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace FaiscaMcQueen_Transportes.Controllers
{
    public class AtivosController : Controller
    {
        private readonly FaiscaMcQueenContext _context;
        private readonly IMemoryCache _cache;
        private const string ATIVOS_LIST_CACHE_KEY = "ativos_list";
        private const int CACHE_DURATION_MINUTES = 30;

        public AtivosController(FaiscaMcQueenContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            List<AtivoViewModel> ativos;

            if (!_cache.TryGetValue(ATIVOS_LIST_CACHE_KEY, out ativos))
            {
                ativos = await _context.Ativos
                     .Select(a => new AtivoViewModel
                     {
                         Id = a.Id,
                         Matricula = a.Matricula,
                         Tipo = a.Tipo,
                         Marca = a.Marca,
                         Modelo = a.Modelo,
                         UltimasIntervencoes = new List<IntervencaoDetalhesViewModel>()
                     }).ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _cache.Set(ATIVOS_LIST_CACHE_KEY, ativos, cacheOptions);
            }
        
            return View(ativos);
        }

        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            string cacheKey = $"ativo_details_{id}";

            if (!_cache.TryGetValue(cacheKey, out AtivoViewModel? viewModel))
            {
                var ativo = await _context.Ativos
                .FirstOrDefaultAsync(m => m.Id == id);

                if (ativo == null)
                    return NotFound();

                var ultimasIntervencoes = await _context.Intervencoes
                    .Where(i => i.AtivoId == id)
                    .Include(i => i.Tecnico)
                    .OrderByDescending(i => i.DataCriacao)
                    .Take(5)
                    .ToListAsync();

                viewModel = new AtivoViewModel
                {
                    Id = ativo.Id,
                    Matricula = ativo.Matricula,
                    Tipo = ativo.Tipo,
                    Marca = ativo.Marca,
                    Modelo = ativo.Modelo,
                    UltimasIntervencoes = ultimasIntervencoes.Select(i => new IntervencaoDetalhesViewModel
                    {
                        Id = i.Id,
                        Estado = i.Estado.ToString(),
                        DataCriacao = i.DataCriacao,
                        DataInicio = i.DataInicio,
                        DataFim = i.DataFim,
                        Descricao = i.Descricao,
                        TecnicoNome = i.Tecnico?.Nome ?? "N/A",
                    }).ToList()
                };
                var cacheOptions = new MemoryCacheEntryOptions()
                       .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _cache.Set(cacheKey, viewModel, cacheOptions);
            }

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

                _cache.Remove(ATIVOS_LIST_CACHE_KEY);
                _cache.Remove($"ativo_details_{id}");


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

                _cache.Remove(ATIVOS_LIST_CACHE_KEY);
                _cache.Remove($"ativo_details_{id}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

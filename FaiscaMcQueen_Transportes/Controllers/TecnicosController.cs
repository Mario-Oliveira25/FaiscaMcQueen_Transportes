using FaiscaMcQueen_Transportes.Data;
using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using FaiscaMcQueen_Transportes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FaiscaMcQueen_Transportes.Controllers
{
    public class TecnicosController : Controller
    {
        private readonly FaiscaMcQueenContext _context;
        private readonly IMemoryCache _cache;
        private const string CACHE_TECNICOS_LIST = "tecnicos_list";
        private const string CACHE_TECNICO_DETAILS = "tecnico_details_{0}";
        private const int CACHE_DURATION_MINUTES = 30;

        public TecnicosController(FaiscaMcQueenContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { })]
        public async Task<IActionResult> Index()
        {
            if (!_cache.TryGetValue(CACHE_TECNICOS_LIST, out List<Tecnico> tecnicos))
            {
                // Se não estiver em cache, consultar base de dados
                tecnicos = await _context.Tecnicos.ToListAsync();

                // Armazenar em cache com expiração de 30 minutos
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _cache.Set(CACHE_TECNICOS_LIST, tecnicos, cacheOptions);
            }


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

                _cache.Remove(CACHE_TECNICOS_LIST);

                return RedirectToAction("Details", new {id = novoTecnico.Id});
            }

            return View(viewModel);
        }

        [HttpGet]
        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "id" })]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null) RedirectToAction("Index");
            string cacheKey = string.Format(CACHE_TECNICO_DETAILS, id);

            if (!_cache.TryGetValue(cacheKey, out TecnicoViewModel viewModel))
            {
                var tecnico = await _context.Tecnicos
                                          .Include(t => t.Intervencoes)
                                          .FirstOrDefaultAsync(t => t.Id == id);

                if (tecnico == null)
                {
                    return RedirectToAction("Index");
                }

                 viewModel = new TecnicoViewModel
            {
                Id = tecnico.Id,
                Nome = tecnico.Nome,
                Nif = tecnico.Nif,
                DataNascimento = tecnico.DataNascimento,
                Especialidade = tecnico.Especialidade,

                ListaIntervencoes = tecnico.Intervencoes
            };
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _cache.Set(cacheKey, viewModel, cacheOptions);
            }

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

                _cache.Remove(CACHE_TECNICOS_LIST);
                _cache.Remove(string.Format(CACHE_TECNICO_DETAILS, tecnico.Id));

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
                Nif = tecnico.Nif,
                DataNascimento = tecnico.DataNascimento,
                Especialidade = tecnico.Especialidade
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

                _cache.Remove(CACHE_TECNICOS_LIST);
                _cache.Remove(string.Format(CACHE_TECNICO_DETAILS, tecnico.Id));
            }

            return RedirectToAction("Index");
        }
    }
}

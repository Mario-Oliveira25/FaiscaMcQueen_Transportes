using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using FaiscaMcQueen_Transportes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FaiscaMcQueen_Transportes.Controllers
{
    [Authorize]
    public class IntervencaoController : Controller
    {

        private readonly Data.FaiscaMcQueenContext _context;
        private readonly IMemoryCache _cache;
        private const string INTERVENCOES_LIST_CACHE_KEY = "intervencoes_list";
        private const string ATIVOS_LIST_CACHE_KEY = "ativos_for_intervencao";
        private const string TECNICOS_LIST_CACHE_KEY = "tecnicos_for_intervencao";
        private const int CACHE_DURATION_MINUTES = 30;

        public IntervencaoController(Data.FaiscaMcQueenContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            List<Intervencao> intervencoes;

            if (!_cache.TryGetValue(INTERVENCOES_LIST_CACHE_KEY, out intervencoes))
            {
                intervencoes = await _context.Intervencoes.Include(i => i.Tecnico)
                .Include(i => i.Ativo).ToListAsync();

                // Guardar no cache por 30 minutos
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _cache.Set(INTERVENCOES_LIST_CACHE_KEY, intervencoes, cacheOptions);
            }
            return View(intervencoes);
        }

        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            string cacheKey = $"intervencao_details_{id}";
            if (!_cache.TryGetValue(cacheKey, out Intervencao? intervencao))
            {
                intervencao = await _context.Intervencoes.Include(i => i.Tecnico).Include(i => i.Ativo).FirstOrDefaultAsync(a => a.Id == id);

                if (intervencao == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _cache.Set(cacheKey, intervencao, cacheOptions);
            }

            return View(intervencao);
        }
        [Authorize(Roles = "Chefe de Equipa")]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new RegistoIntervencaoViewModel();

            if (!_cache.TryGetValue(ATIVOS_LIST_CACHE_KEY, out List<SelectListItem>? ativos))
            {
                ativos = _context.Ativos
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Matricula })
                    .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _cache.Set(ATIVOS_LIST_CACHE_KEY, ativos, cacheOptions);
            }

            if (!_cache.TryGetValue(TECNICOS_LIST_CACHE_KEY, out List<SelectListItem> tecnicos))
            {
                tecnicos = _context.Tecnicos
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                    .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _cache.Set(TECNICOS_LIST_CACHE_KEY, tecnicos, cacheOptions);
            }

            model.Ativos =ativos;
            model.Tecnicos = tecnicos;
              

            return View(model);
        }

        [Authorize(Roles = "Chefe de Equipa")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistoIntervencaoViewModel model)
        {

            var intervencaoTecnicoExistente = await _context.Intervencoes
                .FirstOrDefaultAsync(i => i.TecnicoId == model.TecnicoId &&
                                           i.DataInicio.Date == model.DataIntervencao.Date &&
                                           (i.Estado == Intervencao.estado.Pendente || i.Estado == Intervencao.estado.EmCurso));
            var intervencaoAtivoExistente = await _context.Intervencoes
                .FirstOrDefaultAsync(i => i.AtivoId == model.AtivoId &&
                                            (i.Estado == Intervencao.estado.Pendente ||
                                            i.Estado == Intervencao.estado.EmCurso));

            if (intervencaoTecnicoExistente != null)
            {
                return Json(new
                {
                    success = false,
                    field = "DataIntervencao",
                    message = "Este técnico já tem uma intervenção agendada para esta data."
                });
            }
            if (intervencaoAtivoExistente != null)
            {
                return Json(new
                {
                    success = false,
                    field = "AtivoId",
                    message = "Este ativo já tem uma intervenção pendente ou em curso."
                });
            }

            if (ModelState.IsValid)
            {
                var intervencao = new Intervencao
                {
                    Id = Guid.NewGuid(),
                    AtivoId = model.AtivoId,
                    TecnicoId = model.TecnicoId,
                    DataCriacao = DateTime.Now,
                    DataInicio = model.DataIntervencao,
                    DataFim = null,
                    Descricao = model.Descricao,
                    Estado = model.Estado
                };

                _context.Intervencoes.Add(intervencao);
                await _context.SaveChangesAsync();

                _cache.Remove(INTERVENCOES_LIST_CACHE_KEY);

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
                return RedirectToAction(nameof(Index));
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
            var ativoId = intervencao.AtivoId;
            var tecnicoId = intervencao.TecnicoId;

            _context.Intervencoes.Remove(intervencao);
            await _context.SaveChangesAsync();

            _cache.Remove(INTERVENCOES_LIST_CACHE_KEY);
            _cache.Remove($"intervencao_details_{id}");


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
            var intervencao = await _context.Intervencoes.Include(i => i.Tecnico)
                .Include(i => i.Ativo).FirstOrDefaultAsync(i => i.Id == id);
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

                    _cache.Remove(INTERVENCOES_LIST_CACHE_KEY);
                    _cache.Remove($"intervencao_details_{id}");
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

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            var intervencao = await _context.Intervencoes.FindAsync(id);

            if (intervencao == null)
                return NotFound();

            
            switch (intervencao.Estado)
            {
                case Intervencao.estado.Pendente:
                    intervencao.Estado = Intervencao.estado.EmCurso;
                    intervencao.DataInicio = DateTime.Now; 
                    break;

                case Intervencao.estado.EmCurso:
                    intervencao.Estado = Intervencao.estado.Concluido;
                    intervencao.DataFim = DateTime.Now; 
                    break;

                case Intervencao.estado.Concluido:
                    // Não permite mudar automaticamente
                    return Json(new { success = false, message = "Intervenção concluída. Use 'Reabrir' para criar nova." });
            }

            _context.Intervencoes.Update(intervencao);
            await _context.SaveChangesAsync();

           
            _cache.Remove("intervencoes_list");

            return Json(new { success = true, message = $"Estado alterado para {intervencao.Estado}" });
        }

        [Authorize(Roles = "Chefe de Equipa")]
        [HttpGet]
        public async Task<IActionResult> Reopen(Guid id)
        {
            var intervencaoOriginal = await _context.Intervencoes
                .Include(i => i.Tecnico)
                .Include(i => i.Ativo)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (intervencaoOriginal == null)
                return NotFound();

            if (intervencaoOriginal.Estado != Intervencao.estado.Concluido)
                return Json(new { success = false, message = "Só é possível reabrir intervenções concluídas." });

            // Preencher o formulário com os dados da intervenção original
            var model = new RegistoIntervencaoViewModel
            {
                AtivoId = (Guid)intervencaoOriginal.AtivoId,
                TecnicoId = (Guid)intervencaoOriginal.TecnicoId,
                DataIntervencao = DateTime.Now,
                Descricao = intervencaoOriginal.Descricao, // Reutilizar descrição
                Estado = Intervencao.estado.Pendente
            };

            // Carregar dropdowns
            model.Ativos = _context.Ativos
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Matricula,
                    Selected = c.Id == intervencaoOriginal.AtivoId // Pré-selecionar
                })
                .ToList();

            model.Tecnicos = _context.Tecnicos
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome,
                    Selected = c.Id == intervencaoOriginal.TecnicoId // Pré-selecionar
                })
                .ToList();

            return View("Create", model);
        }
        public bool IntervencaoExists(Guid id)
        {
            return _context.Intervencoes.Any(e => e.Id == id);

        }
    }
}

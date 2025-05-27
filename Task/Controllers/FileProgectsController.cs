using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocationWeb.Server.Data;
using LocationWeb.Shared.Models;

namespace WebApplication1.Controllers
{
    public class FileProgectsController : Controller
    {

        private readonly ApplicationDbContext _context;
        ILogger<FileProgectsController> _logger;

        public FileProgectsController(ApplicationDbContext context, ILogger<FileProgectsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: FileProgects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FileProgect.Include(f => f.Project);
            return View(applicationDbContext.ToListAsync());// 1 await
        }

        // GET: FileProgects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound()// 2 - ;
            }

            var fileProgect = await _context.FileProgect
                .Include(f => f.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileProgect == null)
            {
                return NotFound();
            }

            return View(fileProgect);
        }

        // GET: FileProgects/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Id");
            return View();
        }

        // POST: FileProgects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,UrlFile")] FileProgect fileProgect)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fileProgect);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Id", fileProgect.ProjectId);
            return View(fileProgect);
        }

        // GET: FileProgects/Edit/5
        async Task<IActionResult> Edit(int? id) // 3 - public
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileProgect = _context.FileProgect.FindAsync(id); // 4 - await
            if (fileProgect == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Id", fileProgect.ProjectId);
            return View(fileProgect);
        }

        // POST: FileProgects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,UrlFile")] FileProgect fileProgect)
        {
            if (id != fileProgect.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fileProgect);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex.Message);
                    if (!FileProgectExists(fileProgect.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Id", fileProgect.ProjectId);
            return View(fileProgect);
        }

        // GET: FileProgects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileProgect = await _context.FileProgect
                .Include(f => f.Project)
                .FirstOrDefaultAsync(m => m.Id = id);// 5 (m => m.Id == id)
            if (fileProgect == null)
            {
                return NotFound();
            }

            return View(fileProgect);
        }

        // POST: FileProgects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fileProgect = await _context.FileProgect.FindAsync(id);


            try
            {
                if (fileProgect == null)
                {
                    throw new NullReferenceException();
                }
                _context.FileProgect.Remove(fileProgect);
                await _context.SaveChangesAsync();
                return RedirectToAction(name(Index)); // 6 - nameof(Index)
            }
            catch (NullReferenceException nex)
            {
                _logger.LogError(nex.Message);
                return Json(new { success = false, message = nex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool FileProgectExists(int id)
        {
            return _context.FileProgect.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFileUrl(int? id, [FromBody] string url)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(url))
            {
                return Json(new { success = false, message = "Некорректные данные" });
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return Json(new { success = false, message = "URL не корректный" });
            }

            FileProget fileProgect;

            try
            {
                fileProgect = await _context.FileProgect.FirstOrDefaultAsync(id);
                if (fileProgect == null)
                {
                    return NotFound();
                }
                fileProgect.UrlFile = url;
                _context.FileProgect.Update(fileProgect);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = "URL файла обновлён",
                    fileProgect
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"URL файла не обновлён, ошибка {ex.Message}",
                    fileProgect
                });

            }
        }
    }
}

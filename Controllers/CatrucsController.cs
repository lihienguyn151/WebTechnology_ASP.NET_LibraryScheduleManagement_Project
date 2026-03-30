using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyLichTruc.Data;
using QuanLyLichTruc.Models;

namespace QuanLyLichTruc.Controllers
{
    public class CatrucsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatrucsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catrucs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Catruc.Include(c => c.Canbo).Include(c => c.Ngaytruc);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Catrucs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catruc = await _context.Catruc
                .Include(c => c.Canbo)
                .Include(c => c.Ngaytruc)
                .FirstOrDefaultAsync(m => m.CatrucID == id);
            if (catruc == null)
            {
                return NotFound();
            }

            return View(catruc);
        }

        // GET: Catrucs/Create
        public IActionResult Create()
        {
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu");
            ViewData["NgaytrucID"] = new SelectList(_context.Ngaytruc, "NgaytrucID", "NgaytrucID");
            return View();
        }

        // POST: Catrucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatrucID,SoThuTuCa,DiemDanh,TrangThai,CreatedAt,UpdatedAt,CanboID,NgaytrucID")] Catruc catruc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catruc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu", catruc.CanboID);
            ViewData["NgaytrucID"] = new SelectList(_context.Ngaytruc, "NgaytrucID", "NgaytrucID", catruc.NgaytrucID);
            return View(catruc);
        }

        // GET: Catrucs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catruc = await _context.Catruc.FindAsync(id);
            if (catruc == null)
            {
                return NotFound();
            }
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu", catruc.CanboID);
            ViewData["NgaytrucID"] = new SelectList(_context.Ngaytruc, "NgaytrucID", "NgaytrucID", catruc.NgaytrucID);
            return View(catruc);
        }

        // POST: Catrucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatrucID,SoThuTuCa,DiemDanh,TrangThai,CreatedAt,UpdatedAt,CanboID,NgaytrucID")] Catruc catruc)
        {
            if (id != catruc.CatrucID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catruc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatrucExists(catruc.CatrucID))
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
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu", catruc.CanboID);
            ViewData["NgaytrucID"] = new SelectList(_context.Ngaytruc, "NgaytrucID", "NgaytrucID", catruc.NgaytrucID);
            return View(catruc);
        }

        // GET: Catrucs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catruc = await _context.Catruc
                .Include(c => c.Canbo)
                .Include(c => c.Ngaytruc)
                .FirstOrDefaultAsync(m => m.CatrucID == id);
            if (catruc == null)
            {
                return NotFound();
            }

            return View(catruc);
        }

        // POST: Catrucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catruc = await _context.Catruc.FindAsync(id);
            if (catruc != null)
            {
                _context.Catruc.Remove(catruc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatrucExists(int id)
        {
            return _context.Catruc.Any(e => e.CatrucID == id);
        }
    }
}

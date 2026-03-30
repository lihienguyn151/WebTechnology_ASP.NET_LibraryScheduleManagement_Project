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
    public class XeplichesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public XeplichesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Xepliches
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Xeplich.Include(x => x.Canbo).Include(x => x.Lichtruc);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Xepliches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xeplich = await _context.Xeplich
                .Include(x => x.Canbo)
                .Include(x => x.Lichtruc)
                .FirstOrDefaultAsync(m => m.XeplichID == id);
            if (xeplich == null)
            {
                return NotFound();
            }

            return View(xeplich);
        }

        // GET: Xepliches/Create
        public IActionResult Create()
        {
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu");
            ViewData["LichtrucID"] = new SelectList(_context.Lichtruc, "LichtrucID", "GhiChu");
            return View();
        }

        // POST: Xepliches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("XeplichID,SoBuoiDaXep,CreatedAt,UpdatedAt,LichtrucID,CanboID")] Xeplich xeplich)
        {
            if (ModelState.IsValid)
            {
                _context.Add(xeplich);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu", xeplich.CanboID);
            ViewData["LichtrucID"] = new SelectList(_context.Lichtruc, "LichtrucID", "GhiChu", xeplich.LichtrucID);
            return View(xeplich);
        }

        // GET: Xepliches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xeplich = await _context.Xeplich.FindAsync(id);
            if (xeplich == null)
            {
                return NotFound();
            }
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu", xeplich.CanboID);
            ViewData["LichtrucID"] = new SelectList(_context.Lichtruc, "LichtrucID", "GhiChu", xeplich.LichtrucID);
            return View(xeplich);
        }

        // POST: Xepliches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("XeplichID,SoBuoiDaXep,CreatedAt,UpdatedAt,LichtrucID,CanboID")] Xeplich xeplich)
        {
            if (id != xeplich.XeplichID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xeplich);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XeplichExists(xeplich.XeplichID))
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
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu", xeplich.CanboID);
            ViewData["LichtrucID"] = new SelectList(_context.Lichtruc, "LichtrucID", "GhiChu", xeplich.LichtrucID);
            return View(xeplich);
        }

        // GET: Xepliches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xeplich = await _context.Xeplich
                .Include(x => x.Canbo)
                .Include(x => x.Lichtruc)
                .FirstOrDefaultAsync(m => m.XeplichID == id);
            if (xeplich == null)
            {
                return NotFound();
            }

            return View(xeplich);
        }

        // POST: Xepliches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var xeplich = await _context.Xeplich.FindAsync(id);
            if (xeplich != null)
            {
                _context.Xeplich.Remove(xeplich);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XeplichExists(int id)
        {
            return _context.Xeplich.Any(e => e.XeplichID == id);
        }
    }
}

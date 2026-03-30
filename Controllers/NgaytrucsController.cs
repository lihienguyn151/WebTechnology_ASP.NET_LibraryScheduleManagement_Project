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
    public class NgaytrucsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NgaytrucsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ngaytrucs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ngaytruc.Include(n => n.Lichtruc);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ngaytrucs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngaytruc = await _context.Ngaytruc
                .Include(n => n.Lichtruc)
                .FirstOrDefaultAsync(m => m.NgaytrucID == id);
            if (ngaytruc == null)
            {
                return NotFound();
            }

            return View(ngaytruc);
        }

        // GET: Ngaytrucs/Create
        public IActionResult Create()
        {
            ViewData["LichtrucID"] = new SelectList(_context.Lichtruc, "LichtrucID", "GhiChu");
            return View();
        }

        // POST: Ngaytrucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NgaytrucID,Ngay,Thu,Tuan,CreatedAt,UpdatedAt,LichtrucID")] Ngaytruc ngaytruc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ngaytruc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LichtrucID"] = new SelectList(_context.Lichtruc, "LichtrucID", "GhiChu", ngaytruc.LichtrucID);
            return View(ngaytruc);
        }

        // GET: Ngaytrucs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngaytruc = await _context.Ngaytruc.FindAsync(id);
            if (ngaytruc == null)
            {
                return NotFound();
            }
            ViewData["LichtrucID"] = new SelectList(_context.Lichtruc, "LichtrucID", "GhiChu", ngaytruc.LichtrucID);
            return View(ngaytruc);
        }

        // POST: Ngaytrucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NgaytrucID,Ngay,Thu,Tuan,CreatedAt,UpdatedAt,LichtrucID")] Ngaytruc ngaytruc)
        {
            if (id != ngaytruc.NgaytrucID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ngaytruc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NgaytrucExists(ngaytruc.NgaytrucID))
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
            ViewData["LichtrucID"] = new SelectList(_context.Lichtruc, "LichtrucID", "GhiChu", ngaytruc.LichtrucID);
            return View(ngaytruc);
        }

        // GET: Ngaytrucs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngaytruc = await _context.Ngaytruc
                .Include(n => n.Lichtruc)
                .FirstOrDefaultAsync(m => m.NgaytrucID == id);
            if (ngaytruc == null)
            {
                return NotFound();
            }

            return View(ngaytruc);
        }

        // POST: Ngaytrucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ngaytruc = await _context.Ngaytruc.FindAsync(id);
            if (ngaytruc != null)
            {
                _context.Ngaytruc.Remove(ngaytruc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NgaytrucExists(int id)
        {
            return _context.Ngaytruc.Any(e => e.NgaytrucID == id);
        }
    }
}

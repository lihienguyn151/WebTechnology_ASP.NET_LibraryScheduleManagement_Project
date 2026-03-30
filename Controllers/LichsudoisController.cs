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
    public class LichsudoisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichsudoisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lichsudois
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Lichsudoi.Include(l => l.Catruc);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Lichsudois/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichsudoi = await _context.Lichsudoi
                .Include(l => l.Catruc)
                .FirstOrDefaultAsync(m => m.LichsudoiID == id);
            if (lichsudoi == null)
            {
                return NotFound();
            }

            return View(lichsudoi);
        }

        // GET: Lichsudois/Create
        public IActionResult Create()
        {
            ViewData["CatrucID"] = new SelectList(_context.Catruc, "CatrucID", "CatrucID");
            return View();
        }

        // POST: Lichsudois/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LichsudoiID,NguoiDoi,NguoiDuocYeuCau,CamuondoiID,TrangThai,LiDo,CreatedAt,UpdatedAt,CatrucID")] Lichsudoi lichsudoi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichsudoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatrucID"] = new SelectList(_context.Catruc, "CatrucID", "CatrucID", lichsudoi.CatrucID);
            return View(lichsudoi);
        }

        // GET: Lichsudois/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichsudoi = await _context.Lichsudoi.FindAsync(id);
            if (lichsudoi == null)
            {
                return NotFound();
            }
            ViewData["CatrucID"] = new SelectList(_context.Catruc, "CatrucID", "CatrucID", lichsudoi.CatrucID);
            return View(lichsudoi);
        }

        // POST: Lichsudois/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LichsudoiID,NguoiDoi,NguoiDuocYeuCau,CamuondoiID,TrangThai,LiDo,CreatedAt,UpdatedAt,CatrucID")] Lichsudoi lichsudoi)
        {
            if (id != lichsudoi.LichsudoiID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichsudoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichsudoiExists(lichsudoi.LichsudoiID))
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
            ViewData["CatrucID"] = new SelectList(_context.Catruc, "CatrucID", "CatrucID", lichsudoi.CatrucID);
            return View(lichsudoi);
        }

        // GET: Lichsudois/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichsudoi = await _context.Lichsudoi
                .Include(l => l.Catruc)
                .FirstOrDefaultAsync(m => m.LichsudoiID == id);
            if (lichsudoi == null)
            {
                return NotFound();
            }

            return View(lichsudoi);
        }

        // POST: Lichsudois/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichsudoi = await _context.Lichsudoi.FindAsync(id);
            if (lichsudoi != null)
            {
                _context.Lichsudoi.Remove(lichsudoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichsudoiExists(int id)
        {
            return _context.Lichsudoi.Any(e => e.LichsudoiID == id);
        }
    }
}

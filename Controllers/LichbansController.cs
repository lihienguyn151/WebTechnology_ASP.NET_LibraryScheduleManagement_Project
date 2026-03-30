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
    public class LichbansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichbansController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Lấy thông tin người dùng từ session
        public Canbo GetCanBo()
        {
            var email = HttpContext.Session.GetString("canbo");
            if (email != "")
            {
                var canbo = _context.Canbo.FirstOrDefault(cb => cb.Email == email);
                return canbo;
            }
            return null;
        }

        // GET: Lichbans
        public async Task<IActionResult> Index()
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var applicationDbContext = _context.Lichban.Include(l => l.Canbo).OrderByDescending(lb => lb.CreatedAt);
            ViewData["dscb"] = _context.Canbo.ToList();
            return View(await applicationDbContext.ToListAsync());
        }

        // POST: Lichbans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DateTime NgayBan, int CaBan, string LiDo, int CanboID)
        {
            var lichban = new Lichban();
            lichban.NgayBan = NgayBan;
            lichban.CaBan = CaBan;
            lichban.LiDo = LiDo;
            lichban.CanboID = CanboID;

            if (ModelState.IsValid)
            {
                _context.Add(lichban);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CanboID"] = new SelectList(_context.Canbo, "CanboID", "ChucVu", lichban.CanboID);
            return View(lichban);
        }

        // GET: Lichbans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var lichban = await _context.Lichban.FirstOrDefaultAsync(m => m.LichbanID == id);
            if (lichban == null)
            {
                return NotFound();
            }
            _context.Remove(lichban);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

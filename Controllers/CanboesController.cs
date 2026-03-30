using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyLichTruc.Data;
using QuanLyLichTruc.Models;

namespace QuanLyLichTruc.Controllers
{
    public class CanboesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Canbo> _passwordHasher;

        public CanboesController(ApplicationDbContext context, IPasswordHasher<Canbo> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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

        // GET: Canboes
        public async Task<IActionResult> Index()
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }
            return View(await _context.Canbo.ToListAsync());
        }

        // GET: Canboes/Create
        public IActionResult Create()
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }
            return View();
        }

        // POST: Canboes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string HoTen, string MaSo, string SoDienThoai, string Email, string ChucVu, int QuyenHan)
        {
            var canbo = new Canbo();
            canbo.HoTen = HoTen;
            canbo.MaSo = MaSo;
            canbo.SoDienThoai = SoDienThoai;
            canbo.Email = Email;
            canbo.ChucVu = ChucVu;
            canbo.QuyenHan = QuyenHan;
            canbo.TrangThai = true;
            canbo.MatKhau = _passwordHasher.HashPassword(canbo, "Agu@123");
            if (ModelState.IsValid)
            {
                _context.Add(canbo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(canbo);
        }

        // GET: Canboes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var canbo = await _context.Canbo.FindAsync(id);
            if (canbo == null)
            {
                return NotFound();
            }
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }
            return View(canbo);
        }

        // POST: Canboes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int CanboID, string HoTen, string MaSo, string SoDienThoai, string Email, string ChucVu, int QuyenHan)
        {
            if (id != CanboID)
            {
                return NotFound();
            }

            var canbo = await _context.Canbo.FindAsync(id);

            if (ModelState.IsValid && canbo != null)
            {
                try
                {
                    canbo.HoTen = HoTen;
                    canbo.MaSo = MaSo;
                    canbo.SoDienThoai = SoDienThoai;
                    canbo.Email = Email;
                    canbo.ChucVu = ChucVu;
                    canbo.QuyenHan = QuyenHan;
                    canbo.UpdatedAt = DateTime.Now;
                    _context.Update(canbo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CanboExists(CanboID))
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
            return View(canbo);
        }

        private bool CanboExists(int id)
        {
            return _context.Canbo.Any(e => e.CanboID == id);
        }

        //Hiển thị trang xem lịch theo cán bộ
        public async Task<IActionResult> ScheduleView(int id, int kt)
        {
            var canbo = await _context.Canbo.FindAsync(id);
            if (canbo != null)
            {
                DateTime ngayhienhanh = DateTime.Now;
                if (kt == 0)
                {
                    ViewData["dsct_cb"] = _context.Catruc.Include(ct => ct.Canbo).Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(ct => ct.CanboID == id && ct.Ngaytruc.Lichtruc.Thang == ngayhienhanh.Month && ct.Ngaytruc.Lichtruc.Nam == ngayhienhanh.Year).OrderByDescending(ct => ct.CreatedAt).ToList();
                }
                else
                {
                    ViewData["dsct_cb"] = _context.Catruc.Include(ct => ct.Canbo).Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(ct => ct.CanboID == id).OrderByDescending(ct => ct.CreatedAt).ToList();
                }
                ViewData["canbo"] = GetCanBo();
                if (ViewData["canbo"] == null)
                {
                    return RedirectToAction("Login", controllerName: "Home");
                }
                return View(canbo);
            }
            return NotFound();
        }


        //Vô hiệu hóa tài khoản
        public async Task<IActionResult> VoHieuHoa(int id)
        {
            var canbo = _context.Canbo.FirstOrDefault(cb => cb.CanboID == id);
            if (canbo != null)
            {
                if (canbo.TrangThai == true)
                {
                    canbo.TrangThai = false;
                }
                else
                {
                    canbo.TrangThai = true;
                }
                _context.Update(canbo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

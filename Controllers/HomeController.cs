using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyLichTruc.Data;
using QuanLyLichTruc.Models;
using System.Diagnostics;

namespace QuanLyLichTruc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Canbo> _passwordHasher;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IPasswordHasher<Canbo> passwordHasher)
        {
            _logger = logger;
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

        public IActionResult Index()
        {
            DateTime ngayhienhanh = DateTime.Now;
            int thang = ngayhienhanh.Month;
            int nam = ngayhienhanh.Year;
            var lichtruc = _context.Lichtruc.FirstOrDefault(lt => lt.Thang == thang && lt.Nam == nam);

            ViewData["canbo"] = GetCanBo();
            return View(lichtruc);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Hiển thị trang đăng nhập người dùng hệ thống
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string MatKhau)
        {
            var canbo = _context.Canbo.FirstOrDefault(cb => cb.Email == Email);

            //Kiểm tra đăng nhập
            if (canbo != null && _passwordHasher.VerifyHashedPassword(canbo, canbo.MatKhau, MatKhau) == PasswordVerificationResult.Success && canbo.TrangThai == true)
            {
                HttpContext.Session.SetString("canbo", canbo.Email);
                return RedirectToAction(nameof(Index));
            }

            //Đăng nhập thất bại
            return RedirectToAction(nameof(Login));
        }

        //Hiển thị trang đăng ký người dùng
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string MaSo, string HoTen, string ChucVu, string SoDienThoai, string Email, string MatKhau, string NhapLaiMatKhau)
        {
            //Kiểm tra thông tin
            if (MatKhau != NhapLaiMatKhau)
            {
                return RedirectToAction(nameof(Register));
            }

            //Thêm cán bộ mới
            var canbo = new Canbo();
            canbo.MaSo = MaSo;
            canbo.HoTen = HoTen;
            canbo.ChucVu = ChucVu;
            canbo.SoDienThoai = SoDienThoai;
            canbo.Email = Email;
            canbo.MatKhau = _passwordHasher.HashPassword(canbo, MatKhau);
            canbo.QuyenHan = 2;
            canbo.TrangThai = true;
            _context.Add(canbo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }

        //Đăng xuất tài khoản
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.SetString("canbo", "");
            ViewData["canbo"] = GetCanBo();
            return RedirectToAction(nameof(Index));
        }

        //Trang cập nhật thông tin cá nhân
        public async Task<IActionResult> UserInformation()
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserInformation(int CanboID, string Email, string ChucVu, string SoDienThoai, string MatKhau, string NhapLaiMatKhau)
        {
            //Kiểm tra dữ liệu
            var dscb = _context.Canbo.Where(cb => cb.Email == Email).ToList();
            if (MatKhau != NhapLaiMatKhau || dscb.Count > 0)
            {
                return RedirectToAction(nameof(UserInformation));
            }
            var canbo = _context.Canbo.Find(CanboID);
            canbo.Email = Email;
            canbo.ChucVu = ChucVu;
            canbo.SoDienThoai = SoDienThoai;
            canbo.MatKhau = _passwordHasher.HashPassword(canbo, MatKhau);
            canbo.UpdatedAt = DateTime.Now;
            _context.Update(canbo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserInformation));
        }

        //Trang xác nhận ca trực
        public async Task<IActionResult> ScheduleConfirm(int? full)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }
            var canbo = GetCanBo();
            var lichtruc = _context.Lichtruc.OrderByDescending(lt => lt.LichtrucID).FirstOrDefault();
            var dsct = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(ct => ct.Lichtruc).Where(ct => ct.CanboID == canbo.CanboID && ct.Ngaytruc.LichtrucID == lichtruc.LichtrucID && ct.TrangThai == 2).OrderByDescending(ct => ct.CatrucID).ToList();

            if (full != null)
            {
                dsct = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(ct => ct.Lichtruc).Where(ct => ct.CanboID == canbo.CanboID).OrderByDescending(ct => ct.CatrucID).ToList();
                return View(dsct);
            }
            return View(dsct);
        }

        //Xác nhận ca trực
        public async Task<IActionResult> Confirm(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var catruc = _context.Catruc.Find(id);
            if (catruc != null)
            {
                catruc.TrangThai = 1;
                catruc.UpdatedAt = DateTime.Now;
                _context.Update(catruc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ScheduleConfirm));
        }

        //Trang phản hồi yêu cầu đổi lịch
        public async Task<IActionResult> ResponseChange(int? full)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }
            var canbo = GetCanBo();
            var dslsd = _context.Lichsudoi.Include(lsd => lsd.Catruc).ThenInclude(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(lsd => lsd.LichsudoiID == 0).ToList();
            var dsct = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).ToList();

            //Vòng lặp thêm lịch sử đổi
            foreach (var catruc in dsct)
            {
                var lichsudoi = _context.Lichsudoi.Include(lsd => lsd.Catruc).ThenInclude(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(lsd => lsd.CamuondoiID == catruc.CatrucID && lsd.NguoiDuocYeuCau == canbo.HoTen).ToList();
                if (lichsudoi.Count != 0)
                {
                    dslsd.AddRange(lichsudoi);
                }
            }

            if (full == null)
            {
                dslsd = dslsd.Where(lsd => lsd.TrangThai == false).ToList();
            }

            dslsd = dslsd.OrderByDescending(lsd => lsd.LichsudoiID).ToList();
            ViewData["dsct"] = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).ToList();
            return View(dslsd);
        }

        public async Task<IActionResult> Accept(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var lichsudoi = _context.Lichsudoi.Include(lsd => lsd.Catruc).ThenInclude(ct => ct.Canbo).FirstOrDefault(lsd => lsd.LichsudoiID == id);

            //Lấy hai ca trực và cập nhật
            var cahientai = _context.Catruc.Include(ct => ct.Canbo).FirstOrDefault(ct => ct.CatrucID == lichsudoi.CatrucID);
            var temp = cahientai.CanboID;
            var camuondoi = _context.Catruc.Include(ct => ct.Canbo).FirstOrDefault(ct => ct.CatrucID == lichsudoi.CamuondoiID);

            cahientai.CanboID = camuondoi.CanboID;
            cahientai.UpdatedAt = DateTime.Now;
            _context.Update(cahientai);
            await _context.SaveChangesAsync();

            camuondoi.CanboID = temp;
            camuondoi.UpdatedAt = DateTime.Now;
            _context.Update(camuondoi);
            await _context.SaveChangesAsync();

            //Cập nhật trạng thái của lịch sử đổi
            lichsudoi.TrangThai = true;
            lichsudoi.UpdatedAt = DateTime.Now;
            _context.Update(lichsudoi);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ResponseChange));
        }

        //Trang quản lý lịch bận
        public async Task<IActionResult> SetBusyList()
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }
            var canbo = GetCanBo();

            //Lấy danh sách lịch bận theo cán bộ
            var dslb = _context.Lichban.Include(lb => lb.Canbo).Where(lb => lb.CanboID == canbo.CanboID).OrderByDescending(lb => lb.LichbanID).ToList();

            return View(dslb);
        }

        [HttpPost]
        public async Task<IActionResult> SetBusyList(int CanboID, DateTime NgayBan, int CaBan, string LiDo)
        {
            //Thêm lịch bận mới
            var lichban = new Lichban();
            lichban.NgayBan = NgayBan;
            lichban.CaBan = CaBan;
            lichban.LiDo = LiDo;
            lichban.CanboID = CanboID;
            _context.Add(lichban);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(SetBusyList));
        }

        public async Task<IActionResult> Delete(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var lichban = _context.Lichban.Find(id);
            if (lichban != null)
            {
                _context.Remove(lichban);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(SetBusyList));
        }

        //Trang xem chi tiết lịch trực tại trang chủ
        public async Task<IActionResult> Details(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }
            var canbo = GetCanBo();

            var lichtruc = await _context.Lichtruc
                .FirstOrDefaultAsync(m => m.LichtrucID == id);
            if (lichtruc == null)
            {
                return NotFound();
            }

            //Lấy danh sách ngày trực theo tuần
            ViewData["dsnt_tuan1"] = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID).Where(nt => nt.Tuan == 1).ToList();
            ViewData["dsnt_tuan2"] = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID).Where(nt => nt.Tuan == 2).ToList();
            ViewData["dsnt_tuan3"] = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID).Where(nt => nt.Tuan == 3).ToList();
            ViewData["dsnt_tuan4"] = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID).Where(nt => nt.Tuan == 4).ToList();
            ViewData["dsnt_tuan5"] = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID).Where(nt => nt.Tuan == 5).ToList();
            ViewData["dsnt_tuan6"] = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID).Where(nt => nt.Tuan == 6).ToList();

            //Lấy danh sách ca trực thuộc lịch
            var dsct = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(ct => ct.Ngaytruc.Lichtruc.LichtrucID == lichtruc.LichtrucID);
            ViewData["dsct"] = dsct.Include(ct => ct.Canbo).ToList();

            //Lấy danh sách cán bộ và lịch sử đổi ca
            ViewData["dscb"] = _context.Canbo.ToList();
            ViewData["dslsd"] = _context.Lichsudoi.ToList();

            //Lấy danh sách ca trực của cán bộ
            ViewData["dsct_cb"] = _context.Catruc.Include(ct => ct.Canbo).Include(ct => ct.Ngaytruc).ThenInclude(ct => ct.Lichtruc).Where(ct => ct.Ngaytruc.LichtrucID == lichtruc.LichtrucID && ct.CanboID == canbo.CanboID).ToList();

            return View(lichtruc);
        }

        //Điểm danh trực
        public async Task<IActionResult> DiemDanh(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var catruc = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).FirstOrDefault(ct => ct.CatrucID == id);
            id = catruc.Ngaytruc.LichtrucID;

            //Cập nhật trạng thái điểm danh
            catruc.DiemDanh = true;
            catruc.UpdatedAt = DateTime.Now;
            _context.Update(catruc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        //Xác nhận trực
        public async Task<IActionResult> XacNhanTruc(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var catruc = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).FirstOrDefault(ct => ct.CatrucID == id);
            id = catruc.Ngaytruc.LichtrucID;

            //Cập nhật trạng thái ca trực
            catruc.TrangThai = 1;
            catruc.UpdatedAt = DateTime.Now;
            _context.Update(catruc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        //Yêu cầu đổi ca
        [HttpPost]
        public async Task<IActionResult> YeuCauDoiCa(int NguoiDoiID, int NguoiDuocYeuCauID, int CaHienTai, int CamuondoiID, string LiDo)
        {
            //Tạo lịch sử đổi
            var catruc = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).FirstOrDefault(ct => ct.CatrucID == CaHienTai);
            int id = catruc.Ngaytruc.LichtrucID;
            var nguoidoi = _context.Canbo.Find(NguoiDoiID);
            var nguoiduocyeucau = _context.Canbo.Find(NguoiDuocYeuCauID);

            var lichsudoi = new Lichsudoi();
            lichsudoi.NguoiDoi = nguoidoi.HoTen;
            lichsudoi.NguoiDuocYeuCau = nguoiduocyeucau.HoTen;
            lichsudoi.CamuondoiID = CamuondoiID;
            lichsudoi.CatrucID = CaHienTai;
            lichsudoi.LiDo = LiDo;
            lichsudoi.TrangThai = false;
            _context.Add(lichsudoi);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using QuanLyLichTruc.Data;
using QuanLyLichTruc.Models;
using ClosedXML.Excel;

namespace QuanLyLichTruc.Controllers
{
    public class LichtrucsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichtrucsController(ApplicationDbContext context)
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

        // GET: Lichtrucs
        public async Task<IActionResult> Index(int? nam)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            DateTime ngayhienhanh = DateTime.Now;
            int thangsau = (ngayhienhanh.Month != 12) ? ngayhienhanh.Month + 1 : 1;
            int namsau = (ngayhienhanh.Month != 12) ? ngayhienhanh.Year : ngayhienhanh.Year + 1;
            var dslsd = _context.Lichsudoi.Include(lsd => lsd.Catruc).ThenInclude(ct => ct.Canbo);
            var lichtruc = _context.Lichtruc.FirstOrDefault(lt => lt.Thang == thangsau && lt.Nam == namsau);

            ViewData["dslsd"] = dslsd.Include(lsd => lsd.Catruc).ThenInclude(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(lsd => lsd.Catruc.Ngaytruc.Lichtruc.Thang == ngayhienhanh.Month && lsd.Catruc.Ngaytruc.Lichtruc.Nam == ngayhienhanh.Year).OrderByDescending(lsd => lsd.UpdatedAt).ToList();
            ViewData["dsct"] = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(ct => ct.Ngaytruc.Lichtruc.Thang == ngayhienhanh.Month && ct.Ngaytruc.Lichtruc.Nam == ngayhienhanh.Year).ToList();
            ViewData["count"] = (ViewData["dslsd"] as List<Lichsudoi>).Count;
            ViewData["kiemtra"] = (lichtruc == null) ? true : false;

            //Lấy lịch trực
            var dslt = new List<Lichtruc>();
            if (nam != null)
            {
                dslt = await _context.Lichtruc.Where(lt => lt.Nam == nam).OrderByDescending(lt => lt.Thang).ToListAsync();
            }
            else
            {
                dslt = await _context.Lichtruc.OrderByDescending(lt => lt.Nam).ThenByDescending(lt => lt.Thang).ToListAsync();
            }
            return View(dslt);
        }

        // GET: Lichtrucs/Details/5
        public async Task<IActionResult> Details(int? id)
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

            //Lấy danh sách thống kê buổi trực theo cán bộ
            ViewData["dstk"] = ThongKeBuoiTruc_CanBo(id);

            return View(lichtruc);
        }

        // POST: Lichtrucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string GhiChu)
        {
            DateTime ngayhienhanh = DateTime.Now;
            //int thangsau = (ngayhienhanh.Month != 12) ? ngayhienhanh.Month + 1 : 1;
            //int namsau = (ngayhienhanh.Month != 12) ? ngayhienhanh.Year : ngayhienhanh.Year + 1;
            int thangsau = 2;
            int namsau = 2026;

            var lichtruc = new Lichtruc();

            if (ModelState.IsValid)
            {
                //Thêm lịch trực
                lichtruc.Thang = thangsau;
                lichtruc.Nam = namsau;
                lichtruc.GhiChu = GhiChu;
                lichtruc.TrangThai = true;
                _context.Add(lichtruc);
                await _context.SaveChangesAsync();

                //Thêm ngày trực
                int songay = DateTime.DaysInMonth(namsau, thangsau);
                int tuan = 1;
                string thang_ch = (thangsau < 10) ? "0" + thangsau : thangsau.ToString();
                for (int i = 1; i <= songay;i++)
                {
                    var ngaytruc = new Ngaytruc();
                    string ngay_ch = (i < 10) ? "0" + i : i.ToString();
                    DateTime ngay = DateTime.Parse(namsau + "-" + thang_ch + "-" + ngay_ch);
                    int thu = (int) ngay.DayOfWeek;
                    if (thu == 0)
                        thu = 7;

                    ngaytruc.Ngay = i;
                    ngaytruc.Thu = thu;
                    ngaytruc.Tuan = tuan;
                    ngaytruc.LichtrucID = lichtruc.LichtrucID;
                    _context.Add(ngaytruc);
                    await _context.SaveChangesAsync();

                    if (thu % 7 == 0)
                        tuan++;
                }

                //Thêm dữ liệu xếp lịch cán bộ
                var dscb = _context.Canbo.ToList();
                foreach (var cb in dscb)
                {
                    var xeplich = new Xeplich();
                    xeplich.SoBuoiDaXep = 0;
                    xeplich.LichtrucID = lichtruc.LichtrucID;
                    xeplich.CanboID = cb.CanboID;
                    _context.Add(xeplich);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Lichtrucs/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var lichtruc = await _context.Lichtruc.FindAsync(id);
            if (lichtruc == null)
            {
                return NotFound();
            }
            return View(lichtruc);
        }

        // POST: Lichtrucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LichtrucID,Thang,Nam,GhiChu,TrangThai,CreatedAt,UpdatedAt")] Lichtruc lichtruc)
        {
            if (id != lichtruc.LichtrucID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichtruc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichtrucExists(lichtruc.LichtrucID))
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
            return View(lichtruc);
        }

        // GET: Lichtrucs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichtruc = await _context.Lichtruc
                .FirstOrDefaultAsync(m => m.LichtrucID == id);
            if (lichtruc == null)
            {
                return NotFound();
            }

            return View(lichtruc);
        }

        // POST: Lichtrucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichtruc = await _context.Lichtruc.FindAsync(id);
            if (lichtruc != null)
            {
                _context.Lichtruc.Remove(lichtruc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichtrucExists(int id)
        {
            return _context.Lichtruc.Any(e => e.LichtrucID == id);
        }

        public async Task<IActionResult> CapNhatTrangThai(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var lichtruc = await _context.Lichtruc.FindAsync(id);
            if (lichtruc != null)
            {
                if (lichtruc.TrangThai == true)
                {
                    lichtruc.TrangThai = false;
                }
                else
                {
                    lichtruc.TrangThai = true;
                }
                _context.Update(lichtruc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //Thêm cán bộ trực trên lịch
        [HttpPost]
        public async Task<IActionResult> ThemCanBo(int id, int canbo, int ngaytruc, int soca)
        {
            var catruc = new Catruc();
            catruc.CanboID = canbo;
            catruc.NgaytrucID = ngaytruc;
            catruc.SoThuTuCa = soca;
            catruc.DiemDanh = false;
            catruc.TrangThai = 2;
            _context.Add(catruc);
            await _context.SaveChangesAsync();

            catruc = _context.Catruc.Include(ct => ct.Ngaytruc).FirstOrDefault(ct => ct.CatrucID == catruc.CatrucID);
            var xeplich = _context.Xeplich.FirstOrDefault(xl => xl.CanboID == canbo && xl.LichtrucID == catruc.Ngaytruc.LichtrucID);
            xeplich.SoBuoiDaXep = xeplich.SoBuoiDaXep + 1;
            xeplich.UpdatedAt = DateTime.Now;
            _context.Update(xeplich);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        //Rút lịch của cán bộ trên lịch
        public async Task<IActionResult> RutLich(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var catruc = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).FirstOrDefault(ct => ct.CatrucID == id);
            if (catruc != null)
            {
                id = catruc.Ngaytruc.LichtrucID;

                var xeplich = _context.Xeplich.FirstOrDefault(xl => xl.CanboID == catruc.CanboID && xl.LichtrucID == catruc.Ngaytruc.LichtrucID);
                xeplich.SoBuoiDaXep = xeplich.SoBuoiDaXep - 1;
                xeplich.UpdatedAt = DateTime.Now;
                _context.Update(xeplich);
                await _context.SaveChangesAsync();

                _context.Remove(catruc);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Details), new { id });
        }

        //Điểm danh cán bộ trực
        public async Task<IActionResult> DiemDanh(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var catruc = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).FirstOrDefault(ct => ct.CatrucID == id);
            if (catruc != null)
            {
                id = catruc.Ngaytruc.LichtrucID;

                catruc.DiemDanh = true;
                catruc.UpdatedAt = DateTime.Now;
                _context.Update(catruc);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        //Đổi lịch cán bộ trực
        public async Task<IActionResult> DoiLich(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var catruc = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).FirstOrDefault(ct => ct.CatrucID == id);
            ViewData["dscb"] = _context.Canbo.ToList();
            return View(catruc);
        }

        [HttpPost]
        public async Task<IActionResult> DoiLich(int id, int CatrucID, int CanboID)
        {
            var catruc = _context.Catruc.Find(CatrucID);
            if (catruc != null) {
                int macb = catruc.CanboID;

                //Cập nhật ca trực
                catruc.CanboID = CanboID;
                catruc.UpdatedAt = DateTime.Now;
                _context.Update(catruc);
                await _context.SaveChangesAsync();

                //Cập nhật xếp lịch
                var xeplich = _context.Xeplich.FirstOrDefault(xl => xl.CanboID == CanboID && xl.LichtrucID == id);
                xeplich.SoBuoiDaXep = xeplich.SoBuoiDaXep + 1;
                xeplich.UpdatedAt = DateTime.Now;
                _context.Update(xeplich);
                await _context.SaveChangesAsync();

                xeplich = _context.Xeplich.FirstOrDefault(xl => xl.CanboID == macb && xl.LichtrucID == id);
                xeplich.SoBuoiDaXep = xeplich.SoBuoiDaXep - 1;
                xeplich.UpdatedAt = DateTime.Now;
                _context.Update(xeplich);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        public class DanhSachXepLich
        {
            //Khai báo thuộc tính
            public int CanboID { get; set; }
            public int? SoBuoiDaXep { get; set; }
        }

        public async Task<IActionResult> SapXepLich(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var lichtruc = _context.Lichtruc.Find(id);
            var dscb = _context.Canbo.ToList();
            DateTime ngayhienhanh = DateTime.Now;
            var dsxl = _context.Xeplich.Where(xl => xl.LichtrucID == id).ToList();

            //Lấy danh sách ngày trực
            var dsnt = _context.Ngaytruc.Where(nt => nt.LichtrucID == id).ToList();
            var dsxl_sosanh = _context.Xeplich.Include(xl => xl.Lichtruc).Where(xl => xl.Lichtruc.Thang == ngayhienhanh.Month && xl.Lichtruc.Nam == ngayhienhanh.Year).OrderBy(xl => xl.SoBuoiDaXep).ToList();
            bool kiemtra = false;
            if (dsxl_sosanh.Count == 0)
            {
                kiemtra = true;
            }

            //Chuẩn hóa danh sách so sánh
            var ds_sosanh = new List<DanhSachXepLich>();
            foreach (var xl in dsxl_sosanh)
            {
                var temp = new DanhSachXepLich();
                temp.CanboID = xl.CanboID;
                temp.SoBuoiDaXep = xl.SoBuoiDaXep;
                ds_sosanh.Add(temp);
            }
            foreach (var cb in dscb)
            {
                var kt = dsxl_sosanh.FirstOrDefault(xl => xl.CanboID == cb.CanboID);
                if (kt == null)
                {
                    var temp = new DanhSachXepLich();
                    temp.CanboID = cb.CanboID;
                    temp.SoBuoiDaXep = 0;
                    ds_sosanh.Add(temp);
                }
            }

            //Chạy vòng lặp thêm lịch trực
            foreach (var nt in dsnt)
            {
                //Kiểm tra nếu ngày trực là chủ nhật
                if (nt.Thu == 7)
                {
                    continue;
                }

                //Thêm ca trực 1
                var catruc = _context.Catruc.FirstOrDefault(ct => ct.NgaytrucID == nt.NgaytrucID && ct.SoThuTuCa == 1);
                if (catruc == null)
                {
                    int macanbo;
                    if (kiemtra == true)
                    {
                        macanbo = dsxl[0].CanboID;
                    }
                    else
                    {
                        macanbo = ds_sosanh[0].CanboID;
                    }

                    //Thêm ca trực
                    catruc = new Catruc();
                    catruc.CanboID = macanbo;
                    catruc.NgaytrucID = nt.NgaytrucID;
                    catruc.SoThuTuCa = 1;
                    catruc.DiemDanh = false;
                    catruc.TrangThai = 2;
                    _context.Add(catruc);
                    await _context.SaveChangesAsync();

                    //Cập nhật xếp lịch
                    var xeplich = dsxl.FirstOrDefault(xl => xl.CanboID == macanbo);
                    xeplich.SoBuoiDaXep = xeplich.SoBuoiDaXep + 1;
                    xeplich.UpdatedAt = DateTime.Now;
                    _context.Update(xeplich);
                    await _context.SaveChangesAsync();

                    //Cập nhật danh sách so sánh
                    if (kiemtra == true)
                    {
                        dsxl = dsxl.OrderBy(xl => xl.SoBuoiDaXep).ToList();
                    }
                    else
                    {
                        ds_sosanh[0].SoBuoiDaXep = ds_sosanh[0].SoBuoiDaXep + 1;
                        ds_sosanh = ds_sosanh.OrderBy(xl => xl.SoBuoiDaXep).ToList();
                    }
                }

                //Thêm ca trực 2
                catruc = _context.Catruc.FirstOrDefault(ct => ct.NgaytrucID == nt.NgaytrucID && ct.SoThuTuCa == 2);
                if (catruc == null)
                {
                    int macanbo;
                    if (kiemtra == true)
                    {
                        macanbo = dsxl[0].CanboID;
                    }
                    else
                    {
                        macanbo = ds_sosanh[0].CanboID;
                    }

                    //Thêm ca trực
                    catruc = new Catruc();
                    catruc.CanboID = macanbo;
                    catruc.NgaytrucID = nt.NgaytrucID;
                    catruc.SoThuTuCa = 2;
                    catruc.DiemDanh = false;
                    catruc.TrangThai = 2;
                    _context.Add(catruc);
                    await _context.SaveChangesAsync();

                    //Cập nhật xếp lịch
                    var xeplich = dsxl.FirstOrDefault(xl => xl.CanboID == macanbo);
                    xeplich.SoBuoiDaXep = xeplich.SoBuoiDaXep + 1;
                    xeplich.UpdatedAt = DateTime.Now;
                    _context.Update(xeplich);
                    await _context.SaveChangesAsync();

                    //Cập nhật danh sách so sánh
                    if (kiemtra == true)
                    {
                        dsxl = dsxl.OrderBy(xl => xl.SoBuoiDaXep).ToList();
                    }
                    else
                    {
                        ds_sosanh[0].SoBuoiDaXep = ds_sosanh[0].SoBuoiDaXep + 1;
                        ds_sosanh = ds_sosanh.OrderBy(xl => xl.SoBuoiDaXep).ToList();
                    }
                }
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        //Hiển thị trang xem lịch sử đổi lịch
        public async Task<IActionResult> LichSuDoi(int id)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var temp = _context.Lichsudoi.Include(lsd => lsd.Catruc).ThenInclude(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(lsd => lsd.Catruc.Ngaytruc.LichtrucID == id);
            var dslsd = temp.Include(lsd => lsd.Catruc).ThenInclude(ct => ct.Canbo).ToList();

            ViewData["dsct"] = _context.Catruc.Include(ct => ct.Canbo).Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(ct => ct.Ngaytruc.LichtrucID == id).ToList();
            ViewData["malich"] = id;
            return View(dslsd);
        }

        //Đổi lịch và cập nhật lịch sử đổi
        [HttpPost]
        public async Task<IActionResult> ThemLichSu(int LichTrucID, int CaHienTai, int CaMuonDoi)
        {
            //Tìm và cập nhật ca trực
            var cahientai = _context.Catruc.Include(ct => ct.Canbo).FirstOrDefault(ct => ct.CatrucID == CaHienTai);
            var temp = cahientai.CanboID;
            var camuondoi = _context.Catruc.Include(ct => ct.Canbo).FirstOrDefault(ct => ct.CatrucID == CaMuonDoi);

            cahientai.CanboID = camuondoi.CanboID;
            cahientai.UpdatedAt = DateTime.Now;
            _context.Update(cahientai);
            await _context.SaveChangesAsync();

            camuondoi.CanboID = temp;
            camuondoi.UpdatedAt = DateTime.Now;
            _context.Update(camuondoi);
            await _context.SaveChangesAsync();

            //Thêm lịch sử đổi lịch
            var lichsudoi = new Lichsudoi();
            lichsudoi.NguoiDoi = camuondoi.Canbo.HoTen;
            lichsudoi.NguoiDuocYeuCau = cahientai.Canbo.HoTen;
            lichsudoi.CamuondoiID = camuondoi.CatrucID;
            lichsudoi.CatrucID = cahientai.CatrucID;
            lichsudoi.LiDo = "";
            lichsudoi.TrangThai = true;
            _context.Add(lichsudoi);
            await _context.SaveChangesAsync();

            int id = LichTrucID;

            return RedirectToAction(nameof(LichSuDoi), new { id });
        }

        //Tạo lớp và hàm thống kê
        public List<ThongKe> ThongKeBuoiTruc_CanBo(int? malich)
        {
            var dstk = new List<ThongKe>();
            var dscb = _context.Canbo.ToList();
            foreach (var canbo in dscb)
            {
                var thongke = new ThongKe();
                thongke.CanboID = canbo.CanboID;
                thongke.HoTen = canbo.HoTen;

                var kt = _context.Xeplich.Where(xl => xl.CanboID == canbo.CanboID && xl.LichtrucID == malich).ToList();
                if (kt.Count == 0)
                {
                    thongke.SoBuoiDaXep = 0;
                    thongke.SoBuoiThuBay = 0;
                    thongke.SoBuoiToi = 0;
                    thongke.SoBuoiTrua = 0;
                    dstk.Add(thongke);
                    continue;
                }

                //Tổng số buổi trực
                thongke.SoBuoiDaXep = _context.Xeplich.FirstOrDefault(xl => xl.CanboID == canbo.CanboID && xl.LichtrucID == malich).SoBuoiDaXep;

                //Tổng số buổi thứ bảy
                thongke.SoBuoiThuBay = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(ct => ct.CanboID == canbo.CanboID && ct.Ngaytruc.LichtrucID == malich && ct.Ngaytruc.Thu == 6).ToList().Count();

                //Tổng số buổi trưa
                thongke.SoBuoiTrua = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(ct => ct.CanboID == canbo.CanboID && ct.Ngaytruc.LichtrucID == malich && ct.SoThuTuCa == 1).ToList().Count();

                //Tổng số buổi tối
                thongke.SoBuoiToi = _context.Catruc.Include(ct => ct.Ngaytruc).ThenInclude(nt => nt.Lichtruc).Where(ct => ct.CanboID == canbo.CanboID && ct.Ngaytruc.LichtrucID == malich && ct.SoThuTuCa == 2).ToList().Count();

                dstk.Add(thongke);
            }

            return dstk;
        }

        //Duyệt yêu cầu đổi lịch
        public async Task<IActionResult> DuyetYeuCau(int id, int malsd)
        {
            ViewData["canbo"] = GetCanBo();
            if (ViewData["canbo"] == null)
            {
                return RedirectToAction("Login", controllerName: "Home");
            }

            var lichsudoi = _context.Lichsudoi.Include(lsd => lsd.Catruc).ThenInclude(ct => ct.Canbo).FirstOrDefault(lsd => lsd.LichsudoiID == malsd);

            //Tìm và cập nhật ca trực
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

            return RedirectToAction(nameof(LichSuDoi), new { id });
        }

        //Xuất lịch Excel
        public IActionResult ExportExcel(int id)
        {
            //Lấy dữ liệu để xuất lịch trực
            var lichtruc = _context.Lichtruc.Find(id);
            var thang = lichtruc.Thang < 10 ? "0" + lichtruc.Thang : lichtruc.Thang.ToString();

            var dsnt_tuan1 = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID && nt.Tuan == 1).ToList();
            var dsnt_tuan2 = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID && nt.Tuan == 2).ToList();
            var dsnt_tuan3 = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID && nt.Tuan == 3).ToList();
            var dsnt_tuan4 = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID && nt.Tuan == 4).ToList();
            var dsnt_tuan5 = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID && nt.Tuan == 5).ToList();
            var dsnt_tuan6 = _context.Ngaytruc.Include(nt => nt.Lichtruc).Where(nt => nt.LichtrucID == lichtruc.LichtrucID && nt.Tuan == 6).ToList();

            //Tạo trang tính
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.AddWorksheet("LichTruc");

                //Chỉnh độ rộng cột
                ws.Column("A").Width = 10;
                ws.Column("B").Width = 25;
                ws.Column("C").Width = 25;
                ws.Column("D").Width = 25;
                ws.Column("E").Width = 25;
                ws.Column("F").Width = 25;
                ws.Column("G").Width = 25;
                ws.Column("H").Width = 25;

                //Header
                ws.Cell("A1").Value = "LỊCH TRỰC THƯ VIỆN";
                ws.Range("A1:H1").Merge();
                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A1").Style.Font.FontSize = 15;

                ws.Cell("A2").Value = "Tháng " + thang + " - năm " + lichtruc.Nam;
                ws.Range("A2:H2").Merge();
                ws.Cell("A2").Style.Font.Bold = true;
                ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A2").Style.Font.FontSize = 14;

                ws.Cell("B4").Value = "Thứ hai";
                ws.Cell("C4").Value = "Thứ ba";
                ws.Cell("D4").Value = "Thứ tư";
                ws.Cell("E4").Value = "Thứ năm";
                ws.Cell("F4").Value = "Thứ sáu";
                ws.Cell("G4").Value = "Thứ bảy";
                ws.Cell("H4").Value = "Chủ nhật";
                ws.Range("B4:H4").Style.Font.Bold = true;
                ws.Range("B4:H4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Khai báo biến
                var column_name = new List<char>()
                {
                    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
                };

                //Hiển thị lịch tuần 1
                ws.Cell("A5").Value = "Tuần 1";
                ws.Cell("A6").Value = "Ca trưa";
                ws.Cell("A7").Value = "Ca tối";
                ws.Range("A5:A7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A5").Style.Font.Bold = true;
                ws.Range("B5:H5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var ngaytruc in dsnt_tuan1)
                {
                    var column = column_name[(int) ngaytruc.Thu];
                    var ngay = ngaytruc.Ngay < 10 ? "0" + ngaytruc.Ngay : ngaytruc.Ngay.ToString();
                    ws.Cell(column + "5").Value = ngay + "/" + thang;

                    //Lấy thông tin ca trực
                    var dsct = _context.Catruc.Include(ct => ct.Canbo).Where(ct => ct.NgaytrucID == ngaytruc.NgaytrucID).ToList();
                    foreach (var catruc in dsct)
                    {
                        if (catruc.SoThuTuCa == 1)
                        {
                            ws.Cell(column + "6").Value = catruc.Canbo.HoTen;
                        }
                        else
                        {
                            ws.Cell(column + "7").Value = catruc.Canbo.HoTen;
                        }
                    }
                }

                //Hiển thị lịch tuần 2
                ws.Cell("A9").Value = "Tuần 2";
                ws.Cell("A10").Value = "Ca trưa";
                ws.Cell("A11").Value = "Ca tối";
                ws.Range("A9:A11").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A9").Style.Font.Bold = true;
                ws.Range("B9:H9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var ngaytruc in dsnt_tuan2)
                {
                    var column = column_name[(int)ngaytruc.Thu];
                    var ngay = ngaytruc.Ngay < 10 ? "0" + ngaytruc.Ngay : ngaytruc.Ngay.ToString();
                    ws.Cell(column + "9").Value = ngay + "/" + thang;

                    //Lấy thông tin ca trực
                    var dsct = _context.Catruc.Include(ct => ct.Canbo).Where(ct => ct.NgaytrucID == ngaytruc.NgaytrucID).ToList();
                    foreach (var catruc in dsct)
                    {
                        if (catruc.SoThuTuCa == 1)
                        {
                            ws.Cell(column + "10").Value = catruc.Canbo.HoTen;
                        }
                        else
                        {
                            ws.Cell(column + "11").Value = catruc.Canbo.HoTen;
                        }
                    }
                }

                //Hiển thị lịch tuần 3
                ws.Cell("A13").Value = "Tuần 3";
                ws.Cell("A14").Value = "Ca trưa";
                ws.Cell("A15").Value = "Ca tối";
                ws.Range("A13:A15").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A13").Style.Font.Bold = true;
                ws.Range("B13:H13").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var ngaytruc in dsnt_tuan3)
                {
                    var column = column_name[(int)ngaytruc.Thu];
                    var ngay = ngaytruc.Ngay < 10 ? "0" + ngaytruc.Ngay : ngaytruc.Ngay.ToString();
                    ws.Cell(column + "13").Value = ngay + "/" + thang;

                    //Lấy thông tin ca trực
                    var dsct = _context.Catruc.Include(ct => ct.Canbo).Where(ct => ct.NgaytrucID == ngaytruc.NgaytrucID).ToList();
                    foreach (var catruc in dsct)
                    {
                        if (catruc.SoThuTuCa == 1)
                        {
                            ws.Cell(column + "14").Value = catruc.Canbo.HoTen;
                        }
                        else
                        {
                            ws.Cell(column + "15").Value = catruc.Canbo.HoTen;
                        }
                    }
                }

                //Hiển thị lịch tuần 4
                ws.Cell("A17").Value = "Tuần 4";
                ws.Cell("A18").Value = "Ca trưa";
                ws.Cell("A19").Value = "Ca tối";
                ws.Range("A17:A19").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A17").Style.Font.Bold = true;
                ws.Range("B17:H17").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var ngaytruc in dsnt_tuan4)
                {
                    var column = column_name[(int)ngaytruc.Thu];
                    var ngay = ngaytruc.Ngay < 10 ? "0" + ngaytruc.Ngay : ngaytruc.Ngay.ToString();
                    ws.Cell(column + "17").Value = ngay + "/" + thang;

                    //Lấy thông tin ca trực
                    var dsct = _context.Catruc.Include(ct => ct.Canbo).Where(ct => ct.NgaytrucID == ngaytruc.NgaytrucID).ToList();
                    foreach (var catruc in dsct)
                    {
                        if (catruc.SoThuTuCa == 1)
                        {
                            ws.Cell(column + "18").Value = catruc.Canbo.HoTen;
                        }
                        else
                        {
                            ws.Cell(column + "19").Value = catruc.Canbo.HoTen;
                        }
                    }
                }

                //Hiển thị lịch tuần 5
                ws.Cell("A21").Value = "Tuần 5";
                ws.Cell("A22").Value = "Ca trưa";
                ws.Cell("A23").Value = "Ca tối";
                ws.Range("A21:A23").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A21").Style.Font.Bold = true;
                ws.Range("B21:H21").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var ngaytruc in dsnt_tuan5)
                {
                    var column = column_name[(int)ngaytruc.Thu];
                    var ngay = ngaytruc.Ngay < 10 ? "0" + ngaytruc.Ngay : ngaytruc.Ngay.ToString();
                    ws.Cell(column + "21").Value = ngay + "/" + thang;

                    //Lấy thông tin ca trực
                    var dsct = _context.Catruc.Include(ct => ct.Canbo).Where(ct => ct.NgaytrucID == ngaytruc.NgaytrucID).ToList();
                    foreach (var catruc in dsct)
                    {
                        if (catruc.SoThuTuCa == 1)
                        {
                            ws.Cell(column + "22").Value = catruc.Canbo.HoTen;
                        }
                        else
                        {
                            ws.Cell(column + "23").Value = catruc.Canbo.HoTen;
                        }
                    }
                }

                //Hiển thị lịch tuần 6
                ws.Cell("A25").Value = "Tuần 6";
                ws.Cell("A26").Value = "Ca trưa";
                ws.Cell("A27").Value = "Ca tối";
                ws.Range("A25:A27").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A25").Style.Font.Bold = true;
                ws.Range("B25:H25").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var ngaytruc in dsnt_tuan6)
                {
                    var column = column_name[(int)ngaytruc.Thu];
                    var ngay = ngaytruc.Ngay < 10 ? "0" + ngaytruc.Ngay : ngaytruc.Ngay.ToString();
                    ws.Cell(column + "25").Value = ngay + "/" + thang;

                    //Lấy thông tin ca trực
                    var dsct = _context.Catruc.Include(ct => ct.Canbo).Where(ct => ct.NgaytrucID == ngaytruc.NgaytrucID).ToList();
                    foreach (var catruc in dsct)
                    {
                        if (catruc.SoThuTuCa == 1)
                        {
                            ws.Cell(column + "26").Value = catruc.Canbo.HoTen;
                        }
                        else
                        {
                            ws.Cell(column + "27").Value = catruc.Canbo.HoTen;
                        }
                    }
                }

                //Xuất file Excel
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "lichtruc" + (lichtruc.Thang < 10 ? "0"+lichtruc.Thang : lichtruc.Thang) + lichtruc.Nam + ".xlsx"
                    );
                }
            }
        }
    }
}

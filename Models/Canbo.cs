using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyLichTruc.Models
{
    public class Canbo
    {
        //Khai báo thuộc tính
        public int CanboID { get; set; }

        [Display(Name = "Họ tên")]
        [StringLength(150)]
        [Required]
        public string? HoTen { get; set; }

        [Display(Name = "Mã số")]
        [StringLength(20)]
        [Required]
        public string? MaSo { get; set; }

        [Display(Name = "Số điện thoại")]
        [StringLength(15)]
        [Required]
        public string? SoDienThoai { get; set; }

        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [Display(Name = "Chức vụ")]
        [StringLength(100)]
        [Required]
        public string? ChucVu { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required]
        public string? MatKhau { get; set; }

        [Display(Name = "Trạng thái tài khoản")]
        [DefaultValue(true)]
        public bool TrangThai { get; set; }

        [Display(Name = "Quyền hạn")]
        [Required]
        public int QuyenHan { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Khai báo mối quan hệ
        public ICollection<Xeplich>? Xepliches { get; set; }
        public ICollection<Lichban>? Lichbans { get; set; }
        public ICollection<Catruc>? Catrucs { get; set; }
    }
}

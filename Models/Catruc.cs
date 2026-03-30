using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyLichTruc.Models
{
    public class Catruc
    {
        //Khai báo thuộc tính
        public int CatrucID { get; set; }

        [Display(Name = "Số ca")]
        [Range(1,2)]
        [Required]
        public int? SoThuTuCa { get; set; }

        [Display(Name = "Điểm danh")]
        [DefaultValue(false)]
        [Required]
        public bool DiemDanh { get; set; }

        [Display(Name = "Trạng thái")]
        [DefaultValue(2)]
        [Required]
        public int? TrangThai { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Khai báo mối quan hệ
        public int CanboID { get; set; }
        public Canbo? Canbo { get; set; }
        public int NgaytrucID { get; set; }
        public Ngaytruc? Ngaytruc { get; set; }
        public ICollection<Lichsudoi>? Lichsudoi { get; set; }
    }
}

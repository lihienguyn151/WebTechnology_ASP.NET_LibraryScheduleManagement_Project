using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyLichTruc.Models
{
    public class Lichtruc
    {
        //Khai báo thuộc tính
        public int LichtrucID { get; set; }

        [Display(Name = "Tháng")]
        [Range(1,12)]
        [Required]
        public int? Thang { get; set; }

        [Display(Name = "Năm")]
        [Range(0,3000)]
        [Required]
        public int? Nam { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(200)]
        [Required]
        public string GhiChu { get; set; }

        [Display(Name = "Trạng thái")]
        [DefaultValue(true)]
        public bool TrangThai { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Khai báo mối quan hệ
        public ICollection<Ngaytruc>? Ngaytrucs { get; set; }
        public ICollection<Xeplich>? Xepliches { get; set; }
    }
}

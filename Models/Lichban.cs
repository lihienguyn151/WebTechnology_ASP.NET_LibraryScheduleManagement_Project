using System.ComponentModel.DataAnnotations;

namespace QuanLyLichTruc.Models
{
    public class Lichban
    {
        //Khai báo thuộc tính
        public int LichbanID { get; set; }

        [Display(Name = "Ngày bận")]
        [Required]
        public DateTime NgayBan { get; set; }

        [Display(Name = "Ca bận")]
        [Range(1,2)]
        [Required]
        public int CaBan { get; set; }

        [Display(Name = "Lí do")]
        [StringLength(500)]
        public string? LiDo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Khai báo mối quan hệ
        public int CanboID { get; set; }
        public Canbo? Canbo { get; set; }
    }
}

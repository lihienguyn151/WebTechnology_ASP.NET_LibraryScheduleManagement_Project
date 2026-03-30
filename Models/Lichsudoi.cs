using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyLichTruc.Models
{
    public class Lichsudoi
    {
        //Khai báo thuộc tính
        public int LichsudoiID { get; set; }

        [Display(Name = "Người đổi")]
        [StringLength(200)]
        [Required]
        public string? NguoiDoi { get; set; }

        [Display(Name = "Người được yêu cầu")]
        [StringLength(200)]
        [Required]
        public string? NguoiDuocYeuCau { get; set; }

        [Display(Name = "Ca muốn đổi")]
        [Required]
        public int CamuondoiID { get; set; }

        [Display(Name = "Trạng thái")]
        [DefaultValue(false)]
        [Required]
        public bool TrangThai { get; set; }

        [Display(Name = "Lí do")]
        [StringLength(500)]
        [Required]
        public string? LiDo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Khai báo mối quan hệ
        [Display(Name = "Ca hiện tại")]
        public int CatrucID { get; set; }
        public Catruc? Catruc { get; set; }
    }
}

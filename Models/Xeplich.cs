using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyLichTruc.Models
{
    public class Xeplich
    {
        //Khai báo thuộc tính
        public int XeplichID { get; set; }

        [Display(Name = "Số buổi đã xếp")]
        [DefaultValue(0)]
        [Range(0,10)]
        public int? SoBuoiDaXep { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Khai báo mối quan hệ
        public int LichtrucID { get; set; }
        public Lichtruc? Lichtruc { get; set; }
        public int CanboID { get; set; }
        public Canbo? Canbo { get; set; }
    }
}

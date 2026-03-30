using System.ComponentModel.DataAnnotations;

namespace QuanLyLichTruc.Models
{
    public class Ngaytruc
    {
        //Khai báo thuộc tính
        public int NgaytrucID { get; set; }

        [Display(Name = "Ngày")]
        [Range(1,31)]
        public int? Ngay { get; set; }

        [Display(Name = "Thứ")]
        [Range(0,10)]
        public int? Thu { get; set; }

        [Display(Name = "Tuần")]
        [Range(1,6)]
        public int? Tuan { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Khai báo mối quan hệ
        public int LichtrucID { get; set; }
        public Lichtruc? Lichtruc { get; set; }
        public ICollection<Catruc>? Catrucs { get; set; }
    }
}

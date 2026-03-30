using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyLichTruc.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Canbo",
                columns: table => new
                {
                    CanboID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaSo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    QuyenHan = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canbo", x => x.CanboID);
                });

            migrationBuilder.CreateTable(
                name: "Lichtruc",
                columns: table => new
                {
                    LichtrucID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lichtruc", x => x.LichtrucID);
                });

            migrationBuilder.CreateTable(
                name: "Lichban",
                columns: table => new
                {
                    LichbanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayBan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CaBan = table.Column<int>(type: "int", nullable: false),
                    LiDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CanboID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lichban", x => x.LichbanID);
                    table.ForeignKey(
                        name: "FK_Lichban_Canbo_CanboID",
                        column: x => x.CanboID,
                        principalTable: "Canbo",
                        principalColumn: "CanboID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ngaytruc",
                columns: table => new
                {
                    NgaytrucID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<int>(type: "int", nullable: true),
                    Thu = table.Column<int>(type: "int", nullable: true),
                    Tuan = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LichtrucID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ngaytruc", x => x.NgaytrucID);
                    table.ForeignKey(
                        name: "FK_Ngaytruc_Lichtruc_LichtrucID",
                        column: x => x.LichtrucID,
                        principalTable: "Lichtruc",
                        principalColumn: "LichtrucID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Xeplich",
                columns: table => new
                {
                    XeplichID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoBuoiDaXep = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LichtrucID = table.Column<int>(type: "int", nullable: false),
                    CanboID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Xeplich", x => x.XeplichID);
                    table.ForeignKey(
                        name: "FK_Xeplich_Canbo_CanboID",
                        column: x => x.CanboID,
                        principalTable: "Canbo",
                        principalColumn: "CanboID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Xeplich_Lichtruc_LichtrucID",
                        column: x => x.LichtrucID,
                        principalTable: "Lichtruc",
                        principalColumn: "LichtrucID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catruc",
                columns: table => new
                {
                    CatrucID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoThuTuCa = table.Column<int>(type: "int", nullable: false),
                    DiemDanh = table.Column<bool>(type: "bit", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CanboID = table.Column<int>(type: "int", nullable: false),
                    NgaytrucID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catruc", x => x.CatrucID);
                    table.ForeignKey(
                        name: "FK_Catruc_Canbo_CanboID",
                        column: x => x.CanboID,
                        principalTable: "Canbo",
                        principalColumn: "CanboID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catruc_Ngaytruc_NgaytrucID",
                        column: x => x.NgaytrucID,
                        principalTable: "Ngaytruc",
                        principalColumn: "NgaytrucID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lichsudoi",
                columns: table => new
                {
                    LichsudoiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDoi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NguoiDuocYeuCau = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CamuondoiID = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    LiDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CatrucID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lichsudoi", x => x.LichsudoiID);
                    table.ForeignKey(
                        name: "FK_Lichsudoi_Catruc_CatrucID",
                        column: x => x.CatrucID,
                        principalTable: "Catruc",
                        principalColumn: "CatrucID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catruc_CanboID",
                table: "Catruc",
                column: "CanboID");

            migrationBuilder.CreateIndex(
                name: "IX_Catruc_NgaytrucID",
                table: "Catruc",
                column: "NgaytrucID");

            migrationBuilder.CreateIndex(
                name: "IX_Lichban_CanboID",
                table: "Lichban",
                column: "CanboID");

            migrationBuilder.CreateIndex(
                name: "IX_Lichsudoi_CatrucID",
                table: "Lichsudoi",
                column: "CatrucID");

            migrationBuilder.CreateIndex(
                name: "IX_Ngaytruc_LichtrucID",
                table: "Ngaytruc",
                column: "LichtrucID");

            migrationBuilder.CreateIndex(
                name: "IX_Xeplich_CanboID",
                table: "Xeplich",
                column: "CanboID");

            migrationBuilder.CreateIndex(
                name: "IX_Xeplich_LichtrucID",
                table: "Xeplich",
                column: "LichtrucID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lichban");

            migrationBuilder.DropTable(
                name: "Lichsudoi");

            migrationBuilder.DropTable(
                name: "Xeplich");

            migrationBuilder.DropTable(
                name: "Catruc");

            migrationBuilder.DropTable(
                name: "Canbo");

            migrationBuilder.DropTable(
                name: "Ngaytruc");

            migrationBuilder.DropTable(
                name: "Lichtruc");
        }
    }
}

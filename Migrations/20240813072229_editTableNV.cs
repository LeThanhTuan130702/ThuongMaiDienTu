using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuongMaiDienTu.Migrations
{
    /// <inheritdoc />
    public partial class editTableNV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "NhanVien",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DienThoai",
                table: "NhanVien",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GioiTinh",
                table: "NhanVien",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HieuLuc",
                table: "NhanVien",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Hinh",
                table: "NhanVien",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                table: "NhanVien",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RandomKey",
                table: "NhanVien",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VaiTro",
                table: "NhanVien",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "DienThoai",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "HieuLuc",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "Hinh",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "RandomKey",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "VaiTro",
                table: "NhanVien");
        }
    }
}

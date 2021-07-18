using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductApi.Migrations
{
    public partial class AddProductDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Manufacturer_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subtype_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cost = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Product_Manufacturer_Manufacturer_Id",
                        column: x => x.Manufacturer_Id,
                        principalTable: "Product_Manufacturer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Products_Product_Subtype_Subtype_Id",
                        column: x => x.Subtype_Id,
                        principalTable: "Product_Subtype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Products_Product_Type_Type_Id",
                        column: x => x.Type_Id,
                        principalTable: "Product_Type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Manufacturer_Id",
                table: "Products",
                column: "Manufacturer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Subtype_Id",
                table: "Products",
                column: "Subtype_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Type_Id",
                table: "Products",
                column: "Type_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}

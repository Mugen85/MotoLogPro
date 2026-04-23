using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable


namespace MotoLogPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDictionaryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BikeModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BikeModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BikeModels_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: ["Id", "Name"],
                values: new object[,]
                {
                    { 1, "Yamaha" },
                    { 2, "Honda" },
                    { 3, "Ducati" },
                    { 4, "BMW" },
                    { 5, "KTM" }
                });

            migrationBuilder.InsertData(
                table: "BikeModels",
                columns: ["Id", "BrandId", "Name"],
                values: new object[,]
                {
                    { 1, 1, "Ténéré 700" },
                    { 2, 1, "MT-07" },
                    { 3, 1, "Tracer 9" },
                    { 4, 2, "Africa Twin" },
                    { 5, 2, "Transalp 750" },
                    { 6, 2, "CBR600RR" },
                    { 7, 3, "Multistrada V4" },
                    { 8, 3, "DesertX" },
                    { 9, 4, "R 1250 GS" },
                    { 10, 4, "F 850 GS" },
                    { 11, 5, "1290 Super Adventure" },
                    { 12, 5, "890 Duke" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BikeModels_BrandId",
                table: "BikeModels",
                column: "BrandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BikeModels");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}

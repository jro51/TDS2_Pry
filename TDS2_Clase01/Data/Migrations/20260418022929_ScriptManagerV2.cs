using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TDS2_Clase01.Data.Migrations
{
    /// <inheritdoc />
    public partial class ScriptManagerV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colaborador_Empresa_EmpresaIdEmpresa",
                table: "Colaborador");

            migrationBuilder.DropIndex(
                name: "IX_Colaborador_EmpresaIdEmpresa",
                table: "Colaborador");

            migrationBuilder.DropColumn(
                name: "EmpresaIdEmpresa",
                table: "Colaborador");

            migrationBuilder.CreateIndex(
                name: "IX_Colaborador_IdEmpresa",
                table: "Colaborador",
                column: "IdEmpresa");

            migrationBuilder.AddForeignKey(
                name: "FK_Colaborador_Empresa_IdEmpresa",
                table: "Colaborador",
                column: "IdEmpresa",
                principalTable: "Empresa",
                principalColumn: "IdEmpresa",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colaborador_Empresa_IdEmpresa",
                table: "Colaborador");

            migrationBuilder.DropIndex(
                name: "IX_Colaborador_IdEmpresa",
                table: "Colaborador");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaIdEmpresa",
                table: "Colaborador",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Colaborador_EmpresaIdEmpresa",
                table: "Colaborador",
                column: "EmpresaIdEmpresa");

            migrationBuilder.AddForeignKey(
                name: "FK_Colaborador_Empresa_EmpresaIdEmpresa",
                table: "Colaborador",
                column: "EmpresaIdEmpresa",
                principalTable: "Empresa",
                principalColumn: "IdEmpresa",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

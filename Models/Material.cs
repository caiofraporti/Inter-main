// ============================================================
// Models/Material.cs
// Tabela "Materiais"
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Materiais")]
    public class Material
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome do material é obrigatório")]
        [StringLength(100)]
        [Column("nome_material")]
        public string NomeMaterial { get; set; } = "";

        [StringLength(200)]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }
    }
}

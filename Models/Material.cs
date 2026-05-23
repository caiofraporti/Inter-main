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
    }
}

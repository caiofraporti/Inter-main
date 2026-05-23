// ============================================================
// Models/TipoProduto.cs
// Tabela "Tipo_Produtos"
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Produtos")]
    public class Produto
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome do produto é obrigatório")]
        [StringLength(100)]
        [Column("nome_produto")]
        public string NomeProduto { get; set; } = "";

        [StringLength(200)]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "Valor unitário é obrigatório")]
        [Column("valor_unitario", TypeName = "decimal(10,2)")]
        public decimal ValorUnitario { get; set; }
    }
}

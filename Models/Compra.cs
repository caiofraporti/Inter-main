// ============================================================
// Models/Compra.cs
// Tabela "Compras"
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Compras")]
    public class Compra
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Data é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Total { get; set; }

        [Column("nr_parcelas")]
        public int? NrParcelas { get; set; }

        [Required(ErrorMessage = "Fornecedor é obrigatório")]
        [Column("fornecedor_id")]
        public int FornecedorId { get; set; }

        [Required(ErrorMessage = "Funcionário é obrigatório")]
        [Column("funcionario_id")]
        public int FuncionarioId { get; set; }

        // Navegação
        [ForeignKey("FornecedorId")]
        public Fornecedor? Fornecedor { get; set; }

        [ForeignKey("FuncionarioId")]
        public Funcionario? Funcionario { get; set; }
    }
}

// ============================================================
// Models/Venda.cs
// Tabela "Vendas"
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Vendas")]
    public class Venda
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

        [Required(ErrorMessage = "Cliente é obrigatório")]
        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Funcionário é obrigatório")]
        [Column("funcionario_id")]
        public int FuncionarioId { get; set; }

        // Navegação
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        [ForeignKey("FuncionarioId")]
        public Funcionario? Funcionario { get; set; }
    }
}

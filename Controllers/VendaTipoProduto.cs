// ============================================================
// Models/VendaTipoProduto.cs
// Tabela "vendas_tipo_produtos" — itens de uma venda
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("vendas_tipo_produtos")]
    public class VendaTipoProduto
    {
        [Column("venda_codigo")]
        public int VendaCodigo { get; set; }

        [Column("tipo_produto_codigo")]
        public int TipoProdutoCodigo { get; set; }

        [Column("item")]
        public int Item { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Column("qtd")]
        public int Qtd { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Column("preco", TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        [Column("dimensoes")]
        [StringLength(20)]
        public string? Dimensoes { get; set; }

        // Navegação
        [ForeignKey("VendaCodigo")]
        public Venda? Venda { get; set; }

        [ForeignKey("TipoProdutoCodigo")]
        public TipoProduto? TipoProduto { get; set; }
    }
}

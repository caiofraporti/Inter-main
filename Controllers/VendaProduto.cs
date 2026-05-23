// ============================================================
// Models/VendaProduto.cs
// Tabela "vendas_produtos" — itens de uma venda
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("vendas_produtos")]
    public class VendasProduto
    {
        [Column("venda_codigo")]
        public int VendaCodigo { get; set; }

        [Column("produto_codigo")]
        public int ProdutoCodigo { get; set; }

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

        [ForeignKey("ProdutoCodigo")]
        public Produto? Produto { get; set; }
    }
}

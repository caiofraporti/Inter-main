using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("compras_materiais")]
    public class CompraMaterial
    {
        [Column("compra_codigo")]
        public int CompraCodigo { get; set; }

        [Column("material_codigo")]
        public int MaterialCodigo { get; set; }

        public int Item { get; set; }
        public int Qtd { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        [StringLength(20)]
        public string? Dimensoes { get; set; }

        [StringLength(200)]
        public string? Descricao { get; set; }

        // Navegação
        [ForeignKey("CompraCodigo")]
        public Compra? Compra { get; set; }

        [ForeignKey("MaterialCodigo")]
        public Material? Material { get; set; }
    }
}

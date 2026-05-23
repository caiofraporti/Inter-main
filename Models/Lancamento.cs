// ============================================================
// Models/Lancamento.cs
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Lancamentos")]
    public class Lancamento
    {
        [Key]
        public int Codigo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Valor { get; set; }

        [Column("parcela_nr")]
        [StringLength(10)]
        public string? ParcelaNr { get; set; }

        [Column("data_vencimento")]
        [DataType(DataType.Date)]
        public DateTime? DataVencimento { get; set; }

        [Column("data_pagamento")]
        [DataType(DataType.Date)]
        public DateTime? DataPagamento { get; set; }

        [Column("venda_codigo")]
        public int? VendaCodigo { get; set; }

        [Column("compra_codigo")]
        public int? CompraCodigo { get; set; }

        [Column("situacao_codigo")]
        public int? SituacaoCodigo { get; set; }

        // Navegação
        [ForeignKey("SituacaoCodigo")]
        public Situacao? Situacao { get; set; }

        [ForeignKey("VendaCodigo")]
        public Venda? Venda { get; set; }

        [ForeignKey("CompraCodigo")]
        public Compra? Compra { get; set; }
    }
}

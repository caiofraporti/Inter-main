// ============================================================
// Models/Lancamento.cs
// Tabela "Lancamentos" — contas a pagar e receber
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

        // decimal(10,2) = valor monetário com 2 casas decimais
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

        // Chaves estrangeiras — podem ser nulas (nullable int?)
        [Column("venda_codigo")]
        public int? VendaCodigo { get; set; }

        [Column("compra_codigo")]
        public int? CompraCodigo { get; set; }

        [Column("situacao_codigo")]
        public int? SituacaoCodigo { get; set; }

        // Propriedade de navegação para acessar a situação
        [ForeignKey("SituacaoCodigo")]
        public Situacao? Situacao { get; set; }
    }
}

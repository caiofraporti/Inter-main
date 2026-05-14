// ============================================================
// Data/AppDbContext.cs
// Contexto do Entity Framework — é a "ponte" entre o C# e o banco
// Segue o mesmo padrão do professor (DatabaseContext no exemplo dele)
// ============================================================

using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Models;

namespace MoveisCarrara.Data
{
    // AppDbContext herda de DbContext (classe base do Entity Framework)
    public class AppDbContext : DbContext
    {
        // O construtor recebe as opções (string de conexão etc.)
        // e repassa para a classe base — padrão obrigatório do EF
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // -----------------------------------------------------------
        // DbSet<T> representa uma tabela do banco
        // Cada DbSet permite: Select, Insert, Update, Delete
        // Ex: _context.Pessoas.ToList() = SELECT * FROM Pessoas
        // -----------------------------------------------------------
        public DbSet<Pessoa>      Pessoas      { get; set; }
        public DbSet<Cliente>     Clientes     { get; set; }
        public DbSet<Fornecedor>  Fornecedores { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Situacao>    Situacoes    { get; set; }
        public DbSet<Lancamento>  Lancamentos  { get; set; }
        public DbSet<TipoProduto>  TipoProdutos  { get; set; }
        public DbSet<Material>     Materiais     { get; set; }
        public DbSet<Venda>        Vendas        { get; set; }
        public DbSet<Compra>       Compras       { get; set; }

        // -----------------------------------------------------------
        // OnModelCreating: configurações extras do mapeamento
        // -----------------------------------------------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeia os nomes das colunas que diferem do padrão C#
            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.ToTable("Pessoas");
                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.Nome).HasColumnName("nome");
                entity.Property(p => p.NomeSocial).HasColumnName("nome_social");
                entity.Property(p => p.TipoPessoa).HasColumnName("tipo_pessoa");
                entity.Property(p => p.Documento).HasColumnName("documento");
                entity.Property(p => p.TipoEndereco).HasColumnName("tipo_endereco");
                entity.Property(p => p.Logradouro).HasColumnName("logradouro");
                entity.Property(p => p.Numero).HasColumnName("numero");
                entity.Property(p => p.Bairro).HasColumnName("bairro");
                entity.Property(p => p.Cidade).HasColumnName("cidade");
                entity.Property(p => p.Cep).HasColumnName("cep");
                entity.Property(p => p.Telefone).HasColumnName("telefone");
                entity.Property(p => p.Email).HasColumnName("email");
            });

            modelBuilder.Entity<Funcionario>(entity =>
            {
                entity.ToTable("Funcionarios");
                entity.Property(f => f.Usuario).HasColumnName("usuario");
                entity.Property(f => f.Senha).HasColumnName("senha");
            });

            modelBuilder.Entity<Situacao>(entity =>
            {
                entity.Property(s => s.Codigo).HasColumnName("codigo");
                entity.Property(s => s.Descricao).HasColumnName("descricao");
            });

            modelBuilder.Entity<TipoProduto>(entity =>
            {
                entity.Property(t => t.Codigo).HasColumnName("codigo");
                entity.Property(t => t.NomeProduto).HasColumnName("nome_produto");
                entity.Property(t => t.Descricao).HasColumnName("descricao");
                entity.Property(t => t.ValorUnitario).HasColumnName("valor_unitario");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.Property(m => m.Codigo).HasColumnName("codigo");
                entity.Property(m => m.NomeMaterial).HasColumnName("nome_material");
                entity.Property(m => m.Descricao).HasColumnName("descricao");
                entity.Property(m => m.Preco).HasColumnName("preco");
            });
        }
    }
}

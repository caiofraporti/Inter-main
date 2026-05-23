using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class VendasController : Controller
    {
        private readonly AppDbContext _context;

        public VendasController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarLogin() =>
            HttpContext.Session.GetString("UsuarioLogado") != null;

        private async Task CarregarViewBags()
        {
            ViewBag.Clientes     = await _context.Clientes.Include(c => c.Pessoa).ToListAsync();
            ViewBag.Funcionarios = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            ViewBag.Produtos     = await _context.Produtos.OrderBy(p => p.NomeProduto).ToListAsync();
        }

        // GET /Vendas/Index
        public async Task<IActionResult> Index()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lista = await _context.Vendas
                .Include(v => v.Cliente).ThenInclude(c => c!.Pessoa)
                .Include(v => v.Funcionario).ThenInclude(f => f!.Pessoa)
                .OrderByDescending(v => v.Data)
                .ToListAsync();

            return View(lista);
        }

        // GET /Vendas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var venda = await _context.Vendas
                .Include(v => v.Cliente).ThenInclude(c => c!.Pessoa)
                .Include(v => v.Funcionario).ThenInclude(f => f!.Pessoa)
                .FirstOrDefaultAsync(v => v.Codigo == id);

            if (venda == null) return NotFound();

            var itens = await _context.VendaProdutos
                .Include(i => i.Produto)
                .Where(i => i.VendaCodigo == id)
                .OrderBy(i => i.Item)
                .ToListAsync();

            var lancamentos = await _context.Lancamentos
                .Include(l => l.Situacao)
                .Where(l => l.VendaCodigo == id)
                .OrderBy(l => l.DataVencimento)
                .ToListAsync();

            ViewBag.Itens        = itens;
            ViewBag.Lancamentos  = lancamentos;
            return View(venda);
        }

        // GET /Vendas/Create
        public async Task<IActionResult> Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");
            await CarregarViewBags();
            return View();
        }

        // POST /Vendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Venda         venda,
            List<int>     itemProdutoId,
            List<int>     itemQtd,
            List<decimal> itemPreco,
            List<string>  itemDimensoes,
            List<string>  itemDescricao)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            ModelState.Remove("Cliente");
            ModelState.Remove("Funcionario");

            if (ModelState.IsValid)
            {
                // Calcular total
                decimal total = 0;
                for (int i = 0; i < itemProdutoId.Count; i++)
                    if (itemProdutoId[i] != 0)
                        total += itemQtd[i] * itemPreco[i];

                venda.Total = total;

                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();

                // Salvar itens
                for (int i = 0; i < itemProdutoId.Count; i++)
                {
                    if (itemProdutoId[i] == 0) continue;

                    var item = new VendaProduto
                    {
                        VendaCodigo   = venda.Codigo,
                        ProdutoCodigo = itemProdutoId[i],
                        Item          = i + 1,
                        Qtd           = itemQtd[i],
                        Preco         = itemPreco[i],
                        Dimensoes     = itemDimensoes.Count > i ? itemDimensoes[i] : null,
                        Descricao     = itemDescricao.Count  > i ? itemDescricao[i]  : null
                    };
                    _context.VendaProdutos.Add(item);
                }

                await _context.SaveChangesAsync();

                // Gerar parcelas em Lancamentos
                if (venda.NrParcelas.HasValue && venda.NrParcelas.Value > 0 && total > 0)
                {
                    decimal valorParcela = Math.Round(total / venda.NrParcelas.Value, 2);
                    decimal acumulado    = 0;

                    for (int p = 1; p <= venda.NrParcelas.Value; p++)
                    {
                        // Última parcela recebe o arredondamento residual
                        decimal valorEsta = (p == venda.NrParcelas.Value)
                            ? total - acumulado
                            : valorParcela;

                        acumulado += valorEsta;

                        var lancamento = new Lancamento
                        {
                            Valor           = valorEsta,
                            ParcelaNr       = $"{p}/{venda.NrParcelas.Value}",
                            DataVencimento  = venda.Data.AddDays(30 * p),
                            DataPagamento   = null,
                            VendaCodigo     = venda.Codigo,
                            CompraCodigo    = null,
                            SituacaoCodigo  = 1   // Pendente
                        };
                        _context.Lancamentos.Add(lancamento);
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Sucesso"] = "Venda cadastrada com sucesso!";
                return RedirectToAction("Index");
            }

            await CarregarViewBags();
            return View(venda);
        }

        // GET /Vendas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var venda = await _context.Vendas
                .Include(v => v.Cliente).ThenInclude(c => c!.Pessoa)
                .Include(v => v.Funcionario).ThenInclude(f => f!.Pessoa)
                .FirstOrDefaultAsync(v => v.Codigo == id);

            if (venda == null) return NotFound();

            var itens = await _context.VendaProdutos
                .Where(i => i.VendaCodigo == id)
                .OrderBy(i => i.Item)
                .ToListAsync();

            ViewBag.Itens = itens;
            await CarregarViewBags();
            return View(venda);
        }

        // POST /Vendas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int           id,
            Venda         venda,
            List<int>     itemProdutoId,
            List<int>     itemQtd,
            List<decimal> itemPreco,
            List<string>  itemDimensoes,
            List<string>  itemDescricao)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            ModelState.Remove("Cliente");
            ModelState.Remove("Funcionario");

            if (ModelState.IsValid)
            {
                decimal total = 0;
                for (int i = 0; i < itemProdutoId.Count; i++)
                    if (itemProdutoId[i] != 0)
                        total += itemQtd[i] * itemPreco[i];

                venda.Codigo = id;
                venda.Total  = total;

                _context.Vendas.Update(venda);

                // Remover itens antigos
                var itensAntigos = _context.VendaProdutos.Where(i => i.VendaCodigo == id);
                _context.VendaProdutos.RemoveRange(itensAntigos);
                await _context.SaveChangesAsync();

                // Inserir novos itens
                for (int i = 0; i < itemProdutoId.Count; i++)
                {
                    if (itemProdutoId[i] == 0) continue;

                    var item = new VendaProduto
                    {
                        VendaCodigo   = id,
                        ProdutoCodigo = itemProdutoId[i],
                        Item          = i + 1,
                        Qtd           = itemQtd[i],
                        Preco         = itemPreco[i],
                        Dimensoes     = itemDimensoes.Count > i ? itemDimensoes[i] : null,
                        Descricao     = itemDescricao.Count  > i ? itemDescricao[i]  : null
                    };
                    _context.VendaProdutos.Add(item);
                }

                await _context.SaveChangesAsync();

                // Recriar parcelas: remove as antigas e gera novas
                var lancamentosAntigos = _context.Lancamentos.Where(l => l.VendaCodigo == id);
                _context.Lancamentos.RemoveRange(lancamentosAntigos);
                await _context.SaveChangesAsync();

                if (venda.NrParcelas.HasValue && venda.NrParcelas.Value > 0 && total > 0)
                {
                    decimal valorParcela = Math.Round(total / venda.NrParcelas.Value, 2);
                    decimal acumulado    = 0;

                    for (int p = 1; p <= venda.NrParcelas.Value; p++)
                    {
                        decimal valorEsta = (p == venda.NrParcelas.Value)
                            ? total - acumulado
                            : valorParcela;

                        acumulado += valorEsta;

                        var lancamento = new Lancamento
                        {
                            Valor           = valorEsta,
                            ParcelaNr       = $"{p}/{venda.NrParcelas.Value}",
                            DataVencimento  = venda.Data.AddDays(30 * p),
                            DataPagamento   = null,
                            VendaCodigo     = id,
                            CompraCodigo    = null,
                            SituacaoCodigo  = 1   // Pendente
                        };
                        _context.Lancamentos.Add(lancamento);
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Sucesso"] = "Venda alterada com sucesso!";
                return RedirectToAction("Index");
            }

            var itensExistentes = await _context.VendaProdutos
                .Where(i => i.VendaCodigo == id)
                .OrderBy(i => i.Item)
                .ToListAsync();

            ViewBag.Itens = itensExistentes;
            await CarregarViewBags();
            return View(venda);
        }

        // POST /Vendas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // Remove lancamentos vinculados
            var lancamentos = _context.Lancamentos.Where(l => l.VendaCodigo == id);
            _context.Lancamentos.RemoveRange(lancamentos);

            // Remove itens
            var itens = _context.VendaProdutos.Where(i => i.VendaCodigo == id);
            _context.VendaProdutos.RemoveRange(itens);

            var venda = await _context.Vendas.FindAsync(id);
            if (venda != null) _context.Vendas.Remove(venda);

            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Venda excluída com sucesso!";
            return RedirectToAction("Index");
        }
    }
}

// ============================================================
// Controllers/VendasController.cs
// ============================================================

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
            ViewBag.Produtos     = await _context.TipoProdutos.OrderBy(p => p.NomeProduto).ToListAsync();
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
            Venda venda,
            List<int>     itemProdutoId,
            List<int>     itemQtd,
            List<decimal> itemPreco,
            List<string>  itemDimensoes)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // Remove validações de navegação que o EF resolve sozinho
            ModelState.Remove("Cliente");
            ModelState.Remove("Funcionario");

            if (ModelState.IsValid)
            {
                // 1) Calcula o total com base nos itens
                decimal total = 0;
                for (int i = 0; i < itemProdutoId.Count; i++)
                    total += itemQtd[i] * itemPreco[i];

                venda.Total = total;

                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();

                // 2) Grava os itens em vendas_tipo_produtos
                for (int i = 0; i < itemProdutoId.Count; i++)
                {
                    if (itemProdutoId[i] == 0) continue; // linha vazia

                    var item = new VendaTipoProduto
                    {
                        VendaCodigo      = venda.Codigo,
                        TipoProdutoCodigo = itemProdutoId[i],
                        Item             = i + 1,
                        Qtd              = itemQtd[i],
                        Preco            = itemPreco[i],
                        Dimensoes        = itemDimensoes.Count > i ? itemDimensoes[i] : null
                    };
                    _context.VendaTipoProdutos.Add(item);
                }

                await _context.SaveChangesAsync();
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

            // Carrega os itens existentes desta venda
            var itens = await _context.VendaTipoProdutos
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
            int id,
            Venda venda,
            List<int>     itemProdutoId,
            List<int>     itemQtd,
            List<decimal> itemPreco,
            List<string>  itemDimensoes)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            ModelState.Remove("Cliente");
            ModelState.Remove("Funcionario");

            if (ModelState.IsValid)
            {
                // Recalcula total
                decimal total = 0;
                for (int i = 0; i < itemProdutoId.Count; i++)
                    total += itemQtd[i] * itemPreco[i];

                venda.Codigo = id;
                venda.Total  = total;

                _context.Vendas.Update(venda);

                // Remove os itens antigos e regrava
                var itensAntigos = _context.VendaTipoProdutos.Where(i => i.VendaCodigo == id);
                _context.VendaTipoProdutos.RemoveRange(itensAntigos);
                await _context.SaveChangesAsync();

                for (int i = 0; i < itemProdutoId.Count; i++)
                {
                    if (itemProdutoId[i] == 0) continue;

                    var item = new VendaTipoProduto
                    {
                        VendaCodigo       = id,
                        TipoProdutoCodigo = itemProdutoId[i],
                        Item              = i + 1,
                        Qtd               = itemQtd[i],
                        Preco             = itemPreco[i],
                        Dimensoes         = itemDimensoes.Count > i ? itemDimensoes[i] : null
                    };
                    _context.VendaTipoProdutos.Add(item);
                }

                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Venda alterada com sucesso!";
                return RedirectToAction("Index");
            }

            var itensExistentes = await _context.VendaTipoProdutos
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

            // Remove itens antes de remover a venda (FK)
            var itens = _context.VendaTipoProdutos.Where(i => i.VendaCodigo == id);
            _context.VendaTipoProdutos.RemoveRange(itens);

            var venda = await _context.Vendas.FindAsync(id);
            if (venda != null) _context.Vendas.Remove(venda);

            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Venda excluída com sucesso!";
            return RedirectToAction("Index");
        }
    }
}

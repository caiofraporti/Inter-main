using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class ComprasController : Controller
    {
        private readonly AppDbContext _context;

        public ComprasController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarLogin() =>
            HttpContext.Session.GetString("UsuarioLogado") != null;

        private async Task CarregarViewBags()
        {
            ViewBag.Fornecedores = await _context.Fornecedores.Include(f => f.Pessoa).ToListAsync();
            ViewBag.Funcionarios = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            ViewBag.Materiais    = await _context.Materiais.OrderBy(m => m.NomeMaterial).ToListAsync();
        }

        // GET /Compras
        public async Task<IActionResult> Index()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lista = await _context.Compras
                .Include(c => c.Fornecedor).ThenInclude(f => f!.Pessoa)
                .Include(c => c.Funcionario).ThenInclude(f => f!.Pessoa)
                .OrderByDescending(c => c.Data)
                .ToListAsync();

            return View(lista);
        }

        // GET /Compras/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var compra = await _context.Compras
                .Include(c => c.Fornecedor).ThenInclude(f => f!.Pessoa)
                .Include(c => c.Funcionario).ThenInclude(f => f!.Pessoa)
                .FirstOrDefaultAsync(c => c.Codigo == id);

            if (compra == null) return NotFound();

            var itens = await _context.CompraMateriais
                .Include(i => i.Material)
                .Where(i => i.CompraCodigo == id)
                .OrderBy(i => i.Item)
                .ToListAsync();

            var lancamentos = await _context.Lancamentos
                .Include(l => l.Situacao)
                .Where(l => l.CompraCodigo == id)
                .OrderBy(l => l.DataVencimento)
                .ToListAsync();

            ViewBag.Itens       = itens;
            ViewBag.Lancamentos = lancamentos;
            return View(compra);
        }

        // GET /Compras/Create
        public async Task<IActionResult> Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");
            await CarregarViewBags();
            return View();
        }

        // POST /Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Compra        compra,
            List<int>     itemMaterialId,
            List<int>     itemQtd,
            List<decimal> itemPreco,
            List<string>  itemDimensoes,
            List<string>  itemDescricao)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            ModelState.Remove("Fornecedor");
            ModelState.Remove("Funcionario");

            if (ModelState.IsValid)
            {
                // Calcular total pelos itens
                decimal total = 0;
                for (int i = 0; i < itemMaterialId.Count; i++)
                    if (itemMaterialId[i] != 0)
                        total += itemQtd[i] * itemPreco[i];

                compra.Total = total;

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                // Salvar itens
                for (int i = 0; i < itemMaterialId.Count; i++)
                {
                    if (itemMaterialId[i] == 0) continue;

                    var item = new CompraMaterial
                    {
                        CompraCodigo   = compra.Codigo,
                        MaterialCodigo = itemMaterialId[i],
                        Item           = i + 1,
                        Qtd            = itemQtd[i],
                        Preco          = itemPreco[i],
                        Dimensoes      = itemDimensoes.Count > i ? itemDimensoes[i] : null,
                        Descricao      = itemDescricao.Count  > i ? itemDescricao[i]  : null
                    };
                    _context.CompraMateriais.Add(item);
                }

                await _context.SaveChangesAsync();

                // Gerar parcelas em Lancamentos
                if (compra.NrParcelas.HasValue && compra.NrParcelas.Value > 0 && total > 0)
                {
                    decimal valorParcela = Math.Round(total / compra.NrParcelas.Value, 2);
                    decimal acumulado    = 0;

                    for (int p = 1; p <= compra.NrParcelas.Value; p++)
                    {
                        decimal valorEsta = (p == compra.NrParcelas.Value)
                            ? total - acumulado
                            : valorParcela;

                        acumulado += valorEsta;

                        var lancamento = new Lancamento
                        {
                            Valor          = valorEsta,
                            ParcelaNr      = $"{p}/{compra.NrParcelas.Value}",
                            DataVencimento = compra.Data.AddDays(30 * p),
                            DataPagamento  = null,
                            VendaCodigo    = null,
                            CompraCodigo   = compra.Codigo,
                            SituacaoCodigo = 1   // Pendente
                        };
                        _context.Lancamentos.Add(lancamento);
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Sucesso"] = "Compra cadastrada com sucesso!";
                return RedirectToAction("Index");
            }

            await CarregarViewBags();
            return View(compra);
        }

        // GET /Compras/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var compra = await _context.Compras
                .Include(c => c.Fornecedor).ThenInclude(f => f!.Pessoa)
                .Include(c => c.Funcionario).ThenInclude(f => f!.Pessoa)
                .FirstOrDefaultAsync(c => c.Codigo == id);

            if (compra == null) return NotFound();

            var itens = await _context.CompraMateriais
                .Where(i => i.CompraCodigo == id)
                .OrderBy(i => i.Item)
                .ToListAsync();

            ViewBag.Itens = itens;
            await CarregarViewBags();
            return View(compra);
        }

        // POST /Compras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int           id,
            Compra        compra,
            List<int>     itemMaterialId,
            List<int>     itemQtd,
            List<decimal> itemPreco,
            List<string>  itemDimensoes,
            List<string>  itemDescricao)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            ModelState.Remove("Fornecedor");
            ModelState.Remove("Funcionario");

            if (ModelState.IsValid)
            {
                decimal total = 0;
                for (int i = 0; i < itemMaterialId.Count; i++)
                    if (itemMaterialId[i] != 0)
                        total += itemQtd[i] * itemPreco[i];

                compra.Codigo = id;
                compra.Total  = total;

                _context.Compras.Update(compra);

                // Remover itens antigos
                var itensAntigos = _context.CompraMateriais.Where(i => i.CompraCodigo == id);
                _context.CompraMateriais.RemoveRange(itensAntigos);
                await _context.SaveChangesAsync();

                // Inserir novos itens
                for (int i = 0; i < itemMaterialId.Count; i++)
                {
                    if (itemMaterialId[i] == 0) continue;

                    var item = new CompraMaterial
                    {
                        CompraCodigo   = id,
                        MaterialCodigo = itemMaterialId[i],
                        Item           = i + 1,
                        Qtd            = itemQtd[i],
                        Preco          = itemPreco[i],
                        Dimensoes      = itemDimensoes.Count > i ? itemDimensoes[i] : null,
                        Descricao      = itemDescricao.Count  > i ? itemDescricao[i]  : null
                    };
                    _context.CompraMateriais.Add(item);
                }

                await _context.SaveChangesAsync();

                // Recriar parcelas
                var lancamentosAntigos = _context.Lancamentos.Where(l => l.CompraCodigo == id);
                _context.Lancamentos.RemoveRange(lancamentosAntigos);
                await _context.SaveChangesAsync();

                if (compra.NrParcelas.HasValue && compra.NrParcelas.Value > 0 && total > 0)
                {
                    decimal valorParcela = Math.Round(total / compra.NrParcelas.Value, 2);
                    decimal acumulado    = 0;

                    for (int p = 1; p <= compra.NrParcelas.Value; p++)
                    {
                        decimal valorEsta = (p == compra.NrParcelas.Value)
                            ? total - acumulado
                            : valorParcela;

                        acumulado += valorEsta;

                        var lancamento = new Lancamento
                        {
                            Valor          = valorEsta,
                            ParcelaNr      = $"{p}/{compra.NrParcelas.Value}",
                            DataVencimento = compra.Data.AddDays(30 * p),
                            DataPagamento  = null,
                            VendaCodigo    = null,
                            CompraCodigo   = id,
                            SituacaoCodigo = 1
                        };
                        _context.Lancamentos.Add(lancamento);
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Sucesso"] = "Compra alterada com sucesso!";
                return RedirectToAction("Index");
            }

            var itensExistentes = await _context.CompraMateriais
                .Where(i => i.CompraCodigo == id)
                .OrderBy(i => i.Item)
                .ToListAsync();

            ViewBag.Itens = itensExistentes;
            await CarregarViewBags();
            return View(compra);
        }

        // POST /Compras/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // Remove lancamentos vinculados
            var lancamentos = _context.Lancamentos.Where(l => l.CompraCodigo == id);
            _context.Lancamentos.RemoveRange(lancamentos);

            // Remove itens
            var itens = _context.CompraMateriais.Where(i => i.CompraCodigo == id);
            _context.CompraMateriais.RemoveRange(itens);

            var compra = await _context.Compras.FindAsync(id);
            if (compra != null) _context.Compras.Remove(compra);

            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Compra excluída com sucesso!";
            return RedirectToAction("Index");
        }
    }
}

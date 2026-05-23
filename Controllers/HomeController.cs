// ============================================================
// Controllers/HomeController.cs
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // GET /Home/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UsuarioLogado") != null)
                return RedirectToAction("Dashboard");
            return View();
        }

        // POST /Home/Login
        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string senha)
        {
            var funcionario = await _context.Funcionarios
                .Include(f => f.Pessoa)
                .FirstOrDefaultAsync(f => f.Usuario == usuario && f.Senha == senha);

            if (funcionario == null)
            {
                ViewBag.Erro = "Usuário ou senha incorretos.";
                return View();
            }

            HttpContext.Session.SetString("UsuarioLogado", funcionario.Pessoa?.Nome ?? usuario);
            HttpContext.Session.SetInt32("FuncionarioId", funcionario.PessoaId);
            return RedirectToAction("Dashboard");
        }

        // GET /Home/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("UsuarioLogado") == null)
                return RedirectToAction("Login");

            var hoje      = DateTime.Today;
            var em30Dias  = hoje.AddDays(30);
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var fimMes    = inicioMes.AddMonths(1).AddDays(-1);

            // Contas a vencer nos próximos 30 dias (não pagas)
            var contasVencer = await _context.Lancamentos
                .Include(l => l.Situacao)
                .Where(l => l.DataPagamento == null &&
                            l.DataVencimento >= hoje &&
                            l.DataVencimento <= em30Dias)
                .OrderBy(l => l.DataVencimento)
                .ToListAsync();

            // Contas recebidas no mês vigente (venda_codigo != null = contas a receber)
            var contasRecebidas = await _context.Lancamentos
                .Where(l => l.DataPagamento != null &&
                            l.DataPagamento.Value >= inicioMes &&
                            l.DataPagamento.Value <= fimMes &&
                            l.VendaCodigo != null)
                .OrderByDescending(l => l.DataPagamento)
                .ToListAsync();

            // Últimas 5 vendas
            var ultimasVendas = await _context.Vendas
                .Include(v => v.Cliente).ThenInclude(c => c!.Pessoa)
                .Include(v => v.Funcionario).ThenInclude(f => f!.Pessoa)
                .OrderByDescending(v => v.Data)
                .Take(5)
                .ToListAsync();

            ViewBag.ContasVencer    = contasVencer;
            ViewBag.TotalVencer     = contasVencer.Sum(l => l.Valor ?? 0);
            ViewBag.ContasRecebidas = contasRecebidas;
            ViewBag.TotalRecebido   = contasRecebidas.Sum(l => l.Valor ?? 0);
            ViewBag.UltimasVendas   = ultimasVendas;
            ViewBag.Usuario         = HttpContext.Session.GetString("UsuarioLogado");

            return View();
        }

        // GET /Home/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

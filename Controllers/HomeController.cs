// ============================================================
// Controllers/HomeController.cs
// Responsável pelo Login e Dashboard
//
// No padrão MVC:
//   Controller = recebe a requisição, chama o banco, devolve View
//   Action     = cada método público é uma "página"
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class HomeController : Controller
    {
        // _context é a conexão com o banco (injetada automaticamente)
        private readonly AppDbContext _context;

        // O ASP.NET injeta o AppDbContext aqui automaticamente
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // ===========================================================
        // GET /Home/Login  — exibe o formulário de login
        // ===========================================================
        public IActionResult Login()
        {
            // Se já estiver logado, vai direto pro dashboard
            if (HttpContext.Session.GetString("UsuarioLogado") != null)
                return RedirectToAction("Dashboard");

            return View();
        }

        // ===========================================================
        // POST /Home/Login — processa o formulário de login
        // [HttpPost] = só responde quando o formulário for enviado
        // ===========================================================
        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string senha)
        {
            // Busca no banco um funcionário com usuário e senha informados
            // .Include(f => f.Pessoa) carrega também os dados pessoais
            var funcionario = await _context.Funcionarios
                .Include(f => f.Pessoa)
                .FirstOrDefaultAsync(f => f.Usuario == usuario && f.Senha == senha);

            if (funcionario == null)
            {
                // ViewBag transmite dados para a View sem precisar de Model
                ViewBag.Erro = "Usuário ou senha incorretos.";
                return View();
            }

            // Salva o nome do usuário na sessão (fica disponível em todas as páginas)
            HttpContext.Session.SetString("UsuarioLogado", funcionario.Pessoa?.Nome ?? usuario);
            HttpContext.Session.SetInt32("FuncionarioId", funcionario.PessoaId);

            return RedirectToAction("Dashboard");
        }
        
        // ===========================================================
        // GET /Home/Dashboard — página principal após login
        // ===========================================================
        public async Task<IActionResult> Dashboard()
        
        {
            // Verifica se está logado — se não, manda para o login
            if (HttpContext.Session.GetString("UsuarioLogado") == null)
                return RedirectToAction("Login");

            // Busca os lançamentos com vencimento próximo (próximos 30 dias)
            // e os que já foram pagos no mês atual
            var hoje = DateTime.Today;
            var fimMes = new DateTime(hoje.Year, hoje.Month,
                DateTime.DaysInMonth(hoje.Year, hoje.Month));

            // Contas a vencer: DataPagamento nula + vencimento até fim do mês
            var contasVencer = await _context.Lancamentos
                .Include(l => l.Situacao)
                .Where(l => l.DataPagamento == null &&
                            l.DataVencimento <= fimMes)
                .OrderBy(l => l.DataVencimento)
                .Take(10)   // limita a 10 registros
                .ToListAsync();

            // Contas pagas no mês atual
            var contasPagas = await _context.Lancamentos
                .Where(l => l.DataPagamento != null &&
                            l.DataPagamento.Value.Month == hoje.Month &&
                            l.DataPagamento.Value.Year == hoje.Year)
                .OrderByDescending(l => l.DataPagamento)
                .Take(10)
                .ToListAsync();

            // Passa os dados para a View via ViewBag
            ViewBag.ContasVencer = contasVencer;
            ViewBag.ContasPagas  = contasPagas;
            ViewBag.Usuario      = HttpContext.Session.GetString("UsuarioLogado");

            return View();
        
        }
  
        // ===========================================================
        // GET /Home/Logout — encerra a sessão e volta ao login
        // ===========================================================
        public IActionResult Logout()
        {
            // Remove todos os dados da sessão
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

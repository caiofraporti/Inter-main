// ============================================================
// Controllers/ClientesController.cs
// CRUD completo de Clientes
//
// Rotas geradas automaticamente pelo ASP.NET MVC:
//   GET  /Clientes         → Index  (listar)
//   GET  /Clientes/Create  → Create (formulário cadastro)
//   POST /Clientes/Create  → Create (salvar)
//   GET  /Clientes/Edit/5  → Edit   (formulário alterar)
//   POST /Clientes/Edit/5  → Edit   (salvar alteração)
//   POST /Clientes/Delete/5 → Delete (excluir)
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // Método auxiliar: verifica se o usuário está logado
        // Reutilizado em todas as Actions
        private bool VerificarLogin()
        {
            return HttpContext.Session.GetString("UsuarioLogado") != null;
        }

        // ===========================================================
        // GET /Clientes — lista todos os clientes
        // ===========================================================
        public async Task<IActionResult> Index()
        {
            //if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // JOIN entre Clientes e Pessoas via Include
            // Equivale a: SELECT * FROM Clientes c JOIN Pessoas p ON c.pessoa_id = p.id
            var clientes = await _context.Clientes
                .Include(c => c.Pessoa)   // carrega os dados da tabela Pessoas
                .ToListAsync();

            return View(clientes);
        }

        // ===========================================================
        // GET /Clientes/Create — exibe formulário de cadastro
        // ===========================================================
        public IActionResult Create()
        {
           // if (!VerificarLogin()) return RedirectToAction("Login", "Home");
            return View(new Pessoa());;
        }

        // ===========================================================
        // POST /Clientes/Create — recebe e salva o formulário
        // [HttpPost] = executado quando o botão "Cadastrar" é clicado
        // [ValidateAntiForgeryToken] = segurança contra CSRF
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pessoa pessoa)
        {
            //if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // ModelState.IsValid verifica as validações dos atributos do Model
            // Ex: [Required], [StringLength], [EmailAddress]
            if (ModelState.IsValid)
            {
                // 1) Salva a Pessoa primeiro (para gerar o Id)
                _context.Pessoas.Add(pessoa);
                await _context.SaveChangesAsync();

                // 2) Cria o vínculo na tabela Clientes com o Id gerado
                var cliente = new Cliente { PessoaId = pessoa.Id };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                // TempData exibe mensagem de sucesso na próxima tela
                TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
                return RedirectToAction("Index");
            }

            // Se os dados forem inválidos, volta ao formulário com os erros
            return View(pessoa);
        }

        // ===========================================================
        // GET /Clientes/Edit/5 — formulário para alterar cliente
        // id = código da pessoa (passado pela URL)
        // ===========================================================
        public async Task<IActionResult> Edit(int id)
        {
            //if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // Busca o cliente pelo id no banco
            var cliente = await _context.Clientes
                .Include(c => c.Pessoa)
                .FirstOrDefaultAsync(c => c.PessoaId == id);

            // Se não encontrar, retorna 404
            if (cliente == null) return NotFound();

            // Passa o objeto Pessoa para a View (para preencher o formulário)
            return View(cliente.Pessoa);
        }

        // ===========================================================
        // POST /Clientes/Edit/5 — salva as alterações
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pessoa pessoa)
        {
            //if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                // Garante que o Id do objeto bate com o da URL
                pessoa.Id = id;

                // _context.Update marca o objeto como "modificado"
                // Na próxima SaveChangesAsync ele executa o UPDATE
                _context.Pessoas.Update(pessoa);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Cliente alterado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(pessoa);
        }

        // ===========================================================
        // POST /Clientes/Delete/5 — exclui o cliente
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           // if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // Busca o registro na tabela Clientes
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                // 1) Remove o vínculo em Clientes primeiro (FK)
                _context.Clientes.Remove(cliente);

                // 2) Depois remove a Pessoa
                var pessoa = await _context.Pessoas.FindAsync(id);
                if (pessoa != null)
                    _context.Pessoas.Remove(pessoa);

                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Cliente excluído com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}

// ============================================================
// Controllers/MateriaisController.cs
// CRUD de Materiais
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class MateriaisController : Controller
    {
        private readonly AppDbContext _context;

        public MateriaisController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarLogin() =>
            HttpContext.Session.GetString("UsuarioLogado") != null;

        // GET /Materiais
        public async Task<IActionResult> Index()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lista = await _context.Materiais
                .OrderBy(m => m.NomeMaterial)
                .ToListAsync();

            return View(lista);
        }

        // GET /Materiais/Create
        public IActionResult Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");
            return View();
        }

        // POST /Materiais/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Material material)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                _context.Materiais.Add(material);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Material cadastrado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(material);
        }

        // GET /Materiais/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var material = await _context.Materiais.FindAsync(id);
            if (material == null) return NotFound();

            return View(material);
        }

        // POST /Materiais/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Material material)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                material.Codigo = id;
                _context.Materiais.Update(material);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Material alterado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(material);
        }

        // POST /Materiais/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var material = await _context.Materiais.FindAsync(id);
            if (material != null)
            {
                _context.Materiais.Remove(material);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Material excluído com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}

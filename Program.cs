// ============================================================
// Program.cs — Ponto de entrada da aplicação ASP.NET MVC
// Segue o mesmo padrão do professor (fatec-todo-list)
// ============================================================

using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// STRING DE CONEXÃO com o SQL Server
// Troque "localhost\\SQLEXPRESS" pelo nome do seu servidor
// Troque "MoveisCarrara" pelo nome do banco que você criou
// -----------------------------------------------------------
string connStr = "Server=localhost\\SQLEXPRESS;Database=MoveisCarrara;Trusted_Connection=True;TrustServerCertificate=True";

// -----------------------------------------------------------
// Registra o DbContext (Entity Framework) no sistema de
// injeção de dependência, informando qual banco usar
// -----------------------------------------------------------
builder.Services
    .AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connStr));

// -----------------------------------------------------------
// Habilita o padrão MVC (Controllers + Views)
// -----------------------------------------------------------
builder.Services.AddControllersWithViews();
// -----------------------------------------------------------
// Habilita sessão para guardar o usuário logado
// -----------------------------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // sessão expira em 30 min
    options.Cookie.HttpOnly = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// -----------------------------------------------------------
// Serve arquivos estáticos (CSS, JS, imagens da pasta wwwroot)
// -----------------------------------------------------------
app.UseStaticFiles();

// -----------------------------------------------------------
// Ativa o middleware de sessão
// -----------------------------------------------------------
app.UseRouting();
app.UseSession();

// -----------------------------------------------------------
// Define a rota padrão: ao abrir o sistema vai para Login
// Formato: /Controller/Action/id
// -----------------------------------------------------------
app.MapControllerRoute("default", "{controller=Home}/{action=Login}/{id?}");

app.Run();

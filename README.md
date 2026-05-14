# Móveis Carrara — Sistema de Gestão
**ASP.NET MVC 8 + Entity Framework + Bootstrap 5**

---

## 📁 Estrutura do Projeto

```
MoveisCarrara/
├── Controllers/
│   ├── HomeController.cs        ← Login + Dashboard
│   ├── ClientesController.cs    ← CRUD de Clientes
│   ├── FornecedoresController.cs← CRUD de Fornecedores
│   └── ContasController.cs      ← CRUD de Lançamentos
├── Models/
│   ├── Pessoa.cs                ← Tabela Pessoas
│   ├── Cliente.cs               ← Tabela Clientes
│   ├── Fornecedor.cs            ← Tabela Fornecedores
│   ├── Funcionario.cs           ← Tabela Funcionarios
│   ├── Situacao.cs              ← Tabela Situacao
│   └── Lancamento.cs            ← Tabela Lancamentos
├── Data/
│   └── AppDbContext.cs          ← Contexto do Entity Framework
├── Views/
│   ├── Shared/_Layout.cshtml   ← Layout com sidebar (padrão)
│   ├── Home/
│   │   ├── Login.cshtml
│   │   └── Dashboard.cshtml
│   ├── Clientes/
│   │   ├── Index.cshtml  (Listar)
│   │   ├── Create.cshtml (Cadastrar)
│   │   └── Edit.cshtml   (Alterar)
│   ├── Fornecedores/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   └── Edit.cshtml
│   └── Contas/
│       ├── Index.cshtml
│       ├── Create.cshtml
│       └── Edit.cshtml
├── Program.cs                   ← Ponto de entrada (igual ao professor)
└── MoveisCarrara.csproj
```

---

## ▶️ PASSO A PASSO PARA RODAR

### 1. Pré-requisitos
- .NET 8 SDK: https://dotnet.microsoft.com/download
- SQL Server Express instalado e rodando

### 2. Abrir a pasta no VS Code
```bash
cd MoveisCarrara
code .
```

### 3. Restaurar os pacotes (baixa o Entity Framework)
```bash
dotnet restore
```

### 4. ⚠️ IMPORTANTE — Ajustar a string de conexão
Abra `Program.cs` e altere a linha:
```csharp
string connStr = "Server=localhost\\SQLEXPRESS;Database=MoveisCarrara;...";
```
- `localhost\\SQLEXPRESS` → nome do seu servidor SQL Server
- `MoveisCarrara` → nome do banco (já criado pelo script SQL do professor)

### 5. Executar o projeto
```bash
dotnet run
```
Acesse no navegador: **http://localhost:5000**

### 6. Login
Use um dos usuários já inseridos no banco pelo script SQL:
| Usuário      | Senha    |
|-------------|----------|
| joao_user   | 123456   |
| maria_user  | 123456   |
| admin       | admin123 |

---

## 📚 EXPLICAÇÃO DO PADRÃO MVC (para apresentação)

### O que é MVC?
- **Model** = os dados (classes que representam as tabelas do banco)
- **View** = o HTML que o usuário vê
- **Controller** = o "cérebro" que conecta os dois

### Fluxo de uma requisição:
```
Usuário clica em "Listar Clientes"
       ↓
GET /Clientes  →  ClientesController.Index()
       ↓
_context.Clientes.Include(c => c.Pessoa).ToListAsync()
       ↓  (SELECT * FROM Clientes JOIN Pessoas)
List<Cliente>  →  Views/Clientes/Index.cshtml
       ↓
HTML renderizado para o usuário
```

### Entity Framework — principais comandos usados:
```csharp
// SELECT * FROM Tabela
await _context.Clientes.ToListAsync();

// SELECT com JOIN
await _context.Clientes.Include(c => c.Pessoa).ToListAsync();

// SELECT WHERE
await _context.Clientes.Where(c => c.PessoaId == id).FirstOrDefaultAsync();

// INSERT
_context.Clientes.Add(novoCliente);
await _context.SaveChangesAsync();

// UPDATE
_context.Clientes.Update(clienteAlterado);
await _context.SaveChangesAsync();

// DELETE
_context.Clientes.Remove(clienteParaExcluir);
await _context.SaveChangesAsync();
```

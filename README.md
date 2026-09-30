# Cadastro de Produtos — WPS (C# / ADO.NET / SQLite)

Aplicação console em **C# (.NET 6)** para cadastrar e gerenciar produtos, demonstrando uso de **ADO.NET** com **SQLite**, comandos SQL parametrizados, tratamento de exceções e registro de operações em arquivo de log.

---

## Pré-requisitos

| Requisito | Versão mínima |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 6.0 |

Verifique a instalação:

```bash
dotnet --version
```

---

## Estrutura do projeto

```
cs_development_cp05/
├── cadastro-produtos-plan.md       ← Plano de desenvolvimento
├── README.md                       ← Este arquivo
└── CadastroProdutos/
    ├── CadastroProdutos.csproj
    ├── appsettings.json            ← Connection string
    ├── Program.cs                  ← Menu interativo (UI)
    ├── DatabaseInitializer.cs      ← Cria banco e tabela na inicialização
    ├── Models/
    │   └── Produto.cs              ← Classe de modelo
    ├── Repositories/
    │   └── ProdutoRepository.cs    ← CRUD via ADO.NET (camada de dados)
    ├── Services/
    │   └── LogService.cs           ← Registro de operações em arquivo
    └── Scripts/
        └── create_table.sql        ← DDL de criação da tabela Produtos
```

---

## Como executar

### 1. Clone ou baixe o repositório

```bash
git clone <url-do-repositorio>
cd cs_development_cp05
```

### 2. Restaure as dependências

```bash
cd CadastroProdutos
dotnet restore
```

### 3. Execute a aplicação

```bash
dotnet run
```

---

## Banco de dados

O banco SQLite é criado **automaticamente** na primeira execução em:

```
CadastroProdutos/bin/Debug/net6.0/database/produtos.db
```

Não é necessário nenhuma configuração manual. O [`DatabaseInitializer`](CadastroProdutos/DatabaseInitializer.cs) cria o diretório `database/` e executa o script [`Scripts/create_table.sql`](CadastroProdutos/Scripts/create_table.sql) usando `CREATE TABLE IF NOT EXISTS`.

### Connection string

Definida em [`appsettings.json`](CadastroProdutos/appsettings.json):

```json
{
  "ConnectionStrings": {
    "SQLite": "Data Source=database/produtos.db"
  }
}
```

---

## Log de operações

Todas as operações (inserções, consultas, atualizações, exclusões e erros) são registradas em:

```
CadastroProdutos/bin/Debug/net6.0/logs/app.log
```

O diretório `logs/` é criado automaticamente. Formato de cada linha:

```
[2025-01-15 14:32:07] INSERT: produto 'Notebook Dell' inserido com sucesso.
[2025-01-15 14:32:20] SELECT ALL: 5 produto(s) listados.
[2025-01-15 14:32:35] DELETE: produto Id=3. Linhas afetadas: 1.
```

---

## Funcionalidades (menu)

| Opção | Descrição |
|---|---|
| 1 | **Inserir produto** — coleta Nome, Preço, Estoque e Categoria |
| 2 | **Listar produtos** — exibe todos os produtos em formato tabular |
| 3 | **Buscar por ID** — exibe detalhes de um produto específico |
| 4 | **Atualizar produto** — edita campos (Enter mantém o valor atual) |
| 5 | **Excluir produto** — exclui com confirmação prévia |
| 6 | **Sair** |

---

## Requisitos técnicos atendidos

- ✅ Conexão com SQLite via **ADO.NET** (`Microsoft.Data.Sqlite`)
- ✅ CRUD completo: `Inserir`, `Listar`, `BuscarPorId`, `Atualizar`, `Excluir`
- ✅ **SQL parametrizado** em todas as operações (prevenção contra SQL Injection)
- ✅ `ExecuteNonQuery` nos comandos INSERT, UPDATE e DELETE
- ✅ `ExecuteReader` nos comandos SELECT com mapeamento manual do `SqliteDataReader`
- ✅ Tratamento de `SqliteException` no repositório
- ✅ Registro de operações em arquivo (`logs/app.log`)
- ✅ Connection string em `appsettings.json`
- ✅ Separação de responsabilidades: `ProdutoRepository` (dados) × `Program.cs` (UI)
- ✅ Script SQL de criação da tabela em `Scripts/create_table.sql`

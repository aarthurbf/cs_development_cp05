using Microsoft.Extensions.Configuration;
using CadastroProdutos;
using CadastroProdutos.Models;
using CadastroProdutos.Repositories;
using CadastroProdutos.Services;

// Configuração
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = configuration.GetConnectionString("SQLite")
    ?? throw new InvalidOperationException("Connection string 'SQLite' não encontrada no appsettings.json.");

// Inicialização do banco
DatabaseInitializer.Initialize(connectionString);

var repo = new ProdutoRepository(connectionString);

LogService.Log("Aplicação iniciada.");

// Loop do menu
bool executando = true;
while (executando)
{
    ExibirMenu();
    var opcao = Console.ReadLine()?.Trim();

    switch (opcao)
    {
        case "1": InserirProduto(); break;
        case "2": ListarProdutos(); break;
        case "3": BuscarProdutoPorId(); break;
        case "4": AtualizarProduto(); break;
        case "5": ExcluirProduto(); break;
        case "6":
            LogService.Log("Aplicação encerrada pelo usuário.");
            Console.WriteLine("\nAté logo!");
            executando = false;
            break;
        default:
            Console.WriteLine("\n[!] Opção inválida. Digite um número de 1 a 6.");
            Pausar();
            break;
    }
}

// Métodos de menu

void ExibirMenu()
{
    Console.Clear();
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║      CADASTRO DE PRODUTOS - WPS      ║");
    Console.WriteLine("╠══════════════════════════════════════╣");
    Console.WriteLine("║  1. Inserir produto                  ║");
    Console.WriteLine("║  2. Listar produtos                  ║");
    Console.WriteLine("║  3. Buscar produto por ID            ║");
    Console.WriteLine("║  4. Atualizar produto                ║");
    Console.WriteLine("║  5. Excluir produto                  ║");
    Console.WriteLine("║  6. Sair                             ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.Write("Escolha uma opção: ");
}

void InserirProduto()
{
    Console.Clear();
    Console.WriteLine("=== INSERIR PRODUTO ===\n");

    var nome = LerString("Nome: ");
    if (nome is null) return;

    var preco = LerDecimal("Preço: ");
    if (preco is null) return;

    var estoque = LerInteiro("Estoque: ");
    if (estoque is null) return;

    var categoria = LerString("Categoria: ");
    if (categoria is null) return;

    var produto = new Produto
    {
        Nome      = nome,
        Preco     = preco.Value,
        Estoque   = estoque.Value,
        Categoria = categoria
    };

    try
    {
        repo.Inserir(produto);
        Console.WriteLine("\n[OK] Produto inserido com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[ERRO] {ex.Message}");
    }

    Pausar();
}

void ListarProdutos()
{
    Console.Clear();
    Console.WriteLine("=== LISTA DE PRODUTOS ===\n");

    try
    {
        var produtos = repo.Listar();

        if (produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
        }
        else
        {
            Console.WriteLine($"{"ID",-6} {"Nome",-30} {"Preço",10} {"Estoque",8} {"Categoria",-20}");
            Console.WriteLine(new string('─', 80));
            foreach (var p in produtos)
                Console.WriteLine(p);
            Console.WriteLine(new string('─', 80));
            Console.WriteLine($"Total: {produtos.Count} produto(s).");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] {ex.Message}");
    }

    Pausar();
}

void BuscarProdutoPorId()
{
    Console.Clear();
    Console.WriteLine("=== BUSCAR PRODUTO POR ID ===\n");

    var id = LerInteiro("ID do produto: ");
    if (id is null) return;

    try
    {
        var produto = repo.BuscarPorId(id.Value);

        if (produto is null)
            Console.WriteLine($"\n[!] Produto com ID {id} não encontrado.");
        else
        {
            Console.WriteLine();
            Console.WriteLine($"  ID       : {produto.Id}");
            Console.WriteLine($"  Nome     : {produto.Nome}");
            Console.WriteLine($"  Preço    : R$ {produto.Preco:F2}");
            Console.WriteLine($"  Estoque  : {produto.Estoque}");
            Console.WriteLine($"  Categoria: {produto.Categoria}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] {ex.Message}");
    }

    Pausar();
}

void AtualizarProduto()
{
    Console.Clear();
    Console.WriteLine("=== ATUALIZAR PRODUTO ===\n");

    var id = LerInteiro("ID do produto a atualizar: ");
    if (id is null) return;

    Produto? existente;
    try
    {
        existente = repo.BuscarPorId(id.Value);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] {ex.Message}");
        Pausar();
        return;
    }

    if (existente is null)
    {
        Console.WriteLine($"\n[!] Produto com ID {id} não encontrado.");
        Pausar();
        return;
    }

    Console.WriteLine($"\nProduto atual: {existente}\n");
    Console.WriteLine("Preencha os novos valores (Enter para manter o atual):\n");

    var nome      = LerStringOpcional($"Nome [{existente.Nome}]: ",      existente.Nome);
    var preco     = LerDecimalOpcional($"Preço [{existente.Preco:F2}]: ", existente.Preco);
    var estoque   = LerInteiroOpcional($"Estoque [{existente.Estoque}]: ", existente.Estoque);
    var categoria = LerStringOpcional($"Categoria [{existente.Categoria}]: ", existente.Categoria);

    existente.Nome      = nome;
    existente.Preco     = preco;
    existente.Estoque   = estoque;
    existente.Categoria = categoria;

    try
    {
        var atualizado = repo.Atualizar(existente);
        Console.WriteLine(atualizado
            ? "\n[OK] Produto atualizado com sucesso!"
            : "\n[!] Nenhuma linha foi afetada.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] {ex.Message}");
    }

    Pausar();
}

void ExcluirProduto()
{
    Console.Clear();
    Console.WriteLine("=== EXCLUIR PRODUTO ===\n");

    var id = LerInteiro("ID do produto a excluir: ");
    if (id is null) return;

    Produto? existente;
    try
    {
        existente = repo.BuscarPorId(id.Value);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] {ex.Message}");
        Pausar();
        return;
    }

    if (existente is null)
    {
        Console.WriteLine($"\n[!] Produto com ID {id} não encontrado.");
        Pausar();
        return;
    }

    Console.WriteLine($"\nProduto: {existente}");
    Console.Write("\nConfirma a exclusão? (s/N): ");
    var confirmacao = Console.ReadLine()?.Trim().ToLower();

    if (confirmacao != "s")
    {
        Console.WriteLine("Operação cancelada.");
        Pausar();
        return;
    }

    try
    {
        var excluido = repo.Excluir(id.Value);
        Console.WriteLine(excluido
            ? "\n[OK] Produto excluído com sucesso!"
            : "\n[!] Nenhuma linha foi afetada.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] {ex.Message}");
    }

    Pausar();
}

// Helpers de leitura com validação─

string? LerString(string label)
{
    Console.Write(label);
    var valor = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(valor))
    {
        Console.WriteLine("[!] Valor não pode ser vazio.");
        Pausar();
        return null;
    }
    return valor;
}

decimal? LerDecimal(string label)
{
    Console.Write(label);
    var entrada = Console.ReadLine()?.Trim().Replace(",", ".");
    if (!decimal.TryParse(entrada, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var valor) || valor < 0)
    {
        Console.WriteLine("[!] Valor numérico inválido.");
        Pausar();
        return null;
    }
    return valor;
}

int? LerInteiro(string label)
{
    Console.Write(label);
    var entrada = Console.ReadLine()?.Trim();
    if (!int.TryParse(entrada, out var valor) || valor < 0)
    {
        Console.WriteLine("[!] Número inteiro inválido.");
        Pausar();
        return null;
    }
    return valor;
}

string LerStringOpcional(string label, string valorAtual)
{
    Console.Write(label);
    var entrada = Console.ReadLine()?.Trim();
    return string.IsNullOrWhiteSpace(entrada) ? valorAtual : entrada;
}

decimal LerDecimalOpcional(string label, decimal valorAtual)
{
    Console.Write(label);
    var entrada = Console.ReadLine()?.Trim().Replace(",", ".");
    if (string.IsNullOrWhiteSpace(entrada)) return valorAtual;
    return decimal.TryParse(entrada, System.Globalization.NumberStyles.Any,
        System.Globalization.CultureInfo.InvariantCulture, out var valor) && valor >= 0
        ? valor
        : valorAtual;
}

int LerInteiroOpcional(string label, int valorAtual)
{
    Console.Write(label);
    var entrada = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(entrada)) return valorAtual;
    return int.TryParse(entrada, out var valor) && valor >= 0 ? valor : valorAtual;
}

void Pausar()
{
    Console.WriteLine("\nPressione Enter para continuar...");
    Console.ReadLine();
}

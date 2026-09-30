using Microsoft.Data.Sqlite;
using CadastroProdutos.Models;
using CadastroProdutos.Services;

namespace CadastroProdutos.Repositories;

public class ProdutoRepository
{
    private readonly string _connectionString;

    public ProdutoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // INSERT
    public bool Inserir(Produto produto)
    {
        const string sql = @"
            INSERT INTO Produtos (Nome, Preco, Estoque, Categoria)
            VALUES (@Nome, @Preco, @Estoque, @Categoria)";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Nome",      produto.Nome);
            cmd.Parameters.AddWithValue("@Preco",     (double)produto.Preco);
            cmd.Parameters.AddWithValue("@Estoque",   produto.Estoque);
            cmd.Parameters.AddWithValue("@Categoria", produto.Categoria);

            var linhasAfetadas = cmd.ExecuteNonQuery();
            LogService.Log($"INSERT: produto '{produto.Nome}' inserido com sucesso.");
            return linhasAfetadas > 0;
        }
        catch (SqliteException ex)
        {
            LogService.Log($"ERRO INSERT: {ex.Message}");
            throw new Exception($"Erro ao inserir produto: {ex.Message}", ex);
        }
    }

    // SELECT ALL
    public List<Produto> Listar()
    {
        const string sql = "SELECT Id, Nome, Preco, Estoque, Categoria FROM Produtos ORDER BY Id";
        var lista = new List<Produto>();

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                lista.Add(MapearProduto(reader));

            LogService.Log($"SELECT ALL: {lista.Count} produto(s) listados.");
            return lista;
        }
        catch (SqliteException ex)
        {
            LogService.Log($"ERRO SELECT ALL: {ex.Message}");
            throw new Exception($"Erro ao listar produtos: {ex.Message}", ex);
        }
    }

    // SELECT BY ID
    public Produto? BuscarPorId(int id)
    {
        const string sql = "SELECT Id, Nome, Preco, Estoque, Categoria FROM Produtos WHERE Id = @Id";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var produto = MapearProduto(reader);
                LogService.Log($"SELECT BY ID: produto Id={id} encontrado.");
                return produto;
            }

            LogService.Log($"SELECT BY ID: produto Id={id} não encontrado.");
            return null;
        }
        catch (SqliteException ex)
        {
            LogService.Log($"ERRO SELECT BY ID (Id={id}): {ex.Message}");
            throw new Exception($"Erro ao buscar produto: {ex.Message}", ex);
        }
    }

    // UPDATE
    public bool Atualizar(Produto produto)
    {
        const string sql = @"
            UPDATE Produtos
            SET Nome      = @Nome,
                Preco     = @Preco,
                Estoque   = @Estoque,
                Categoria = @Categoria
            WHERE Id = @Id";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id",        produto.Id);
            cmd.Parameters.AddWithValue("@Nome",      produto.Nome);
            cmd.Parameters.AddWithValue("@Preco",     (double)produto.Preco);
            cmd.Parameters.AddWithValue("@Estoque",   produto.Estoque);
            cmd.Parameters.AddWithValue("@Categoria", produto.Categoria);

            var linhasAfetadas = cmd.ExecuteNonQuery();
            LogService.Log($"UPDATE: produto Id={produto.Id} atualizado. Linhas afetadas: {linhasAfetadas}.");
            return linhasAfetadas > 0;
        }
        catch (SqliteException ex)
        {
            LogService.Log($"ERRO UPDATE (Id={produto.Id}): {ex.Message}");
            throw new Exception($"Erro ao atualizar produto: {ex.Message}", ex);
        }
    }

    // DELETE
    public bool Excluir(int id)
    {
        const string sql = "DELETE FROM Produtos WHERE Id = @Id";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            var linhasAfetadas = cmd.ExecuteNonQuery();
            LogService.Log($"DELETE: produto Id={id}. Linhas afetadas: {linhasAfetadas}.");
            return linhasAfetadas > 0;
        }
        catch (SqliteException ex)
        {
            LogService.Log($"ERRO DELETE (Id={id}): {ex.Message}");
            throw new Exception($"Erro ao excluir produto: {ex.Message}", ex);
        }
    }

    // Mapeamento manual DataReader → Produto
    private static Produto MapearProduto(SqliteDataReader reader)
    {
        return new Produto
        {
            Id        = reader.GetInt32(reader.GetOrdinal("Id")),
            Nome      = reader.GetString(reader.GetOrdinal("Nome")),
            Preco     = (decimal)reader.GetDouble(reader.GetOrdinal("Preco")),
            Estoque   = reader.GetInt32(reader.GetOrdinal("Estoque")),
            Categoria = reader.GetString(reader.GetOrdinal("Categoria"))
        };
    }
}

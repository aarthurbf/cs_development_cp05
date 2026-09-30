namespace CadastroProdutos.Models;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public string Categoria { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{Id:D4}] {Nome,-30} | R$ {Preco,8:F2} | Estoque: {Estoque,5} | {Categoria}";
    }
}

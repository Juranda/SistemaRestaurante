using SistemaRestaurante.Application.Repositorios;
using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class ProdutoRepositorioEmMemoria : IProdutoRepositorio
{
    private readonly List<Produto> _produtos;

    public ProdutoRepositorioEmMemoria()
    {
        _produtos =
        [
            Produto.Criar(1, "Frango Grelhado", 25.90, setorPreparoId: 3, disponivel: true).Value!,
            Produto.Criar(2, "Filé ao Molho",   45.00, setorPreparoId: 3, disponivel: true).Value!,
            Produto.Criar(3, "Suco de Laranja",  8.00, setorPreparoId: 2, disponivel: true).Value!,
            Produto.Criar(4, "Refrigerante",     6.00, setorPreparoId: 2, disponivel: true).Value!,
            Produto.Criar(5, "Prato do Dia",    22.00, setorPreparoId: 3, disponivel: false).Value!,
        ];
    }

    public Task<Produto?> ObterPorIdAsync(int id)
    {
        var produto = _produtos.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(produto);
    }

    public Task<IEnumerable<Produto>> ObterTodosAsync(int page, int pageSize)
    {
        var resultado = _produtos
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return Task.FromResult(resultado);
    }

    public Task<IEnumerable<Produto>> ObterTodosDisponiveisAsync()
    {
        var resultado = _produtos.Where(x => x.Disponivel);
        return Task.FromResult(resultado);
    }

    public Task<IEnumerable<Produto>> ObterTodosIndisponiveisAsync()
    {
        var resultado = _produtos.Where(x => !x.Disponivel);
        return Task.FromResult(resultado);
    }

    public Task<Produto> CriarAsync(Produto produto)
    {
        if (_produtos.Any(x => x.Id == produto.Id))
        {
            throw new InvalidOperationException(
                $"Já existe um produto com Id {produto.Id}");
        }

        _produtos.Add(produto);
        return Task.FromResult(produto);
    }

    public Task<Produto> AtualizarAsync(Produto produto)
    {
        var index = _produtos.FindIndex(x => x.Id == produto.Id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"Produto {produto.Id} não encontrado.");
        }

        _produtos[index] = produto;
        return Task.FromResult(produto);
    }

    public Task<bool> RemoverAsync(int id)
    {
        var produto = _produtos.FirstOrDefault(x => x.Id == id);

        if (produto is null)
            return Task.FromResult(false);

        _produtos.Remove(produto);
        return Task.FromResult(true);
    }
}
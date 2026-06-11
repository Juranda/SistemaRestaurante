using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Domain.Repositorios;

public interface ICardapioRepositorio
{
    public Task<Cardapio> ObterPorIdAsync(int id);
    public Task<Cardapio> CriarAsync(Cardapio cardapio);
    public Task<Cardapio> AtualizarAsync(Cardapio cardapio);
    public Task<bool> RemoverAsync(int id);
}

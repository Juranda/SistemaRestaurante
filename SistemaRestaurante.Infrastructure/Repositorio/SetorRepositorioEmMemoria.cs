using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class SetorRepositorioEmMemoria : ISetorRepositorio
{
    private readonly List<Setor> _setores;

    public SetorRepositorioEmMemoria()
    {
        _setores =
        [
            Setor.Criar(1,"Salão",TipoSetor.CRIA_PEDIDO).Value!,
            Setor.Criar(2, "Copa", TipoSetor.REALIZA_PEDIDO).Value!,
            Setor.Criar(3, "Cozinha", TipoSetor.REALIZA_PEDIDO).Value!,
        ];
    }

    public Task<IEnumerable<Setor>> ObterTodosAsync()
    {
        return Task.FromResult(
            _setores.AsEnumerable());
    }

    public Task<Setor?> ObterPorId(int id)
    {
        var setor = _setores.FirstOrDefault(
            x => x.Id == id);

        return Task.FromResult(setor);
    }

    public Task<Setor> CriarAsync(Setor setor)
    {
        if (_setores.Any(x => x.Id == setor.Id))
        {
            throw new InvalidOperationException(
                $"Já existe um setor com Id {setor.Id}");
        }

        _setores.Add(setor);

        return Task.FromResult(setor);
    }

    public Task<Setor> AtualizarAsync(Setor setor)
    {
        var index = _setores.FindIndex(
            x => x.Id == setor.Id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"Setor {setor.Id} não encontrado.");
        }

        _setores[index] = setor;

        return Task.FromResult(setor);
    }

    public Task<bool> RemoverAsync(int id)
    {
        var setor = _setores.FirstOrDefault(
            x => x.Id == id);

        if (setor is null)
        {
            return Task.FromResult(false);
        }

        _setores.Remove(setor);

        return Task.FromResult(true);
    }
}
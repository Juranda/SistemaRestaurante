using SistemaRestaurante.Application.DTO.Autenticacao;

namespace SistemaRestaurante.Application.InterfacesDeServicos;

public interface IAutenticacaoServico
{
    public Task LogarUsuario(LogarUsuario usuario);
    public Task DesconectarUsuario();
}
using System.Data.SqlTypes;
using SistemaRestaurante.Application.DTO.Autenticacao;
using SistemaRestaurante.Application.InterfacesDeServicos;
using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Errors;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Application.UseCases;

public class UsuarioLogaUseCase(
    IUsuarioRepositorio usuarioRepositorio,
    IAutenticacaoServico autenticacaoServico,
    ISenhaHasher senhaHasher,
    ISetorRepositorio setorRepositorio
)
{
    private readonly IUsuarioRepositorio usuarioRepositorio = usuarioRepositorio;
    private readonly IAutenticacaoServico autenticacaoServico = autenticacaoServico;
    private readonly ISenhaHasher senhaHasher = senhaHasher;
    private readonly ISetorRepositorio setorRepositorio = setorRepositorio;

    public async Task<Result<UsuarioLogado>> Execute(string nome, string senha)
    {
        Usuario? usuario = await usuarioRepositorio.ObterUsuarioAsync(nome);

        if (usuario is null)
        {
            return ErrosUsuario.UsuarioNaoExiste();
        }

        Setor? setor = await setorRepositorio.ObterPorId(usuario.SetorId);

        if (setor is null)
        {
            return ErrosSetor.SetorNaoExiste();
        }

        try
        {
            bool senhaCorreta = await senhaHasher.VerificarSenha(senha, usuario.HashSenha);

            if (senhaCorreta == false)
            {
                return ErrosUsuario.UsuarioNaoExiste();
            }

            var logarUsuario = new LogarUsuario(
                usuario.Id,
                usuario.Nome,
                setor.Id,
                setor.Nome
            );

            await autenticacaoServico.LogarUsuario(logarUsuario);

            return new UsuarioLogado(
                logarUsuario.Id,
                logarUsuario.Nome,
                Setor: setor
            );
        }
        catch
        {
            return ErrosUsuario.UsuarioNaoExiste();
        }
    }
}

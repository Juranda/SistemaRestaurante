using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Usuario
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public const int MIN_CARACTERES_NOME = 5;
    public const int MAX_CARACTERES_NOME = 255;
    public string HashSenha { get; private set; }
    public const int MIN_CARACTERES_SENHA = 5;
    public const int MAX_CARACTERES_SENHA = 255;
    public int SetorId { get; private set; }

    private Usuario(int id, string nome, string hashSenha, int setorId)
    {
        Id = id;
        Nome = nome;
        HashSenha = hashSenha;
        SetorId = setorId;
    }

    public static Result<Usuario> Criar(int id, string nome, string hashSenha, int setorId)
    {
        var usuario = new Usuario(id, nome, hashSenha, setorId);
        var result = usuario.Validar();

        if (result.IsError)
        {
            return (Result<Usuario>)result;
        }

        return usuario;
    }

    public Result Atualizar(string nome, string hashSenha, int setorId)
    {
        var usuario = new Usuario(Id, nome, hashSenha, setorId);
        var result = usuario.Validar();

        if (result.IsError)
        {
            return (Result<Usuario>)result;
        }

        Nome = nome;
        HashSenha = hashSenha;
        SetorId = setorId;
        return Result.Success();
    }

    public Result Validar()
    {
        return Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(Nome), Nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarTexto(nameof(HashSenha), HashSenha, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarNumero(nameof(SetorId), SetorId, 1, int.MaxValue)
        );
    }
}

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
        var result = Result.All(
            Validacoes.ValidarNumero(nameof(id), id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(nome), nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarTexto(nameof(hashSenha), hashSenha, MIN_CARACTERES_SENHA, MAX_CARACTERES_SENHA),
            Validacoes.ValidarNumero(nameof(setorId), setorId, 1, int.MaxValue)
        );

        if (result.IsError)
        {
            return (Result<Usuario>)result;
        }

        return new Usuario(id, nome, hashSenha, setorId);
    }

    public Result Atualizar(string nome, string passwordHash, int setorId)
    {
        var result = Result.All(
            Validacoes.ValidarTexto(nameof(nome), nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarTexto(nameof(passwordHash), passwordHash, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarNumero(nameof(setorId), setorId, 1, int.MaxValue)
        );

        if (result.IsError)
        {
            return (Result<Usuario>)result;
        }

        Nome = nome;
        return Result.Success();
    }
}

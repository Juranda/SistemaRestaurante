using SistemaRestaurante.Application.InterfacesDeServicos;

namespace SistemaRestaurante.Infrastructure.Servicos;

public class BCryptSenhaHasher : ISenhaHasher
{
    public Task<string> FazerHash(string senha)
    {
        return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 12));
    }

    public Task<bool> VerificarSenha(string senha, string hash)
    {
        return Task.FromResult(BCrypt.Net.BCrypt.Verify(senha, hash));
    }
}
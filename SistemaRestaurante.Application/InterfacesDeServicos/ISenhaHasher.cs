namespace SistemaRestaurante.Application.InterfacesDeServicos;

public interface ISenhaHasher
{
    public Task<string> FazerHash(string senha);
    public Task<bool> VerificarSenha(string senha, string hash);
}
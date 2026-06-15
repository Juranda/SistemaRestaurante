using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class UsuarioRepositorioEmMemoria : IUsuarioRepositorio
{
    private readonly List<Usuario> _usuarios = [];

    public UsuarioRepositorioEmMemoria()
    {
        var ganconResult = Usuario.Criar(
            id: 1,
            nome: "Garçon",
            hashSenha: "$2a$12$VEYlve5PMZs6B.0WyS7O4uhwcrRexc62CNZmTp2OEthHI7ZSzH4lm",
            setorId: 1
        );

        var copaResult = Usuario.Criar(
            2,
            "Copa",
            "$2a$12$VEYlve5PMZs6B.0WyS7O4uhwcrRexc62CNZmTp2OEthHI7ZSzH4lm",
            2
        );

        var cozinhaResult = Usuario.Criar(
            3,
            "Cozinha",
            "$2a$12$VEYlve5PMZs6B.0WyS7O4uhwcrRexc62CNZmTp2OEthHI7ZSzH4lm",
            3
        );

        
        if(ganconResult.IsError || copaResult.IsError || cozinhaResult.IsError)
        {
            throw new Exception("Usuarios invalidos");
        }

        _usuarios =
        [
            ganconResult.Value!, 
            copaResult.Value!, 
            cozinhaResult.Value!
        ];
    }

    public Task<Usuario> CriarAsync(Usuario usuario)
    {
        _usuarios.Add(usuario);

        return Task.FromResult(usuario);
    }

    public Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        var index = _usuarios.FindIndex(x => x.Id == usuario.Id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"Usuário {usuario.Id} não encontrado.");
        }

        _usuarios[index] = usuario;

        return Task.FromResult(usuario);
    }

    public Task<bool> ExisteAsync(string nome)
    {
        bool existe = _usuarios.Any(x =>
            x.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(existe);
    }

    public Task<Usuario?> ObterUsuarioAsync(string nome)
    {
        Usuario? usuario = _usuarios.FirstOrDefault(x =>
            x.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(usuario);
    }

    public Task<bool> RemoverAsync(int id)
    {
        Usuario? usuario =
            _usuarios.FirstOrDefault(x => x.Id == id);

        if (usuario is null)
        {
            return Task.FromResult(false);
        }

        _usuarios.Remove(usuario);

        return Task.FromResult(true);
    }
}
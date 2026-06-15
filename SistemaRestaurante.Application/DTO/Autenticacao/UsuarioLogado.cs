using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Application.DTO.Autenticacao;

public record UsuarioLogado(
    int Id,
    string Nome,
    Setor Setor
);
using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Application.DTO.Autenticacao;

public record LogarUsuario(
    int Id,
    string Nome,
    int SetorId,
    string SetorNome,
    TipoSetor SetorTipo
);
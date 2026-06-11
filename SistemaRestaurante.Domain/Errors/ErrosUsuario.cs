

namespace SistemaRestaurante.Domain.Errors;

public static class ErrosUsuario
{
    public static Error UsuarioInvalido(IReadOnlyList<Error> errors) => new Error("USER-DOM-001", "Valores invalidos para usuario", ErrorTypes.Validation, ValidationErrors.GetFieldErros(errors));
    public static Error CredenciaisInvalidas() => new Error("USER-APP-001", "Credenciais invalidas", ErrorTypes.Validation);
    public static Error NaoAutorizado() => new Error("USER-APP-002", "Não autorizado", ErrorTypes.Unauthorized);
    public static Error UsuarioNaoExiste() => new Error("USER-INF-001", "Usuario não existe", ErrorTypes.NotFound);
    public static Error UsuarioJaExiste() => new Error("USER-INF-002", "Usuario já existe", ErrorTypes.Conflict);
}
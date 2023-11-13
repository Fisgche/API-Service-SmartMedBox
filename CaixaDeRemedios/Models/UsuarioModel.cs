namespace CaixaDeRemedios.Models
{
    public class UsuarioModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
        public string Id { get; set; } = string.Empty;
        public bool IsAdministrador { get; set; } 
        public string ChaveEsp32 { get; set; } = string.Empty;
    }
}

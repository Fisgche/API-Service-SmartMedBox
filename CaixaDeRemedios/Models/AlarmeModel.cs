namespace CaixaDeRemedios.Models
{
    public class AlarmeModel
    {

        public string Id { get; set; } = string.Empty;

        public string IdRemedio { get; set; } = string.Empty;

        public string IdUsuario { get; set; } = string.Empty;

        public DateTime DataHoraCadastro { get; set; }

        public bool Ativo = true;


    }
}

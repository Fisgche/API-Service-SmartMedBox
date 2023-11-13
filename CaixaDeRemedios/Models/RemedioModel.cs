namespace CaixaDeRemedios.Models
{
    public class RemedioModel
    {

        public string IdUsuario { get; set; } =  string.Empty;

        public string Id { get; set; } = string.Empty;

        public string NomeRemedio { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public DateTime DataHoraCadastro { get; set; }

        public string HorarioInicio { get; set; } = string.Empty;

        public string HorarioProximoRemedio { get; set; } = string.Empty;

        public int Frequencia { get; set; }

        public int Recipiente { get; set; }
    }
}

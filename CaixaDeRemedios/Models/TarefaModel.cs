namespace CaixaDeRemedios.Models
{
    public class TarefaModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public int Status { get; set; }

        public TarefaModel() { }    
    }
}

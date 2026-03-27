namespace EduQuizAI.API.Models;

public class Evaluacion
{
    public int Id { get; set; }
    public string Tema { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string Asignatura { get; set; } = string.Empty;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public List<Pregunta> Preguntas { get; set; } = new();
}

public class Pregunta
{
    public int Id { get; set; }
    public int EvaluacionId { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public string OpcionA { get; set; } = string.Empty;
    public string OpcionB { get; set; } = string.Empty;
    public string OpcionC { get; set; } = string.Empty;
    public string OpcionD { get; set; } = string.Empty;
    public int RespuestaCorrectaIndex { get; set; }
    public Evaluacion? Evaluacion { get; set; }
}
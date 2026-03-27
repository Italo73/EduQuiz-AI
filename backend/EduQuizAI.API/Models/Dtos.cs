namespace EduQuizAI.API.Models;

public record GenerarRequest(
    string Tema,
    string Nivel,
    string Asignatura,
    int Cantidad = 5
);

public record PreguntaDto(
    string Enunciado,
    string OpcionA,
    string OpcionB,
    string OpcionC,
    string OpcionD,
    int RespuestaCorrectaIndex
);
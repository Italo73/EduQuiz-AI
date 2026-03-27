using System.Text;
using System.Text.Json;
using EduQuizAI.API.Models;

namespace EduQuizAI.API.Services;

public interface IAIService
{
    Task<List<Pregunta>> GenerarPreguntas(string tema, string nivel, string asignatura, int cantidad);
}

public class ClaudeService : IAIService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<ClaudeService> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ClaudeService(HttpClient http, IConfiguration config, ILogger<ClaudeService> logger)
    {
        _http = http;
        _apiKey = config["Claude:ApiKey"]
            ?? throw new InvalidOperationException("Claude:ApiKey no configurada en appsettings.");
        _logger = logger;
    }

    public async Task<List<Pregunta>> GenerarPreguntas(
        string tema, string nivel, string asignatura, int cantidad)
    {
        var prompt = "Eres un profesor experto creando material educativo para colegios chilenos.\n\n" +
                        $"Genera exactamente {cantidad} preguntas de selección múltiple sobre:\n" +
                        $"- Tema: {tema}\n" +
                        $"- Asignatura: {asignatura}\n" +
                        $"- Nivel: {nivel}\n\n" +
                        "Requisitos:\n" +
                        "- Preguntas claras, precisas y apropiadas para el nivel\n" +
                        "- Exactamente 4 opciones por pregunta (A, B, C, D)\n" +
                        "- Solo una respuesta correcta por pregunta\n" +
                        "- Opciones plausibles (no trampas obvias)\n\n" +
                        "Responde ÚNICAMENTE con un JSON array válido, sin texto adicional, sin markdown:\n" +
                        "[\n" +
                        "  {\n" +
                        "    \"enunciado\": \"¿Texto de la pregunta?\",\n" +
                        "    \"opcionA\": \"Primera opción\",\n" +
                        "    \"opcionB\": \"Segunda opción\",\n" +
                        "    \"opcionC\": \"Tercera opción\",\n" +
                        "    \"opcionD\": \"Cuarta opción\",\n" +
                        "    \"respuestaCorrectaIndex\": 0\n" +
                        "  }\n" +
                        "]\n\n" +
                        "respuestaCorrectaIndex debe ser 0=A, 1=B, 2=C, 3=D.";

        var requestBody = new
        {
            model = "claude-sonnet-4-20250514",
            max_tokens = 2000,
            messages = new[] { new { role = "user", content = prompt } }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
        httpRequest.Headers.Add("x-api-key", _apiKey);
        httpRequest.Headers.Add("anthropic-version", "2023-06-01");
        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        _logger.LogInformation("Generando {Cantidad} preguntas sobre '{Tema}' para {Nivel}", cantidad, tema, nivel);

        var response = await _http.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var claudeResponse = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

        var rawText = claudeResponse
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString()!
            .Trim();

        if (rawText.StartsWith("```"))
        {
            rawText = rawText
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();
        }

        var dtos = JsonSerializer.Deserialize<List<PreguntaDto>>(rawText, JsonOpts)
            ?? throw new Exception("No se pudo parsear la respuesta de Claude.");

        return dtos.Select(d => new Pregunta
        {
            Enunciado = d.Enunciado,
            OpcionA = d.OpcionA,
            OpcionB = d.OpcionB,
            OpcionC = d.OpcionC,
            OpcionD = d.OpcionD,
            RespuestaCorrectaIndex = d.RespuestaCorrectaIndex
        }).ToList();
    }
}
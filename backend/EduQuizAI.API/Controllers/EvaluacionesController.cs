using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduQuizAI.API.Data;
using EduQuizAI.API.Models;
using EduQuizAI.API.Services;

namespace EduQuizAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluacionesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAIService _ai;
    private readonly ILogger<EvaluacionesController> _logger;

    public EvaluacionesController(AppDbContext db, IAIService ai, ILogger<EvaluacionesController> logger)
    {
        _db = db;
        _ai = ai;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var evaluaciones = await _db.Evaluaciones
            .Include(e => e.Preguntas)
            .OrderByDescending(e => e.CreadoEn)
            .ToListAsync();

        return Ok(evaluaciones);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var evaluacion = await _db.Evaluaciones
            .Include(e => e.Preguntas)
            .FirstOrDefaultAsync(e => e.Id == id);

        return evaluacion is null ? NotFound() : Ok(evaluacion);
    }

    [HttpPost("generar")]
    public async Task<IActionResult> Generar([FromBody] GenerarRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Tema))
            return BadRequest(new { error = "El tema no puede estar vacío." });

        if (req.Cantidad < 1 || req.Cantidad > 20)
            return BadRequest(new { error = "La cantidad debe estar entre 1 y 20." });

        try
        {
            var preguntas = await _ai.GenerarPreguntas(req.Tema, req.Nivel, req.Asignatura, req.Cantidad);

            var evaluacion = new Evaluacion
            {
                Tema = req.Tema,
                Nivel = req.Nivel,
                Asignatura = req.Asignatura,
                Preguntas = preguntas
            };

            _db.Evaluaciones.Add(evaluacion);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Evaluación #{Id} creada con {N} preguntas", evaluacion.Id, preguntas.Count);

            return CreatedAtAction(nameof(GetById), new { id = evaluacion.Id }, evaluacion);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error llamando a la API de Claude");
            return StatusCode(502, new { error = "Error al comunicarse con la IA. Verifica tu API Key." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando evaluación");
            return StatusCode(500, new { error = "Error interno al generar la evaluación." });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var evaluacion = await _db.Evaluaciones.FindAsync(id);
        if (evaluacion is null) return NotFound();

        _db.Evaluaciones.Remove(evaluacion);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
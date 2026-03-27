using Microsoft.EntityFrameworkCore;
using EduQuizAI.API.Models;

namespace EduQuizAI.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Evaluacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tema).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Nivel).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Asignatura).IsRequired().HasMaxLength(100);
            entity.HasMany(e => e.Preguntas)
                  .WithOne(p => p.Evaluacion)
                  .HasForeignKey(p => p.EvaluacionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Pregunta>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Enunciado).IsRequired();
        });
    }
}
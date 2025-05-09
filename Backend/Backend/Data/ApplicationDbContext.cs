using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Escola> Escolas { get; set; }
    public DbSet<Utilizador> Utilizadores { get; set; }
    public DbSet<Sala> Salas { get; set; }
    public DbSet<UC> UCs { get; set; }
    public DbSet<Turma> Turmas { get; set; }
    public DbSet<BlocoHorario> BlocosHorario { get; set; }
    public DbSet<Curso> Cursos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração para evitar múltiplos caminhos de exclusão em cascata
        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Turma)
            .WithMany(t => t.BlocosHorario)
            .HasForeignKey(b => b.TurmaFK)
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata

        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Professor)
            .WithMany(p => p.BlocosHorario)
            .HasForeignKey(b => b.ProfessorFK)
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata

        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Sala)
            .WithMany(s => s.BlocosHorario)
            .HasForeignKey(b => b.SalaFK)
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata

        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.UnidadeCurricular)
            .WithMany()
            .HasForeignKey(b => b.UnidadeCurricularFK)
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata

        // Configuração para outras entidades, se necessário
        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Curso)
            .WithMany()
            .HasForeignKey(t => t.CursoFK)
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata
    }
}


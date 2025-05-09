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

        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Disciplina)
            .WithMany(uc => uc.Turmas)
            .HasForeignKey(t => t.DisciplinaFK)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Escola>()
            .Property(e => e.Nome)
            .IsRequired();

        modelBuilder.Entity<Sala>()
            .Property(s => s.Nome)
            .IsRequired();

        modelBuilder.Entity<UC>()
            .Property(uc => uc.NomeUC)
            .IsRequired();

        modelBuilder.Entity<Utilizador>()
            .Property(u => u.Nome)
            .IsRequired();

        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Turma)
            .WithMany(t => t.BlocosHorario)
            .HasForeignKey(b => b.TurmaFK)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.UnidadeCurricular)
            .WithMany()
            .HasForeignKey(b => b.UnidadeCurricularFK)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

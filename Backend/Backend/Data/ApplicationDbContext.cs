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

    //TODO: REVER BEM ISTO
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Utilizador-Escola one-to-many relationship
        modelBuilder.Entity<Utilizador>()
            .HasOne(u => u.Escola)
            .WithMany()
            .HasForeignKey(u => u.EscolaFK)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Escola-Utilizador many-to-many relationship (MembrosComissaoHorarios)
        modelBuilder.Entity<Escola>()
            .HasMany(e => e.MembrosComissaoHorarios)
            .WithMany(u => u.EscolasOndeEnsina)
            .UsingEntity(j => j.ToTable("EscolaUtilizadorComissao"));

        // Configure UC-Utilizador many-to-many relationship (Docentes)
        modelBuilder.Entity<UC>()
            .HasMany(uc => uc.Docentes)
            .WithMany(u => u.DisciplinasLecionadas)
            .UsingEntity(j => j.ToTable("UCDocentes"));

        // BlocoHorario - Tipologia
        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Tipologia)
            .WithMany()
            .HasForeignKey(b => b.TipologiaFK)
            .OnDelete(DeleteBehavior.Restrict);

        // BlocoHorario - Disciplina
        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Disciplina)
            .WithMany()
            .HasForeignKey(b => b.DisciplinaFK)
            .OnDelete(DeleteBehavior.Restrict);

        // BlocoHorario - Professor
        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Professor)
            .WithMany()
            .HasForeignKey(b => b.ProfessorFK)
            .OnDelete(DeleteBehavior.Restrict);

        // BlocoHorario - Sala
        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Sala)
            .WithMany()
            .HasForeignKey(b => b.SalaFK)
            .OnDelete(DeleteBehavior.Restrict);

        // BlocoHorario - Turma
        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Turma)
            .WithMany()
            .HasForeignKey(b => b.TurmaFK)
            .OnDelete(DeleteBehavior.Restrict);
    }

}

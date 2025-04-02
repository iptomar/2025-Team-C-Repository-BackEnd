using Backend_v02.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend_v02.Data;

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

        // Fix BlocoHorario issue with Tipologia
        modelBuilder.Entity<BlocoHorario>()
            .HasOne(b => b.Tipologia)
            .WithMany()
            .HasForeignKey(b => b.TipologiaFK)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

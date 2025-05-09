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

}

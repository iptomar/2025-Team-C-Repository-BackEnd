using Backend.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace Backend.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Ver se já existem dados
            if (context.Escolas.Any() || context.Utilizadores.Any())
            {
                return;
            }

            // Adicionar escolas
            var escolas = new Escola[]
            {
                new Escola { Nome = "Escola Superior de Tecnologia de Tomar", Localizacao = "Tomar" },
                new Escola { Nome = "Escola Superior de Gestão de Tomar", Localizacao = "Tomar" },
                new Escola { Nome = "Escola Superior de Tecnologia de Abrantes", Localizacao = "Abrantes" }
            };
            context.Escolas.AddRange(escolas);
            context.SaveChanges();

            // Adicionar Roles
            string[] roles = new string[] { "Administrador", "Docente", "MembroComissao" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Adicionar users
            var utilizadores = new Utilizador[]
            {
                new Utilizador { Nome = "João Silva", Email = "joao.silva@ipt.pt", Funcao = "Docente", EscolaFK = escolas[0].IdEscola },
                new Utilizador { Nome = "Ana Costa", Email = "ana.costa@ipt.pt", Funcao = "Docente", EscolaFK = escolas[1].IdEscola },
                new Utilizador { Nome = "Carlos Santos", Email = "carlos.santos@ipt.pt", Funcao = "Docente", EscolaFK = escolas[2].IdEscola },
                new Utilizador { Nome = "Maria Lopes", Email = "maria.lopes@ipt.pt", Funcao = "MembroComissao", EscolaFK = escolas[0].IdEscola },
                new Utilizador { Nome = "António Ferreira", Email = "antonio.ferreira@ipt.pt", Funcao = "Administrador" }
            };
            context.Utilizadores.AddRange(utilizadores);
            context.SaveChanges();

            // Adicionar salas
            var salas = new Sala[]
            {
                new Sala { Nome = "A101", Lugares = 30, TipoSala = "Sala de Aula", Localizacao = "Bloco A", EscolaFK = escolas[0].IdEscola },
                new Sala { Nome = "B203", Lugares = 25, TipoSala = "Laboratório", Localizacao = "Bloco B", EscolaFK = escolas[0].IdEscola },
                new Sala { Nome = "C105", Lugares = 50, TipoSala = "Auditório", Localizacao = "Bloco C", EscolaFK = escolas[1].IdEscola },
                new Sala { Nome = "D001", Lugares = 20, TipoSala = "Sala de Computadores", Localizacao = "Bloco D", EscolaFK = escolas[2].IdEscola }
            };
            context.Salas.AddRange(salas);
            context.SaveChanges();

            // Adicionar cursos
            var ucs = new UC[]
            {
                new UC { NomeDisciplina = "Programação", TipoDisciplina = "Teórica/Prática", GrauAcademico = "Licenciatura", Semestre = "1º Semestre" },
                new UC { NomeDisciplina = "Algoritmos", TipoDisciplina = "Teórica", GrauAcademico = "Licenciatura", Semestre = "1º Semestre" },
                new UC { NomeDisciplina = "Bases de Dados", TipoDisciplina = "Prática", GrauAcademico = "Licenciatura", Semestre = "2º Semestre" },
                new UC { NomeDisciplina = "Sistemas de Informação", TipoDisciplina = "Teórica/Prática", GrauAcademico = "Mestrado", Semestre = "2º Semestre" }
            };
            context.UCs.AddRange(ucs);
            context.SaveChanges();

            // Adicionar turmas
            var turmas = new Turma[]
            {
                new Turma { Nome = "Turma A", DisciplinaFK = ucs[0].IdDisciplina },
                new Turma { Nome = "Turma B", DisciplinaFK = ucs[0].IdDisciplina },
                new Turma { Nome = "Turma C", DisciplinaFK = ucs[1].IdDisciplina },
                new Turma { Nome = "Turma D", DisciplinaFK = ucs[2].IdDisciplina },
                new Turma { Nome = "Turma E", DisciplinaFK = ucs[3].IdDisciplina }
            };
            context.Turmas.AddRange(turmas);
            context.SaveChanges();

            // Adicionar blocos do horário
            var blocosHorario = new BlocoHorario[]       //TODO: tem de se ajeitar o dia da semana para ser mostrado em português
            {
                new BlocoHorario {
                    HoraInicio = new TimeSpan(9, 0, 0),
                    HoraFim = new TimeSpan(11, 0, 0),
                    DiaSemana = DayOfWeek.Monday,
                    ProfessorFK = utilizadores[0].IdUtilizador,
                    DisciplinaFK = ucs[0].IdDisciplina,
                    SalaFK = salas[0].IdSala,
                    TurmaFK = turmas[0].IdTurma,
                    TipologiaFK = ucs[0].IdDisciplina
                },
                new BlocoHorario {
                    HoraInicio = new TimeSpan(14, 0, 0),
                    HoraFim = new TimeSpan(16, 0, 0),
                    DiaSemana = DayOfWeek.Wednesday,
                    ProfessorFK = utilizadores[1].IdUtilizador,
                    DisciplinaFK = ucs[1].IdDisciplina,
                    SalaFK = salas[1].IdSala,
                    TurmaFK = turmas[2].IdTurma,
                    TipologiaFK = ucs[1].IdDisciplina
                }
            };

            /*public static string DiaSemanaEmPortugues(this DayOfWeek diaSemana)
            {
                return diaSemana switch
                {
                    DayOfWeek.Monday => "Segunda-feira",
                    DayOfWeek.Tuesday => "Terça-feira",
                    DayOfWeek.Wednesday => "Quarta-feira",
                    DayOfWeek.Thursday => "Quinta-feira",
                    DayOfWeek.Friday => "Sexta-feira",
                    DayOfWeek.Saturday => "Sábado",
                    DayOfWeek.Sunday => "Domingo",
                    _ => "Desconhecido"
                };
            }*/
            context.BlocosHorario.AddRange(blocosHorario);
            context.SaveChanges();

            // ligar professores às aulas/cursos
            utilizadores[0].DisciplinasLecionadas = new UC[] { ucs[0], ucs[2] };
            utilizadores[1].DisciplinasLecionadas = new UC[] { ucs[1] };
            utilizadores[2].DisciplinasLecionadas = new UC[] { ucs[3] };

            context.SaveChanges();
        }
    }
}
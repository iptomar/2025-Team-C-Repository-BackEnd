using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Garante que a base de dados está criada
            context.Database.EnsureCreated();

            // Verifica se a base de dados já tem dados
            if (!roleManager.Roles.Any())
            {
                await CreateRolesAsync(roleManager);
            }

            if (!userManager.Users.Any())
            {
                await CreateUsersAsync(userManager, context);
            }

            if (!context.Escolas.Any())
            {
                await CreateEscolasAsync(context);
            }

            if (!context.Salas.Any())
            {
                await CreateSalasAsync(context);
            }

            if (!context.Cursos.Any())
            {
                await CreateCursosAsync(context);
            }

            if (!context.UCs.Any())
            {
                await CreateUCsAsync(context);
            }

            if (!context.Turmas.Any())
            {
                await CreateTurmasAsync(context);
            }

            if (!context.BlocosHorario.Any())
            {
                await CreateBlocosHorarioAsync(context);
            }

            await context.SaveChangesAsync();
            // *************************************
        }

        // Criar roles na base de dados
        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Administrador", "MembroComissao", "Docente" };

            foreach (var roleName in roleNames)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Criar utilizadores na base de dados
        private static async Task CreateUsersAsync(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            // Cria o utilizador administrador
            var adminUser = new IdentityUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Administrador");

            // Cria um membro da comissão
            var commissionUser = new IdentityUser
            {
                UserName = "comissao@example.com",
                Email = "comissao@example.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(commissionUser, "Comissao123!");
            await userManager.AddToRoleAsync(commissionUser, "MembroComissao");

            // Cria um conjunto de professores
            var professors = new[]
            {
                new { Email = "joao.silva@example.com", Name = "João Silva" },
                new { Email = "maria.santos@example.com", Name = "Maria Santos" },
                new { Email = "antonio.pereira@example.com", Name = "António Pereira" },
                new { Email = "ana.costa@example.com", Name = "Ana Costa" }
            };

            // Adiciona os professores à base de dados (Identity)
            foreach (var professor in professors)
            {
                var user = new IdentityUser
                {
                    UserName = professor.Email,
                    Email = professor.Email,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Docente123!");
                await userManager.AddToRoleAsync(user, "Docente");

                // Adiciona os professores à tabela Utilizadores
                var utilizador = new Utilizador
                {
                    Nome = professor.Name,
                    Email = professor.Email,
                    Funcao = "Docente",
                    UserId = user.Id,
                    Categoria = "Professor Adjunto"
                };
                context.Utilizadores.Add(utilizador);
            }

            // Adiciona o utilizador administrador à tabela Utilizadores
            context.Utilizadores.Add(new Utilizador
            {
                Nome = "Administrador Sistema",
                Email = "admin@example.com",
                Funcao = "Administrador",
                UserId = adminUser.Id
            });

            // Adiciona o membro da comissão à tabela Utilizadores
            context.Utilizadores.Add(new Utilizador
            {
                Nome = "Membro da Comissão",
                Email = "comissao@example.com",
                Funcao = "MembroComissao",
                UserId = commissionUser.Id
            });
        }

        // Criar escolas na base de dados
        private static async Task CreateEscolasAsync(ApplicationDbContext context)
        {
            var escolas = new List<Escola>
            {
                new Escola { Nome = "Escola Superior de Tecnologia de Tomar", Localizacao = "Tomar" },
                new Escola { Nome = "Escola Superior de Gestão de Tomar", Localizacao = "Tomar" },
                new Escola { Nome = "Escola Superior de Tecnologia de Abrantes", Localizacao = "Abrantes" }
            };

            await context.Escolas.AddRangeAsync(escolas);
        }

        // Criar salas na base de dados
        private static async Task CreateSalasAsync(ApplicationDbContext context)
        {
            // Buscar escolas para associar as salas
            var escolas = await context.Escolas.ToListAsync();
            if (!escolas.Any()) return;

            // Criar uma lista de salas
            var salas = new List<Sala>();

            // Criar salas para cada escola
            foreach (var escola in escolas)
            {
                for (int i = 1; i <= 5; i++)
                {
                    salas.Add(new Sala
                    {
                        Nome = $"Sala {i:D2}",
                        Lugares = 30 + (i * 5),
                        TipoSala = i % 3 == 0 ? "Laboratório" : "Sala de aula",
                        Localizacao = escola.Localizacao,
                        EscolaFK = escola.IdEscola,
                        Escola = escola
                    });
                }
            }

            await context.Salas.AddRangeAsync(salas);
        }

        // Criar cursos na base de dados
        private static async Task CreateCursosAsync(ApplicationDbContext context)
        {
            var escolas = await context.Escolas.ToListAsync();
            if (!escolas.Any()) return;

            var cursos = new List<Curso>
            {
                new Curso { Nome = "Engenharia Informática", Grau = "Licenciatura", EscolaFK = escolas[0].IdEscola },
                new Curso { Nome = "Gestão de Empresas", Grau = "Licenciatura", EscolaFK = escolas[1].IdEscola },
                new Curso { Nome = "Design e Tecnologia das Artes Gráficas", Grau = "Licenciatura", EscolaFK = escolas[0].IdEscola },
                new Curso { Nome = "Tecnologias de Informação e Comunicação", Grau = "Licenciatura", EscolaFK = escolas[2].IdEscola },
                new Curso { Nome = "Engenharia Informática - Internet das Coisas", Grau = "Mestrado", EscolaFK = escolas[0].IdEscola }
            };

            await context.Cursos.AddRangeAsync(cursos);
            await context.SaveChangesAsync();
        }

        // Criar unidades curriculares (UCs) na base de dados
        private static async Task CreateUCsAsync(ApplicationDbContext context)
        {
            var cursos = await context.Cursos.ToListAsync();
            if (!cursos.Any()) return;

            var ucs = new List<UC>
            {
                new UC { NomeUC = "Programação", TipoUC = "Obrigatória", GrauAcademico = "Licenciatura",
                    Tipologia = "Teórico-prática", Semestre = "1º", Ano = 1, CursoFK = cursos[0].IdCurso },

                new UC { NomeUC = "Base de Dados", TipoUC = "Obrigatória", GrauAcademico = "Licenciatura",
                    Tipologia = "Teórico-prática", Semestre = "1º", Ano = 2, CursoFK = cursos[0].IdCurso },

                new UC { NomeUC = "Redes de Computadores", TipoUC = "Obrigatória", GrauAcademico = "Licenciatura",
                    Tipologia = "Teórico-prática", Semestre = "2º", Ano = 2, CursoFK = cursos[0].IdCurso },

                new UC { NomeUC = "Gestão Financeira", TipoUC = "Obrigatória", GrauAcademico = "Licenciatura",
                    Tipologia = "Teórica", Semestre = "1º", Ano = 1, CursoFK = cursos[1].IdCurso },

                new UC { NomeUC = "IoT Fundamentals", TipoUC = "Obrigatória", GrauAcademico = "Mestrado",
                    Tipologia = "Teórico-prática", Semestre = "1º", Ano = 1, CursoFK = cursos[4].IdCurso }
            };

            await context.UCs.AddRangeAsync(ucs);
            await context.SaveChangesAsync();
        }

        // Criar turmas na base de dados
        private static async Task CreateTurmasAsync(ApplicationDbContext context)
        {
            var ucs = await context.UCs.Include(u => u.Curso).ToListAsync();
            if (!ucs.Any()) return;

            var turmas = new List<Turma>();

            foreach (var uc in ucs)
            {
                // Criar turmas para cada unidade curricular
                for (char letra = 'A'; letra <= 'B'; letra++)
                {
                    turmas.Add(new Turma
                    {
                        Nome = $"Turma {letra}",
                        DisciplinaFK = uc.IdUC,
                        Disciplina = uc,
                        CursoFK = uc.CursoFK,
                        Curso = uc.Curso
                    });
                }
            }

            await context.Turmas.AddRangeAsync(turmas);
            await context.SaveChangesAsync();
        }

        // Criar blocos de horário na base de dados
        private static async Task CreateBlocosHorarioAsync(ApplicationDbContext context)
        {
            var professores = await context.Utilizadores.Where(u => u.Funcao == "Docente").ToListAsync();
            var salas = await context.Salas.ToListAsync();
            var turmas = await context.Turmas.ToListAsync();

            if (!professores.Any() || !salas.Any() || !turmas.Any())
                return;

            var blocosHorario = new List<BlocoHorario>();
            var random = new Random();

            // Criar blocos de horário para as turmas
            foreach (var turma in turmas.Take(10))
            {
                var professor = professores[random.Next(professores.Count)];
                var sala = salas[random.Next(salas.Count)];

                // Criar blocos de horário para cada dia da semana (2 blocos por dia)
                for (int diaSemana = 1; diaSemana <= 5; diaSemana++)
                {
                    // Calcular o dia da semana
                    var hoje = DateOnly.FromDateTime(DateTime.Today);
                    int diff = diaSemana - (int)hoje.DayOfWeek;
                    var dia = hoje.AddDays(diff);

                    // Criar dois blocos de horário para cada dia
                    blocosHorario.Add(new BlocoHorario
                    {
                        HoraInicio = new TimeSpan(8 + random.Next(3), 0, 0),
                        HoraFim = new TimeSpan(9 + random.Next(3), 30, 0),
                        Dia = dia,
                        ProfessorFK = professor.IdUtilizador,
                        Professor = professor,
                        UnidadeCurricularFK = turma.DisciplinaFK,
                        UnidadeCurricular = turma.Disciplina,
                        SalaFK = sala.IdSala,
                        Sala = sala,
                        TurmaFK = turma.IdTurma,
                        Turma = turma
                    });

                    diaSemana += random.Next(2);
                }
            }

            await context.BlocosHorario.AddRangeAsync(blocosHorario);
        }
    }
}

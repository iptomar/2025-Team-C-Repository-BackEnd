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
        /// <summary>
        /// Inicializa a base de dados com dados de teste
        /// </summary>
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Garante que a base de dados existe e aplica migrações pendentes
            context.Database.Migrate();

            // Verifica se já existem dados na base de dados
            if (context.Utilizadores.Any())
            {
                return; // A base de dados já foi inicializada
            }

            // Criar roles necessárias
            await CriarRolesAsync(roleManager);

            // Criar utilizadores de teste
            //await CriarUtilizadoresTesteAsync(context, userManager);

            // Criar escolas
            var escolas = CriarEscolas();
            await context.Escolas.AddRangeAsync(escolas);
            await context.SaveChangesAsync();

            // Criar salas
            var salas = CriarSalas(escolas);
            await context.Salas.AddRangeAsync(salas);
            await context.SaveChangesAsync();

            // Criar cursos
            //var cursos = CriarCursos(escolas);
            //await context.Cursos.AddRangeAsync(cursos);
            //await context.SaveChangesAsync();

            // Criar UCs
            //var ucs = CriarUCs(cursos);
            //await context.UCs.AddRangeAsync(ucs);
            //await context.SaveChangesAsync();

            // Atribuir UCs aos docentes
            //await AtribuirUCsAosDocentesAsync(context, ucs);

            // Criar turmas
            //var turmas = CriarTurmas(cursos);
            //await context.Turmas.AddRangeAsync(turmas);
            //await context.SaveChangesAsync();

            // Criar blocos de horário
            //var blocos = CriarBlocosHorario(context);
            //await context.BlocosHorario.AddRangeAsync(blocos);
            //await context.SaveChangesAsync();
        }

        /// <summary>
        /// Cria as roles necessárias no sistema
        /// </summary>
        private static async Task CriarRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Administrador", "MembroComissao", "Docente" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        /// <summary>
        /// Cria utilizadores de teste para cada tipo de perfil
        /// </summary>
        private static async Task CriarUtilizadoresTesteAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            // Criar administrador
            await CriarUtilizadorAsync(context, userManager, "admin@ipt.pt", "Admin123!", "Administrador", "Administrador do Sistema");

            // Criar membro da comissão
            await CriarUtilizadorAsync(context, userManager, "comissao@ipt.pt", "Comissao123!", "MembroComissao", "Membro da Comissão");

            // Criar docentes
            await CriarUtilizadorAsync(context, userManager, "joaosilva@ipt.pt", "Docente123!", "Docente", "João Silva", "Professor Adjunto");
            await CriarUtilizadorAsync(context, userManager, "marialopes@ipt.pt", "Docente123!", "Docente", "Maria Lopes", "Professor Auxiliar");
            await CriarUtilizadorAsync(context, userManager, "antonioferreira@ipt.pt", "Docente123!", "Docente", "António Ferreira", "Professor Catedrático");
            await CriarUtilizadorAsync(context, userManager, "anasantos@ipt.pt", "Docente123!", "Docente", "Ana Santos", "Assistente");
            await CriarUtilizadorAsync(context, userManager, "pedrooliveira@ipt.pt", "Docente123!", "Docente", "Pedro Oliveira", "Professor Associado");
        }

        /// <summary>
        /// Cria um utilizador no Identity e na tabela Utilizadores
        /// </summary>
        private static async Task CriarUtilizadorAsync(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role,
            string nome,
            string categoria = null)
        {
            // Verificar se o utilizador já existe
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // Criar utilizador para o Identity
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, role);

                // Criar utilizador na tabela Utilizadores
                var utilizador = new Utilizador
                {
                    Nome = nome,
                    Email = email,
                    Funcao = role,
                    Categoria = categoria,
                    UserId = user.Id
                };

                if (!string.IsNullOrEmpty(categoria))
                {
                    // Definir CategoriaId para docentes com base na categoria
                    switch (categoria)
                    {
                        case "Professor Adjunto": utilizador.CategoriaId = 1; break;
                        case "Professor Auxiliar": utilizador.CategoriaId = 2; break;
                        case "Professor Catedrático": utilizador.CategoriaId = 3; break;
                        case "Assistente": utilizador.CategoriaId = 4; break;
                        case "Professor Associado": utilizador.CategoriaId = 5; break;
                        default: utilizador.CategoriaId = null; break;
                    }
                }

                await context.Utilizadores.AddAsync(utilizador);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Cria escolas de teste
        /// </summary>
        private static List<Escola> CriarEscolas()
        {
            return new List<Escola>
            {
                new Escola { Nome = "Escola Superior de Tecnologia de Abrantes", Localizacao = "Abrantes" },
                new Escola { Nome = "Escola Superior de Gestão de Tomar", Localizacao = "Tomar" },
                new Escola { Nome = "Escola Superior de Tecnologia de Tomar", Localizacao = "Tomar" }
            };
        }

        /// <summary>
        /// Cria salas de teste para cada escola
        /// </summary>
        private static List<Sala> CriarSalas(List<Escola> escolas)
        {
            var salas = new List<Sala>();

            // Salas para ESTT
            var estt = escolas[0];
            salas.AddRange(new List<Sala>
            {
                new Sala { Nome = "B207", Lugares = 30, TipoSala = "Laboratório", Localizacao = "Tomar", EscolaFK = estt.IdEscola },
                new Sala { Nome = "B201", Lugares = 40, TipoSala = "Sala de Aula", Localizacao = "Tomar", EscolaFK = estt.IdEscola },
                new Sala { Nome = "A204", Lugares = 60, TipoSala = "Auditório", Localizacao = "Tomar", EscolaFK = estt.IdEscola },
                new Sala { Nome = "C106", Lugares = 25, TipoSala = "Laboratório", Localizacao = "Tomar", EscolaFK = estt.IdEscola },
            });

            // Salas para ESGT
            var esgt = escolas[1];
            salas.AddRange(new List<Sala>
            {
                new Sala { Nome = "G104", Lugares = 35, TipoSala = "Sala de Aula", Localizacao = "Tomar", EscolaFK = esgt.IdEscola },
                new Sala { Nome = "G202", Lugares = 50, TipoSala = "Sala de Aula", Localizacao = "Tomar", EscolaFK = esgt.IdEscola },
                new Sala { Nome = "G301", Lugares = 70, TipoSala = "Auditório", Localizacao = "Tomar", EscolaFK = esgt.IdEscola },
            });

            // Salas para ESTA
            var esta = escolas[2];
            salas.AddRange(new List<Sala>
            {
                new Sala { Nome = "LE1", Lugares = 25, TipoSala = "Laboratório", Localizacao = "Abrantes", EscolaFK = esta.IdEscola },
                new Sala { Nome = "B102", Lugares = 40, TipoSala = "Sala de Aula", Localizacao = "Abrantes", EscolaFK = esta.IdEscola },
                new Sala { Nome = "A005", Lugares = 60, TipoSala = "Auditório", Localizacao = "Abrantes", EscolaFK = esta.IdEscola },
            });

            return salas;
        }

        /// <summary>
        /// Cria cursos de teste para cada escola
        /// </summary>
        private static List<Curso> CriarCursos(List<Escola> escolas)
        {
            var cursos = new List<Curso>();

            // Cursos para ESTT
            var estt = escolas[0];
            cursos.AddRange(new List<Curso>
            {
                new Curso { Nome = "Engenharia Informática", Grau = "Licenciatura", EscolaFK = estt.IdEscola },
                new Curso { Nome = "Tecnologias de Informação e Comunicação", Grau = "Licenciatura", EscolaFK = estt.IdEscola },
                new Curso { Nome = "Design e Tecnologia das Artes Gráficas", Grau = "Licenciatura", EscolaFK = estt.IdEscola },
            });

            // Cursos para ESGT
            var esgt = escolas[1];
            cursos.AddRange(new List<Curso>
            {
                new Curso { Nome = "Gestão de Empresas", Grau = "Licenciatura", EscolaFK = esgt.IdEscola },
                new Curso { Nome = "Gestão de Recursos Humanos", Grau = "Licenciatura", EscolaFK = esgt.IdEscola },
            });

            // Cursos para ESTA
            var esta = escolas[2];
            cursos.AddRange(new List<Curso>
            {
                new Curso { Nome = "Engenharia Mecânica", Grau = "Licenciatura", EscolaFK = esta.IdEscola },
                new Curso { Nome = "Tecnologia e Gestão Industrial", Grau = "Licenciatura", EscolaFK = esta.IdEscola },
            });

            return cursos;
        }

        /// <summary>
        /// Cria UCs de teste
        /// </summary>
        private static List<UC> CriarUCs(List<Curso> cursos)
        {
            var ucs = new List<UC>();

            // UCs para Engenharia Informática
            var ei = cursos[0];
            ucs.AddRange(new List<UC>
            {
                new UC { NomeUC = "Programação Orientada a Objetos", TipoUC = "Teórico-prática", GrauAcademico = "Licenciatura", Semestre = "1º", Ano = 2, CursoFK = ei.IdCurso },
                new UC { NomeUC = "Bases de Dados", TipoUC = "Teórico-prática", GrauAcademico = "Licenciatura", Semestre = "1º", Ano = 2, CursoFK = ei.IdCurso },
                new UC { NomeUC = "Sistemas Operativos", TipoUC = "Teórico-prática", GrauAcademico = "Licenciatura", Semestre = "1º", Ano = 2, CursoFK = ei.IdCurso },
                new UC { NomeUC = "Desenvolvimento Web", TipoUC = "Teórico-prática", GrauAcademico = "Licenciatura", Semestre = "2º", Ano = 2, CursoFK = ei.IdCurso },
                new UC { NomeUC = "Inteligência Artificial", TipoUC = "Teórico-prática", GrauAcademico = "Licenciatura", Semestre = "1º", Ano = 3, CursoFK = ei.IdCurso },
            });

            // UCs para Gestão de Empresas
            var ge = cursos[3];
            ucs.AddRange(new List<UC>
            {
                new UC { NomeUC = "Contabilidade", TipoUC = "Teórica", GrauAcademico = "Licenciatura", Semestre = "1º", Ano = 1, CursoFK = ge.IdCurso },
                new UC { NomeUC = "Marketing", TipoUC = "Teórico-prática", GrauAcademico = "Licenciatura", Semestre = "2º", Ano = 2, CursoFK = ge.IdCurso },
            });

            // UCs para Engenharia Mecânica
            var em = cursos[5];
            ucs.AddRange(new List<UC>
            {
                new UC { NomeUC = "Desenho Técnico", TipoUC = "Prática", GrauAcademico = "Licenciatura", Semestre = "2º", Ano = 1, CursoFK = em.IdCurso },
                new UC { NomeUC = "Sistemas Digitais", TipoUC = "Teórico-prática", GrauAcademico = "Licenciatura", Semestre = "1º", Ano = 1, CursoFK = em.IdCurso },
            });

            return ucs;
        }

        /// <summary>
        /// Atribui UCs aos docentes
        /// </summary>
        private static async Task AtribuirUCsAosDocentesAsync(ApplicationDbContext context, List<UC> ucs)
        {
            var docentes = await context.Utilizadores
                .Where(u => u.Funcao == "Docente")
                .ToListAsync();

            if (!docentes.Any()) return;

            // Atribuir UCs aos docentes de forma distribuída
            for (int i = 0; i < ucs.Count; i++)
            {
                var docente = docentes[i % docentes.Count];

                if (docente.UCsLecionadas == null)
                {
                    docente.UCsLecionadas = new List<UC>();
                }

                docente.UCsLecionadas.Add(ucs[i]);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Cria turmas de teste para os cursos
        /// </summary>
        private static List<Turma> CriarTurmas(List<Curso> cursos)
        {
            var turmas = new List<Turma>();

            foreach (var curso in cursos)
            {
                // Criar turmas para cada curso
                turmas.AddRange(new List<Turma>
                {
                    new Turma { Nome = $"{curso.Nome} - Turma A", CursoFK = curso.IdCurso },
                    new Turma { Nome = $"{curso.Nome} - Turma B", CursoFK = curso.IdCurso }
                });
            }

            return turmas;
        }

        /// <summary>
        /// Cria blocos de horário de teste
        /// </summary>
        private static List<BlocoHorario> CriarBlocosHorario(ApplicationDbContext context)
        {
            var salas = context.Salas.ToList();
            var docentes = context.Utilizadores.Where(u => u.Funcao == "Docente").ToList();
            var ucs = context.UCs.ToList();
            var turmas = context.Turmas.ToList();

            var blocos = new List<BlocoHorario>();
            var random = new Random();

            // Data base para os horários (segunda-feira da próxima semana)
            var dataBase = DateTime.Today.AddDays(((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7);

            // Criar blocos de horário para cada dia da semana (segunda a sexta)
            for (int dia = 0; dia < 5; dia++)
            {
                var dataAtual = DateOnly.FromDateTime(dataBase.AddDays(dia));

                // Criar blocos em horários de manhã e tarde
                var horasInicio = new TimeSpan[] {
                    new TimeSpan(8, 0, 0),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(14, 0, 0),
                    new TimeSpan(16, 0, 0)
                };

                foreach (var horaInicio in horasInicio)
                {
                    // Cada bloco tem 2 horas
                    var horaFim = horaInicio.Add(new TimeSpan(2, 0, 0));

                    // Selecionar aleatoriamente alguns docentes, UCs, salas e turmas para criar blocos
                    for (int i = 0; i < 3; i++)
                    {
                        if (docentes.Count == 0 || ucs.Count == 0 || salas.Count == 0 || turmas.Count == 0)
                            continue;

                        var docenteIndex = random.Next(docentes.Count);
                        var ucIndex = random.Next(ucs.Count);
                        var salaIndex = random.Next(salas.Count);
                        var turmaIndex = random.Next(turmas.Count);

                        blocos.Add(new BlocoHorario
                        {
                            HoraInicio = horaInicio,
                            HoraFim = horaFim,
                            Dia = dataAtual,
                            ProfessorFK = docentes[docenteIndex].IdUtilizador,
                            UnidadeCurricularFK = ucs[ucIndex].IdUC,
                            SalaFK = salas[salaIndex].IdSala,
                            TurmaFK = turmas[turmaIndex].IdTurma
                        });
                    }
                }
            }

            return blocos;
        }
    }
}

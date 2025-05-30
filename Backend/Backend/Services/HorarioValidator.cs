using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class HorarioValidator
    {
        private readonly ApplicationDbContext _context;

        public HorarioValidator(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica se um bloco de horário é válido (sem conflitos e cumpre restrições)
        /// </summary>
        /// <param name="blocoHorario">Bloco a ser validado</param>
        /// <param name="blocoId">ID do bloco atual (para exclusão na validação durante edição)</param>
        /// <returns>Resultado da validação e mensagem de erro, se houver</returns>
        public async Task<(bool isValid, string errorMessage)> ValidarBlocoHorario(BlocoHorario blocoHorario, int? blocoId = null)
        {
            // Verificar sobreposição de sala
            var conflitoDeSala = await VerificarConflitoDeSala(blocoHorario, blocoId);
            if (conflitoDeSala.temConflito)
                return (false, conflitoDeSala.mensagem);

            // Verificar sobreposição de professor
            var conflitoDeProfessor = await VerificarConflitoDeProfessor(blocoHorario, blocoId);
            if (conflitoDeProfessor.temConflito)
                return (false, conflitoDeProfessor.mensagem);

            // Verificar limite de horas consecutivas
            var limiteHoras = await VerificarLimiteHorasConsecutivas(blocoHorario);
            if (!limiteHoras.valido)
                return (false, limiteHoras.mensagem);

            // Verificar horário de refeições
            var refeicoes = VerificarHorarioRefeicoes(blocoHorario);
            if (!refeicoes.valido)
                return (false, refeicoes.mensagem);

            return (true, string.Empty);
        }

        /// <summary>
        /// Verifica se há conflito de sala para o bloco de horário
        /// </summary>
        private async Task<(bool temConflito, string mensagem)> VerificarConflitoDeSala(BlocoHorario blocoHorario, int? blocoId)
        {
            // Procurar blocos que usam a mesma sala no mesmo dia
            var blocosMesmaSala = await _context.BlocosHorario
                .Where(b => b.SalaFK == blocoHorario.SalaFK &&
                            b.Dia == blocoHorario.Dia &&
                            (blocoId == null || b.IdBloco != blocoId))
                .ToListAsync();

            // Verificar sobreposição de horários
            foreach (var bloco in blocosMesmaSala)
            {
                if ((blocoHorario.HoraInicio >= bloco.HoraInicio && blocoHorario.HoraInicio < bloco.HoraFim) ||
                    (blocoHorario.HoraFim > bloco.HoraInicio && blocoHorario.HoraFim <= bloco.HoraFim) ||
                    (blocoHorario.HoraInicio <= bloco.HoraInicio && blocoHorario.HoraFim >= bloco.HoraFim))
                {
                    var sala = await _context.Salas.FindAsync(blocoHorario.SalaFK);
                    return (true, $"Conflito de horário na sala {sala.Nome} entre {bloco.HoraInicio} e {bloco.HoraFim}");
                }
            }

            return (false, string.Empty);
        }

        /// <summary>
        /// Verifica se há conflito de professor para o bloco de horário
        /// </summary>
        private async Task<(bool temConflito, string mensagem)> VerificarConflitoDeProfessor(BlocoHorario blocoHorario, int? blocoId)
        {
            // Procurar blocos que têm o mesmo professor no mesmo dia
            var blocosMesmoProfessor = await _context.BlocosHorario
                .Where(b => b.ProfessorFK == blocoHorario.ProfessorFK &&
                            b.Dia == blocoHorario.Dia &&
                            (blocoId == null || b.IdBloco != blocoId))
                .ToListAsync();

            // Verificar sobreposição de horários
            foreach (var bloco in blocosMesmoProfessor)
            {
                if ((blocoHorario.HoraInicio >= bloco.HoraInicio && blocoHorario.HoraInicio < bloco.HoraFim) ||
                    (blocoHorario.HoraFim > bloco.HoraInicio && blocoHorario.HoraFim <= bloco.HoraFim) ||
                    (blocoHorario.HoraInicio <= bloco.HoraInicio && blocoHorario.HoraFim >= bloco.HoraFim))
                {
                    var professor = await _context.Utilizadores.FindAsync(blocoHorario.ProfessorFK);
                    return (true, $"Conflito de horário para o professor {professor.Nome} entre {bloco.HoraInicio} e {bloco.HoraFim}");
                }
            }

            return (false, string.Empty);
        }

        /// <summary>
        /// Verifica se o professor não ultrapassou o limite de 6 horas consecutivas de aulas
        /// </summary>
        private async Task<(bool valido, string mensagem)> VerificarLimiteHorasConsecutivas(BlocoHorario blocoHorario)
        {
            // Procurar todos os blocos do mesmo professor no mesmo dia
            var blocosDoProfessor = await _context.BlocosHorario
                .Where(b => b.ProfessorFK == blocoHorario.ProfessorFK &&
                            b.Dia == blocoHorario.Dia)
                .OrderBy(b => b.HoraInicio)
                .ToListAsync();

            // Adicionar o bloco atual à lista para verificação (se não existir ainda)
            if (!blocosDoProfessor.Any(b => b.IdBloco == blocoHorario.IdBloco))
            {
                blocosDoProfessor.Add(blocoHorario);
                blocosDoProfessor = blocosDoProfessor.OrderBy(b => b.HoraInicio).ToList();
            }

            // Verificar sequências de aulas sem intervalo significativo
            TimeSpan inicioSequencia = TimeSpan.Zero;
            TimeSpan fimSequencia = TimeSpan.Zero;
            bool primeiroBloco = true;

            foreach (var bloco in blocosDoProfessor)
            {
                if (primeiroBloco)
                {
                    inicioSequencia = bloco.HoraInicio;
                    fimSequencia = bloco.HoraFim;
                    primeiroBloco = false;
                }
                else
                {
                    // Se o próximo bloco começar em menos de 30 minutos após o anterior
                    // considera-se como parte da mesma sequência
                    if (bloco.HoraInicio - fimSequencia <= TimeSpan.FromMinutes(30))
                    {
                        fimSequencia = bloco.HoraFim > fimSequencia ? bloco.HoraFim : fimSequencia;
                    }
                    else
                    {
                        // Nova sequência
                        inicioSequencia = bloco.HoraInicio;
                        fimSequencia = bloco.HoraFim;
                    }
                }

                // Verificar se a sequência atual ultrapassou 6 horas
                if ((fimSequencia - inicioSequencia) > TimeSpan.FromHours(6))
                {
                    var professor = await _context.Utilizadores.FindAsync(blocoHorario.ProfessorFK);
                    return (false, $"O professor {professor.Nome} ultrapassaria o limite de 6 horas consecutivas de aula");
                }
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Verifica se o bloco respeita os horários de refeição (almoço e jantar)
        /// </summary>
        private (bool valido, string mensagem) VerificarHorarioRefeicoes(BlocoHorario blocoHorario)
        {
            // Definir períodos de refeição
            var periodoAlmoco = (inicio: new TimeSpan(13, 0, 0), fim: new TimeSpan(14, 0, 0));
            var periodoJantar = (inicio: new TimeSpan(20, 0, 0), fim: new TimeSpan(21, 0, 0));

            // Verificar se o bloco engloba completamente um período de refeição
            bool englobaAlmoco = blocoHorario.HoraInicio <= periodoAlmoco.inicio && blocoHorario.HoraFim >= periodoAlmoco.fim;
            bool englobaJantar = blocoHorario.HoraInicio <= periodoJantar.inicio && blocoHorario.HoraFim >= periodoJantar.fim;

            // Se englobar um período, deve ter pelo menos 1 hora para a refeição
            if (englobaAlmoco)
            {
                return (false, "O horário engloba o período de almoço. Deve ser reservada uma hora para refeição.");
            }

            if (englobaJantar)
            {
                return (false, "O horário engloba o período de jantar. Deve ser reservada uma hora para refeição.");
            }

            return (true, string.Empty);
        }
    }
}
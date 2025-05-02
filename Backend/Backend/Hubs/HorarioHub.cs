using Microsoft.AspNetCore.SignalR;
using Backend.DTO;

namespace Backend.Hubs
{
    public class HorarioHub : Hub
    {
        // Método genérico para atualizações
        public async Task AtualizarBlocosHorario(string mensagem)
        {
            await Clients.All.SendAsync("ReceberAtualizacao", mensagem);
        }

        // Métodos específicos para cada tipo de operação
        public async Task NotificarNovo(BlocoHorarioDTO bloco)
        {
            await Clients.All.SendAsync("BlocoAdicionado", bloco);
        }

        public async Task NotificarEdicao(BlocoHorarioDTO bloco)
        {
            await Clients.All.SendAsync("BlocoEditado", bloco);
        }

        public async Task NotificarExclusao(int blocoId)
        {
            await Clients.All.SendAsync("BlocoExcluido", blocoId);
        }
    }
}

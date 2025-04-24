using Microsoft.AspNetCore.SignalR;
namespace Backend.Hubs {
    public class HorarioHub : Hub {
        // Método que pode ser chamado pelo cliente para enviar mensagens
        public async Task AtualizarBlocosHorario(string mensagem) {
            // Envia a mensagem para todos os clientes conectados
            await Clients.All.SendAsync("ReceberAtualizacao", mensagem);
        }
    }
}

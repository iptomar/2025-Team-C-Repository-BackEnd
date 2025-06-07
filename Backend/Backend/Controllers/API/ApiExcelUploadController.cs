using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiExcelUploadController : ControllerBase
    {
        /// <summary>
        /// Recebe dados de folhas Excel e processa-os.
        /// </summary>
        /// <param name="sheetsData">Dicionário com o nome da folha como chave e os dados como valor.</param>
        [Authorize(Roles = "Administrador,MembroComissao")]
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromBody] Dictionary<string, object> sheetsData)
        {
            if (sheetsData == null || sheetsData.Count == 0)
                return BadRequest(new { message = "Nenhuma folha recebida." });

            // TODO: Processar cada folha conforme necessário
            foreach (var sheet in sheetsData)
            {
                var sheetName = sheet.Key;
                var data = sheet.Value;
                // ...
            }

            // Retornar sucesso
            return Ok(new { message = "Folhas processadas com sucesso." });
        }

        // ...
    }
}

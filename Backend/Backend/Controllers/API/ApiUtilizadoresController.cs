using Backend.Data;
using Backend.DTO;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUtilizadoresController : ControllerBase
    {

        /// <summary>
        /// Permite a interação com a base de dados.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Autenticação
        /// Faz a pesquisa de utilizadores, adição, remoção... na base de dados.
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Autenticação
        /// Permite a autenticação de utilizadores, login, logout...
        /// </summary>
        public SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// appsettings.json
        /// </summary>
        private IConfiguration _config;

        public ApiUtilizadoresController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost]
        [Route("createUser")]
        public async Task<ActionResult<IdentityUser>> CreateUser([FromQuery] string role, [FromQuery] string email, [FromQuery] string password, [FromQuery] string nome)
        {
            // Criar um novo utilizador no AspNetUsers
            IdentityUser identityUser = new IdentityUser();
            identityUser.UserName = email;
            identityUser.Email = email;
            identityUser.NormalizedEmail = email.ToUpper();
            identityUser.NormalizedUserName = email.ToUpper();
            identityUser.EmailConfirmed = true;
            identityUser.PasswordHash = null;
            identityUser.Id = Guid.NewGuid().ToString();
            identityUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, password);

            // Adicionar o utilizador à base de dados
            var result = await _userManager.CreateAsync(identityUser);

            // Se o utilizador foi criado com sucesso, adicionar à tabela de utilizadores
            if (result.Succeeded)
            {
                // Verificar se a role é válida
                var rolesPermitidos = new HashSet<string> { "Administrador", "MembroComissao", "Docente" };
                if (rolesPermitidos.Contains(role))
                {
                    // Adicionar role ao utilizador
                    var roleResult = await _userManager.AddToRoleAsync(identityUser, role);
                    if (!roleResult.Succeeded)
                    {
                        return BadRequest("Erro ao adicionar role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }

                    // Adicionar o utilizador à tabela de utilizadores
                    Utilizador utilizador = new Utilizador();
                    utilizador.Nome = nome;
                    utilizador.Email = email;
                    utilizador.UserId = identityUser.Id;
                    utilizador.Funcao = role;

                    await _context.AddAsync(utilizador);
                    _context.SaveChanges();
                    return Ok(identityUser);
                }
                return BadRequest("Role Inválida.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost]
        [Route("loginUser")]
        // Autenticação: necessário -> Email e Password
        // Retorna um token JWT
        public async Task<ActionResult> SignInUserAsync([FromQuery] string email, [FromQuery] string password)
        {
            // Procurar o utilizador na base de dados
            IdentityUser user = await _userManager.FindByEmailAsync(email);

            // Se o utilizador existir
            if (user != null)
            {
                // Verificar se a password está correta
                // Se for válida, faz o login
                // A verificação é baseada em hash
                PasswordVerificationResult passWorks = new PasswordHasher<IdentityUser>().VerifyHashedPassword(user, user.PasswordHash, password);
                if (passWorks == PasswordVerificationResult.Success)
                {
                    // Buscar o utilizador na tabela Utilizadores
                    var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.UserId == user.Id);


                    // Obtém a lista de roles do utilizador
                    var roles = await _userManager.GetRolesAsync(user);
                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email)
                    };

                    // Adiciona o utilizadorId como claim
                    if (utilizador != null)
                    {
                        claims.Add(new Claim("utilizadorId", utilizador.IdUtilizador.ToString()));
                    }

                    // Adiciona as roles como claims
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    // Se o login for bem sucedido, gera um token (JWT)
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    // Cria o token
                    var Sectoken = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(120),
                        signingCredentials: credentials);

                    var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                    // Retorna o token
                    return Ok(token);
                }
            }

            // Se chegar aqui, é porque algo falhou.
            return BadRequest("Erro ao fazer login...");
        }

        /// <summary>
        /// Logout do utilizador
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("logoutUser")]
        public async Task<ActionResult> LogoutUser()
        {
            // Faz logout do utilizador
            await _signInManager.SignOutAsync();
            return Ok("O utilizador fez logout com sucesso!");
        }

        /// <summary>
        /// Retorna todos os utilizadores com função de Docente.
        /// </summary>
        /// <returns>Lista de docentes</returns>
        [HttpGet]
        [Route("GetDocentes")]
        public async Task<ActionResult<IEnumerable<UtilizadorDTO>>> GetDocentes()
        {
            // Faz a busca dos utilizadores, cujo a sua função é "Docente"
            var docentes = await _context.Utilizadores
                .Where(u => u.Funcao == "Docente")
                .Select(u => new UtilizadorDTO
                {
                    IdUtilizador = u.IdUtilizador,
                    Nome = u.Nome,
                    Email = u.Email,
                    Funcao = u.Funcao,
                    Categoria = u.Categoria
                })
                .OrderBy(u => u.Nome)
                .ToListAsync();
            return Ok(docentes);
        }
    }
}
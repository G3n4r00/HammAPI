using HammAPI.Models;
using HammAPI.Repository;
using HammAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HammAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela autenticação e registro de usuários.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IAuthService _authService;

        /// <summary>
        /// Construtor do controlador de autenticação.
        /// </summary>
        /// <param name="userRepo">Repositório de usuários para acesso aos dados de usuário.</param>
        /// <param name="authService">Serviço responsável por autenticação e geração de tokens JWT.</param>
        public AuthController(IUserRepository userRepo, IAuthService authService)
        {
            _userRepo = userRepo;
            _authService = authService;
        }

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <param name="dto">Dados necessários para o registro, incluindo nome, email e senha.</param>
        /// <returns>
        /// Retorna o ID e o e-mail do usuário criado.  
        /// - 201 Created: Usuário criado com sucesso.  
        /// - 409 Conflict: E-mail já cadastrado.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null) return Conflict("Email já cadastrado.");

            var usuario = new Usuario
            {
                PrimeiroNome = dto.PrimeiroNome,
                UltimoNome = dto.UltimoNome,
                Email = dto.Email,
                SenhaHash = _authService.HashPassword(dto.Senha)
            };

            await _userRepo.AddAsync(usuario);
            return CreatedAtAction(null, new { id = usuario.Id }, new { usuario.Id, usuario.Email });
        }

        /// <summary>
        /// Realiza o login de um usuário existente e gera um token JWT.
        /// </summary>
        /// <param name="dto">Credenciais de login contendo e-mail e senha.</param>
        /// <returns>
        /// Retorna um token JWT válido para autenticação.  
        /// - 200 OK: Login bem-sucedido e token gerado.  
        /// - 401 Unauthorized: Credenciais inválidas.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Credenciais inválidas.");

            if (!_authService.VerifyPassword(dto.Senha, user.SenhaHash))
                return Unauthorized("Credenciais inválidas.");

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }

    /// <summary>
    /// Dados necessários para registrar um novo usuário.
    /// </summary>
    /// <param name="PrimeiroNome">Primeiro nome do usuário.</param>
    /// <param name="UltimoNome">Último nome do usuário.</param>
    /// <param name="Email">Endereço de e-mail do usuário.</param>
    /// <param name="Senha">Senha em texto puro a ser criptografada.</param>
    public record RegisterDto(string PrimeiroNome, string UltimoNome, string Email, string Senha);

    /// <summary>
    /// Dados de login utilizados para autenticação.
    /// </summary>
    /// <param name="Email">Endereço de e-mail do usuário.</param>
    /// <param name="Senha">Senha do usuário.</param>
    public record LoginDto(string Email, string Senha);
}

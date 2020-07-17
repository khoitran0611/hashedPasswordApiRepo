using System.Threading.Tasks;
using AutoMapper;
using HashedPassword.Data;
using HashedPasswordApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace HashedPasswordApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly DbContext _context;
        private ILogger<AuthenticateController> _logger;
        private readonly IMapper _mapper;

        public AuthenticateController(DbContext context, ILogger<AuthenticateController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(AccountDto accountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAccount = _context.Set<Account>().
            FirstOrDefault(a => a.Username == accountDto.Username);

            if (existingAccount != null)
            {
                return BadRequest("Account already exist");
            }
            var account = _mapper.Map<Account>(accountDto);

            //Hash the password and save to database

            account.PasswordSalt = GenerateSalt();
            account.PasswordHashed = Create(accountDto.Password, account.PasswordSalt);

            await _context.Set<Account>().AddAsync(account);
            await _context.SaveChangesAsync();

            return Ok(account);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(AccountDto accountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAccount = await _context.Set<Account>()
            .FirstOrDefaultAsync(a => a.Username == accountDto.Username);

            if (existingAccount == null)
            {
                return NotFound(accountDto);
            }

            if (!ValidateHashedPassword(accountDto.Password, existingAccount.PasswordSalt, existingAccount.PasswordHashed))
            {
                return BadRequest("The user info doesn't match");
            }

            return Ok(existingAccount);
        }
        #region Helper
        private string GenerateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        private string Create(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        private bool ValidateHashedPassword(string passwordToValidate, string salt, string hashedPassword)
        {
            return Create(passwordToValidate, salt) == hashedPassword;
        }
    }
    #endregion
}
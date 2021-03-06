﻿//  __  _  __    __   ___ __  ___ ___  
// |  \| |/__\ /' _/ / _//__\| _ \ __| 
// | | ' | \/ |`._`.| \_| \/ | v / _|  
// |_|\__|\__/ |___/ \__/\__/|_|_\___| 
// 
// Copyright (C) 2019 - NosCore
// 
// NosCore is a free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NosCore.Configuration;
using NosCore.Core.Encryption;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using NosCore.Core.I18N;
using NosCore.Data;
using NosCore.Data.Enumerations.I18N;

namespace NosCore.Core.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly WebApiConfiguration _apiConfiguration;
        private readonly IGenericDao<AccountDto> _accountDao;

        public TokenController(WebApiConfiguration apiConfiguration, IGenericDao<AccountDto> accountDao)
        {
            _apiConfiguration = apiConfiguration;
            _accountDao = accountDao;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ConnectUser(string userName, string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BadRequest(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.AUTH_ERROR)));
            }

            var account = _accountDao.FirstOrDefault(s => s.Name == userName);
            if (!(account?.Password.ToLower().Equals(password.ToSha512()) ?? false))
            {
                return BadRequest(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.AUTH_INCORRECT));
            }

            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userName),
                new Claim(ClaimTypes.Role, account.Authority.ToString())
            });
            var keyByteArray = Encoding.Default.GetBytes(_apiConfiguration.Password.ToSha512());
            var signinKey = new SymmetricSecurityKey(keyByteArray);
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = claims,
                Issuer = "Issuer",
                Audience = "Audience",
                SigningCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            });
            return Ok(handler.WriteToken(securityToken));
        }
    }
}
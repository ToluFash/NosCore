﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NosCore.Data;
using Microsoft.AspNetCore.Authorization;
using NosCore.DAL;
using NosCore.Core.Encryption;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NosCore.Core.Logger;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NosCore.Domain;
using Microsoft.Extensions.Configuration;

namespace NosCore.WorldServer.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post(string UserName, string Password)
        {
            if (ModelState.IsValid)
            {
                AccountDTO account = DAOFactory.AccountDAO.FirstOrDefault(s => s.Name == UserName);


                if (account != null && account.Password.ToLower().Equals(EncryptionHelper.Sha512(Password)))
                {
                    var claims = new ClaimsIdentity(new[]
                    {
                          new Claim(ClaimTypes.NameIdentifier, UserName),
                          new Claim(ClaimTypes.Role, account.Authority.ToString()),
                    });
                    var keyByteArray = Encoding.ASCII.GetBytes(EncryptionHelper.Sha512("NosCorePassword"));//TODO replace by configured one
                    var signinKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyByteArray);
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
                else
                {
                    return BadRequest(LogLanguage.Instance.GetMessageFromKey("AUTH_INCORRECT"));
                }
            }
            return BadRequest(BadRequest(LogLanguage.Instance.GetMessageFromKey("AUTH_ERROR")));
        }

        [AllowAnonymous]
        [HttpPost("ConnectServer")]
        public async Task<IActionResult> ConnectServer(string ServerToken)
        {
            if (ModelState.IsValid)
            {
                if (ServerToken == "NosCorePassword")//TODO replace by configured one
                {
                    var claims = new ClaimsIdentity(new[]
                    {
                          new Claim(ClaimTypes.NameIdentifier, "Server"),
                          new Claim(ClaimTypes.Role, AuthorityType.GameMaster.ToString()),
                    });
                    var keyByteArray = Encoding.ASCII.GetBytes(EncryptionHelper.Sha512("NosCorePassword"));//TODO replace by configured one
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
                else
                {
                    return BadRequest(LogLanguage.Instance.GetMessageFromKey("AUTH_INCORRECT"));
                }
            }
            return BadRequest(BadRequest(LogLanguage.Instance.GetMessageFromKey("AUTH_ERROR")));
        }
    }
}
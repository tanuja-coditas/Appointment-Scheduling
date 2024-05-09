using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class JwtToken
    {
        private readonly IConfiguration _config;

        private readonly  RoleRepo roleRepo;

        public JwtToken(IConfiguration config,RoleRepo roleRepo) {
            _config = config;
            this.roleRepo = roleRepo;
        }
        public string GenerateToken(TblUser user,out string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            string Role = roleRepo.GetRoleName(user.RoleId);
            role = Role;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
                new Claim(ClaimTypes.Role,Role),
                new Claim("username",user.UserName),
                new Claim("imagePath",user.UserImage)
                
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

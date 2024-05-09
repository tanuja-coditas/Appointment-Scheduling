using Domain.Models;
using Repo;
using Services.AuthModels;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using Common;
using Microsoft.Extensions.Configuration;
namespace Services.AuthServices
{
    public class Authentication
    {
        private readonly UserRepo userRepo;
        private readonly RoleRepo roleRepo;
        private readonly IConfiguration config;
        private readonly Encryption encryption;
        private readonly Uploads uploads;

        public Authentication(UserRepo userRepo,RoleRepo roleRepo,IConfiguration config,Encryption encryption,Uploads uploads)
        {
            this.userRepo = userRepo;
            this.roleRepo = roleRepo;
            this.config = config;
            this.encryption = encryption;   
            this.uploads = uploads;
        }

        public  async Task RegisterUser(RegisterModel model)
        {
           
            Guid Role =  roleRepo.GetRoleId(model.Role.ToLower());
            string key = config["PasswordKey"];
            string imagePath = await uploads.UploadImage(model.ImageFile);
            string passwordHash = encryption.Encrypt(model.Password, out string passwordSalt, key);
            TblUser user = new TblUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                RoleId = Role,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedBy = model.FirstName + " " + model.LastName,
                CreatedDate = DateOnly.Parse(DateTime.Today.ToString("yyyy-MM-dd")),
                ModifiedBy = model.FirstName + " " + model.LastName,
                ModifiedDate = DateOnly.Parse(DateTime.Today.ToString("yyyy-MM-dd")),
                UserImage = imagePath,
            };
            await userRepo.CreateUser(user);

        }

        public TblUser AuthenticateUser(LoginModel model)
        {
            TblUser user = userRepo.GetUser(model.UsernameOrEmail);
            if(user!=null)
            {
                string passwordSalt = user.PasswordSalt;
                string key = config["PasswordKey"];
                string passwordHash = encryption.AuthEncrypt(model.Password, passwordSalt,key);
                if(passwordHash == user.PasswordHash)
                {
                    return user;
                }
                return null;
            }
            return user;
        }

        public void UpdatePassword(string email, string password)
        {
            TblUser user = userRepo.GetUser(email);
            string key = config["PasswordKey"];
            string passwordHash = encryption.Encrypt(password, out string passwordSalt, key);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            userRepo.UpdateUser();
            
        }
    }
}

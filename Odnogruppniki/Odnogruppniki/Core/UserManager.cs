using Odnogruppniki.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Odnogruppniki.Core
{
    public class UserManager : IDisposable
    {
        private DBContext _db = new DBContext();

        public static UserManager Create()
        {
            return new UserManager();
        }

        public void Dispose()
        {
        }

        public async Task<bool> IsValid(string login, string password)
        {
            var test = await _db.Users.ToListAsync();
            var user = await _db.Users.FirstOrDefaultAsync(x => string.Equals(login, x.Login));
            if (user != null)
            {
                if (string.Equals(user.Password, PasswordHash.GetPasswordHash(password)))
                {
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                return false;
            }
        }

        public async Task Register(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }

    public class PasswordHash
    {
        public static string GetPasswordHash(string password)
        {
            var hashGenerator = SHA1.Create();
            char[] chr = password.ToCharArray();
            List<byte> str = new List<byte>();
            foreach(var a in chr)
            {
                str.Add((byte)a);
            }
            var resultArray = hashGenerator.ComputeHash(str.ToArray());
            string result = "";
            foreach(var a in resultArray)
            {
                result += a;
            }
            return result;
        }
    }
}
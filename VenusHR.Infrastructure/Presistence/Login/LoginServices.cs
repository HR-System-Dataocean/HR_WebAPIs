using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Core.Login;
using VenusHR.Infrastructure.Services;
using WorkFlow_EF;

namespace VenusHR.Infrastructure.Presistence.Login
{
    public class LoginServices : ILoginServices
    {
        private readonly ApplicationDBContext _context;
        private GeneralOutputClass<object> Result;  
        private readonly IJwtService _jwtService;

        public LoginServices(ApplicationDBContext context, IJwtService jwtService) 
        { 
            _context = context;
            _jwtService = jwtService;
            Result = new GeneralOutputClass<object>();
        }
        public object Login(string username, string password,int Lang, string deviceToken)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Result.ErrorCode = 1;
                if (Lang == 1)
                {
                    Result.ErrorMessage = "Pleas Enter User Name and Password";

                }
                else { Result.ErrorMessage = "برجاء ادخال اسم المستخدم و كلمة المرور"; }
            }
            else
            {
                try
                {
                    string ORGPassword = _context.Sys_Users.Where(U => U.Code == username).Select(U => U.Password).FirstOrDefault();
                    if (ORGPassword == Encrypt(password, "DataOcean", false))
                    {
                        var user = _context.Sys_Users.FirstOrDefault(U => U.Code == username);
                        if (user != null)
                        {
                            user.DeviceToken = deviceToken;  
                            _context.SaveChanges();
                        }
                        var employee = _context.Hrs_Employees.SingleOrDefault(F => F.Code == username);
                        if (employee != null)
                        {
                            var token = _jwtService.GenerateToken(employee);
                            Result.ErrorCode = 1;
                            Result.ErrorMessage = "Success";
                            Result.ResultObject = new
                            {
                                Employee = employee,
                                Token = token,
                                TokenExpiry = DateTime.UtcNow.AddMinutes(60)
                            };
                        }
                        else
                        {
                            Result.ErrorCode = 0;
                            Result.ErrorMessage = "Employee not found";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = ex.Message;
                }

            }
            return Result;
        }

        public string Encrypt(string sToEncrypt, string sPassword, bool ReturnOnlyNumbersAndLetters)
        {
            string result;
            try
            {
                string text = _Encrypt(sToEncrypt, sPassword, "777777", "SHA1", 2, "GLORY_BE_TO_GOD!", 256);
                if (ReturnOnlyNumbersAndLetters)
                {
                    text = _S2N(text);
                }
                result = text;
            }
            catch (Exception ex)
            {
                result = "ERROR";
            }
            return result;
        }

        private string _Encrypt(string plainText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(passPhrase, Encoding.ASCII.GetBytes(saltValue), hashAlgorithm, passwordIterations);

            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Mode = CipherMode.CBC;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                        rijndaelManaged.CreateEncryptor(
                            passwordDeriveBytes.GetBytes(keySize / 8),
                            Encoding.ASCII.GetBytes(initVector)),
                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                        cryptoStream.FlushFinalBlock();
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        private string _S2N(string text)
        {
             return new string(text.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }

        
    }
}

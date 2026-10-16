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
        
        public object Login(string username, string password, int Lang, string deviceToken)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Result.ErrorCode = 0;
                if (Lang == 1)
                {
                    Result.ErrorMessage = "برجاء ادخال اسم المستخدم و كلمة المرور";
                }
                else 
                { 
                    Result.ErrorMessage = "Please enter username and password";
                }
            }
            else
            {
                try
                {
                    string ORGPassword = _context.Sys_Users.Where(U => U.Code == username).Select(U => U.Password).FirstOrDefault();
                    
                    // Check if user exists
                    if (string.IsNullOrEmpty(ORGPassword))
                    {
                        Result.ErrorCode = 0;
                        if (Lang == 1)
                        {
                            Result.ErrorMessage = "اسم المستخدم غير صحيح";
                        }
                        else
                        {
                            Result.ErrorMessage = "Invalid username";
                        }
                    }
                    // Check if password matches
                    else if (ORGPassword != Encrypt(password, "DataOcean", false))
                    {
                        Result.ErrorCode = 0;
                        if (Lang == 1)
                        {
                            Result.ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة";
                        }
                        else
                        {
                            Result.ErrorMessage = "Invalid username or password";
                        }
                    }
                    // Password matches - successful login
                    else
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
                            bool isDirectManager = _context.Hrs_Employees
                                .Any(E => E.ManagerId == employee.id && E.CancelDate == null);
                            Result.ErrorCode = 1;
                            Result.ErrorMessage = Lang == 1 ? "تم تسجيل الدخول بنجاح" : "Login successful";
                            Result.ResultObject = new
                            {
                                Employee = employee,
                                IsHRTeam = employee.IsHRTeam ?? false,
                                IsDirectManager = isDirectManager,
                                Token = token,
                                // Token does not expire by time; it stays valid until logout or a new login.
                                TokenExpiry = (DateTime?)null
                            };
                        }
                        else
                        {
                            Result.ErrorCode = 0;
                            Result.ErrorMessage = Lang == 1 ? "الموظف غير موجود" : "Employee not found";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = Lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                }
            }
            return Result;
        }

        public object ChangePassword(string username, string currentPassword, string newPassword, string confirmPassword, int Lang)
        {
            Result = new GeneralOutputClass<object>();

            GeneralOutputClass<object> Fail(string arabic, string english)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = Lang == 1 ? arabic : english;
                return Result;
            }

            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                return Fail("برجاء إدخال كلمة المرور الحالية والجديدة وتأكيدها", "Current password, new password and confirmation are required");

            if (newPassword != confirmPassword)
                return Fail("كلمة المرور الجديدة وتأكيدها غير متطابقين", "New password and confirmation do not match");

            try
            {
                var user = _context.Sys_Users.FirstOrDefault(U => U.Code == username);
                if (user == null)
                    return Fail("المستخدم غير موجود", "User not found");

                if (user.CanChangePassword == false)
                    return Fail("غير مسموح لهذا المستخدم بتغيير كلمة المرور", "This user is not allowed to change the password");

                if (user.Password != Encrypt(currentPassword, "DataOcean", false))
                    return Fail("كلمة المرور الحالية غير صحيحة", "Current password is incorrect");

                if (currentPassword == newPassword)
                    return Fail("كلمة المرور الجديدة يجب أن تختلف عن الحالية", "New password must be different from the current password");

                if (user.EnforceAlphaNumericPwd == true && !(newPassword.Any(char.IsLetter) && newPassword.Any(char.IsDigit)))
                    return Fail("كلمة المرور يجب أن تحتوي على حروف وأرقام", "Password must contain both letters and numbers");

                var encrypted = Encrypt(newPassword, "DataOcean", false);
                if (encrypted == "ERROR")
                    return Fail("حدث خطأ أثناء تشفير كلمة المرور", "An error occurred while encrypting the password");

                user.Password = encrypted;
                user.PasswordChangedOn = DateTime.Now;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = Lang == 1 ? "تم تغيير كلمة المرور بنجاح" : "Password changed successfully";
            }
            catch (Exception)
            {
                return Fail("حدث خطأ في الخادم", "Server error occurred");
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

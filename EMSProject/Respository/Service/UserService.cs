using EMSProject.Models;
using EMSProject.Models.ViewModel;
using EMSProject.Respository.Contract;
using EMSProject.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EMSProject.Respository.Service
{
    public class UserService:IUser
    {
        private EMSDBContext dbContext;
        public UserService(EMSDBContext context)
        {
            dbContext = context;
        }

        public SignUp Register(SignUp model)
        {
            if (dbContext.Users.Any(e => e.Email == model.Email))
            {
                return null;
            }
            else
            {
                var user = new Users()
                {
                    Email = model.Email,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsVerified = false,
                    IsActive = true
                };
                dbContext.Users.Add(user);
                string Otp = GenerateOTP();
                SendMail(model.Email, Otp);
                var VAccount = new VerifyAccount()
                {
                    OTP = Otp,
                    UserEmail = model.Email,
                    SendTime = DateTime.Now
                };
                dbContext.VerifyAccounts.Add(VAccount);
                dbContext.SaveChanges();
                return model;
            }
        }
        private  void SendMail(string to,string Otp)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress("demo3408@gmail.com");
            mail.Subject = "Verify Your Account";
            string Body = $"Your OTP is <b> {Otp}</b>  <br/>thanks for choosing us.";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("demo3408@gmail.com", "your password"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);
            
        }
        private string GenerateOTP()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var list = Enumerable.Repeat(0, 8).Select(x => chars[random.Next(chars.Length)]);
            var r = string.Join("", list);
            return r;
        }

        public AuthoEnum AuthenticateUser(SignIn model)
        {
            var user = dbContext.Users.SingleOrDefault(e => e.Email == model.Email && e.Password == model.Password);
           if (user == null)
            {
                return AuthoEnum.FAILED;
            }
           else if (user.IsVerified == true)
            {
                return AuthoEnum.SUCCESS;
            }
           else if (user.IsVerified == false)
            {
                return AuthoEnum.NOTVERIFIED;
            }
            else
            {
                return AuthoEnum.NOTACTIVE;
            }
        }

        public VerifyAccountEnum VerifyAccount(string Otp)
        {
            if (dbContext.VerifyAccounts.Any(e => e.OTP == Otp))
            {
                var Acc = dbContext.VerifyAccounts.SingleOrDefault(e => e.OTP == Otp);
                var User = dbContext.Users.SingleOrDefault(e => e.Email == Acc.UserEmail);
                User.IsVerified = true;
                dbContext.VerifyAccounts.Remove(Acc);
                dbContext.Users.Update(User);
                dbContext.SaveChanges();
                return VerifyAccountEnum.OTPVERIFIED;
            }
            else
            {
                return VerifyAccountEnum.INVALIDOTP;
            }
        }

      public  bool UpdateProfile(string Email, string Path)
        {
            var User = dbContext.Users.SingleOrDefault(e => e.Email == Email);
            User.Image = Path;
            dbContext.Users.Update(User);
            dbContext.SaveChanges();
            return true;
        }
    }
}

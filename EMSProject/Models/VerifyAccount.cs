using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMSProject.Models
{
    public class VerifyAccount
    {
        [Key]
        public int Id { get; set; }
        public string  UserEmail { get; set; }
        public string OTP { get; set; }
        public DateTime SendTime { get; set; }
       
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMSProject.Models
{
    public class EMSDBContext:DbContext
    {
        public EMSDBContext(DbContextOptions<EMSDBContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<VerifyAccount>  VerifyAccounts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employees> Employees { get; set; }

    }
}

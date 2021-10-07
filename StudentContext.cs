using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Signup.Models;

namespace Signup.AppData
{
    public class StudentContext : DbContext
    {
       
            public StudentContext(DbContextOptions options)
                : base(options)
            {
            }
            public DbSet<StudentModel> Students { get; set; }
        public DbSet<ProductModel> Product { get; set; }
    }
    }


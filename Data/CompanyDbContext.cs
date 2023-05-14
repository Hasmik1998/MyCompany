using Chess_Up.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Data
{
    public class CompanyDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Request> Requests { get; set; }

        public DbSet<TrinerRequest> TrinerRequests { get; set; }

        public DbSet<Problem> Problems { get; set; }

        public DbSet<Quiz> QuizProblems { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }

        public DbSet<StackOverflowQuestion> StackOverflowQuestions { get; set; }

        public DbSet<StackOverflowAnswer> StackOverflowAnswers { get; set; }

        public DbSet<ChatRoom> CharRooms { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<RatingsData> RatingsData { get; set;}

        public DbSet<PasswordSecurity> PasswordSecurity { get; set; }

        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) :base(options)
        {

        }
    }
}

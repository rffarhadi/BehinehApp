using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using DLLCore.DBContext.Entities;
using DLLCore.DBContext.Entities.Accounting;
using DLLCore.DBContext.Entities.Accounting.Entry;
using DLLCore.DBContext.Entities.Accounting.RawAccounts;
using DLLCore.DBContext.Entities.InvestorSpecific;
using DLLCore.DBContext.Entities.StockHistoy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DLLCore.DBContext
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(32, 6));
        //}


        //تعریف سرمایه‌گذاران
        public DbSet<InvestorProfile> InvestorProfiles { get; set; }
        public DbSet<InvestorOSIProfile> InvestorOSIProfiles { get; set; }
        public DbSet<InfrencesConclutionProfile> InfrencesConclutionProfiles { get; set; }
        public DbSet<InvestorOperation> InvestorOperations { get; set; }


        //حسابداری 
        public DbSet<RawCategories> RawCategoriesTbl { get; set; }
        public DbSet<RawSubCategories> SubCategoriesTbl { get; set; }
        public DbSet<RawLedgers> RawLedgersTbl { get; set; }
        public DbSet<RawSubLedger> RawSubLedgersTbl { get; set; }
        public DbSet<RawDetaile> RawDetailesTbl { get; set; }
        public DbSet<Journal> JournalsTbl { get; set; }


        //جدول سهام
        public DbSet<SecurityHistory> SecurityTbl { get; set; }
        public DbSet<Token> TokenTbl { get; set; }





    }
}
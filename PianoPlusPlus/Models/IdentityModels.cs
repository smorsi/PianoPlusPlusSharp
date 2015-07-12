using Microsoft.AspNet.Identity.EntityFramework;

namespace PianoPlusPlus.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private string p;

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public ApplicationDbContext(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
    }
}
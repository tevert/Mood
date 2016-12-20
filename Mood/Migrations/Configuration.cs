namespace Mood.Migrations
{
    using System.Data.Entity.Migrations;
    using Mood.Util;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDBContext context)
        {
            context.Upsert(m => m.Description, null,
                new Models.Mood() { Id = 1, Description = "angry" },
                new Models.Mood() { Id = 2, Description = "miffed" },
                new Models.Mood() { Id = 3, Description = "neutral" },
                new Models.Mood() { Id = 4, Description = "ok" },
                new Models.Mood() { Id = 5, Description = "fantastic" });
        }
    }
}

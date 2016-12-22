namespace Mood.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDBContext context)
        {
            context.Moods.AddOrUpdate(m => m.Description,
                new Models.Mood() { Description = "Angry" },
                new Models.Mood() { Description = "Miffed" },
                new Models.Mood() { Description = "Neutral" },
                new Models.Mood() { Description = "Ok" },
                new Models.Mood() { Description = "Fantastic" });

            // Find tevert2@gmail.com
            var tyler = context.Users.FirstOrDefault(u => u.UserName == "tevert2@gmail.com");
            if (tyler != null)
            {
                // Make a survey for Tyler
                context.Surveys.AddOrUpdate(s => s.Description,
                    new Models.Survey() { Description = "Default", Owner = tyler });
            }

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var firstError = e.EntityValidationErrors.First().ValidationErrors.First();
                string message = $"Property {firstError.PropertyName}: {firstError.ErrorMessage}";
                throw new System.Exception(message);
            }
        }
    }
}

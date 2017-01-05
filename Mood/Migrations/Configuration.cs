namespace Mood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;

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

            // Special change to migrate ownership of the original survey from Tyler to Jake
            var guid = Guid.Parse("9f9f25f0-926a-4df9-89a3-08164ceea253");
            var originalSurvey = context.Surveys.FirstOrDefault(s => s.Id == guid);
            if (originalSurvey != null)
            {
                var jake = context.Users.FirstOrDefault(u => u.Email == "jacob.scherrer@centare.com");
                if (jake != null)
                {
                    originalSurvey.Owner = jake;
                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var message = new StringBuilder();
                foreach (var error in e.EntityValidationErrors)
                {
                    string properties = error.Entry.CurrentValues.PropertyNames.Select(n => n + "=" + error.Entry.CurrentValues[n]).Aggregate((p1, p2) => p1 + ", " + p2);
                    message.Append($"Entity {error.Entry.Entity.ToString()}: {properties}\n");

                    foreach (var propertyError in error.ValidationErrors)
                    {
                        message.Append($"\tProperty {propertyError.PropertyName}: {propertyError.ErrorMessage}\n");
                    }
                }
                throw new System.Exception(message.ToString());
            }
        }

        internal static void Initialize(string updateDbSetting)
        {
            if (bool.Parse(updateDbSetting))
            {
                var migrator = new DbMigrator(new Configuration());
                migrator.Update();
            }
        }
    }
}

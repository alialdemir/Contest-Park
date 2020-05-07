using System.Reflection;

namespace ContestPark.Notification.API.Migrations
{
    public class MigrationAssembly
    {
        public static Assembly[] GetAssemblies()
        {
            return new Assembly[]
            {
                typeof(Initial).Assembly,
                typeof(NoticeMigration).Assembly,
            };
        }
    }
}

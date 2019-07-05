using System.Reflection;

namespace ContestPark.Follow.API.Migrations
{
    public class MigrationAssembly
    {
        public static Assembly[] GetAssemblies()
        {
            return new Assembly[]
            {
                typeof(Initial).Assembly,
            };
        }
    }
}

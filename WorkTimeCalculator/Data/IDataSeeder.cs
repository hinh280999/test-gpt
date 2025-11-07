using System.Threading.Tasks;

namespace WorkTimeCalculator.Data;

public interface IDataSeeder
{
    Task SeedAsync();
}

using System.Threading.Tasks;

namespace Ecommerce.Admin.Data;

public interface IAdminDbSchemaMigrator
{
    Task MigrateAsync();
}

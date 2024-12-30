using System.Data;

namespace AudioBooks.Application.Data;

public interface ISqlConnection
{
    IDbConnection ConnectionBuild();
}

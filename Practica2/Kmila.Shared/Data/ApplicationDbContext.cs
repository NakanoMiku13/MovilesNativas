using Kmila.Shared.Models;
using SQLite;

namespace Kmila.Shared.Data;

public class ApplicationDbContext : SQLiteAsyncConnection
{
    public ApplicationDbContext(string path) : base(path)
    {
    }
    public async Task InitDBAsync()
    {
        try
        {
            var result = await CreateTableAsync<Project>();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        try
        {
            var result = await CreateTableAsync<ProjectFile>();
        }catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public AsyncTableQuery<Project> Projects => Table<Project>();
    public AsyncTableQuery<ProjectFile> ProjectFiles => Table<ProjectFile>();
}
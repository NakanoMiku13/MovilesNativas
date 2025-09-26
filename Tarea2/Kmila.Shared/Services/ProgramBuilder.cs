using Debugger;
using Kmila.Shared.Models;
namespace Kmila.Shared.Services;

public class ProgramBuilder
{
    public bool Build(ProjectFile file)
    {
        string filePath = Path.Join(file.Project.BasePath, file.FileName);
        Debugger.Repositories.Parser parser = new(filePath);
        parser.ParseFile();
        return true;
    }
}
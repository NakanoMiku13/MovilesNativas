using Debugger;
using Kmila.Shared.Models;
namespace Kmila.Shared.Services;



public class ProgramBuilder : IDisposable
{
    public Debugger.Repositories.Parser Parser { get; private set; } = default!;
    private TimeMachine.Repositories.Timer _timeMachine = default!;
    private readonly CancellationTokenSource _cts = new();
    public bool Build(ProjectFile file)
    {
        string filePath = Path.Join(file.Project.BasePath, file.FileName);
        string ext = Path.GetExtension(filePath);
        if (ext.ToLowerInvariant() != ".vhdl") throw new Exception($"File not valid: {Path.GetFileName(filePath)}");
        Parser = new(filePath);
        Parser.ParseFile();
        if (!Parser.Entities.Any())
            throw new Exception("File not valid");
        return true;
    }

    public async Task SetSimulationTimes(TimeSpan WaitTime)
    {
        _timeMachine = new();
        try
        {
            await Task.Delay(WaitTime, _cts.Token);
        }
        finally
        {
        }
    }
    public void Dispose()
    {
        _cts.Cancel();
        _cts?.Dispose();
    }
}
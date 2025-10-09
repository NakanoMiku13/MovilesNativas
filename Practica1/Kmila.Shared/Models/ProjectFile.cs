using System.ComponentModel.DataAnnotations;
using SQLite;

namespace Kmila.Shared.Models;

public class ProjectFile
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Required(ErrorMessage = "File name required")]
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int ProjectId { get; set; } = -1;
    [Ignore]
    public Project Project { get; set; } = default!;
}

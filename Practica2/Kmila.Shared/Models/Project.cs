using System.ComponentModel.DataAnnotations;
using SQLite;

namespace Kmila.Shared.Models;

public class Project
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Required(ErrorMessage = "Project name required")]
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; } = DateTime.Now;
}
namespace WebApi.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Module> Modules {get; set;} = new List<Module>();
}
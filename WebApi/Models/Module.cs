namespace WebApi.Models;

public class Module
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Course? Course {get; set;}
    public List<Assignment> Assignments {get; set;} = new List<Assignment>();
}
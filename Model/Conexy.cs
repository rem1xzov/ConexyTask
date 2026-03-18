namespace ConexyTask.Model;

public class Conexy
{
    public const int MAX_LENGTH = 1000;
    public Conexy(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public static (Conexy conexy, string Error) Create(Guid id, string name, string description)
    {
        var error = string.Empty;

        if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_LENGTH)
        {
            error = "Name is too long or can not be empty";
        }

        if (description.Length > MAX_LENGTH)
        {
            error = "Description is too long";
        }
        
        var conexy = new Conexy(id, name, description);
        return (conexy, error);
    }
}
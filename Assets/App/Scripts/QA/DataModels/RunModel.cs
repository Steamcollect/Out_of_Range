using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("runs")]
public class RunModel : BaseModel
{
    [PrimaryKey("id", false)] // false car c'est souvent auto-généré (UUID ou Serial)
    public Guid Id { get; set; }

    [Column("created_at")]
    public string CreatedAt { get; set; }

    [Column("build")]
    public string Build { get; set; }
    
    // Constructeur vide requis
    public RunModel() {}
}
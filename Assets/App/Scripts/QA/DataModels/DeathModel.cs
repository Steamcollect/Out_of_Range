using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("deaths")]
public class DeathModel : BaseModel
{
    [PrimaryKey("id", false)] // false car c'est souvent auto-généré (UUID ou Serial)
    public Guid Id { get; set; }

    [Column("created_at")]
    public string CreatedAt { get; set; }

    [Column("run")]
    public Guid Run { get; set; }
    
    [Column("position")]
    public string Position { get; set; }
    
    // Constructeur vide requis
    public DeathModel() {}
}
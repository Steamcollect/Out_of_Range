using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("checkpoints")]
public class CheckpointModel : BaseModel
{
    [PrimaryKey("id", false)] // false car c'est souvent auto-généré (UUID ou Serial)
    public Guid Id { get; set; }

    [Column("created_at")]
    public string CreatedAt { get; set; }

    [Column("run")]
    public Guid Run { get; set; }
    
    // Constructeur vide requis
    public CheckpointModel() {}
}
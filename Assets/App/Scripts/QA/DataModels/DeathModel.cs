using System;
using UnityEngine;
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
    public PositionDto Position { get; set; }
    
    // Constructeur vide requis
    public DeathModel() {}
}

// DTO simple pour sérialiser la position sans boucle de référence (UnityEngine.Vector3 provoque des problèmes avec Newtonsoft.Json)
[Serializable]
public class PositionDto
{
    public float x;
    public float y;
    public float z;

    public PositionDto() { }

    public PositionDto(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public PositionDto(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

using System;
using System.Threading.Tasks;
using UnityEngine;

public class DeathQueries
{
    public async Task<Guid> CreateDeath(Guid runID, Vector3 position)
    {
        var newDeath = new DeathModel
        {
            Run = runID,
            Position = new PositionDto(position),
            CreatedAt = DateTime.UtcNow.ToString("o") // Format ISO 8601
        };

        // Attendre que SupabaseManager soit initialisé (avec timeout)
        if (SupabaseManager.Instance == null)
            throw new InvalidOperationException("SupabaseManager.Instance is null.");

        var timeout = TimeSpan.FromSeconds(10);
        var start = DateTime.UtcNow;
        while (!SupabaseManager.Instance.IsInitialized)
        {
            if (DateTime.UtcNow - start > timeout)
                throw new TimeoutException("Timed out waiting for SupabaseManager to initialize.");

            await Task.Delay(100);
        }

        var response = await SupabaseManager.Instance.Supabase
            .From<DeathModel>()
            .Insert(newDeath);

        if (response.Model != null)
        {
            return response.Model.Id;
        }

        throw new InvalidOperationException("Failed to create Death: no model returned from the database.");
    }
}
using System;
using System.Threading.Tasks;

public class CheckpointQueries
{
    public async Task<Guid> CreateCheckpoint(Guid runID)
    {
        var newCheckpoint = new CheckpointModel
        {
            CreatedAt = DateTime.UtcNow.ToString("o"), // Format ISO 8601
            Run = runID
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
            .From<CheckpointModel>()
            .Insert(newCheckpoint);

        if (response.Model != null)
        {
            return response.Model.Id;
        }

        throw new InvalidOperationException("Failed to create Checkpoint: no model returned from the database.");
    }
}
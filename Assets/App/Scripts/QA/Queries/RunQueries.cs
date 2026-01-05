using System;
using System.Threading.Tasks;

public class RunQueries
{
    public async Task<Guid> CreateRun(string buildVersion)
    {
        var newRun = new RunModel
        {
            Build = buildVersion,
            CreatedAt = DateTime.UtcNow.ToString("o") // Format ISO 8601
        };

        // Attendre que SupabaseManager soit initialisé (avec timeout)
        if (SupabaseManager.Instance == null)
            throw new InvalidOperationException("SupabaseManager.Instance is null.");

        var timeout = TimeSpan.FromSeconds(10);
        var start = DateTime.UtcNow;
        while (!SupabaseManager.Instance.isInitialized)
        {
            if (DateTime.UtcNow - start > timeout)
                throw new TimeoutException("Timed out waiting for SupabaseManager to initialize.");

            await Task.Delay(100);
        }

        var response = await SupabaseManager.Instance.Supabase
            .From<RunModel>()
            .Insert(newRun);

        if (response.Model != null)
        {
            return response.Model.Id;
        }

        throw new InvalidOperationException("Failed to create Run: no model returned from the database.");
    }
}

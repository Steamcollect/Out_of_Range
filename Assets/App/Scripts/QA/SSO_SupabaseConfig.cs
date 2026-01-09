using UnityEngine;

[CreateAssetMenu(fileName = "SSO_SupabaseConfig", menuName = "SSO/_/SSO_SupabaseConfig")]
public class SSO_SupabaseConfig : ScriptableObject
{
    public string supabaseUrl = "YOUR_SUPABASE_URL";
    public string supabaseAnonKey = "YOUR_SUPABASE_ANON_KEY";
}
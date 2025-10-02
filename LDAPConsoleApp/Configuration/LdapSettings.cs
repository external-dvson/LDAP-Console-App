namespace LDAPConsoleApp.Configuration
{
    public class LdapSettings
    {
        public string Domain { get; set; } = string.Empty;
        public string SecondaryDomain { get; set; } = string.Empty;
        public string[] GroupNames { get; set; } = Array.Empty<string>();
        public string GroupPrefix { get; set; } = string.Empty;
        public int MaxResults { get; set; }
        public int MaxGroupResults { get; set; }
        public int MaxDisplayItems { get; set; }
    }
}
namespace LDAPConsoleApp.Configuration
{
    public class LdapSettings
    {
        public string Domain { get; set; } = "APAC.bosch.com";
        public string SecondaryDomain { get; set; } = "DE.bosch.com";
        public string DefaultGroupName { get; set; } = "IdM2BCD_FCMCONSOLE_TRANSPORT_ADMIN";
        public int MaxResults { get; set; } = 50;
        public int MaxGroupResults { get; set; } = 100;
        public int MaxDisplayItems { get; set; } = 10;
    }
}
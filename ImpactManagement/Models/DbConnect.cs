namespace ImpactManagement.Models
{
    public class DbConnect
    {
        public string ConnectName { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string Schema { get; set; } = string.Empty;

        public string DisplayInfo => $"{ConnectName} ({Ip}:{Port}/{Schema})";
    }
}

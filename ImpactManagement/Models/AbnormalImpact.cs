namespace ImpactManagement.Models
{
    public class AbnormalImpact
    {
        public string SceneName { get; set; } = string.Empty;
        public string ImpactName { get; set; } = string.Empty;
        public string Sql { get; set; } = string.Empty;
        public string GroupField { get; set; } = string.Empty;
        public string ConnectName { get; set; } = string.Empty;
        public bool GroupFlag => !string.IsNullOrWhiteSpace(GroupField);
    }
}

namespace JobScheduler.Common
{
    public struct JobSettings
    {
        public bool RequestRecovery { get; set; }
        public bool StoreDurably { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
    }
}
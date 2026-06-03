namespace SmartHomeAutomationSystem.Models
{
    /// <summary>
    /// Represents a generated report for a specific domain area (devices, rules, users).
    /// </summary>
    public class Report
    {
        /// <summary>Gets or sets the report category (e.g. "Device Usage", "Automation Rules").</summary>
        public required string Type { get; set; }

        /// <summary>Gets or sets the formatted text body of the report.</summary>
        public required string Content { get; set; }

        /// <summary>Gets or sets the timestamp when the report was generated.</summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>Returns the report with its type header and content body.</summary>
        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Type} Report:\n{Content}";
        }
    }
}

using System;

namespace LenBersih.Shared;

public class Report
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateReported { get; set; } = DateTime.UtcNow;
}
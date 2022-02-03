using System;
using System.Net;
using System.Text.Json;
public class BError
{
    public string Message { get; set; } = string.Empty;
    public int Status { get; set; } = 0;
    public string Error { get; set; } = string.Empty;

    public string? Causes { get; set; }

}


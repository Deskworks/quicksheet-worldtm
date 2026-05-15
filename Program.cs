using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// QuickSheet World Time Extension.
/// Prefix: "worldtm". Usage: "worldtm: London, Tokyo, NY"
/// No params = common business timezones (UTC, US coasts, London, Tokyo).
/// Pure local — uses System.TimeZoneInfo, zero network calls.
/// </summary>
class Program
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["UTC"] = "UTC", ["GMT"] = "UTC",
        ["EST"] = "America/New_York", ["EDT"] = "America/New_York",
        ["CST"] = "America/Chicago", ["CDT"] = "America/Chicago",
        ["MST"] = "America/Denver", ["MDT"] = "America/Denver",
        ["PST"] = "America/Los_Angeles", ["PDT"] = "America/Los_Angeles",
        ["CET"] = "Europe/Berlin", ["CEST"] = "Europe/Berlin",
        ["BST"] = "Europe/London", ["JST"] = "Asia/Tokyo",
        ["IST"] = "Asia/Kolkata", ["AEST"] = "Australia/Sydney",
        ["AEDT"] = "Australia/Sydney", ["KST"] = "Asia/Seoul",
        ["SGT"] = "Asia/Singapore", ["HKT"] = "Asia/Hong_Kong",
        ["NY"] = "America/New_York", ["LA"] = "America/Los_Angeles",
        ["SF"] = "America/Los_Angeles", ["London"] = "Europe/London",
        ["Paris"] = "Europe/Paris", ["Berlin"] = "Europe/Berlin",
        ["Amsterdam"] = "Europe/Amsterdam", ["Tokyo"] = "Asia/Tokyo",
        ["Shanghai"] = "Asia/Shanghai", ["Beijing"] = "Asia/Shanghai",
        ["Mumbai"] = "Asia/Kolkata", ["Delhi"] = "Asia/Kolkata",
        ["Sydney"] = "Australia/Sydney", ["Melbourne"] = "Australia/Melbourne",
        ["Seoul"] = "Asia/Seoul", ["Singapore"] = "Asia/Singapore",
        ["HongKong"] = "Asia/Hong_Kong", ["Dubai"] = "Asia/Dubai",
        ["Istanbul"] = "Europe/Istanbul", ["SaoPaulo"] = "America/Sao_Paulo",
        ["Toronto"] = "America/Toronto", ["Vancouver"] = "America/Vancouver",
        ["Chicago"] = "America/Chicago", ["Denver"] = "America/Denver",
    };

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string? line;
        while ((line = Console.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            try
            {
                using var doc = JsonDocument.Parse(line);
                string? type = doc.RootElement.TryGetProperty("type", out var tp) ? tp.GetString() : null;
                switch (type)
                {
                    case "init": HandleInit(); break;
                    case "activate": HandleActivate(doc.RootElement); break;
                    case "deactivate": break;
                }
            }
            catch (Exception ex)
            {
                SendJson(new { type = "error", id = "", message = $"Parse error: {ex.Message}" });
            }
        }
    }

    static void HandleInit()
    {
        SendJson(new
        {
            type = "register",
            prefix = "worldtm",
            name = "World Time",
            version = "1.0.0"
        });
        SendLog("World Time registered. Usage: worldtm: London, Tokyo, NY");
    }

    static void HandleActivate(JsonElement root)
    {
        string id = root.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? "" : "";

        // Collect raw params — each element is one timezone token (already comma-split by QuickSheet)
        var rawTokens = new List<string>();
        if (root.TryGetProperty("params", out var paramsProp) && paramsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var p in paramsProp.EnumerateArray())
            {
                string? val = p.GetString()?.Trim();
                if (!string.IsNullOrEmpty(val))
                    rawTokens.Add(val);
            }
        }

        try
        {
            var tzNames = new List<(string label, string tzId)>();
            foreach (var token in rawTokens)
            {
                var resolved = ResolveTimezone(token);
                if (resolved.HasValue)
                    tzNames.Add(resolved.Value);
            }

            if (tzNames.Count == 0) tzNames = GetDefaultTimezones();

            var now = DateTimeOffset.UtcNow;
            var cells = new List<(int r, int c, string v)>();

            cells.Add((0, 0, "🌍 City"));
            cells.Add((0, 1, "Time"));
            cells.Add((0, 2, "Date"));
            cells.Add((0, 3, "Offset"));
            cells.Add((0, 4, "Status"));

            for (int i = 0; i < tzNames.Count; i++)
            {
                int row = i + 1;
                var (label, tzId) = tzNames[i];

                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
                    var localTime = TimeZoneInfo.ConvertTime(now, tz);
                    var offset = tz.GetUtcOffset(now);
                    int hour = localTime.Hour;

                    string icon = hour switch
                    {
                        >= 6 and < 12 => "🌅",
                        >= 12 and < 17 => "☀️",
                        >= 17 and < 21 => "🌆",
                        _ => "🌙"
                    };

                    string status = hour switch
                    {
                        >= 9 and < 17 => "🟢 Business hrs",
                        >= 7 and < 9 => "🟡 Early",
                        >= 17 and < 19 => "🟡 Late",
                        _ => "🔴 Off hours"
                    };

                    string offsetStr = offset >= TimeSpan.Zero
                        ? $"UTC+{offset.Hours:D2}:{offset.Minutes:D2}"
                        : $"UTC-{Math.Abs(offset.Hours):D2}:{Math.Abs(offset.Minutes):D2}";

                    cells.Add((row, 0, $"{icon} {label}"));
                    cells.Add((row, 1, localTime.ToString("HH:mm")));
                    cells.Add((row, 2, localTime.ToString("ddd MMM dd")));
                    cells.Add((row, 3, offsetStr));
                    cells.Add((row, 4, status));
                }
                catch
                {
                    cells.Add((row, 0, $"⚠️ {label}"));
                    cells.Add((row, 1, "Unknown TZ"));
                    cells.Add((row, 2, ""));
                    cells.Add((row, 3, ""));
                    cells.Add((row, 4, $"Try: {GetSuggestion(label)}"));
                }
            }

            cells.Add((tzNames.Count + 1, 0, $"Updated {now:HH:mm} UTC"));
            SendCells(id, cells);
        }
        catch (Exception ex)
        {
            SendCells(id, new List<(int r, int c, string v)>
            {
                (0, 0, $"⚠️ Error: {ex.Message}"),
                (1, 0, "Usage: worldtm: London, Tokyo, NY, UTC")
            });
        }
    }

    static (string label, string tzId)? ResolveTimezone(string token)
    {
        string key = token.Trim().Replace(" ", "");
        if (string.IsNullOrEmpty(key)) return null;

        // Check aliases
        if (Aliases.TryGetValue(key, out var aliasId))
            return (FriendlyName(token.Trim()), aliasId);

        // Try as full IANA ID
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(token.Trim());
            return (FriendlyName(token.Trim()), token.Trim());
        }
        catch { }

        // Fuzzy match
        var match = TimeZoneInfo.GetSystemTimeZones()
            .FirstOrDefault(tz => tz.Id.Contains(key, StringComparison.OrdinalIgnoreCase)
                || tz.DisplayName.Contains(key, StringComparison.OrdinalIgnoreCase));
        if (match != null)
            return (FriendlyName(key), match.Id);

        return (key, key);
    }

    static List<(string label, string tzId)> GetDefaultTimezones()
    {
        return new()
        {
            ("UTC", "UTC"),
            ("New York", "America/New_York"),
            ("London", "Europe/London"),
            ("Berlin", "Europe/Berlin"),
            ("Tokyo", "Asia/Tokyo"),
            ("Sydney", "Australia/Sydney"),
        };
    }

    static string FriendlyName(string input)
    {
        if (input.Contains('/'))
            return input.Split('/').Last().Replace("_", " ");
        return input;
    }

    static string GetSuggestion(string input)
    {
        var candidates = Aliases.Keys
            .Where(k => k.Contains(input, StringComparison.OrdinalIgnoreCase))
            .Take(3).ToArray();
        return candidates.Length > 0 ? string.Join(", ", candidates) : "America/New_York";
    }

    static void SendCells(string id, List<(int r, int c, string v)> cells)
    {
        SendJson(new
        {
            type = "write",
            id,
            cells = cells.Select(c => new { r = c.r, c = c.c, v = c.v }).ToArray()
        });
    }

    static void SendJson(object obj)
    {
        Console.WriteLine(JsonSerializer.Serialize(obj, JsonOpts));
        Console.Out.Flush();
    }

    static void SendLog(string message)
    {
        SendJson(new { type = "log", message });
    }
}

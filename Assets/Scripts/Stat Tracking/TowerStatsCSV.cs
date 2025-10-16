using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public interface IWaveStatsSink
{
    void AppendTowerRows(string runId, int wave, IEnumerable<ITowerStatsProvider> towers);
    void ClearAllEntries();
}
public class TowerStatsCSV : IWaveStatsSink
{
    private readonly string path;
    private readonly object _lock = new();
    private const string header = "RunID,TowerID,Wave,TotalDamage,AvgDPS,MaxDPS\n";
    public TowerStatsCSV(string filename = "tower_wave_stats.csv")
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string dir = Path.Combine(projectRoot, "Stats Data");
        Directory.CreateDirectory(dir);
        path = Path.Combine(dir, filename);
        if (!File.Exists(path))
            File.WriteAllText(path, header);
    }
    public void AppendTowerRows(string runId, int wave, IEnumerable<ITowerStatsProvider> towers)
    {
        var sb = new StringBuilder();
        foreach (var t in towers)
        {
            sb.AppendLine(string.Join(",",
                runId,
                t.ID,
                wave,
                t.TotalWaveDamage.ToString(CultureInfo.InvariantCulture),
                t.AverageWaveDPS.ToString(CultureInfo.InvariantCulture),
                t.MaxWaveDPS.ToString(CultureInfo.InvariantCulture)
            ));
        }
        lock (_lock) { File.AppendAllText(path, sb.ToString()); }
    }
    public void ClearAllEntries()
    {
        if (File.Exists(path))
        {
            File.WriteAllText(path, header);
            Debug.Log($"Cleared stats at {path}");
        }
    }
    private static string Escape(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        if (s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0) return $"\"{s.Replace("\"", "\"\"")}\"";
        return s;
    }
}
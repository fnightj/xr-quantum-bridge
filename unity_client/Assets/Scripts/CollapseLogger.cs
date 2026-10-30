using UnityEngine;
using System.IO;
using System.Text;

public class CollapseLogger : MonoBehaviour
{
    public static CollapseLogger Instance;

    private string logPath;
    private StringBuilder logBuffer = new StringBuilder();

    void Awake()
    {
        Instance = this;
        logPath = Path.Combine(Application.persistentDataPath, "collapse_log.csv");
        // ±íÍ·
        File.WriteAllText(logPath, "timestamp,click_number,result,server\n");
        Debug.Log("[Logger] Log file at: " + logPath);
    }

    public void Log(int clickNum, string result, string server)
    {
        string line = $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss},{clickNum},{result},{server}\n";
        File.AppendAllText(logPath, line);
        Debug.Log("[Logger] Recorded: " + line.Trim());
    }
}
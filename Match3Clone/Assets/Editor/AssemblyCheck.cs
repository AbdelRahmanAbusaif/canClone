using UnityEditor;
using UnityEngine;
using System.IO;

public class AsmdefPlatformScanner
{
    [MenuItem("Tools/Scan ASMDEF Platforms")]
    public static void ScanAsmdefs()
    {
        string[] asmdefFiles = Directory.GetFiles(Application.dataPath, "*.asmdef", SearchOption.AllDirectories);

        foreach (var file in asmdefFiles)
        {
            string json = File.ReadAllText(file);
            string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace("\\", "/");

            if (json.Contains("\"includePlatforms\""))
            {
                Debug.Log($"🔎 {relativePath} → {json}");
            }
            else
            {
                Debug.Log($"✅ {relativePath} → No platform restriction (Any Platform)");
            }
        }

        Debug.Log("✅ ASMDEF scan complete. Look for any Android-only SDKs that list \"iOS\" in includePlatforms.");
    }
}

using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ProjectTimeTracker : EditorWindow
{
    private static double lastUpdateTime;
    private static double currentSessionSeconds;
    private static double totalSecondsSaved;
    private static string prefsKey = "TimeTracker_ProjectName"; // UPDATE THIS FOR EVERY PROJECT

    static ProjectTimeTracker()
    {
        lastUpdateTime = EditorApplication.timeSinceStartup;
        LoadTotalTime();

        // precaution for crash
        EditorApplication.update += OnUpdate;
        EditorApplication.quitting += SaveTime;
    }

    private static void LoadTotalTime()
    {
        string savedTimeStr = EditorPrefs.GetString(prefsKey, "0");
        if (!double.TryParse(savedTimeStr, out totalSecondsSaved))
        {
            totalSecondsSaved = 0;
        }
    }

    private static void SaveTime()
    {
        double grandTotal = totalSecondsSaved + currentSessionSeconds;
        EditorPrefs.SetString(prefsKey, grandTotal.ToString("F2"));
        
        totalSecondsSaved = grandTotal;
        currentSessionSeconds = 0;
    }

    private static void OnUpdate()
    {
        double now = EditorApplication.timeSinceStartup;
        double deltaTime = now - lastUpdateTime;
        lastUpdateTime = now;

        currentSessionSeconds += deltaTime;

        // automaticly save the time every 5 min (300 sec)
        if (currentSessionSeconds >= 300) 
        {
            SaveTime();
        }
    }

    [MenuItem("Tools/Time Tracker")]
    public static void ShowWindow()
    {
        ProjectTimeTracker window = GetWindow<ProjectTimeTracker>(true, "Project Time Tracker", true);
        window.minSize = new Vector2(280, 190);
        window.maxSize = new Vector2(280, 190);
    }

    void OnGUI()
    {
        double grandTotalSeconds = totalSecondsSaved + currentSessionSeconds;

        GUIStyle timeStyle = new GUIStyle(EditorStyles.boldLabel) { 
            alignment = TextAnchor.MiddleCenter, 
            fontSize = 20 
        };
        GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel) { 
            alignment = TextAnchor.MiddleCenter 
        };

        EditorGUILayout.Space(12);

        // --- 1. Current Session ---
        EditorGUILayout.BeginVertical("helpBox");
        GUILayout.Label("Current Session", headerStyle);
        
        int sHours = Mathf.FloorToInt((float)currentSessionSeconds / 3600f);
        int sMinutes = Mathf.FloorToInt(((float)currentSessionSeconds % 3600f) / 60f);
        int sSeconds = Mathf.FloorToInt((float)currentSessionSeconds % 60f);
        
        GUI.color = new Color(0.4f, 1f, 0.4f); 
        GUILayout.Label(string.Format("{0:D2}:{1:D2}:{2:D2}", sHours, sMinutes, sSeconds), timeStyle);
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(8);

        // --- 2. Total Time ---
        EditorGUILayout.BeginVertical("helpBox");
        GUILayout.Label("Total Time", headerStyle);
        
        int tHours = Mathf.FloorToInt((float)(grandTotalSeconds / 3600.0));
        int tMinutes = Mathf.FloorToInt((float)((grandTotalSeconds % 3600.0) / 60.0));
        int tSeconds = Mathf.FloorToInt((float)(grandTotalSeconds % 60.0));
        
        GUI.color = new Color(0.4f, 0.8f, 1f); 
        GUILayout.Label(string.Format("{0:D2}:{1:D2}:{2:D2}", tHours, tMinutes, tSeconds), timeStyle);
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(12);

        // --- 3. Close Window ---
        if (GUILayout.Button("Close Window", GUILayout.Height(30)))
        {
            this.Close();
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}

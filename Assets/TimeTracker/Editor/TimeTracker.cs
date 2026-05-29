using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ProjectTimeTracker : EditorWindow
{
    private static double sessionStartTime;
    private static string prefsKey = "TotalTime_" + Application.productName;

    static ProjectTimeTracker()
    {
        sessionStartTime = EditorApplication.timeSinceStartup;
        EditorApplication.quitting += OnQuitting;
    }

    private static void OnQuitting()
    {
        double currentSessionTime = EditorApplication.timeSinceStartup - sessionStartTime;
        float totalTimeSaved = EditorPrefs.GetFloat(prefsKey, 0f);
        EditorPrefs.SetFloat(prefsKey, totalTimeSaved + (float)currentSessionTime);
    }

    [MenuItem("Tools/Project Time Tracker")]
    public static void ShowWindow()
    {
        ProjectTimeTracker window = GetWindow<ProjectTimeTracker>(true, "Zaman Takibi", true);
        // Boyutları içeriğe tam sığacak şekilde optimize ettik
        window.minSize = new Vector2(280, 190);
        window.maxSize = new Vector2(280, 190);
    }

    void OnGUI()
    {
        // Zaman Hesaplamaları
        float totalSecondsSaved = EditorPrefs.GetFloat(prefsKey, 0f);
        double currentSessionSeconds = EditorApplication.timeSinceStartup - sessionStartTime;
        float grandTotalSeconds = totalSecondsSaved + (float)currentSessionSeconds;

        // Stil Hazırlıkları
        GUIStyle timeStyle = new GUIStyle(EditorStyles.boldLabel) { 
            alignment = TextAnchor.MiddleCenter, 
            fontSize = 20 // Okunabilirliği artırmak için biraz büyüttük
        };
        GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel) { 
            alignment = TextAnchor.MiddleCenter 
        };

        EditorGUILayout.Space(12);

        // --- 1. ŞİMDİKİ OTURUM ---
        EditorGUILayout.BeginVertical("helpBox");
        GUILayout.Label("ŞİMDİKİ OTURUM", headerStyle);
        
        int sHours = Mathf.FloorToInt((float)currentSessionSeconds / 3600f);
        int sMinutes = Mathf.FloorToInt(((float)currentSessionSeconds % 3600f) / 60f);
        int sSeconds = Mathf.FloorToInt((float)currentSessionSeconds % 60f);
        
        GUI.color = new Color(0.4f, 1f, 0.4f); // Yeşil
        GUILayout.Label(string.Format("{0:D2}:{1:D2}:{2:D2}", sHours, sMinutes, sSeconds), timeStyle);
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(8);

        // --- 2. TOPLAM SÜRE ---
        EditorGUILayout.BeginVertical("helpBox");
        GUILayout.Label("TOPLAM ÇALIŞMA SÜRESİ", headerStyle);
        
        int tHours = Mathf.FloorToInt(grandTotalSeconds / 3600f);
        int tMinutes = Mathf.FloorToInt((grandTotalSeconds % 3600f) / 60f);
        int tSeconds = Mathf.FloorToInt(grandTotalSeconds % 60f);
        
        GUI.color = new Color(0.4f, 0.8f, 1f); // Mavi
        GUILayout.Label(string.Format("{0:D2}:{1:D2}:{2:D2}", tHours, tMinutes, tSeconds), timeStyle);
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(12);

        // --- 3. KAPAT BUTONU ---
        if (GUILayout.Button("Pencereyi Kapat", GUILayout.Height(30)))
        {
            this.Close();
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
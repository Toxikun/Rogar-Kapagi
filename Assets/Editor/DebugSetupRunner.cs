using UnityEditor;
using UnityEngine;

public static class DebugSetupRunner
{
    [MenuItem("Tools/Debug Setup Run")]
    public static void Run()
    {
        try
        {
            CubeEscapeSetup.Setup();
            Debug.Log("Setup completed without errors!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Setup failed: {ex.Message}\n{ex.StackTrace}");
        }
    }
}

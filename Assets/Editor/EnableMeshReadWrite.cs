using UnityEngine;
using UnityEditor;

public class EnableReadWrite
{
    [MenuItem("Tools/Enable Read/Write on Selected Model")]
    static void Enable()
    {
       
        Object obj = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(obj);

        ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
        if (importer != null)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
            Debug.Log("Enabled Read/Write for: " + obj.name);
        }
        else
        {
            Debug.LogWarning("Please select a valid model file first.");
        }
    }
}

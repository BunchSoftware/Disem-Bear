using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CreateTextFile
{
    [MenuItem("Assets/Create/Text File/New Text File")]
    private static void CreateNewTextFile()
    {
        string folderGUID = Selection.assetGUIDs[0];
        string projectFolderPath = AssetDatabase.GUIDToAssetPath(folderGUID);
        string folderDirectory = Path.GetFullPath(projectFolderPath);

        string contents = "This is a new text file!";

        File.WriteAllText(folderDirectory + "/NewTextFile.txt", contents, Encoding.UTF8);

        AssetDatabase.Refresh();
    }
}

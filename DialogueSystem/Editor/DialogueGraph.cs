using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

[Serializable]
[Graph(AssetExtension)]
public class DialogueGraph : Graph
{
    public const string AssetExtension = "dialoguegraph";
    
    [MenuItem("Assets/Create/Dialogue Graph")]
    private static void CreateAssetFile()
    {
        GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>();
    }
}

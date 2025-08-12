using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace DH.Core.Editor.StateMachine
{
    public class StateGenerator : EditorWindow
    {
        private MonoScript stateMachineScript;
        private string outputPath = "Assets/Scripts/StateMachine/";
        private Type stateMachineType;
        private string enumName;
        private string[] stateNames;
        private Dictionary<string, bool> stateSelection = new Dictionary<string, bool>();
        private bool useCustomPrefix = false;
        private string customPrefix = "";

        [MenuItem("Tools/State Machine Generator")]
        public static void ShowWindow()
        {
            GetWindow<StateGenerator>("State Machine Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("State Machine Generator", EditorStyles.boldLabel);

            stateMachineScript = (MonoScript)EditorGUILayout.ObjectField("State Machine Script", stateMachineScript, typeof(MonoScript), false);
            outputPath = EditorGUILayout.TextField("Output Path", outputPath);

            if (stateMachineScript != null && GUILayout.Button("Load StateMachine Info"))
            {
                LoadStateMachineInfo();
            }

            if (stateMachineType != null)
            {
                GUILayout.Label($"Detected StateMachine: {stateMachineType.Name}");
                GUILayout.Label($"Enum: {enumName}");

                useCustomPrefix = EditorGUILayout.Toggle("Use Custom Prefix", useCustomPrefix);
                if (useCustomPrefix)
                {
                    customPrefix = EditorGUILayout.TextField("Custom Prefix", customPrefix);
                }

                if (stateNames != null && stateNames.Length > 0)
                {
                    GUILayout.Label("Select States to Generate:");
                    foreach (var state in stateNames)
                    {
                        stateSelection[state] = EditorGUILayout.Toggle(state, stateSelection[state]);
                    }
                }

                if (GUILayout.Button("Generate Selected States"))
                {
                    GenerateStateScripts();
                }
            }
        }

        private void LoadStateMachineInfo()
        {
            stateMachineType = stateMachineScript.GetClass();
            if (stateMachineType == null)
            {
                Debug.LogError("Invalid script selected. Make sure it is a StateMachine class.");
                return;
            }

            Type[] nestedTypes = stateMachineType.GetNestedTypes();
            foreach (Type nested in nestedTypes)
            {
                if (nested.IsEnum)
                {
                    enumName = nested.Name;
                    stateNames = Enum.GetNames(nested);
                    stateSelection = stateNames.ToDictionary(state => state, state => false);
                    return;
                }
            }

            Debug.LogError("No Enum found inside the selected StateMachine!");
        }

        private void GenerateStateScripts()
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            int createdCount = 0;
            foreach (var state in stateSelection.Where(s => s.Value).Select(s => s.Key))
            {
                string prefix = useCustomPrefix ? customPrefix : enumName.Replace("State", "");
                string className = $"{prefix}{state}State";
                string filePath = Path.Combine(outputPath, $"{className}.cs");

                if (File.Exists(filePath))
                {
                    Debug.Log($"[SKIPPED] {className} already exists.");
                    continue;
                }

                string scriptContent = $@"using DH.Core.StateMachine;
                using UnityEngine;

public class {className} : BaseState<{stateMachineType.Name}.{enumName}, {stateMachineType.Name}>
{{
    public override {stateMachineType.Name}.{enumName} StateKey => {stateMachineType.Name}.{enumName}.{state};

    public {className}({stateMachineType.Name} stateManager) : base(stateManager)
    {{
    }}
}}";

                File.WriteAllText(filePath, scriptContent);
                createdCount++;
            }

            AssetDatabase.Refresh();
            Debug.Log($"Generated {createdCount} state scripts for {stateMachineType.Name} in {outputPath}");
        }
    }
}
#endif
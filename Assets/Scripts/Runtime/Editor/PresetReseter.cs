using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Runtime.Editor
{
    public class PresetReseter : EditorWindow
    {
        private const string MenuPath = "Tools/资源检查/应用刷新Preset Manager";
    private const string AndroidBuildTargetName = "Android";
    private const string IosBuildTargetName = "iPhone";
    private static readonly Regex PlatformSettingPathRegex =
        new Regex(@"^m_PlatformSettings\.Array\.data\[(\d+)\]\.(m_BuildTarget|m_TextureFormat)$", RegexOptions.Compiled);

    private readonly List<RuleEntry> rules = new List<RuleEntry>();
    private readonly Dictionary<string, bool> typeFoldoutStates = new Dictionary<string, bool>();
    private Vector2 scrollPos;

    private class RuleEntry
    {
        public PresetType PresetType;
        public int RuleIndex;
        public DefaultPreset Rule;

        public bool IsExecutable =>
            Rule.enabled && Rule.preset != null && !string.IsNullOrWhiteSpace(Rule.filter);

        public string SkipReason
        {
            get
            {
                if (!Rule.enabled)
                {
                    return "Disabled";
                }

                if (Rule.preset == null)
                {
                    return "Preset is None";
                }

                if (string.IsNullOrWhiteSpace(Rule.filter))
                {
                    return "Filter is empty";
                }

                return string.Empty;
            }
        }
    }

    private struct TextureSubsetValues
    {
        public int? AndroidTextureFormat;
        public int? SpriteMeshType;
        public uint? SpriteExtrude;
        public bool? IsReadable;

        public bool HasValues =>
            AndroidTextureFormat.HasValue || SpriteMeshType.HasValue || SpriteExtrude.HasValue || IsReadable.HasValue;
    }

    [MenuItem(MenuPath)]
    private static void OpenWindow()
    {
        var window = GetWindow<PresetReseter>("Preset Manager Refresh");
        window.minSize = new Vector2(760f, 480f);
        window.RefreshRules();
    }

    private void OnEnable()
    {
        RefreshRules();
    }

    private void OnGUI()
    {
        DrawToolbar();
        DrawRuleList();
        DrawBottomActions();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("重新加载", EditorStyles.toolbarButton, GUILayout.Width(120f)))
        {
            RefreshRules();
        }

        int validCount = rules.Count(r => r.IsExecutable);
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Rules: {rules.Count}, Executable: {validCount}", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("只能在Preset Manager中修改规则。", MessageType.Info);
    }

    private void DrawRuleList()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (rules.Count == 0)
        {
            EditorGUILayout.HelpBox("No Preset Manager rules found.", MessageType.Info);
            EditorGUILayout.EndScrollView();
            return;
        }

        var groupedRules = rules.GroupBy(r => r.PresetType.GetManagedTypeName());
        foreach (var group in groupedRules)
        {
            var typeName = group.Key;
            var groupRules = group.ToList();
            int executableCount = groupRules.Count(r => r.IsExecutable);

            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.BeginHorizontal();
            bool isExpanded = GetFoldoutState(typeName);
            bool newExpanded = EditorGUILayout.Foldout(isExpanded, typeName, true);
            SetFoldoutState(typeName, newExpanded);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{groupRules.Count} rules / {executableCount} executable", EditorStyles.miniBoldLabel);

            using (new EditorGUI.DisabledScope(executableCount == 0))
            {
                if (GUILayout.Button("刷新该类型", GUILayout.Width(100f)))
                {
                    RefreshRulesByType(typeName);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (newExpanded)
            {
                EditorGUI.indentLevel++;
                foreach (var entry in groupRules)
                {
                    DrawSingleRuleEntry(entry);
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSingleRuleEntry(RuleEntry entry)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Rule [{entry.RuleIndex}]", EditorStyles.boldLabel);
        using (new EditorGUI.DisabledScope(!entry.IsExecutable))
        {
            if (GUILayout.Button("刷新该规则", GUILayout.Width(120f)))
            {
                RefreshSingleRule(entry);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Enabled", entry.Rule.enabled ? "<color=green>Yes</color>" : "<color=red>No</color>", new GUIStyle(EditorStyles.label) { richText = true });
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filter", GUILayout.Width(EditorGUIUtility.labelWidth - 4f));
        EditorGUILayout.SelectableLabel(
            string.IsNullOrWhiteSpace(entry.Rule.filter) ? "<EMPTY>" : entry.Rule.filter,
            EditorStyles.textField,
            GUILayout.Height(EditorGUIUtility.singleLineHeight));
        EditorGUILayout.EndHorizontal();

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Preset", entry.Rule.preset, typeof(Preset), false);
        }

        if (!entry.IsExecutable)
        {
            EditorGUILayout.HelpBox($"Skipped in refresh: {entry.SkipReason}.", MessageType.Warning);
        }

        EditorGUILayout.EndVertical();
    }

    private bool GetFoldoutState(string typeName)
    {
        if (!typeFoldoutStates.TryGetValue(typeName, out var state))
        {
            state = true;
            typeFoldoutStates[typeName] = state;
        }

        return state;
    }

    private void SetFoldoutState(string typeName, bool state)
    {
        typeFoldoutStates[typeName] = state;
    }

    private void RefreshRulesByType(string typeName)
    {
        var executableRules = rules
            .Where(r => r.PresetType.GetManagedTypeName() == typeName)
            .Where(r => r.IsExecutable)
            .ToList();
        var plan = BuildPlanFromRules(executableRules);
        ApplyPlan(plan, $"Type {typeName}");
    }

    private void DrawBottomActions()
    {
        EditorGUILayout.Space(8f);
        int validCount = rules.Count(r => r.IsExecutable);
        using (new EditorGUI.DisabledScope(validCount == 0))
        {
            if (GUILayout.Button("刷新全部可执行规则（按 Preset Manager 顺序）", GUILayout.Height(30f)))
            {
                RefreshAllExecutableRules();
            }
        }
    }

    private void RefreshRules()
    {
        rules.Clear();
        foreach (var presetType in Preset.GetAllDefaultTypes())
        {
            var rows = Preset.GetDefaultPresetsForType(presetType);
            for (int i = 0; i < rows.Length; i++)
            {
                rules.Add(new RuleEntry
                {
                    PresetType = presetType,
                    RuleIndex = i,
                    Rule = rows[i]
                });
            }
        }

        foreach (var typeName in rules.Select(r => r.PresetType.GetManagedTypeName()).Distinct())
        {
            if (!typeFoldoutStates.ContainsKey(typeName))
            {
                typeFoldoutStates[typeName] = true;
            }
        }
    }

    private static void RefreshSingleRule(RuleEntry entry)
    {
        if (!entry.IsExecutable)
        {
            Debug.LogWarning($"[PresetReset] Rule skipped: {entry.SkipReason}.");
            return;
        }

        var plan = BuildPlanFromRules(new[] { entry });
        ApplyPlan(plan, $"Rule [{entry.RuleIndex}] {entry.PresetType.GetManagedTypeName()}");
    }

    private void RefreshAllExecutableRules()
    {
        var executableRules = rules.Where(r => r.IsExecutable).ToList();
        var plan = BuildPlanFromRules(executableRules);
        ApplyPlan(plan, "All executable rules");
    }

    private static Dictionary<string, List<RuleEntry>> BuildPlanFromRules(IEnumerable<RuleEntry> selectedRules)
    {
        var plan = new Dictionary<string, List<RuleEntry>>();
        foreach (var entry in selectedRules)
        {
            var guids = AssetDatabase.FindAssets(entry.Rule.filter);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/") || AssetDatabase.IsValidFolder(path))
                {
                    continue;
                }

                var importer = AssetImporter.GetAtPath(path);
                if (importer == null || !entry.Rule.preset.CanBeAppliedTo(importer))
                {
                    continue;
                }

                if (!plan.TryGetValue(path, out var presetList))
                {
                    presetList = new List<RuleEntry>();
                    plan[path] = presetList;
                }

                presetList.Add(entry);
            }
        }

        return plan;
    }

    private static void ApplyPlan(Dictionary<string, List<RuleEntry>> presetPlanByPath, string tag)
    {
        if (presetPlanByPath.Count == 0)
        {
            Debug.LogWarning($"[PresetReset] {tag}: no matched assets.");
            return;
        }

        int refreshedCount = 0;
        var entries = presetPlanByPath.ToList();

        try
        {
            for (int i = 0; i < entries.Count; i++)
            {
                var path = entries[i].Key;
                var importer = AssetImporter.GetAtPath(path);
                if (importer == null)
                {
                    continue;
                }

                bool anyApplied = false;
                foreach (var ruleEntry in entries[i].Value)
                {
                    if (ruleEntry.Rule.preset == null)
                    {
                        continue;
                    }

                    anyApplied |= ApplyRuleToImporter(importer, ruleEntry);
                }

                if (anyApplied)
                {
                    importer.SaveAndReimport();
                    refreshedCount++;
                }

                if (i % 25 == 0)
                {
                    EditorUtility.DisplayProgressBar("Reset assets by Preset Manager", path, (float)i / entries.Count);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        Debug.Log($"[PresetReset] {tag}: planned {entries.Count}, refreshed {refreshedCount}.");
    }

    private static bool ApplyRuleToImporter(AssetImporter importer, RuleEntry ruleEntry)
    {
        // TextureImporter rules are applied in a targeted way to avoid overriding custom importer tweaks.
        if (importer is TextureImporter textureImporter && ruleEntry.Rule.preset.CanBeAppliedTo(textureImporter))
        {
            return ApplyTextureSubsetValues(textureImporter, ruleEntry.Rule.preset);
        }

        return ruleEntry.Rule.preset.ApplyTo(importer);
    }

    private static bool ApplyTextureSubsetValues(TextureImporter importer, Preset preset)
    {
        if (!TryExtractTextureSubsetValues(preset, out var subset))
        {
            return false;
        }

        bool changed = false;

        if (subset.AndroidTextureFormat.HasValue)
        {
            var targetFormat = (TextureImporterFormat)subset.AndroidTextureFormat.Value;
            changed |= ApplyPlatformTextureFormat(importer, AndroidBuildTargetName, targetFormat);
            changed |= ApplyPlatformTextureFormat(importer, IosBuildTargetName, targetFormat);
        }

        if (subset.SpriteMeshType.HasValue || subset.SpriteExtrude.HasValue)
        {
            var textureSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(textureSettings);

            bool textureSettingsChanged = false;
            if (subset.SpriteMeshType.HasValue)
            {
                var targetMeshType = (SpriteMeshType)subset.SpriteMeshType.Value;
                if (textureSettings.spriteMeshType != targetMeshType)
                {
                    textureSettings.spriteMeshType = targetMeshType;
                    textureSettingsChanged = true;
                }
            }

            if (subset.SpriteExtrude.HasValue && textureSettings.spriteExtrude != subset.SpriteExtrude.Value)
            {
                textureSettings.spriteExtrude = subset.SpriteExtrude.Value;
                textureSettingsChanged = true;
            }

            if (textureSettingsChanged)
            {
                importer.SetTextureSettings(textureSettings);
                changed = true;
            }
        }

        bool shouldSyncReadWrite = preset != null &&
                                   preset.name.IndexOf("_rw", StringComparison.OrdinalIgnoreCase) >= 0 &&
                                   subset.IsReadable.HasValue;
        if (shouldSyncReadWrite && importer.isReadable != subset.IsReadable.Value)
        {
            importer.isReadable = subset.IsReadable.Value;
            changed = true;
        }

        return changed;
    }

    private static bool ApplyPlatformTextureFormat(TextureImporter importer, string buildTargetName, TextureImporterFormat targetFormat)
    {
        var settings = importer.GetPlatformTextureSettings(buildTargetName);
        if (settings.overridden && settings.format == targetFormat)
        {
            return false;
        }

        settings.overridden = true;
        settings.format = targetFormat;
        importer.SetPlatformTextureSettings(settings);
        return true;
    }

    private static bool TryExtractTextureSubsetValues(Preset preset, out TextureSubsetValues subset)
    {
        subset = default;

        var serializedPreset = new SerializedObject(preset);
        var presetProperties = serializedPreset.FindProperty("m_Properties");
        if (presetProperties == null || !presetProperties.isArray)
        {
            return false;
        }

        var buildTargetByIndex = new Dictionary<int, string>();
        var textureFormatByIndex = new Dictionary<int, int>();

        for (int i = 0; i < presetProperties.arraySize; i++)
        {
            var item = presetProperties.GetArrayElementAtIndex(i);
            var propertyPath = item.FindPropertyRelative("propertyPath")?.stringValue;
            var propertyValue = item.FindPropertyRelative("value")?.stringValue;
            if (string.IsNullOrEmpty(propertyPath) || propertyValue == null)
            {
                continue;
            }

            if (propertyPath == "m_SpriteMeshType" && int.TryParse(propertyValue, out int meshType))
            {
                subset.SpriteMeshType = meshType;
                continue;
            }

            if (propertyPath == "m_SpriteExtrude" && uint.TryParse(propertyValue, out uint spriteExtrude))
            {
                subset.SpriteExtrude = spriteExtrude;
                continue;
            }

            if (propertyPath == "m_IsReadable")
            {
                if (int.TryParse(propertyValue, out int isReadableInt))
                {
                    subset.IsReadable = isReadableInt != 0;
                }
                else if (bool.TryParse(propertyValue, out bool isReadableBool))
                {
                    subset.IsReadable = isReadableBool;
                }
                continue;
            }

            var match = PlatformSettingPathRegex.Match(propertyPath);
            if (!match.Success || !int.TryParse(match.Groups[1].Value, out int platformIndex))
            {
                continue;
            }

            var fieldName = match.Groups[2].Value;
            if (fieldName == "m_BuildTarget")
            {
                buildTargetByIndex[platformIndex] = propertyValue;
                continue;
            }

            if (fieldName == "m_TextureFormat" && int.TryParse(propertyValue, out int textureFormat))
            {
                textureFormatByIndex[platformIndex] = textureFormat;
            }
        }

        foreach (var buildTargetEntry in buildTargetByIndex)
        {
            if (!string.Equals(buildTargetEntry.Value, AndroidBuildTargetName, StringComparison.Ordinal))
            {
                continue;
            }

            if (textureFormatByIndex.TryGetValue(buildTargetEntry.Key, out int androidFormat))
            {
                subset.AndroidTextureFormat = androidFormat;
            }
            break;
        }

        return subset.HasValues;
    }
    }
}
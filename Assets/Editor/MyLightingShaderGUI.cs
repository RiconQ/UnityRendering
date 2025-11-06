using UnityEngine;
using UnityEditor;
using Codice.Client.BaseCommands;
using Unity.VisualScripting;

public class MyLightingShaderGUI : ShaderGUI
{
    private Material m_target;
    private MaterialEditor m_editor;
    private MaterialProperty[] m_properties;

    private static GUIContent m_staticLabel = new GUIContent();


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        m_target = materialEditor.target as Material;
        m_editor = materialEditor;
        m_properties = properties;
        DoMain();
        DoSecondary();
    }

    private void DoMain()
    {
        GUILayout.Label("Main Maps", EditorStyles.boldLabel);

        MaterialProperty mainTex = FindProperty("_MainTex");
        m_editor.TexturePropertySingleLine(MakeLabel(mainTex, "Albedo (RGB)"), mainTex, FindProperty("_Tint"));
        DoMetallic();
        DoSmoothness();
        DoNormals();
        DoOcclusion();
        DoEmission();
        DoDetailMask();
        m_editor.TextureScaleOffsetProperty(mainTex);
    }

    private void DoNormals()
    {
        MaterialProperty map = FindProperty("_NormalMap");
        Texture tex = map.textureValue;
        EditorGUI.BeginChangeCheck();
        m_editor.TexturePropertySingleLine(MakeLabel(map), map, tex ? FindProperty("_BumpScale") : null);
        if (EditorGUI.EndChangeCheck() && tex != map.textureValue)
        {
            SetKeyword("_NORMAL_MAP", map.textureValue);
        }
    }

    private void DoMetallic()
    {
        MaterialProperty map = FindProperty("_MetallicMap");
        EditorGUI.BeginChangeCheck();
        m_editor.TexturePropertySingleLine(
                MakeLabel(map, "Metallic (R)"), map,
                map.textureValue ? null : FindProperty("_Metallic")
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_METALLIC_MAP", map.textureValue);
        }
    }

    private void DoSmoothness()
    {
        SmoothnessSource source = SmoothnessSource.Uniform;
        if (IsKeywordEnabled("_SMOOTHNESS_ALBEDO"))
        {
            source = SmoothnessSource.Albedo;
        }
        else if (IsKeywordEnabled("_SMOOTHNESS_METALLIC"))
        {
            source = SmoothnessSource.Metallic;
        }
        MaterialProperty slider = FindProperty("_Smoothness");
        EditorGUI.indentLevel += 2;
        m_editor.ShaderProperty(slider, MakeLabel(slider));
        EditorGUI.indentLevel += 1;

        EditorGUI.BeginChangeCheck();
        source = (SmoothnessSource)EditorGUILayout.EnumPopup(MakeLabel("Source"), source);
        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Smoothness Source");
            SetKeyword("_SMOOTHNESS_ALBEDO", source == SmoothnessSource.Albedo);
            SetKeyword("_SMOOTHNESS_METALLIC", source == SmoothnessSource.Metallic);
        }

        EditorGUI.indentLevel -= 3;
    }

    private void DoEmission()
    {
        MaterialProperty map = FindProperty("_EmissionMap");
        EditorGUI.BeginChangeCheck();
        m_editor.TexturePropertySingleLine(
                MakeLabel(map, "Emission (RGB)"), map, FindProperty("_Emission")
            );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_EMISSION_MAP", map.textureValue);
        }
    }

    private void DoOcclusion()
    {
        MaterialProperty map = FindProperty("_OcclusionMap");
        EditorGUI.BeginChangeCheck();
        m_editor.TexturePropertySingleLine(
                MakeLabel(map, "Occlusion (G)"), map,
                map.textureValue ? FindProperty("_OcclusionStrength") : null
            );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_OCCLUSION_MAP", map.textureValue);
        }
    }

    private void DoDetailMask()
    {
        MaterialProperty mask = FindProperty("_DetailMask");
        EditorGUI.BeginChangeCheck();
        m_editor.TexturePropertySingleLine(
            MakeLabel(mask, "Detail Mask (A)"), mask
            );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_DETAIL_MASK", mask.textureValue);
        }
    }

    private void DoSecondary()
    {
        GUILayout.Label("Secondary Maps", EditorStyles.boldLabel);

        MaterialProperty detailTex = FindProperty("_DetailTex");
        EditorGUI.BeginChangeCheck();
        m_editor.TexturePropertySingleLine(
            MakeLabel(detailTex, "Albedo (RGB) multiplied by 2"), detailTex
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_DETAIL_ALBEDO_MAP", detailTex.textureValue);
        }
        DoSecondaryNormals();

        m_editor.TextureScaleOffsetProperty(detailTex);
    }

    private void DoSecondaryNormals()
    {
        MaterialProperty map = FindProperty("_DetailNormalMap");
        EditorGUI.BeginChangeCheck();
        m_editor.TexturePropertySingleLine(
            MakeLabel(map), map,
            map.textureValue ? FindProperty("_DetailBumpScale") : null
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_DETAIL_NORMAL_MAP", map.textureValue);
        }
    }

    private static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        m_staticLabel.text = property.displayName;
        m_staticLabel.tooltip = tooltip;
        return m_staticLabel;
    }

    static GUIContent MakeLabel(string text, string tooltip = null)
    {
        m_staticLabel.text = text;
        m_staticLabel.tooltip = tooltip;
        return m_staticLabel;
    }

    private MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, m_properties);
    }

    private void SetKeyword(string keyword, bool state)
    {
        if (state)
        {
            foreach (Material m in m_editor.targets)
            {
                m.EnableKeyword(keyword);
            }
        }
        else
        {
            foreach (Material m in m_editor.targets)
            {
                m.DisableKeyword(keyword);
            }
        }
    }

    private bool IsKeywordEnabled(string keyword)
    {
        return m_target.IsKeywordEnabled(keyword);
    }

    private void RecordAction(string label)
    {
        m_editor.RegisterPropertyChangeUndo(label);
    }

    private enum SmoothnessSource
    {
        Uniform,
        Albedo,
        Metallic
    }
}

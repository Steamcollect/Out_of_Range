using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RangeOverload_CombatStyle)), CanEditMultipleObjects]
public class RangeOverload_CombatStyleEditor : Editor
{
    SerializedProperty p_CurentTemperature;
    SerializedProperty p_CurrentState;

    SerializedProperty p_AttackCooldown;
    SerializedProperty p_ShootTemperature;

    SerializedProperty p_DefaultCoolsPerSec;
    SerializedProperty p_OverloadCoolsPerSec;
    SerializedProperty p_NerfCoolsPerSec;
    SerializedProperty p_BuffCoolsPerSec;

    SerializedProperty p_RangeToReset;
    SerializedProperty p_RangeToBuff;
    SerializedProperty p_RangeToNerf;

    SerializedProperty p_MeshRenderer;
    SerializedProperty p_ColorOverTemperature;

    SerializedProperty p_AttackPoint;
    SerializedProperty p_MuzzleFlashPrefab;
    SerializedProperty p_BulletPrefab;

    SerializedProperty p_OnAttackFeedback;
    SerializedProperty p_OnReloadFeedback;

    SerializedProperty p_HandleCoolsInput;
    SerializedProperty p_HandleCoolsSkillInput;

    bool foldVisual = true;
    bool foldReferences = true;
    bool foldInput = true;

    void OnEnable()
    {
        p_CurentTemperature = serializedObject.FindProperty("m_CurentTemperature");
        p_CurrentState = serializedObject.FindProperty("m_CurrentState");

        p_AttackCooldown = serializedObject.FindProperty("m_AttackCooldown");
        p_ShootTemperature = serializedObject.FindProperty("m_ShootTemperature");

        p_DefaultCoolsPerSec = serializedObject.FindProperty("m_DefaultCoolsPerSec");
        p_OverloadCoolsPerSec = serializedObject.FindProperty("m_OverloadCoolsPerSec");
        p_NerfCoolsPerSec = serializedObject.FindProperty("m_NerfCoolsPerSec");
        p_BuffCoolsPerSec = serializedObject.FindProperty("m_BuffCoolsPerSec");

        p_RangeToReset = serializedObject.FindProperty("m_RangeToReset");
        p_RangeToBuff = serializedObject.FindProperty("m_RangeToBuff");
        p_RangeToNerf = serializedObject.FindProperty("m_RangeToNerf");

        p_MeshRenderer = serializedObject.FindProperty("m_MeshRenderer");
        p_ColorOverTemperature = serializedObject.FindProperty("m_ColorOverTemperature");

        p_AttackPoint = serializedObject.FindProperty("m_AttackPoint");
        p_MuzzleFlashPrefab = serializedObject.FindProperty("m_MuzzleFlashPrefab");
        p_BulletPrefab = serializedObject.FindProperty("m_BulletPrefab");

        p_OnAttackFeedback = serializedObject.FindProperty("m_OnAttackFeedback");
        p_OnReloadFeedback = serializedObject.FindProperty("m_OnReloadFeedback");

        p_HandleCoolsInput = serializedObject.FindProperty("m_HandleCoolsInput");
        p_HandleCoolsSkillInput = serializedObject.FindProperty("m_HandleCoolsSkillInput");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(p_CurentTemperature);
        EditorGUILayout.PropertyField(p_CurrentState);

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(p_AttackCooldown);

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(p_ShootTemperature);

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(p_DefaultCoolsPerSec);
        EditorGUILayout.PropertyField(p_OverloadCoolsPerSec);
        EditorGUILayout.PropertyField(p_NerfCoolsPerSec);
        EditorGUILayout.PropertyField(p_BuffCoolsPerSec);

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(p_RangeToReset);
        EditorGUILayout.PropertyField(p_RangeToBuff);
        EditorGUILayout.PropertyField(p_RangeToNerf);

        GUILayout.Space(10);

        Rect sliderRect = EditorGUILayout.GetControlRect();
        sliderRect.height = 4;
        float unit = sliderRect.width / 100f;
        
        EditorGUI.DrawRect(sliderRect, Color.gray);

        if(p_CurrentState.enumValueIndex == 4)
        {
            EditorGUI.DrawRect(new Rect(sliderRect.x, sliderRect.y, p_CurentTemperature.floatValue * unit, sliderRect.height), Color.yellow);
        }
        else if(p_CurrentState.enumValueIndex == 2 || p_CurrentState.enumValueIndex == 5)
        {
            EditorGUI.DrawRect(new Rect(sliderRect.x, sliderRect.y, p_CurentTemperature.floatValue * unit, sliderRect.height), Color.red);
        }
        else
        {
            // Nerf range
            Vector2 nerf = p_RangeToNerf.vector2Value;
            float nerfStart = sliderRect.x + nerf.x * unit;
            float nerfWidth = Mathf.Max(0f, (nerf.y - nerf.x) * unit);
            EditorGUI.DrawRect(new Rect(nerfStart, sliderRect.y, nerfWidth, sliderRect.height), Color.red);

            // Reset range
            Vector2 reset = p_RangeToReset.vector2Value;
            float resetStart = sliderRect.x + reset.x * unit;
            float resetWidth = Mathf.Max(0f, (reset.y - reset.x) * unit);
            EditorGUI.DrawRect(new Rect(resetStart, sliderRect.y, resetWidth, sliderRect.height), Color.blue);

            // Buff range
            Vector2 buff = p_RangeToBuff.vector2Value;
            float buffStart = sliderRect.x + buff.x * unit;
            float buffWidth = Mathf.Max(0f, (buff.y - buff.x) * unit);
            EditorGUI.DrawRect(new Rect(buffStart, sliderRect.y, buffWidth, sliderRect.height), Color.yellow);
        }

        EditorGUI.DrawRect(new Rect(p_CurentTemperature.floatValue * unit + sliderRect.x - 2, sliderRect.y - 2, 4, sliderRect.height + 4), Color.white);

        GUILayout.Space(10);

        EditorGUILayout.Space(10);
        foldVisual = EditorGUILayout.Foldout(foldVisual, "Visual");
        if (foldVisual)
        {
            EditorGUILayout.PropertyField(p_MeshRenderer);
            EditorGUILayout.PropertyField(p_ColorOverTemperature);
        }

        EditorGUILayout.Space(10);
        foldReferences = EditorGUILayout.Foldout(foldReferences, "References");
        if (foldReferences)
        {
            EditorGUILayout.PropertyField(p_AttackPoint);
            EditorGUILayout.PropertyField(p_MuzzleFlashPrefab);
            EditorGUILayout.PropertyField(p_BulletPrefab);

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(p_OnAttackFeedback);
            EditorGUILayout.PropertyField(p_OnReloadFeedback);
        }
        
        EditorGUILayout.Space(10);
        foldInput = EditorGUILayout.Foldout(foldInput, "Input");
        if (foldInput)
        {
            EditorGUILayout.PropertyField(p_HandleCoolsInput);
            EditorGUILayout.PropertyField(p_HandleCoolsSkillInput);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

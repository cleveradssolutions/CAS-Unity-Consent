//
//  Clever Ads Solutions Unity Consent Plugin
//
//  Copyright © 2021 CleverAdsSolutions. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System;
using Utils = CAS.UEditor.CASEditorUtils;
using CAS.UEditor;

namespace CAS.UserConsent
{
    [CustomEditor( typeof( ConsentRequestParameters ) )]
    public class ConsentRequestParametersEditor : Editor
    {
        private const string packageName = "com.cleversolutions.ads.consent.unity";
        private const string rootCASFolderPath = "Assets/CleverAdsSolutionsConsent";
        private const string gitRepoName = "CAS-Unity-Consent";
        private const string templateUIPrefabName = "ConsentUITemplate.prefab";
        private const string templateSettingsPrefabName = "NetworkPolicy.prefab";
        private const string customUIPrefabName = "UserConsentUI.prefab";
        private const string customSettingsPrefabName = "SettingToggleUI.prefab";

        private SerializedProperty showInEditorProp;
        private SerializedProperty withAudienceDefinitionProp;
        private SerializedProperty withDeclineOptionProp;
        private SerializedProperty withMediationSettingsProp;
        private SerializedProperty withRequestTrackingTransparencyProp;
        private SerializedProperty uiPrefabProp;
        private SerializedProperty settingsTogglePrefabProp;
        private SerializedProperty privacyPolicyUrlProp;
        private SerializedProperty termsOfUseUrlProp;

        private ReorderableList privacyPolicyList;
        private ReorderableList termsOfUseList;

        private SerializedProperty currentListProp;

        private string newCASVersion = null;
        private bool allowedPackageUpdate = false;

        private void OnEnable()
        {
            var props = serializedObject;
            showInEditorProp = props.FindProperty( "showInEditor" );
            withAudienceDefinitionProp = props.FindProperty( "withAudienceDefinition" );
            withDeclineOptionProp = props.FindProperty( "withDeclineOption" );
            withMediationSettingsProp = props.FindProperty( "withMediationSettings" );
            withRequestTrackingTransparencyProp = props.FindProperty( "withRequestTrackingTransparency" );
            uiPrefabProp = props.FindProperty( "uiPrefab" );
            settingsTogglePrefabProp = props.FindProperty( "settingsTogglePrefab" );
            privacyPolicyUrlProp = props.FindProperty( "privacyPolicyUrl" );
            termsOfUseUrlProp = props.FindProperty( "termsOfUseUrl" );

            privacyPolicyList = new ReorderableList( props, privacyPolicyUrlProp, true, true, true, true )
            {
                drawHeaderCallback = DrawPrivacyPolicyHeader,
                drawElementCallback = DrawURLElement,
                elementHeight = EditorGUIUtility.singleLineHeight * 2.0f + 8.0f
            };

            termsOfUseList = new ReorderableList( props, termsOfUseUrlProp, true, true, true, true )
            {
                drawHeaderCallback = DrawTermsOfUseHeader,
                drawElementCallback = DrawURLElement,
                elementHeight = privacyPolicyList.elementHeight
            };

            allowedPackageUpdate = Utils.IsPackageExist( packageName );

            EditorApplication.delayCall +=
                () => newCASVersion = Utils.GetNewVersionOrNull( gitRepoName, UserConsent.version, false );
        }

        protected override void OnHeaderGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label( "CleverAdsSolutions", HelpStyles.largeTitleStyle );
            GUILayout.Label( "Consent", HelpStyles.largeTitleStyle, GUILayout.ExpandWidth( false ) );
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            Utils.OnHeaderGUI( gitRepoName, allowedPackageUpdate, UserConsent.version, ref newCASVersion );
        }

        public override void OnInspectorGUI()
        {
            var obj = serializedObject;
            obj.UpdateIfRequiredOrScript();

            showInEditorProp.boolValue = GUILayout.Toggle(
                showInEditorProp.boolValue,
                " Test in Unity Editor" );
            withAudienceDefinitionProp.boolValue = GUILayout.Toggle(
                withAudienceDefinitionProp.boolValue,
                " With Age of audience requeset" );
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox( "The user is prompted for the year of birth and the audience is determined automatically.", MessageType.None );
            EditorGUI.indentLevel--;
            withRequestTrackingTransparencyProp.boolValue = GUILayout.Toggle(
                withRequestTrackingTransparencyProp.boolValue,
                " With iOS App Tracking Transparency requeset" );
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox( "The iOS 14.5+ users is prompted for permission to track the Advertising ID.", MessageType.None );
            EditorGUI.indentLevel--;
            withDeclineOptionProp.boolValue = GUILayout.Toggle(
                withDeclineOptionProp.boolValue,
                " With option of Decline consent" );
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox( "The user is given the choice to opt out dialog.", MessageType.None );
            EditorGUI.indentLevel--;

            bool enableTogglePrefab = withMediationSettingsProp.boolValue;
            if (enableTogglePrefab != EditorGUILayout.ToggleLeft( "With Mediaiton Networks settings", enableTogglePrefab ))
            {
                enableTogglePrefab = !enableTogglePrefab;
                withMediationSettingsProp.boolValue = enableTogglePrefab;
            }
            EditorGUI.BeginDisabledGroup( !enableTogglePrefab );
            DrawPrefabSelector( "Network UI Prefab",
                settingsTogglePrefabProp, templateSettingsPrefabName, customSettingsPrefabName,
                typeof( MediationPolicyUI ) );
            EditorGUI.EndDisabledGroup();
            if (!enableTogglePrefab)
                settingsTogglePrefabProp.objectReferenceValue = null;
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox( "The user is provided with advanced consent settings for each active network in the game.", MessageType.None );
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            DrawPrefabSelector( "Consent UI Prefab",
                uiPrefabProp, templateUIPrefabName, customUIPrefabName,
                typeof( UserConsentUI ) );
            EditorGUILayout.Space();

            currentListProp = privacyPolicyUrlProp;
            privacyPolicyList.DoLayoutList();
            currentListProp = termsOfUseUrlProp;
            termsOfUseList.DoLayoutList();

            EditorGUILayout.HelpBox( "Use 'Edit > Clear All PlayerPrefs' menu to reset consent state in Unity Editor.", MessageType.Info );
            obj.ApplyModifiedProperties();
        }

        private void DrawURLElement( Rect rect, int index, bool isActive, bool isFocused )
        {
            var height = rect.height;
            var item = currentListProp.GetArrayElementAtIndex( index );
            rect.yMin += 1;
            rect.yMax -= 3;
            if (currentListProp.arraySize > 1)
            {
                rect.yMax -= height * 0.5f;

                var id = item.FindPropertyRelative( "id" );
                var label = ( index == 0 ? "Preferred " : "Platform " );
                if (currentListProp.arraySize > 5)
                    label += id.intValue;
                id.intValue
                    = Convert.ToInt32( EditorGUI.EnumPopup( rect, label, ( RuntimePlatform )id.intValue ) );
                rect.yMin = rect.yMax + 1;
                rect.yMax += height * 0.5f - 1;
            }

            var text = item.FindPropertyRelative( "text" );
            text.stringValue = EditorGUI.TextArea( rect, text.stringValue, HelpStyles.wordWrapTextAred );
        }

        private void DrawPrivacyPolicyHeader( Rect rect )
        {
            EditorGUI.LabelField( rect, "Privacy policy URL" );
        }

        private void DrawTermsOfUseHeader( Rect rect )
        {
            EditorGUI.LabelField( rect, "Terms of Use URL" );
        }

        private void DrawPrefabSelector( string title, SerializedProperty prop, string templateName, string customName, Type objType )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( title );
            if (GUILayout.Button( "Create template", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
            {
                var prefab = CreateCustomUIPrefab( customName, templateName );
                if (prefab)
                    prop.objectReferenceValue = prefab;
            }
            if (GUILayout.Button( "Default", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
            {
                prop.objectReferenceValue = LoadUITemplatePrefab( templateName );
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            prop.objectReferenceValue =
                EditorGUILayout.ObjectField( prop.objectReferenceValue, objType, false );
            EditorGUI.indentLevel--;
        }

        #region Utils
        [MenuItem( "Assets/CleverAdsSolutions/Consent Request Parameters", priority = 1015 )]
        public static void CreateParameters()
        {
            if (!AssetDatabase.IsValidFolder( "Assets/Resources" ))
                AssetDatabase.CreateFolder( "Assets", "Resources" );
            var assetPath = "Assets/Resources/" + ConsentRequestParameters.defaultAssetPath + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<ConsentRequestParameters>( assetPath );
            if (!asset)
            {
                asset = CreateInstance<ConsentRequestParameters>();

                var uiPrefab = LoadUITemplatePrefab( templateUIPrefabName );
                if (uiPrefab)
                    asset.WithUIPrefab( uiPrefab.GetComponent<UserConsentUI>() );

                var togglePrefab = LoadUITemplatePrefab( templateSettingsPrefabName );
                if (togglePrefab)
                    asset.WithMediationSettingsTogglePrefab( togglePrefab.GetComponent<MediationPolicyUI>() );

                AssetDatabase.CreateAsset( asset, assetPath );
            }

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject( asset );
        }

        private static GameObject CreateCustomUIPrefab( string name, string template )
        {
            string resultFileName = "Assets/CleverAdsSolutions/" + name;

            if (File.Exists( resultFileName )
                && EditorUtility.DisplayDialog( "Create " + template, resultFileName + " already exist.", "Cancel", "Replace" ))
                return null;

            var originalPrefab = LoadUITemplatePrefab( template );
            if (!originalPrefab)
                return null;

            if (!AssetDatabase.IsValidFolder( "Assets/CleverAdsSolutions" ))
                AssetDatabase.CreateFolder( "Assets", "CleverAdsSolutions" );

            var objSource = PrefabUtility.InstantiatePrefab( originalPrefab ) as GameObject;
            var asset = PrefabUtility.SaveAsPrefabAsset( objSource, resultFileName );
            DestroyImmediate( objSource );

            return asset;
        }

        private static GameObject LoadUITemplatePrefab( string name )
        {
            string templateFile = "/Prefabs/" + name;

            string path = "Packages/" + packageName + templateFile;
            if (!File.Exists( path ))
            {
                path = rootCASFolderPath + templateFile;
                if (!File.Exists( path ))
                {
                    Debug.LogError( "[CleverAdsSolution Consent] " + name + " not found. Try reimport CAS package." );
                    return null;
                }
            }

            return AssetDatabase.LoadAssetAtPath<GameObject>( path );
        }
        #endregion
    }
}
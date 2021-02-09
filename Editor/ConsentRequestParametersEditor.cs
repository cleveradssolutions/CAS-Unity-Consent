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
        private SerializedProperty consentMessageProp;
        private SerializedProperty settingsMessageProp;

        private ReorderableList privacyPolicyList;
        private ReorderableList termsOfUseList;

        private ReorderableList consentMessageList;
        private ReorderableList settingsMessageList;

        private SerializedProperty currentListProp;

        private string newCASVersion = null;
        private bool allowedPackageUpdate = false;
        private bool trackingDescriptionExist = false;

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
            consentMessageProp = props.FindProperty( "consentMessage" );
            settingsMessageProp = props.FindProperty( "settingsMessage" );

            consentMessageList = new ReorderableList( props, consentMessageProp, true, true, true, true )
            {
                drawHeaderCallback = DrawConsentMessageHeader,
                drawElementCallback = DrawLocalizedMessageElement
            };
            consentMessageList.elementHeight = EditorGUIUtility.singleLineHeight * 3.0f + 8.0f;

            settingsMessageList = new ReorderableList( props, settingsMessageProp, true, true, true, true )
            {
                drawHeaderCallback = DrawSettingsMessageHeader,
                drawElementCallback = DrawLocalizedMessageElement
            };
            settingsMessageList.elementHeight = consentMessageList.elementHeight;

            privacyPolicyList = new ReorderableList( props, privacyPolicyUrlProp, true, true, true, true )
            {
                drawHeaderCallback = DrawPrivacyPolicyHeader,
                drawElementCallback = DrawMediationMessageElement
            };
            privacyPolicyList.elementHeight = EditorGUIUtility.singleLineHeight * 2.0f + 8.0f;

            termsOfUseList = new ReorderableList( props, termsOfUseUrlProp, true, true, true, true )
            {
                drawHeaderCallback = DrawTermsOfUseHeader,
                drawElementCallback = DrawMediationMessageElement
            };
            termsOfUseList.elementHeight = privacyPolicyList.elementHeight;

            allowedPackageUpdate = Utils.IsPackageExist( packageName );

            EditorApplication.delayCall +=
                () => newCASVersion = Utils.GetNewVersionOrNull( gitRepoName, UserConsent.version, false );


            var iosSettings = Utils.GetSettingsAsset( BuildTarget.iOS ); // TODO set false crate
            trackingDescriptionExist = iosSettings && !string.IsNullOrEmpty( iosSettings.trackingUsageDescription );
        }

        public override void OnInspectorGUI()
        {
            var obj = serializedObject;
            obj.UpdateIfRequiredOrScript();

            Utils.LinksToolbarGUI( gitRepoName );

            EditorGUILayout.PropertyField( withAudienceDefinitionProp );

            EditorGUILayout.PropertyField( withRequestTrackingTransparencyProp );
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox( "iOS App Tracking Transparency Request", MessageType.None );
            EditorGUI.indentLevel--;
            if (!trackingDescriptionExist && withRequestTrackingTransparencyProp.boolValue)
            {
                withRequestTrackingTransparencyProp.boolValue = false;
                if (EditorUtility.DisplayDialog( "App Tracking Transparency request",
                    "Please set NSUserTrackingUsageDescription in 'Assets > CleverAdsSolutions > iOS Settings' menu to correct tracking authorization request.",
                    "Open iOS settings", "Disable request" ))
                {
                    Utils.OpenIOSSettingsWindow();
                }
            }

            EditorGUILayout.PropertyField( showInEditorProp );
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel( "Reset messages" );
            if (GUILayout.Button( "to default", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
            {
                consentMessageProp.arraySize = 1;
                var item = consentMessageProp.GetArrayElementAtIndex( 0 );
                item.FindPropertyRelative( "id" ).intValue = ( int )SystemLanguage.English;
                item.FindPropertyRelative( "text" ).stringValue = GetDefaultConsentMessage();

                settingsMessageProp.arraySize = 1;
                item = settingsMessageProp.GetArrayElementAtIndex( 0 );
                item.FindPropertyRelative( "id" ).intValue = ( int )SystemLanguage.English;
                item.FindPropertyRelative( "text" ).stringValue = GetDefaultSettingsMessage();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            Utils.AboutRepoGUI( gitRepoName, allowedPackageUpdate, UserConsent.version, ref newCASVersion );
            EditorGUILayout.Space();

            currentListProp = privacyPolicyUrlProp;
            privacyPolicyList.DoLayoutList();
            currentListProp = termsOfUseUrlProp;
            termsOfUseList.DoLayoutList();
            EditorGUILayout.Space();

            CAS.UEditor.HelpStyles.BeginBoxScope();
            EditorGUILayout.PropertyField( withDeclineOptionProp );
            DrawPrefabSelector( "Consent UI Prefab",
                uiPrefabProp, templateUIPrefabName, customUIPrefabName );

            currentListProp = consentMessageProp;
            consentMessageList.DoLayoutList();
            CAS.UEditor.HelpStyles.EndBoxScope();

            CAS.UEditor.HelpStyles.BeginBoxScope();
            EditorGUILayout.PropertyField( withMediationSettingsProp );
            bool disableTogglePrefab = !withMediationSettingsProp.boolValue;
            EditorGUI.BeginDisabledGroup( disableTogglePrefab );
            DrawPrefabSelector( "Toggle UI Prefab",
                settingsTogglePrefabProp, templateSettingsPrefabName, customSettingsPrefabName );
            currentListProp = settingsMessageProp;
            settingsMessageList.DoLayoutList();
            EditorGUI.EndDisabledGroup();
            if (disableTogglePrefab)
                settingsTogglePrefabProp.objectReferenceValue = null;
            CAS.UEditor.HelpStyles.EndBoxScope();

            obj.ApplyModifiedProperties();
        }

        private void DrawLocalizedMessageElement( Rect rect, int index, bool isActive, bool isFocused )
        {
            var height = rect.height;
            var item = currentListProp.GetArrayElementAtIndex( index );
            rect.yMin += 1;
            rect.yMax -= 1;
            if (currentListProp.arraySize > 1)
            {
                rect.yMax -= height * 0.67f + 2;

                var id = item.FindPropertyRelative( "id" );
                var label = ( index == 0 ? "Preferred " : "Language " );
                if (currentListProp.arraySize > 5)
                    label += id.intValue;
                id.intValue
                    = Convert.ToInt32( EditorGUI.EnumPopup( rect, label, ( SystemLanguage )id.intValue ) );
                rect.yMin = rect.yMax + 1;
                rect.yMax += height * 0.67f - 1;
            }

            var text = item.FindPropertyRelative( "text" );
            text.stringValue = EditorGUI.TextArea( rect, text.stringValue, HelpStyles.wordWrapTextAred );
        }

        private void DrawMediationMessageElement( Rect rect, int index, bool isActive, bool isFocused )
        {
            var height = rect.height;
            var item = currentListProp.GetArrayElementAtIndex( index );
            rect.yMin += 1;
            rect.yMax -= 1;
            if (currentListProp.arraySize > 1)
            {
                rect.yMax -= height * 0.5f + 2;

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

        private void DrawConsentMessageHeader( Rect rect )
        {
            EditorGUI.LabelField( rect, "Consent Message" );
        }

        private void DrawSettingsMessageHeader( Rect rect )
        {
            EditorGUI.LabelField( rect, "Mediation Settings Message" );
        }

        private void DrawPrefabSelector( string title, SerializedProperty prop, string templateName, string customName )
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
            prop.objectReferenceValue =
                EditorGUILayout.ObjectField( prop.objectReferenceValue, typeof( MediationPolicyUI ), false );
            EditorGUILayout.Space();
        }

        #region Utils
        [MenuItem( "Assets/CleverAdsSolutions/Consent Request Parameters" )]
        public static void CreateParameters()
        {
            if (!AssetDatabase.IsValidFolder( "Assets/Resources" ))
                AssetDatabase.CreateFolder( "Assets", "Resources" );
            var assetPath = "Assets/Resources/" + ConsentRequestParameters.defaultAssetPath + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<ConsentRequestParameters>( assetPath );
            if (!asset)
            {
                asset = CreateInstance<ConsentRequestParameters>()
                    .WithConsentMessage( GetDefaultConsentMessage() )
                    .WithSettingsMessage( GetDefaultSettingsMessage() );

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

        private static string GetDefaultConsentMessage()
        {
            return "In-app purchase and advertising messages are the part of our mobile app. " +
                "Also there is an option to watch advertisements for a reward. " +
                "In-app purchasing refers to the buying of goods by using real money from your account. " +
                "You can disable or adjust the ability to make in-app purchases in your device settings. " +
                "In order to improve the quality of our apps we collect and process your personal information. " +
                "By pressing \"Accept\" you agree to these conditions. " +
                "Visit our Privacy Policy to get more information about storage and usage of the received data.";
        }

        private static string GetDefaultSettingsMessage()
        {
            return "With the button \"Accept\" you agree to the use of every individual third party partner stated below. " +
                "In case of any doubts, you can check their privacy policies for details of the technologies and data processing methods used. " +
                "And if needed, you can disable any ad network.";
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
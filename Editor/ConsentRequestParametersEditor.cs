using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System;
using CAS.UEditor;
using Utils = CAS.UEditor.CASEditorUtils;

namespace CAS.UserConsent
{
    [CustomEditor( typeof( ConsentRequestParameters ) )]
    public class ConsentRequestParametersEditor : Editor
    {
        private const string packageName = "com.cleversolutions.ads.consent.unity";
        private const string rootCASFolderPath = "Assets/CleverAdsSolutionsConsent";
        private const string gitRepoName = "CAS-Unity-Consent";

        private SerializedProperty showInEditorProp;
        private SerializedProperty withAudienceDefinitionProp;
        private SerializedProperty withDeclineOptionProp;
        private SerializedProperty withMediationSettingsProp;
        private SerializedProperty uiPrefabProp;
        private SerializedProperty privacyPolicyUrlProp;
        private SerializedProperty termsOfUseUrlProp;
        private SerializedProperty consentMessageProp;
        private SerializedProperty settingsMessageProp;

        private ReorderableList consentMessageList;
        private ReorderableList settingsMessageList;
        private SerializedProperty currentListProp;

        public GUIStyle wordWrapTextArea = null;

        private string newCASVersion = null;
        private bool allowedPackageUpdate = false;

        private void OnEnable()
        {
            var props = serializedObject;
            showInEditorProp = props.FindProperty( "showInEditor" );
            withAudienceDefinitionProp = props.FindProperty( "withAudienceDefinition" );
            withDeclineOptionProp = props.FindProperty( "withDeclineOption" );
            withMediationSettingsProp = props.FindProperty( "withMediationSettings" );
            uiPrefabProp = props.FindProperty( "uiPrefab" );
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

            allowedPackageUpdate = Utils.IsPackageExist( packageName );

            EditorApplication.delayCall +=
                () => newCASVersion = Utils.GetNewVersionOrNull( gitRepoName, UserConsent.version, false );
        }

        public override void OnInspectorGUI()
        {
            if (wordWrapTextArea == null)
            {
                wordWrapTextArea = new GUIStyle( EditorStyles.textArea );
                wordWrapTextArea.wordWrap = true;
            }
            var obj = serializedObject;
            obj.UpdateIfRequiredOrScript();

            Utils.LinksToolbarGUI( gitRepoName );

            EditorGUILayout.LabelField( "Options", EditorStyles.boldLabel );
            EditorGUILayout.PropertyField( withAudienceDefinitionProp );
            EditorGUILayout.PropertyField( withMediationSettingsProp );
            EditorGUILayout.PropertyField( withDeclineOptionProp );
            EditorGUILayout.PropertyField( showInEditorProp );
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "UI Prefab", EditorStyles.boldLabel );
            if (GUILayout.Button( "Create template", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
            {
                uiPrefabProp.objectReferenceValue = CreateCustomUIPrefab();
            }
            if (GUILayout.Button( "Default", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
            {
                uiPrefabProp.objectReferenceValue = LoadUITemplatePrefab();
            }
            EditorGUILayout.EndHorizontal();
            uiPrefabProp.objectReferenceValue =
                EditorGUILayout.ObjectField( uiPrefabProp.objectReferenceValue, typeof( UserConsentUI ), false );
            EditorGUILayout.Space();

            EditorGUILayout.LabelField( "Company URL", EditorStyles.boldLabel );
            EditorGUILayout.PropertyField( privacyPolicyUrlProp );
            EditorGUILayout.PropertyField( termsOfUseUrlProp );
            EditorGUILayout.Space();

            Utils.AboutRepoGUI( gitRepoName, allowedPackageUpdate, UserConsent.version, ref newCASVersion );
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Localized messages", EditorStyles.boldLabel );
            if (GUILayout.Button( "Default", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
            {
                consentMessageProp.arraySize = 1;
                var item = consentMessageProp.GetArrayElementAtIndex( 0 );
                item.FindPropertyRelative( "language" ).enumValueIndex = ( int )SystemLanguage.English;
                item.FindPropertyRelative( "text" ).stringValue = GetDefaultConsentMessage();

                settingsMessageProp.arraySize = 1;
                item = settingsMessageProp.GetArrayElementAtIndex( 0 );
                item.FindPropertyRelative( "language" ).enumValueIndex = ( int )SystemLanguage.English;
                item.FindPropertyRelative( "text" ).stringValue = GetDefaultSettingsMessage();
            }
            EditorGUILayout.EndHorizontal();
            currentListProp = consentMessageProp;
            consentMessageList.DoLayoutList();

            EditorGUI.BeginDisabledGroup( !withMediationSettingsProp.boolValue );
            currentListProp = settingsMessageProp;
            settingsMessageList.DoLayoutList();
            EditorGUI.EndDisabledGroup();

            obj.ApplyModifiedProperties();
        }

        private void DrawLocalizedMessageElement( Rect rect, int index, bool isActive, bool isFocused )
        {
            var height = rect.height;
            var item = currentListProp.GetArrayElementAtIndex( index );
            rect.yMin += 1;
            rect.yMax -= 1;
            rect.yMax -= height * 0.67f + 2;
            var language = item.FindPropertyRelative( "language" );
            var label = ( index == 0 ? "Preferred lang " : "Language " ) + language.enumValueIndex;
            language.enumValueIndex
                = Convert.ToInt32( EditorGUI.EnumPopup( rect, label, ( SystemLanguage )language.enumValueIndex ) );

            rect.yMin = rect.yMax + 1;
            rect.yMax += height * 0.67f - 1;
            var text = item.FindPropertyRelative( "text" );
            text.stringValue = EditorGUI.TextArea( rect, text.stringValue, wordWrapTextArea );
        }

        private void DrawConsentMessageHeader( Rect rect )
        {
            EditorGUI.LabelField( rect, "Consent Message" );
        }

        private void DrawSettingsMessageHeader( Rect rect )
        {
            EditorGUI.LabelField( rect, "Settings Message" );
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
                var prefab = LoadUITemplatePrefab();
                asset = CreateInstance<ConsentRequestParameters>()
                    .WithConsentMessage( GetDefaultConsentMessage() )
                    .WithSettingsMessage( GetDefaultSettingsMessage() );
                if (prefab)
                    asset.WithUIPrefab( prefab.GetComponent<UserConsentUI>() );
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

        private static GameObject CreateCustomUIPrefab()
        {
            const string resultFileName = "Assets/CleverAdsSolutions/UserConsentUI.prefab";

            var originalPrefab = LoadUITemplatePrefab();
            if (!originalPrefab)
                return null;

            if (!AssetDatabase.IsValidFolder( "Assets/CleverAdsSolutions" ))
                AssetDatabase.CreateFolder( "Assets", "CleverAdsSolutions" );

            var objSource = PrefabUtility.InstantiatePrefab( originalPrefab ) as GameObject;
            var asset = PrefabUtility.SaveAsPrefabAsset( objSource, resultFileName );
            DestroyImmediate( objSource );

            return asset;
        }

        private static GameObject LoadUITemplatePrefab()
        {
            const string templateFile = "/Prefabs/ConsentUITemplate.prefab";

            string path = "Packages/" + packageName + templateFile;
            if (!File.Exists( path ))
            {
                path = rootCASFolderPath + templateFile;
                if (!File.Exists( path ))
                {
                    Debug.LogError( "[CleverAdsSolution Consent] User Consent UI Template prefab not found. Try reimport CAS package." );
                    return null;
                }
            }

            return AssetDatabase.LoadAssetAtPath<GameObject>( path );
        }
        #endregion
    }
}
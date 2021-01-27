using UnityEngine;
using UnityEditor;

namespace CAS.UserConsent
{
    [CustomEditor( typeof( UserConsentRequest ) )]
    public class ConsentRequestEditor : Editor
    {
        private SerializedProperty requestOnAwakeProp;
        private SerializedProperty resetUserInfoProp;
        private SerializedProperty resetConsentStatusProp;
        private SerializedProperty parametersProp;
        private SerializedProperty OnConsentProp;

        private bool useGlobalParameters;
        private ConsentRequestParameters paramsInResources;

        private void OnEnable()
        {
            var props = serializedObject;
            requestOnAwakeProp = props.FindProperty( "requestOnAwake" );
            resetUserInfoProp = props.FindProperty( "resetUserInfo" );
            resetConsentStatusProp = props.FindProperty( "resetConsentStatus" );
            parametersProp = props.FindProperty( "parameters" );
            OnConsentProp = props.FindProperty( "OnConsent" );

            useGlobalParameters = parametersProp.objectReferenceValue == null;

            paramsInResources = Resources.Load<ConsentRequestParameters>( ConsentRequestParameters.defaultAssetPath );
        }
        public override void OnInspectorGUI()
        {
            var obj = serializedObject;
            obj.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField( requestOnAwakeProp );
            if (!requestOnAwakeProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox( "To request user consent, call the public method 'Request()'", MessageType.None );
                EditorGUI.indentLevel--;
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField( resetUserInfoProp );
            if (EditorGUI.EndChangeCheck())
                resetConsentStatusProp.boolValue = resetUserInfoProp.boolValue;

            EditorGUI.BeginDisabledGroup( resetUserInfoProp.boolValue );
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField( resetConsentStatusProp );
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

            useGlobalParameters = EditorGUILayout.Toggle( "Use global settings", useGlobalParameters );

            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup( useGlobalParameters );
            if (useGlobalParameters)
            {
                EditorGUILayout.ObjectField( paramsInResources, typeof( ConsentRequestParameters ), false );
            }
            else
            {
                EditorGUILayout.PropertyField( parametersProp );
            }
            EditorGUI.EndDisabledGroup();
            if (useGlobalParameters && !paramsInResources)
                EditorGUILayout.HelpBox( "Please use menu 'Assets > CleverAdsSolutions > Consent Request Parameters' " +
                    "to create and edit global consent request parameters.", MessageType.Warning );

            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField( OnConsentProp );

            obj.ApplyModifiedProperties();
        }
    }
}

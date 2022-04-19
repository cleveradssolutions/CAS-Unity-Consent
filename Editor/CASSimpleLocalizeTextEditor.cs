//
//  Clever Ads Solutions Unity Consent Plugin
//
//  Copyright © 2021 CleverAdsSolutions. All rights reserved.
//

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using CAS.UEditor;
using UnityEngine.UI;

namespace CAS.UserConsent
{
    [CustomEditor( typeof( CASSimpleLocalizeText ) )]
    [CanEditMultipleObjects]
    [Serializable]
    public class CASSimpleLocalizeTextEditor : Editor
    {
        private SerializedProperty textUIUpdateProp;
        private SerializedProperty eventProp;
        private SerializedProperty listProp;
        private ReorderableList list;
        private Text textComponent;

        private void OnEnable()
        {
            textUIUpdateProp = serializedObject.FindProperty( "textUIUpdate" );
            eventProp = serializedObject.FindProperty( "UpdateString" );
            listProp = serializedObject.FindProperty( "text" );

            list = new ReorderableList( serializedObject, listProp, true, false, true, true )
            {
                drawHeaderCallback = DrawHeaderGUI,
                drawElementCallback = DrawElement,
                //headerHeight = 5.0f,
                elementHeight = EditorGUIUtility.singleLineHeight * 3.0f + 8.0f
            };

            if (targets != null && targets.Length == 1)
            {
                var component = target as MonoBehaviour;
                if (component)
                {
                    textComponent = component.GetComponent<Text>();
                    if (textComponent)
                        list.onSelectCallback = OnElementSelected;
                }
            }
        }

        private void DrawHeaderGUI( Rect rect )
        {
            GUI.Label( rect, "Strings (" + listProp.arraySize + ")" );
        }

        private void OnElementSelected( ReorderableList list )
        {
            textComponent.text = listProp.GetArrayElementAtIndex( list.index )
                                         .FindPropertyRelative( "text" )
                                         .stringValue;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField( textUIUpdateProp );
            if (!textUIUpdateProp.boolValue)
                EditorGUILayout.PropertyField( eventProp );

            list.DoLayoutList();
            EditorGUILayout.LabelField( "Select string element to apply content in Text component",
                EditorStyles.wordWrappedMiniLabel );
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawElement( Rect rect, int index, bool isActive, bool isFocused )
        {
            var height = rect.height;
            var item = listProp.GetArrayElementAtIndex( index );
            rect.yMin += 1;
            rect.yMax -= 1;
            rect.yMax -= height * 0.67f + 2;

            var id = item.FindPropertyRelative( "id" );
            var label = "Language ";
            if (listProp.arraySize > 5)
                label += id.intValue;
            id.intValue
                = Convert.ToInt32( EditorGUI.EnumPopup( rect, label, ( SystemLanguage )id.intValue ) );
            rect.yMin = rect.yMax + 1;
            rect.yMax += height * 0.67f - 1;

            var text = item.FindPropertyRelative( "text" );
            text.stringValue = EditorGUI.TextArea( rect, text.stringValue, HelpStyles.wordWrapTextAred );
        }
    }
}
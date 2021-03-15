using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/CAS Simple Localize Text" )]
    public sealed class CASSimpleLocalizeText : MonoBehaviour
    {
        [Serializable]
        public sealed class Event : UnityEvent<string> { }

        public static SystemLanguage language = ( SystemLanguage )222;

        public bool textUIUpdate = true;
        public Event UpdateString;
        public ConsentRequestParameters.TypedText[] text;

        private void Awake()
        {
            if (( int )language == 222)
                language = Application.systemLanguage;
            var newText = text.GetTypedText( ( int )language );
            if (string.IsNullOrEmpty( newText ))
                return;

            if (textUIUpdate)
                GetComponent<Text>().text = newText;
            else
                UpdateString.Invoke( newText );
        }
    }
}

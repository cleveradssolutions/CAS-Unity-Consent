using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAS.UserConsent
{
    [Serializable]
    public sealed class ConsentRequestParameters : ScriptableObject
    {
        [Serializable]
        public sealed class TypedText
        {
            public int id;
            public string text;

            public TypedText() { }
            public TypedText( SystemLanguage language, string text )
            {
                this.id = (int)language;
                this.text = text;
            }

            public TypedText( RuntimePlatform platform, string text )
            {
                this.id = ( int )platform;
                this.text = text;
            }

            public SystemLanguage language { get { return ( SystemLanguage )id; } }

            public RuntimePlatform Platform { get { return ( RuntimePlatform )id; } }
        }

        public const string defaultAssetPath = "CASConsentRequestParameters";

        public Action OnConsent;

        [SerializeField]
        internal bool showInEditor = true;
        [SerializeField]
        internal bool withAudienceDefinition = true;
        [SerializeField]
        internal bool withDeclineOption = false;
        [SerializeField]
        internal bool withMediationSettings = true;
        [SerializeField]
        internal bool withRequestTrackingTransparency = false;

        [SerializeField]
        internal UserConsentUI uiPrefab;

        [SerializeField]
        internal MediationPolicyUI settingsTogglePrefab;

        [SerializeField]
        private TypedText[] privacyPolicyUrl;
        [SerializeField]
        private TypedText[] termsOfUseUrl;

        [SerializeField]
        private TypedText[] consentMessage;
        [SerializeField]
        private TypedText[] settingsMessage;

        internal int resetStatus = 0;

        /// <summary>
        /// Request user consent.
        /// Returns UI otherwise NULL when consent has already been obtained. 
        /// </summary>
        /// <exception cref="NullReferenceException">Consent Request require UserConsentUI prefab!</exception>
        public UserConsentUI Request()
        {
            return ConsentClient.Request( this );
        }

        public ConsentRequestParameters WithCallback( Action listener )
        {
            OnConsent += listener;
            return this;
        }

        public ConsentRequestParameters WithAudienceDefinition()
        {
            withAudienceDefinition = true;
            return this;
        }

        public ConsentRequestParameters DisableAudienceDefinition()
        {
            withAudienceDefinition = false;
            return this;
        }

        public ConsentRequestParameters WithDeclineOption()
        {
            withDeclineOption = true;
            return this;
        }

        public ConsentRequestParameters DisableDeclineOption()
        {
            withDeclineOption = false;
            return this;
        }

        public ConsentRequestParameters WithMediationSettings()
        {
            withMediationSettings = true;
            return this;
        }

        public ConsentRequestParameters DisableMediationSettings()
        {
            withMediationSettings = false;
            return this;
        }

        public ConsentRequestParameters DisableInEditor()
        {
            showInEditor = false;
            return this;
        }

        public ConsentRequestParameters WithUIPrefab( UserConsentUI prefab )
        {
            uiPrefab = prefab;
            return this;
        }

        public ConsentRequestParameters WithMediationSettingsTogglePrefab( MediationPolicyUI prefab )
        {
            settingsTogglePrefab = prefab;
            return this;
        }

        public ConsentRequestParameters WithPrivacyPolicy( string url )
        {
            privacyPolicyUrl = new TypedText[] { new TypedText( SystemLanguage.English, url ) };
            return this;
        }

        public ConsentRequestParameters WithTermsOfUse( string url )
        {
            termsOfUseUrl = new TypedText[] { new TypedText( SystemLanguage.English, url ) };
            return this;
        }

        /// <summary>
        /// Reset saved user consent status and ask the user.
        /// </summary>
        public ConsentRequestParameters WithResetConsentStatus()
        {
            if (resetStatus == 0)
                resetStatus = 1;
            return this;
        }

        /// <summary>
        /// Reset saved user consent status and year of birth and ask the user.
        /// </summary>
        public ConsentRequestParameters WithResetUserInfo()
        {
            resetStatus = 2;
            return this;
        }

        public ConsentRequestParameters WithConsentMessage( string message )
        {
            consentMessage = new TypedText[] { new TypedText( SystemLanguage.English, message ) };
            return this;
        }

        public ConsentRequestParameters WithConsentMessage( Dictionary<SystemLanguage, string> text )
        {
            consentMessage = new TypedText[text.Count];
            int i = 0;
            foreach (var item in text)
            {
                consentMessage[i] = new TypedText( item.Key, item.Value );
                i++;
            }
            return this;
        }

        public ConsentRequestParameters WithSettingsMessage( string message )
        {
            settingsMessage = new TypedText[] { new TypedText( SystemLanguage.English, message ) };
            return this;
        }

        public ConsentRequestParameters WithSettingsMessage( Dictionary<SystemLanguage, string> text )
        {
            settingsMessage = new TypedText[text.Count];
            int i = 0;
            foreach (var item in text)
            {
                settingsMessage[i] = new TypedText( item.Key, item.Value );
                i++;
            }
            return this;
        }

        public string GetPrivacyPolicyUrl(RuntimePlatform platform )
        {
            return GetTypedText( privacyPolicyUrl, ( int )platform );
        }

        public string GetTermsOfUseUrl( RuntimePlatform platform )
        {
            return GetTypedText( termsOfUseUrl, ( int )platform );
        }

        public string GetConsentMessage( SystemLanguage language )
        {
            return GetTypedText( consentMessage, ( int )language );
        }

        public string GetSettingsMessage( SystemLanguage language )
        {
            return GetTypedText( settingsMessage, (int)language );
        }

        private string GetTypedText( TypedText[] source, int id )
        {
            if (source.Length == 0)
                return "";
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].id == id)
                    return source[i].text;
            }
            return source[0].text;
        }
    }
}
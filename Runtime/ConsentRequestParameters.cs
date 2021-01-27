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
        public sealed class LocalizedText
        {
            public SystemLanguage language;
            public string text;

            public LocalizedText( SystemLanguage language, string text )
            {
                this.language = language;
                this.text = text;
            }
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
        internal UserConsentUI uiPrefab;

        [SerializeField]
        internal string privacyPolicyUrl;
        [SerializeField]
        internal string termsOfUseUrl;

        [SerializeField]
        private LocalizedText[] consentMessage;
        [SerializeField]
        private LocalizedText[] settingsMessage;

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

        public ConsentRequestParameters WithPrivacyPolicy( string url )
        {
            privacyPolicyUrl = url;
            return this;
        }

        public ConsentRequestParameters WithTermsOfUse( string url )
        {
            termsOfUseUrl = url;
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
            consentMessage = new LocalizedText[] { new LocalizedText( SystemLanguage.English, message ) };
            return this;
        }

        public ConsentRequestParameters WithConsentMessage( Dictionary<SystemLanguage, string> text )
        {
            consentMessage = new LocalizedText[text.Count];
            int i = 0;
            foreach (var item in text)
            {
                consentMessage[i] = new LocalizedText( item.Key, item.Value );
                i++;
            }
            return this;
        }

        public ConsentRequestParameters WithSettingsMessage( string message )
        {
            settingsMessage = new LocalizedText[] { new LocalizedText( SystemLanguage.English, message ) };
            return this;
        }

        public ConsentRequestParameters WithSettingsMessage( Dictionary<SystemLanguage, string> text )
        {
            settingsMessage = new LocalizedText[text.Count];
            int i = 0;
            foreach (var item in text)
            {
                settingsMessage[i] = new LocalizedText( item.Key, item.Value );
                i++;
            }
            return this;
        }

        public string GetConsentMessage( SystemLanguage language )
        {
            return GetMessage( consentMessage, language );
        }

        public string GetSettingsMessage( SystemLanguage language )
        {
            return GetMessage( settingsMessage, language );
        }

        private string GetMessage( LocalizedText[] source, SystemLanguage language )
        {
            if (source.Length == 0)
                return "";
            string englishMessage = null;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].language == language)
                    return source[i].text;
                if (source[i].language == SystemLanguage.English)
                    englishMessage = source[i].text;
            }
            if (!string.IsNullOrEmpty( englishMessage ))
                return englishMessage;
            return source[0].text;
        }
    }
}
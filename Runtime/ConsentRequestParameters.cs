using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAS.UserConsent
{
    [Serializable]
    public sealed class ConsentRequestParameters : ScriptableObject
    {
        #region Fileds
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

        internal int resetStatus = 0;
        #endregion

        /// <summary>
        /// Request user consent.
        /// Returns UI otherwise NULL when consent has already been obtained. 
        /// </summary>
        /// <exception cref="NullReferenceException">Consent Request require UserConsentUI prefab!</exception>
        [Obsolete( "Renamed to Present()" )]
        public UserConsentUI Request()
        {
            return ConsentClient.Request( this );
        }

        /// <summary>
        /// Request user consent.
        /// Returns UI otherwise NULL when consent has already been obtained. 
        /// </summary>
        /// <exception cref="NullReferenceException">Consent Request require UserConsentUI prefab!</exception>
        public UserConsentUI Present()
        {
            return ConsentClient.Request( this );
        }

        #region Override options
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

        public ConsentRequestParameters WithRequestTrackingTransparency()
        {
            withRequestTrackingTransparency = true;
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
        #endregion

        #region Get text methods
        public string GetPrivacyPolicyUrl( RuntimePlatform platform )
        {
            return privacyPolicyUrl.GetTypedText( ( int )platform );
        }

        public string GetTermsOfUseUrl( RuntimePlatform platform )
        {
            return termsOfUseUrl.GetTypedText( ( int )platform );
        }
        #endregion

        [Serializable]
        public sealed class TypedText
        {
            public int id;
            public string text;

            public TypedText() { }
            public TypedText( SystemLanguage language, string text )
            {
                this.id = ( int )language;
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
    }
}
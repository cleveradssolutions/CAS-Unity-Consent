//
//  Clever Ads Solutions Unity Consent Plugin
//
//  Copyright © 2021 CleverAdsSolutions. All rights reserved.
//

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/Consent UI" )]
    public sealed class UserConsentUI : MonoBehaviour
    {
#pragma warning disable 0649
        [Header( "Panels" )]
        [SerializeField]
        private GameObject consentTextContainer;
        [SerializeField]
        private AudienceDefinitionUI audienceDefinition;
        [SerializeField]
        private MediationSettingsUI mediationSettings;

        [Header( "Components" )]
        [SerializeField]
        private GameObject logoObj;
        [SerializeField]
        private Button privacyPolicyBtn;
        [SerializeField]
        private Button termsOfUseBtn;

        [Header( "Consent options" )]
        [SerializeField]
        private Button acceptBtn;
        [SerializeField]
        private Button declineBtn;
        [SerializeField]
        private Button mediationSettingsBtn;

        [Header( "Optional" )]
        public ConsentRequestParameters parameters;
        public UnityEvent onConsent;
#pragma warning restore 0649

        private void Awake()
        {
            DontDestroyOnLoad( gameObject );

            if (parameters)
                Init( parameters );
        }

        public void Init( ConsentRequestParameters parameters )
        {
            this.parameters = parameters;

            var language = Application.systemLanguage;

            bool requiredAudienceDefinition = parameters.withAudienceDefinition
                && ( parameters.resetStatus == 2 || ConsentClient.GetYearOfBirth() < 0 );

            if (audienceDefinition)
            {
                audienceDefinition.gameObject.SetActive( requiredAudienceDefinition );
                audienceDefinition.OnConsentRequired.AddListener( ShowConsentPanel );
                audienceDefinition.OnUnderAgeOfConsent.AddListener( OnConsentDenied );
            }
            else
            {
                requiredAudienceDefinition = false;
            }

            consentTextContainer.gameObject.SetActive( !requiredAudienceDefinition );

            if (mediationSettings)
            {
                mediationSettings.gameObject.SetActive( false );
                mediationSettings.OnConsent.AddListener( OnConsentDialogWillClose );
                if (parameters.settingsTogglePrefab)
                    mediationSettings.policyPrefab = parameters.settingsTogglePrefab;

                if (mediationSettingsBtn)
                {
                    mediationSettingsBtn.gameObject.SetActive( parameters.withMediationSettings );
                    mediationSettingsBtn.onClick.AddListener( OnOpenOptions );
                }
            }

            if (acceptBtn)
            {
                acceptBtn.gameObject.SetActive( true );
                acceptBtn.onClick.AddListener( OnConsentAccepted );
            }

            if (declineBtn)
            {
                declineBtn.gameObject.SetActive( parameters.withDeclineOption );
                declineBtn.onClick.AddListener( OnConsentDenied );
            }

            if (privacyPolicyBtn)
            {
                if (!string.IsNullOrEmpty( parameters.GetPrivacyPolicyUrl( Application.platform ) ))
                {
                    privacyPolicyBtn.gameObject.SetActive( true );
                    privacyPolicyBtn.onClick.AddListener( OnOpenCompanyPrivacyPolicy );
                }
                else
                {
                    privacyPolicyBtn.gameObject.SetActive( false );
                }
            }

            if (termsOfUseBtn)
            {
                if (!string.IsNullOrEmpty( parameters.GetTermsOfUseUrl( Application.platform ) ))
                {
                    termsOfUseBtn.gameObject.SetActive( true );
                    termsOfUseBtn.onClick.AddListener( OnOpenTermsOfUse );
                }
                else
                {
                    termsOfUseBtn.gameObject.SetActive( false );
                }
            }
        }

        private void OnOpenCompanyPrivacyPolicy()
        {
            Application.OpenURL( parameters.GetPrivacyPolicyUrl( Application.platform ) );
        }

        private void OnOpenTermsOfUse()
        {
            Application.OpenURL( parameters.GetTermsOfUseUrl( Application.platform ) );
        }

        private void ShowConsentPanel()
        {
            consentTextContainer.SetActive( true );
        }

        private void OnOpenOptions()
        {
            logoObj.SetActive( false );
            consentTextContainer.SetActive( false );
            mediationSettings.gameObject.SetActive( true );
        }

        private void OnConsentAccepted()
        {
            consentTextContainer.SetActive( false );
            PlayerPrefs.SetString( ConsentClient.consentStringPref, ConsentClient.consentAccepted );
            MobileAds.settings.userConsent = ConsentStatus.Accepted;
            PlayerPrefs.Save();
            OnConsentDialogWillClose();
        }

        private void OnConsentDenied()
        {
            consentTextContainer.SetActive( false );
            PlayerPrefs.SetString( ConsentClient.consentStringPref, ConsentClient.consentDenied );
            MobileAds.settings.userConsent = ConsentStatus.Denied;
            PlayerPrefs.Save();
            OnConsentDialogWillClose();
        }

        private void OnConsentDialogWillClose()
        {
            try
            {
                if (parameters.withRequestTrackingTransparency)
                {
                    ATTrackingStatus.Request( OnATTResponse );
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogException( e );
            }

            CloseConsentDialog();
        }

        private void OnATTResponse( ATTrackingStatus.AuthorizationStatus status )
        {
            CloseConsentDialog();
        }

        private void CloseConsentDialog()
        {
            Destroy( gameObject );
            if (parameters.OnConsent != null)
                parameters.OnConsent();
            onConsent.Invoke();
        }
    }
}

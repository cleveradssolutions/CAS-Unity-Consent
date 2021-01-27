using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/Consent UI" )]
    public sealed class UserConsentUI : MonoBehaviour
    {
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
        [SerializeField]
        private Text consentText;

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
                mediationSettings.SetMessage( parameters.GetSettingsMessage( language ) );
                mediationSettings.OnConsent.AddListener( OnMediationSettingsApplied );
                if (mediationSettingsBtn)
                {
                    mediationSettingsBtn.gameObject.SetActive( parameters.withMediationSettings );
                    mediationSettingsBtn.onClick.AddListener( OnOpenOptions );
                }
            }

            if (consentText)
            {
                var message = parameters.GetConsentMessage( language );
                if(!string.IsNullOrEmpty( message ))
                    consentText.text = message;
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
                if (!string.IsNullOrEmpty( parameters.privacyPolicyUrl ))
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
                if (!string.IsNullOrEmpty( parameters.termsOfUseUrl ))
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
            Application.OpenURL( parameters.privacyPolicyUrl );
        }

        private void OnOpenTermsOfUse()
        {
            Application.OpenURL( parameters.termsOfUseUrl );
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
            Destroy( gameObject );
            if (parameters.OnConsent != null)
                parameters.OnConsent();
            onConsent.Invoke();
        }

        private void OnConsentDenied()
        {
            consentTextContainer.SetActive( false );

            PlayerPrefs.SetString( ConsentClient.consentStringPref, ConsentClient.consentDenied );
            MobileAds.settings.userConsent = ConsentStatus.Denied;
            PlayerPrefs.Save();
            Destroy( gameObject );
            if (parameters.OnConsent != null)
                parameters.OnConsent();
            onConsent.Invoke();
        }

        private void OnMediationSettingsApplied()
        {
            Destroy( gameObject );
            if (parameters.OnConsent != null)
                parameters.OnConsent();
            onConsent.Invoke();
        }
    }
}

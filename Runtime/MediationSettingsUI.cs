using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/Mediation Settings UI" )]
    public sealed class MediationSettingsUI : MonoBehaviour
    {
#pragma warning disable 0649
        [Header( "Components" )]
        [SerializeField]
        private Transform container;
        [SerializeField]
        private Text message;

        [Header( "Optional" )]
        public MediationPolicyUI policyPrefab;
        public UnityEvent OnConsent;
#pragma warning restore 0649

        private MediationPolicyUI[] items;
        private bool initialized = false;

        private IEnumerator Start()
        {
            string[] privacyPolicyList =
            {
                "https://policies.google.com/technologies/ads",
                "https://vungle.com/privacy/",
                "https://kidoz.net/privacy-policy/",
                "https://answers.chartboost.com/en-us/articles/200780269",
                "https://unity3d.com/legal/privacy-policy",
                "https://www.applovin.com/privacy/",
                "https://www.superawesome.com/privacy-hub/privacy-policy/",
                "https://www.startapp.com/policy/privacy-policy/",
                "https://www.adcolony.com/privacy-policy/",
                "https://developers.facebook.com/docs/audience-network/policy/",
                "https://www.inmobi.com/privacy-policy/",
                "https://www.mobfox.com/privacy-policy/",
                "https://legal.my.com/us/mytarget/privacy/",
                null,
                "https://developers.ironsrc.com/ironsource-mobile/air/ironsource-mobile-privacy-policy/",
                "https://yandex.com/legal/mobileads_sdk_agreement/",
                null,
                "https://advertising.amazon.com/legal/privacy-notice",
                "https://www.verizonmedia.com/policies/us/en/verizonmedia/privacy/",
                "https://www.mopub.com/en/legal/privacy"
            };

            yield return null;
            var active = MobileAds.GetActiveNetworks();

            items = new MediationPolicyUI[active.Length];

            for (int i = 0; i < active.Length; i++)
            {
                var policy = privacyPolicyList[( int )active[i]];
                if (policy != null)
                {
                    var item = Instantiate( policyPrefab, container, false );
                    item.network = active[i];
                    item.SetPrivacyPolicyUrl( policy );
                    items[i] = item;
                    yield return null;
                }
            }
            initialized = true;
        }

        public void SetMessage( string text )
        {
            if (message && !string.IsNullOrEmpty( text ))
                message.text = text;
        }

        public void ApplyMediationConsent()
        {
            if (!initialized)
                return;
            gameObject.SetActive( false );

            var result = new char[items.Length];
            var decline = false;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    result[i] = '-';
                }
                else if (items[i].isAccepted)
                {
                    result[i] = '1';
                }
                else
                {
                    result[i] = '0';
                    decline = true;
                }
            }
            if (decline)
            {
                PlayerPrefs.SetString( ConsentClient.consentStringPref, new string( result ) );
                MobileAds.settings.userConsent = ConsentStatus.Denied;
            }
            else
            {
                PlayerPrefs.SetString( ConsentClient.consentStringPref, ConsentClient.consentAccepted );
                MobileAds.settings.userConsent = ConsentStatus.Accepted;
            }
            initialized = false;

            PlayerPrefs.Save();
            OnConsent.Invoke();
        }

        public void SelectedAll()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                    items[i].isAccepted = true;
            }
        }

        public void DeselectedAll()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                    items[i].isAccepted = false;
            }
        }
    }
}
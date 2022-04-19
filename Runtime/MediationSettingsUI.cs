//
//  Clever Ads Solutions Unity Consent Plugin
//
//  Copyright © 2021 CleverAdsSolutions. All rights reserved.
//

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/Mediation Settings UI" )]
    public sealed class MediationSettingsUI : MonoBehaviour
    {
#pragma warning disable 0649
        [Header( "Components" )]
        [SerializeField]
        private Transform container;

        [Header( "Optional" )]
        public MediationPolicyUI policyPrefab;
        public UnityEvent OnConsent;
#pragma warning restore 0649

        private MediationPolicyUI[] items;
        private bool initialized = false;

        private IEnumerator Start()
        {
            yield return null;
            var active = MobileAds.GetActiveNetworks();
            items = new MediationPolicyUI[active.Length];

            for (int i = 0; i < active.Length; i++)
            {
                var policy = active[i].GetPrivacyPolicy();
                if (!string.IsNullOrEmpty(policy))
                {
                    var netName = active[i].ToString();
                    if (netName.Length > 2)
                    {
                        var item = Instantiate( policyPrefab, container, false );
                        item.network = active[i];
                        item.SetPrivacyPolicyUrl( policy );
                        items[i] = item;
                        yield return null;
                    }
                }
            }
            initialized = true;
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
            ConsentClient.SetMediationExtras();
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
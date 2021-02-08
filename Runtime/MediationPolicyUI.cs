using UnityEngine;
using UnityEngine.UI;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/Mediation Policy UI" )]
    public sealed class MediationPolicyUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private Text label;
#pragma warning restore 0649

        private AdNetwork net;
        private string policyUrl;

        public AdNetwork network
        {
            get { return net; }
            set
            {
                net = value;
                label.text = value.ToString();
            }
        }

        public void SetPrivacyPolicyUrl( string url )
        {
            policyUrl = url;
        }

        public bool isAccepted
        {
            get { return toggle.isOn; }
            set { toggle.isOn = value; }
        }

        public void OnOpenPrivacyPolicy()
        {
            if (!string.IsNullOrEmpty( policyUrl ))
                Application.OpenURL( policyUrl );
        }
    }
}
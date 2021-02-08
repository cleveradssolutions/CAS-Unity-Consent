using System;
using UnityEngine;
using UnityEngine.Events;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/Consent Request" )]
    public class UserConsentRequest : MonoBehaviour
    {
        public bool requestOnAwake = true;
        public bool resetUserInfo = false;
        public bool resetConsentStatus = false;
        public ConsentRequestParameters parameters;
        public UnityEvent OnConsent;

        protected void Start()
        {
            if (requestOnAwake)
                Present();
        }

        [Obsolete("Renamed to Present()")]
        public void Request()
        {
            Present();
        }

        public void Present()
        {
            if (!parameters)
                parameters = UserConsent.BuildRequest();

            if (resetConsentStatus)
                parameters.WithResetConsentStatus();
            if (resetUserInfo)
                parameters.WithResetUserInfo();
            parameters.WithCallback( OnConsentCallback )
                      .Present();
        }

        private void OnConsentCallback()
        {
            OnConsent.Invoke();
        }
    }
}
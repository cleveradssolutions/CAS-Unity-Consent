using System.Collections;
using System.Collections.Generic;
using CAS.UserConsent;
using UnityEngine;
using UnityEngine.Events;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/Request" )]
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
                Request();
        }

        public void Request()
        {
            if (!parameters)
                parameters = UserConsent.BuildRequest();

            if (resetConsentStatus)
                parameters.WithResetConsentStatus();
            if (resetUserInfo)
                parameters.WithResetUserInfo();
            parameters.WithCallback( OnConsentCallback )
                      .Request();
        }

        private void OnConsentCallback()
        {
            OnConsent.Invoke();
        }
    }
}
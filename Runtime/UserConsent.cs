﻿//
//  Clever Ads Solutions Unity Consent Plugin
//
//  Copyright © 2021 CleverAdsSolutions. All rights reserved.
//

using System;
using UnityEngine;

namespace CAS.UserConsent
{
    public static class UserConsent
    {
        public const string version = "2.0.1";

        /// <summary>
        /// User latest consent status values.
        /// If the user has not consent yet then return <see cref="ConsentStatus.Undefined"/>.
        /// </summary>
        public static ConsentStatus GetStatus()
        {
            return ConsentClient.GetStatus();
        }

        /// <summary>
        /// User latest selected year of birth.
        /// If the user has not chosen a year of birth yet then return -1.
        /// </summary>
        public static int GetYearOfBirth()
        {
            return ConsentClient.GetYearOfBirth();
        }

        /// <summary>
        /// User tagged audience.
        /// If the user has not chosen a year of birth yet then return <see cref="Audience.Mixed"/>.
        /// </summary>
        public static Audience GetAudience()
        {
            return ConsentClient.GetAudience( ConsentClient.GetYearOfBirth() );
        }

        /// <summary>
        /// Build User consent request.
        /// Default request parameters can be loaded from asset file in Resources.
        /// Use menu 'Assets > CleverAdsSolutions > Consent Request Parameters' to create default request parameters.
        /// </summary>
        public static ConsentRequestParameters BuildRequest()
        {
            var builder = Resources.Load<ConsentRequestParameters>( ConsentRequestParameters.defaultAssetPath );
            if (builder)
                return UnityEngine.Object.Instantiate( builder );
            return ScriptableObject.CreateInstance<ConsentRequestParameters>();
        }
    }
}
﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace CAS.UserConsent
{
    internal static class ConsentClient
    {
        internal const string consentStringPref = "casUserConsentStr";
        internal const string consentAccepted = "+1";
        internal const string consentDenied = "+0";
        private const string yearOfBirthPref = "casUserYearIfBirth";

        internal static ConsentStatus GetStatus()
        {
            var consent = PlayerPrefs.GetString( consentStringPref, "" );
            if (consent.Length == 0)
                return ConsentStatus.Undefined;
            if (consent == consentDenied)
                return ConsentStatus.Denied;
            if (consent == consentAccepted)
                return ConsentStatus.Accepted;
            var active = MobileAds.GetActiveNetworks();
            if (active.Length != consent.Length)
                return ConsentStatus.Undefined;
            return ConsentStatus.Denied;
        }

        internal static void ResetConsentStatus()
        {
            PlayerPrefs.DeleteKey( consentStringPref );
        }

        internal static int GetYearOfBirth()
        {
            return PlayerPrefs.GetInt( yearOfBirthPref, -1 );
        }

        internal static Audience SetYearOfBirth( int year )
        {
            PlayerPrefs.SetInt( yearOfBirthPref, year );
            var result = GetAudience( year );
            MobileAds.settings.taggedAudience = result;
            return result;
        }

        internal static Audience GetAudience( int year )
        {
            if (year < 0)
                return Audience.Mixed;
            if (DateTime.Now.Year - year < 12)
                return Audience.Children;
            return Audience.NotChildren;
        }

        internal static void ResetYearOfBirth()
        {
            PlayerPrefs.DeleteKey( yearOfBirthPref );
        }

        internal static void SetMediationExtras()
        {
            var consent = PlayerPrefs.GetString( consentStringPref, "" );
            if (consent.Length == 0 || consent == consentDenied || consent == consentAccepted)
                return;

            var active = MobileAds.GetActiveNetworks();
            if (active.Length != consent.Length)
                return;
            var result = new Dictionary<string, string>();
            for (int i = 0; i < consent.Length; i++)
            {
                if (consent[i] != '-')
                {
                    var tag = GetNetworkTag( active[i] );
                    result[tag + "_gdpr"] = consent[i].ToString();
                    //result[tag + "_ccpa"] = consent[i].ToString();
                }
            }
            if (result.Count > 0)
                MediationExtras.SetGlobalEtras( result );
        }

        internal static UserConsentUI Request( ConsentRequestParameters parameters )
        {
            Audience savedAudience = Audience.Mixed;
            if (parameters.withAudienceDefinition && parameters.resetStatus < 2)
            {
                savedAudience = GetAudience( GetYearOfBirth() );
                if (savedAudience != Audience.Mixed)
                    MobileAds.settings.taggedAudience = savedAudience;
            }

            if (parameters.resetStatus == 0 || savedAudience == Audience.Children)
            {
                var savedStatus = GetStatus();
                if (savedStatus != ConsentStatus.Undefined)
                {
                    MobileAds.settings.userConsent = savedStatus;
                    SetMediationExtras();
                    if (parameters.OnConsent != null)
                        parameters.OnConsent();
                    return null;
                }
            }

            if (!parameters.showInEditor && Application.isEditor)
            {
                if (parameters.OnConsent != null)
                    parameters.OnConsent();
                return null;
            }

            if (!parameters.uiPrefab)
                throw new NullReferenceException( "Consent Request require UserConsentUI prefab!" );
            var instance = UnityEngine.Object.Instantiate( parameters.uiPrefab );
            instance.Init( parameters );
            return instance;
        }

        private static string GetNetworkTag( AdNetwork network )
        {
            switch (network)
            {
                case AdNetwork.GoogleAds:
                    return "AM";
                case AdNetwork.Vungle:
                    return "V";
                case AdNetwork.Kidoz:
                    return "K";
                case AdNetwork.Chartboost:
                    return "CB";
                case AdNetwork.UnityAds:
                    return "U";
                case AdNetwork.AppLovin:
                    return "AL";
                case AdNetwork.SuperAwesome:
                    return "SuA";
                case AdNetwork.StartApp:
                    return "StA";
                case AdNetwork.AdColony:
                    return "AC";
                case AdNetwork.FacebookAN:
                    return "FB";
                case AdNetwork.InMobi:
                    return "IM";
                case AdNetwork.MobFox:
                    return "MF";
                case AdNetwork.MyTarget:
                    return "MT";
                case AdNetwork.CrossPromotion:
                    return "P";
                case AdNetwork.IronSource:
                    return "IS";
                case AdNetwork.YandexAds:
                    return "Ya";
                case AdNetwork.OwnVAST:
                    return "Own";
                case AdNetwork.AmazonAds:
                    return "AZ";
                case AdNetwork.Verizon:
                    return "VZ";
                case AdNetwork.MoPub:
                    return "MP";
                default:
                    return string.Empty;
            }
        }
    }
}
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CAS.iOS
{
    /// <summary>
    /// Wraps for the native iOS 14.0 ATTrackingManager.
    /// A class that provides a tracking authorization request and the tracking authorization status of the app.
    /// <see cref="https://developer.apple.com/documentation/apptrackingtransparency/attrackingmanager"/>
    /// </summary>
    public static class AppTrackingTransparency
    {
        public enum Status
        {
            NotDetermined,
            Restricted,
            Denied,
            Authorized
        }

        /// <summary>
        /// Returns information about your application’s tracking authorization status.
        /// Users are able to grant or deny developers tracking privileges on a per-app basis.
        /// Application developers must call <see cref="Request()"/> for the ability to track users.
        ///
        /// The current authorization status. If the user has not yet been prompted to approve access, the return value will either be
        /// <see cref="Status.NotDetermined"/>, or <see cref="Status.Restricted"/> if this value is managed.
        /// Once the user has been prompted, the return value will be either <see cref="Status.Denied"/> or <see cref="Status.Authorized"/>.
        /// </summary>
        public static event Action<Status> OnAuthorizationRequestComplete;

        /// <summary>
        /// Request user tracking authorization with a completion handler returning the user's authorization status.
        /// Users are able to grant or deny developers tracking privileges on a per-app basis.
        /// This method allows developers to determine if access has been granted. On first use, this method will prompt the user to grant or deny access.
        ///
        /// Please set <b>NSUserTrackingUsageDescription</b> in 'Assets > CleverAdsSolutions > iOS Settings' menu to correct tracking authorization request.
        /// 
        /// The completion handler will be called with the result of the user's decision for granting or denying permission to use application tracking.
        /// The completion handler will be called immediately if access to request authorization is restricted.
        /// The completion handler will be called immediately if runtime platform is not iOS 14 or newer.
        /// </summary>
        /// <exception cref="ArgumentNullException">Please subscribe callback OnAuthorizationRequestComplete before call Request()</exception>
        /// <exception cref="ArgumentException">Please set NSUserTrackingUsageDescription in 'Assets > CleverAdsSolutions > iOS Settings' menu to correct tracking authorization request.</exception>
        public static void Request()
        {
            if (OnAuthorizationRequestComplete == null)
                throw new ArgumentNullException( "Please subscribe callback OnAuthorizationRequestComplete before call Request()." );
#if UNITY_IOS || (CASDeveloper && UNITY_EDITOR)
#if UNITY_EDITOR
            var settings = UserConsent.UserConsent.BuildRequest();
            if (string.IsNullOrEmpty( settings.defaultIOSTrakingUsageDescription ))
                throw new ArgumentNullException(
                    "Please set NSUserTrackingUsageDescription in 'Assets > CleverAdsSolutions > Consent Request parameters' menu to correct tracking authorization request." );
            OnAuthorizationRequestComplete( Status.Authorized );
#else
            CASURequestTracking( AuthorizationRequestComplete );
#endif
#else
            OnAuthorizationRequestComplete( Status.Authorized );
#endif
        }

#if UNITY_IOS || (CASDeveloper && UNITY_EDITOR)
        internal delegate void CASUTrackingStatusCallback( int status );

        [DllImport( "__Internal" )]
        internal static extern void CASURequestTracking( CASUTrackingStatusCallback callback );

        [AOT.MonoPInvokeCallback( typeof( CASUTrackingStatusCallback ) )]
        private static void AuthorizationRequestComplete( int status )
        {
            try
            {
                if (OnAuthorizationRequestComplete != null)
                    OnAuthorizationRequestComplete( ( Status )status );
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException( e );
            }
        }
#endif
    }
}

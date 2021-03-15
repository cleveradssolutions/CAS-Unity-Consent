#if UNITY_IOS || CASDeveloper
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace CAS.UserConsent
{
    internal class PostprocessiOSBuild
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild( BuildTarget buildTarget, string path )
        {
            if (buildTarget != BuildTarget.iOS)
                return;
            var parameters = UserConsent.BuildRequest();
            var trakingUsage = parameters.defaultIOSTrakingUsageDescription;
            if (string.IsNullOrEmpty( trakingUsage ))
                return;
            string plistPath = Path.Combine( path, "Info.plist" );
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile( plistPath );
            plist.root.SetString( "NSUserTrackingUsageDescription", trakingUsage );
            File.WriteAllText( plistPath, plist.WriteToString() );
        }
    }
}
#endif
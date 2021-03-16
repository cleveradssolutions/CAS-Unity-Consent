# CleverAdsSolutions-Unity Consent
[![GitHub package.json version](https://img.shields.io/github/package-json/v/cleveradssolutions/CAS-Unity-Consent?label=Unity%20Package)](https://github.com/cleveradssolutions/CAS-Unity/releases/latest)  

Every application, except for children, must make certain disclosures to users in the European Economic Area (EEA) along with the UK and obtain their consent to use cookies or other local storage, where legally required, and to use personal data (such as AdID) to serve ads. This policy reflects the requirements of the EU ePrivacy Directive and the General Data Protection Regulation (GDPR).  

The CAS Unity Consent provides tools for publishers to request consent for personalized ads as well as to handle [Apple's App Tracking Transparency (ATT)](https://developer.apple.com/documentation/apptrackingtransparency) requirements. Publishers can use the UMP SDK to handle either or both of these requests by showing a single a form, as all of the configuration happens in the unity inspector.

## Add the CAS Consent package to Your Project
if you are using Unity 2018.4 or newer then you can add CAS SDK to your Unity project using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html), or you can import the package manually.

#### Option 1 Unity Package Manager
Add the **Game Package Registry by Google**  and CAS dependency to your Unity project.  
Modify `Packages/manifest.json`  to the following form:
```json
{
"scopedRegistries": [
  {
    "name": "Game Package Registry by Google",
    "url": "https://unityregistry-pa.googleapis.com",
    "scopes": [
      "com.google"
    ]
  }
],
"dependencies": {
"com.cleversolutions.ads.consent.unity": "https://github.com/cleveradssolutions/CAS-Unity-Consent.git#1.2.1"
}
}
```
> Note that some other SDKs, such as the Firebase SDK, may contain [EDM4U](https://github.com/googlesamples/unity-jar-resolver) in their .unitypackage. Check if `Assets/ExternalDependencyManager` or `Assets/PlayServicesResolver` folders exist. If these folders exist, remove them before installing any CAS SDK through Unity Package Manager.

#### Option 2 Manual installation
1. Download latest [CleverAdsSolutions_Consent.unitypackage](https://github.com/cleveradssolutions/CAS-Unity-Consent/releases/latest)
2. In your open Unity project, navigate to **Assets > Import Package > Custom Package**.
3. In the *Import Unity Package* window, make sure all of the files are selected and click **Import**.

## Step 2 Configuring the SDK
In your open Unity project, navigate to **Assets > CleverAdsSolutions > Consent request parameters** to create and modify request parameters.

- **With Audience Definition** toggle - With request user year of birth form. 
- **Show in Editor** toggle - Show request in Editor.
- **Privacy policy URL** field - URL to applicaiton privacy policy for each platform or universal.
- **Terms of Use URL** field - URL to application terms of use for each platform or universal.
- **With Decline Option** toggle - With Decline button for refuse to use personal data. 
- **Consent UI Prefab** field - Refference to main canvas prefab.
**Create template** button - Create and set a template prefab in project assets to override the interface.  
**Default** button - Set default prefab from package.
- **With Mediation Settings** toggle - With more options form to select each ad providers consent.
- **Toggle UI prefab** field - Refference to main canvas prefab.  
**Create template** button - Create and set a template prefab in project assets to override the interface.  
**Default** button - Set default prefab from package.
- **With [Request Tracking Transparency](https://developer.apple.com/documentation/apptrackingtransparency)** toggle - With request iOS user authorization to access app-related data for tracking the user or the device.  

## Using the SDK from scripts
### Build request parameters
To load query parameters from an asset into resources, or create default parameters when an asset has not been created, use the following method: 
```csharp
ConsentRequestParameters request = CAS.UserConsent.UserConsent.BuildRequest();
```
To subscribe callback use the following method:
```csharp
request.WithCallback(ContinueInitialzie);

private void ContinueInitialzie(){
 // User consent received 
 // Next step initialize Clever Ads Solutions
 var builder = CAS.MobileAds.BuildManager();
 
 // Apply User consent status to Mediation Manager
 builder.WithUserConsent();
 
 builder.Initialize();
}
```
Override any parameters with following methods:
```csharp
request.DisableInEditor()
       .DisableAudienceDefinition()
       .DisableDeclineOption()
       .DisableMediationSettings()
       .WithAudienceDefinition()
       .WithDeclineOption()
       .WithMediationSettings()
       .WithRequestTrackingTransparency()
       .WithUIPrefab(customUIPrefab)
       .WithMediationSettingsTogglePrefab(customTogleUIPrefab)
       .WithPrivacyPolicy(privcyPolicyURL)
       .WithTermsOfUse(termsOfUseURL)
       .WithResetConsentStatus()
       .WithResetUserInfo()
       .WithConsentMessage(consentMessage)
       .WithSettingsMessage(mediationSettingsMessage);
```
### Present the form
```csharp
UserConsentUI form = request.Present();
if(form){
 // Request form presented and need wait callback to continue loading.
 form.WithCallback(ContinueInitialzie);
}else{
 // Form are not presented.
 ContinueInitialzie();
}
```
### Request the latest consent information
Get the latest consent status value:
```csharp
ConsentStatud status = CAS.UserConsent.UserConsent.GetStatus();
```
Get the latest selected year of birth.
```csharp
int year = CAS.UserConsent.UserConsent.GetYearOfBirth();
```
Get the latest tagged audience.
```csharp
Audience audience = CAS.UserConsent.UserConsent.GetAudience();
```

## Using the SDK components
Add component > CleverAdsSolutions > UserConsent > Request
- **Request On Awake** - Automatically form present while the scene is awake. Else call public method `Present()`
- **Reset User Info** - Reset latest user consent state and selected year of birth.
- **Reset Consent Status** - Reset latest user consent state.
- **Use global settings** - Use consent request parameters from asset in resource. Else set reference to custom consent request parameters asset.
- **OnConsent** - Event on requeset finish.

![image](https://user-images.githubusercontent.com/22005013/107220739-5fde5680-6a1b-11eb-87d0-8bca43a756a4.png)

#### Provide a way for users to change their consent.
![image](https://user-images.githubusercontent.com/22005013/107221407-47bb0700-6a1c-11eb-825d-3d0a2b500016.png)


## iOS App Tracking Transparency authorization request
iOS 14 and above requires publishers to obtain permission to track the user's device across applications.  

1. Open Consent requeset parameters in `Assets > CleverAdsSolution > Consent Request Parameters` menu.
2. Check [iOS App Tracking Transparency authorization request](https://developer.apple.com/documentation/apptrackingtransparency)
3. Define [NSUserTrackingUsageDescription](https://developer.apple.com/documentation/bundleresources/information_property_list/nsusertrackingusagedescription)  
Below is an example description text:
   - This identifier will be used to deliver personalized ads to you.
   - Your data will be used to provide you a better and personalized ad experience.
   - We try to show ads for apps and products that will be most interesting to you based on the apps you use.
   - We try to show ads for apps and products that will be most interesting to you based on the apps you use, the device you are on, and the country you are in.  

> **Important!** CAS does not provide legal advice. Therefore, the information on this page is not a substitute for seeking your own legal counsel to determine the legal requirements of your business and processes, and how to address them.

## GitHub issue tracker
To file bugs, make feature requests, or suggest improvements for the Unity Consent Plugin, please use [GitHub's issue tracker](https://github.com/cleveradssolutions/CAS-Unity-Consent/issues).

## Support
Site: [https://cleveradssolutions.com](https://cleveradssolutions.com)

Technical support: Max  
Skype: m.shevchenko_15  

Network support: Vitaly  
Skype: zanzavital  

mailto:support@cleveradssolutions.com  

## License
The CAS Unity Consent plugin is available under a commercial license. See the LICENSE file for more info.

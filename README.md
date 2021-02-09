# CleverAdsSolutions-Unity Consent
Every application, except for children, must make certain disclosures to users in the European Economic Area (EEA) along with the UK and obtain their consent to use cookies or other local storage, where legally required, and to use personal data (such as AdID) to serve ads. This policy reflects the requirements of the EU ePrivacy Directive and the General Data Protection Regulation (GDPR).

[![GitHub package.json version](https://img.shields.io/github/package-json/v/cleveradssolutions/CAS-Unity-Consent?label=Unity%20Package)](https://github.com/cleveradssolutions/CAS-Unity/releases/latest)  

## Add the CAS Consent package to Your Project
if you are using Unity 2018.4 or newer then you can add CAS SDK to your Unity project using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html), or you can import the package manually.

<details><summary><b>Unity Package Manager</b></summary>

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
"com.cleversolutions.ads.unity": "https://github.com/cleveradssolutions/CAS-Unity-Consent.git#1.1.2"
}
}
```
> Note that some other SDKs, such as the Firebase SDK, may contain [EDM4U](https://github.com/googlesamples/unity-jar-resolver) in their .unitypackage. Check if `Assets/ExternalDependencyManager` or `Assets/PlayServicesResolver` folders exist. If these folders exist, remove them before installing any CAS SDK through Unity Package Manager.
***
</details>
<details><summary><b>Manual installation</b></summary>

1. Download latest [CleverAdsSolutions_Consent.unitypackage](https://github.com/cleveradssolutions/CAS-Unity-Consent/releases/latest)
2. In your open Unity project, navigate to **Assets > Import Package > Custom Package**.
3. In the *Import Unity Package* window, make sure all of the files are selected and click **Import**.
***
</details>

## Step 2 Configuring the SDK
In your open Unity project, navigate to **Assets > CleverAdsSolutions > Consent request parameters** to create and modify request parameters.

- **With Audience Definition** toggle - With request user year of birth form. 
- **With [Request Tracking Transparency](https://developer.apple.com/documentation/apptrackingtransparency)** toggle - With request iOS user authorization to access app-related data for tracking the user or the device.  
Available with [NSUserTrackingUsageDescription](https://github.com/cleveradssolutions/CAS-Unity#include-ios) only.
- **Show in Editor** toggle - Show request in Editor.
- **Reset message** button - Resset Consent messages and Mediation Settings messages to default values.
- **Privacy policy URL** field - URL to applicaiton privacy policy for each platform or universal.
- **Terms of Use URL** field - URL to application terms of use for each platform or universal.
- **With Decline Option** toggle - With Decline button for refuse to use personal data. 
- **Consent UI Prefab** field - Refference to main canvas prefab.
**Create template** button - Create and set a template prefab in project assets to override the interface.  
**Default** button - Set default prefab from package.
- **Consent Message** field - Consent message to user for each system language or universal.
- **With Mediation Settings** toggle - With more options form to select each ad providers consent.
- **Toggle UI prefab** field - Refference to main canvas prefab.  
**Create template** button - Create and set a template prefab in project assets to override the interface.  
**Default** button - Set default prefab from package.
**Mediaiton Settings Message** field - Message to Mediaiton settings form for each system language or universal.

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

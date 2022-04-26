# CleverAdsSolutions-Unity Consent
[![GitHub package.json version](https://img.shields.io/github/package-json/v/cleveradssolutions/CAS-Unity-Consent?label=Unity%20Package)](https://github.com/cleveradssolutions/CAS-Unity/releases/latest)  

Every application, except for children, must make certain disclosures to users in the European Economic Area (EEA) along with the UK and obtain their consent to use cookies or other local storage, where legally required, and to use personal data (such as AdID) to serve ads. This policy reflects the requirements of the EU ePrivacy Directive and the General Data Protection Regulation (GDPR).  

The CAS Unity Consent provides tools for publishers to request consent for personalized ads as well as to handle [Apple's App Tracking Transparency (ATT)](https://developer.apple.com/documentation/apptrackingtransparency) requirements.

## Add the CAS Consent package to Your Project
if you are using Unity 2018.4 or newer then you can add CAS SDK to your Unity project using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html), or you can import the package manually.

### Option 1 Unity Package Manager window
In your open Unity project, navigate to `Window -> Package Manager -> + -> Add package from Git URL` and add url:  

![image](https://user-images.githubusercontent.com/22005013/135641554-c38422fa-995e-4e56-9c3a-a89a72172081.png)

```
https://github.com/cleveradssolutions/CAS-Unity-Consent.git#2.0.1
```

### Option 2 Modify `manifest.json`
Open `YourProject/Packages/manifest.json` file in any text editor and add line in `dependencies` tag:
```json
{ "dependencies": {
"com.cleversolutions.ads.unity": "https://github.com/cleveradssolutions/CAS-Unity-Consent.git#2.0.1",
} }
```

### Option 3 Manual import unity package
1. Download latest [CleverAdsSolutions.unitypackage](https://github.com/cleveradssolutions/CAS-Unity-Consent/releases/latest)
2. In your open Unity project, navigate to `Assets > Import Package > Custom Package`.
3. In the `Import Unity Package` window, make sure all of the files are selected and click **Import**.

## Step 2 Configuring the SDK
In your open Unity project, navigate to **Assets > CleverAdsSolutions > Consent request parameters** to create and modify request parameters.

- **Test in Unity Editor** - Show request in Unity Editor.
- **With Age of audience request** - The user is prompted for the year ir birth and the audience is determined automatically. 
- **With iOS App tracking Transparency request** - The iOS 14.5+ users is prompted for [permission to track the Advertising ID](https://developer.apple.com/documentation/apptrackingtransparency). 
- **With option of Decline consent** - The user is given the choice to opt out dialog. 
- **With Mediation Networks settings** - The user is provided with advanced consent settings for each active network in the game.
- **Network UI Prefab** field - Refference to network toggle prefab.
  - **Create template** button - Create and set a template prefab in project assets to override the interface.  
  - **Default** button - Set default prefab from package.
- **Consent UI Prefab** field - Refference to main canvas prefab.
  - **Create template** button - Create and set a template prefab in project assets to override the interface.  
  - **Default** button - Set default prefab from package.
- **Privacy policy URL** field - URL to applicaiton privacy policy for each platform or universal.
- **Terms of Use URL** field - URL to application terms of use for each platform or universal.

## Using the SDK from scripts
### Build request parameters
To load query parameters from an asset into resources, or create default parameters when an asset has not been created, use the following method: 
```csharp
ConsentRequestParameters request = CAS.UserConsent.UserConsent.BuildRequest();
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
To subscribe callback use the following method:
```csharp
request.WithCallback(() => {
 // User consent received 
 // Next step initialize Clever Ads Solutions
 IMediationManager manager = CAS.MobileAds.BuildManager().Initialize();
});
```
### Present the form
```csharp
UserConsentUI form = request.Present();
```
### Request the latest consent information
Get the latest consent status value:
```csharp
ConsentStatus status = CAS.UserConsent.UserConsent.GetStatus();
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
2. Check **With iOS App tracking Transparency request**
3. Open CAS settings in `Assets > CleverAdsSolution > iOS Settings` menu.
4. Check **Set User Tracking Usage description**
5. Define [NSUserTrackingUsageDescription](https://developer.apple.com/documentation/bundleresources/information_property_list/nsusertrackingusagedescription)  

## GitHub issue tracker
To file bugs, make feature requests, or suggest improvements for the Unity Consent Plugin, please use [GitHub's issue tracker](https://github.com/cleveradssolutions/CAS-Unity-Consent/issues).

## Support
Site: [https://cleveradssolutions.com](https://cleveradssolutions.com)  

mailto:support@cleveradssolutions.com  

## License
The CAS Unity Consent plugin is available under a commercial license. See the LICENSE file for more info.

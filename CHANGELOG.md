# Clever Ads Solutions Unity Plugin Change Log

## [1.2.6] - 2021-07-07
### Bug Fixes
- [iOS] Fixed response of AppTrackingTransparency from wrong thread.

## [1.2.5] - 2021-04-06
### Dependencies
- CleverAdsSolutions - [2.2.2](https://github.com/cleveradssolutions/CAS-Unity/releases)

## [1.2.4] - 2021-04-06
### Dependencies
- CleverAdsSolutions - [2.1.4](https://github.com/cleveradssolutions/CAS-Unity/releases)

## [1.2.3] - 2021-03-24
### Bug Fixes
- Fix wrong network name when Consent plguin get unknown network.

## [1.2.2] - 2021-03-24
### Features
- Added Tapjoy and Fyber mediation networks.
### Bug Fixes
- Fix button `Decline all` localization into Polish and Chinese.
- Fix `IndexOutOfRangeException` from `MediationSettingsUI` when Consent plguin get unknown network.

## [1.2.1] - 2021-03-02
### Bug Fixes
- Fix `AppTrackingTransparency.Request()` in Unity Editor.

## [1.2.0] - 2021-03-02
### Dependencies
- CleverAdsSolutions - [2.0.0](https://github.com/cleveradssolutions/CAS-Unity/releases)
### Features
- Added text localization in 15 languages.
- Added iOS [App Tracking Transparency request](https://developer.apple.com/documentation/apptrackingtransparency) in separated script `CASUAppTracking.m`.
- Sets `MobileAds.targetingOptions.age` on select user year of birth.
### Changes
- Deprecate `MobileAds.BuildManager().WithUserConsent()` no longer needs to be called.

## [1.1.4] - 2021-02-11
### Features
- Remove int parse from string in Audience Definition logic.
### Bug Fixes
- Wrong select year and calculate audience.

## [1.1.3] - 2021-02-10
### Bug Fixes
- Ability to drag the UserConsentUI prefab to field of consent request parameters. 

## [1.1.2] - 2021-02-09
### Changes
- Years text colors, on Audience Definition form, will be saved and changed to the values that are defined in the prefab `AudienceDefinition/Years/[0..4]/Text.color`.

## [1.1.1] - 2021-02-08
### Changes
- Rename methods `Request()` to `Present()`.

## [1.1.0] - 2021-02-08
### Dependencies
- CleverAdsSolutions - [1.9.9](https://github.com/cleveradssolutions/CAS-Unity/releases)
### Features
- Added options to set Privacy Policy and Terms Of Use URLs for each runtime platform.
- Added option to override Mediation settings toggle UI prefab.
- Improvements to Consent request parameters asset editor.
- Improvements to canvas GUI for landscape orientation. 
- [iOS] Added option to request user authorization to access app-related data for tracking the user or the device.

## [1.0.0] - 2021-01-27
### Dependencies
- CleverAdsSolutions - [1.9.6](https://github.com/cleveradssolutions/CAS-Unity/releases)

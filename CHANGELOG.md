# Clever Ads Solutions Unity Plugin Change Log

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

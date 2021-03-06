//
//  CASUAppTracking.m
//  CASUnityPlugin
//
//  Copyright © 2021 Clever Ads Solutions. All rights reserved.
//

#import <AppTrackingTransparency/AppTrackingTransparency.h>

typedef void (*CASUTrackingStatusCallback)(NSInteger status);

void CASURequestTracking(CASUTrackingStatusCallback callback)
{
    if (@available(iOS 14, *)) {
        [ATTrackingManager
         requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
             if (callback) {
                 callback((int)status);
             }
         }];
    } else {
        if (callback) {
            callback(3);
        }
    }
}

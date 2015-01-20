//
//  iAdBannerController.h
//  Unity-iPhone
//
//  Created by lacost on 11/8/13.
//
//

#import <Foundation/Foundation.h>
#import <iAd/iAd.h>


#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif


#include "iAdBannerObject.h"

@interface iAdBannerController : NSObject<ADInterstitialAdDelegate> {
    

}

+ (iAdBannerController *)sharedInstance;


- (void) CreateBannerAd:(int) gravity bannerId: (int) bannerId;
- (void) CreateBannerAd:(int) x y: (int) y  bannerId: (int) bannerId;


- (void) ShowAd: (int) bannerId;
- (void) HideAd: (int) bannerId;
- (void) DestroyBanner: (int) bannerId;


- (void) StartInterstitialAd;
- (void) LoadInterstitialAd;
- (void) ShowInterstitialAd;


@end

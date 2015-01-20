//
//  iAdBannerController.m
//  Unity-iPhone
//
//  Created by lacost on 11/8/13.
//
//

#import "iAdBannerController.h"

@implementation iAdBannerController


static iAdBannerController *sharedHelper = nil;
static NSMutableDictionary* _banners;
static ADInterstitialAd *interstitial = nil;
static BOOL IsShowIntersticialsOnLoad = false;



+ (iAdBannerController *) sharedInstance {
    if (!sharedHelper) {
        _banners = [[NSMutableDictionary alloc] init];
        sharedHelper = [[iAdBannerController alloc] init];
        
    }
    return sharedHelper;
}

- (void) StartInterstitialAd {

     if(interstitial == nil) {
        [self LoadInterstitialAd];
        IsShowIntersticialsOnLoad = true;
    }
}

-(void) LoadInterstitialAd {
   
    if(interstitial == nil) {
        interstitial = [[ADInterstitialAd alloc] init];
        interstitial.delegate = self;
        
         IsShowIntersticialsOnLoad = false;
    }
}

-(void) ShowInterstitialAd {
    if(interstitial != nil) {
        if(interstitial.isLoaded) {
            UIViewController *vc =  UnityGetGLViewController();
            [interstitial presentFromViewController:vc];
        }
    }
}

-(void) ShowVieoAd {
    
}

-(void) CreateBannerAd:(int)gravity bannerId:(int)bannerId {
    
    iAdBannerObject* banner;
    banner = [[iAdBannerObject alloc] init];
    
    [banner CreateBanner:gravity  bannerId:bannerId];
    [_banners setObject:banner forKey:[NSNumber numberWithInt:bannerId]];

}


-(void) CreateBannerAd:(int)x y:(int)y bannerId:(int)bannerId {
    iAdBannerObject* banner;
    banner = [[iAdBannerObject alloc] init];
    
    
    [banner CreateBannerAdPos:x y:y  bannerId:bannerId];
    [_banners setObject:banner forKey:[NSNumber numberWithInt:bannerId]];
    
}

-(void) HideAd:(int)bannerId {
    iAdBannerObject *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner HideAd];
    }
    
}

-(void) ShowAd:(int)bannerId {
    iAdBannerObject *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner ShowAd];
    }
}

- (void) DestroyBanner:(int)bannerId {
    iAdBannerObject *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner HideAd];
         #if UNITY_VERSION < 500
        [banner dealloc];
        #endif
        
        
    }
}



#pragma mark - ADInterstitialAdDelegate

- (void)interstitialAdDidUnload:(ADInterstitialAd *)interstitialAd {
    NSLog(@"interstitialAdDidUnload");
   
#if UNITY_VERSION < 500
    [interstitial release];
#endif
    
    interstitial = nil;
}

- (void)interstitialAd:(ADInterstitialAd *)interstitialAd didFailWithError:(NSError *)error {
   
    NSLog(@"didFailWithError: %@", error.description);
    
#if UNITY_VERSION < 500
    [interstitial release];
#endif
    
    interstitial = nil;
    
     UnitySendMessage("iAdBannerController", "interstitialdidFailWithError", "");
}

- (void)interstitialAdWillLoad:(ADInterstitialAd *)interstitialAd  {
    NSLog(@"interstitialAdWillLoad");
    
    UnitySendMessage("iAdBannerController", "interstitialAdWillLoad", "");
}

- (void)interstitialAdDidLoad:(ADInterstitialAd *)interstitialAd {
    NSLog(@"interstitialAdDidLoad");
    
    if(IsShowIntersticialsOnLoad) {
        UIViewController *vc =  UnityGetGLViewController();
        [interstitial presentFromViewController:vc];
    }
    
    UnitySendMessage("iAdBannerController", "interstitialAdDidLoad", "");
    
}
//- (void)interstitialAdActionDidFinish
- (void)interstitialAdActionDidFinish:(ADInterstitialAd *)interstitialAd {
    NSLog(@"interstitialAdActionDidFinish");
    
#if UNITY_VERSION < 500
    [interstitial release];
#endif
    
    
    interstitial = nil;
    
    UnitySendMessage("iAdBannerController", "interstitialAdActionDidFinish", "");
}

@end

extern "C" {
    
    void _IADCreateBannerAd (int gravity, int bannerId)  {
        [[iAdBannerController sharedInstance] CreateBannerAd:gravity bannerId:bannerId];
    }
    
    void _IADCreateBannerAdPos(int x, int y, int bannerId) {
        [[iAdBannerController sharedInstance] CreateBannerAd:x y:y bannerId:bannerId];
    }
    
    
    void _IADShowAd(int bannerId) {
        [[iAdBannerController sharedInstance] ShowAd:bannerId];
    }
    
    void _IADHideAd(int bannerId) {
        [[iAdBannerController sharedInstance] HideAd:bannerId];
    }

    void _IADDestroyBanner(int bannerId) {
        [[iAdBannerController sharedInstance] DestroyBanner:bannerId];
    }

    void _IADStartInterstitialAd() {
        [[iAdBannerController sharedInstance] StartInterstitialAd];
    }
    
    void _IADLoadInterstitialAd() {
        [[iAdBannerController sharedInstance] LoadInterstitialAd];
    }
    
    void _IADShowInterstitialAd() {
        [[iAdBannerController sharedInstance] ShowInterstitialAd];
    }
    
    
}

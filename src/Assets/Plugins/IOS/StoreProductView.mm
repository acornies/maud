//
//  StoreProductView.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 5/21/14.
//
//

#import "StoreProductView.h"

@implementation StoreProductView

- (void) CreateView:(int)viewId products:(NSArray *)products {
    
    NSLog(@"CreateView");
    
    NSNumber *n = [NSNumber numberWithInt:viewId];
    [self setVid:n];
    
    [self setStoreViewController:[[SKStoreProductViewController alloc] init]];
    
    
    NSMutableDictionary *parameters = [[NSMutableDictionary alloc] init];
   
    
    for (NSString* p in products) {
        NSInteger intVal = [p intValue];
        [parameters setObject:[NSNumber numberWithInt:intVal] forKey:SKStoreProductParameterITunesItemIdentifier];
    }
    
    [self storeViewController].delegate = self;

    [[self storeViewController] loadProductWithParameters:parameters completionBlock:^(BOOL result, NSError *error) {
        if (result) {
            NSLog(@"ok");
            UnitySendMessage("IOSInAppPurchaseManager", "OnProductViewLoaded", [[[self vid] stringValue] UTF8String]);
        } else {
            NSLog(@"no");
            UnitySendMessage("IOSInAppPurchaseManager", "OnProductViewLoadedFailed", [[[self vid] stringValue] UTF8String]);
        }
    }];
    


}

-(void) Show {
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:[self storeViewController]  animated:YES completion:nil];
    
    
}

-(void)productViewControllerDidFinish:(SKStoreProductViewController *)viewController {
    [viewController dismissViewControllerAnimated:YES completion:nil];
    UnitySendMessage("IOSInAppPurchaseManager", "OnProductViewDismissed", [[[self vid] stringValue] UTF8String]);
}



@end

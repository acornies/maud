//
//  StoreProductView.h
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 5/21/14.
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

@interface StoreProductView : NSObject<SKStoreProductViewControllerDelegate>

    @property (strong)  NSNumber *vid;
    @property (strong)  SKStoreProductViewController *storeViewController;

    - (void) CreateView:(int) viewId products: (NSArray *) products;
    - (void) Show;
@end

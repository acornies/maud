//
//  InAppPurchaseManager.h
//
//  Created by Osipov Stanislav on 1/15/13.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

#import "TransactionServer.h"
#import "StoreProductView.h"

#define kInAppPurchaseManagerProductsFetchedNotification @"kInAppPurchaseManagerProductsFetchedNotification"

@interface InAppPurchaseManager : NSObject <SKProductsRequestDelegate, SKRequestDelegate> {
    NSMutableArray * _productIdentifiers;
    NSMutableDictionary * _products;
    TransactionServer * _storeServer;
    
   
}

+ (InAppPurchaseManager *) instance;

- (void) loadStore;
- (void) restorePurchases;
- (void) addProductId:(NSString *) productId;
- (void) buyProduct:(NSString * )productId;

- (void) ShowProductView:(int)viewId;
- (void) CreateProductView:(int) viewId products: (NSArray *) products;


-(void) verifyLastPurchase:(NSString *) verificationURL;


@end

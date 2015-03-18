//
//  TransactionServer.h
//
//  Created by Osipov Stanislav on 1/16/13.
//

#import <Foundation/Foundation.h>
#import "StoreKit/StoreKit.h"
#import "NSData+Base64.h"
#import "ISN_Reachability.h"

@interface TransactionServer : NSObject <SKPaymentTransactionObserver>

-(void) verifyLastPurchase:(NSString *) verificationURL;

@end

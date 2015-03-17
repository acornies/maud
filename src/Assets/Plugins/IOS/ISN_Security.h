//
//  AppEventListener.h
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 5/31/14.
//
//

#import <Foundation/Foundation.h>
#include "ISNDataConvertor.h"
#import "StoreKit/StoreKit.h"


@interface ISN_Security : NSObject <SKRequestDelegate>

+ (id) sharedInstance;

-(void) RetrieveLocalReceipt;
-(void) ReceiptRefreshRequest;
-(void) RetrieveDeviceGUID;


@end

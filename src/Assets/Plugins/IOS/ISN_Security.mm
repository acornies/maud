//
//  AppEventListener.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 5/31/14.
//
//

#import "ISN_Security.h"



@implementation ISN_Security

static ISN_Security *_sharedInstance;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}


- (void) RetrieveLocalReceipt {
    
     NSLog(@"RetrieveLocalRecipe");
    
    NSString *encodedString = @"";
    NSBundle *mainBundle = [NSBundle mainBundle];
    NSURL *receiptURL = [mainBundle appStoreReceiptURL];
    NSError *receiptError;
    BOOL isPresent = [receiptURL checkResourceIsReachableAndReturnError:&receiptError];
    if (isPresent) {
        NSData *receiptData = [NSData dataWithContentsOfURL:receiptURL];
        encodedString = [receiptData base64Encoding];
    }
    
    UnitySendMessage("ISN_Security", "Event_ReceiptLoaded", [ISNDataConvertor NSStringToChar:encodedString]);

}

-(void) ReceiptRefreshRequest {
    NSLog(@"SKReceiptRefreshRequest");
    SKReceiptRefreshRequest *request = [[SKReceiptRefreshRequest alloc] init];
    [request setDelegate:self];
    [request start];
}


-(void) RetrieveDeviceGUID {
    NSUUID *vendorIdentifier = [[UIDevice currentDevice] identifierForVendor];
    uuid_t uuid;
    [vendorIdentifier getUUIDBytes:uuid];
    
    NSData *vendorData = [NSData dataWithBytes:uuid length:16];
    NSString *encodedString = [vendorData base64Encoding];
    UnitySendMessage("ISN_Security", "Event_GUIDLoaded", [ISNDataConvertor NSStringToChar:encodedString]);
}


// SKRequestDelegate

- (void)request:(SKRequest *)request didFailWithError:(NSError *)error {
     UnitySendMessage("ISN_Security", "Event_ReceiptRefreshRequestReceived", [ISNDataConvertor NSStringToChar:@"0"]);
}



- (void)requestDidFinish:(SKRequest *)request {
     UnitySendMessage("ISN_Security", "Event_ReceiptRefreshRequestReceived", [ISNDataConvertor NSStringToChar:@"1"]);
}


extern "C" {
    void _ISN_RetrieveLocalReceipt ()  {
        [[ISN_Security sharedInstance] RetrieveLocalReceipt];
    }
    
    
    void _ISN_RetrieveDeviceGUID ()  {
        [[ISN_Security sharedInstance] RetrieveDeviceGUID];
    }
    
    void _ISN_ReceiptRefreshRequest ()  {
        [[ISN_Security sharedInstance] ReceiptRefreshRequest];
    }
}
@end




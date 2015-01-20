//
//  TransactionServer.m
//
//  Created by Osipov Stanislav on 1/16/13.
//

#import "TransactionServer.h"
#import "ISNDataConvertor.h"


@implementation TransactionServer

NSString* lastTransaction = @"";

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions {
    for (SKPaymentTransaction *transaction in transactions) {
        switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchased:
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self restoreTransaction:transaction];
                break;
            case SKPaymentTransactionStateDeferred:
                [self reportDeferredState:transaction];
                break;
            default:
                break;
        }
    }
}


-(void) verifyLastPurchase:(NSString *)verificationURL {
    NSLog(@"ISN: url: %@",verificationURL);
    
    
    NSURL *url = [NSURL URLWithString:verificationURL];
    NSMutableURLRequest *theRequest = [NSMutableURLRequest requestWithURL:url];
    
   // NSString *st =  lastTransaction;
    
    
    NSString *json = [NSString stringWithFormat:@"{\"receipt-data\":\"%@\"}", lastTransaction];
    
    [theRequest setHTTPBody:[json dataUsingEncoding:NSUTF8StringEncoding]];
    [theRequest setHTTPMethod:@"POST"];
    [theRequest setValue:@"application/x-www-form-urlencoded" forHTTPHeaderField:@"Content-Type"];
    NSString *length = [NSString stringWithFormat:@"%d", [json length]];
    [theRequest setValue:length forHTTPHeaderField:@"Content-Length"];
    NSHTTPURLResponse* urlResponse = nil;
    NSError *error = [[NSError alloc] init];
    NSData *responseData = [NSURLConnection sendSynchronousRequest:theRequest
                                                 returningResponse:&urlResponse
                                                             error:&error];
    NSString *responseString = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
    
  //  NSLog(@"resp: %@",responseString);
    
    NSError *e = nil;
    
    NSDictionary *dic =
    [NSJSONSerialization JSONObjectWithData: [responseString dataUsingEncoding:NSUTF8StringEncoding]
                                    options: NSJSONReadingMutableContainers
                                      error: &e];
    
    NSString *statusCode = [NSString stringWithFormat:@"%d", [[dic objectForKey:@"status"] intValue]];


    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendString:statusCode];
    [data appendString:@"|"];
    [data appendString: responseString];
    [data appendString:@"|"];
    [data appendString: lastTransaction];
    
    NSString *str = [data copy] ;
    #if UNITY_VERSION < 500
    [str autorelease];
    #endif
    
    UnitySendMessage("IOSInAppPurchaseManager", "onVerificationResult", [ISNDataConvertor NSStringToChar:str]);

}



- (NSString *)encodeBase64:(const uint8_t *)input length:(NSInteger)length
{
    NSData * data = [NSData dataWithBytes:input length:length];
    return [data base64EncodedString];
}


- (NSString *)getReceipt:(SKPaymentTransaction *)transaction {
    NSString *Receipt =  [self encodeBase64:(uint8_t *)transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length];
    return Receipt;
}


- (void)reportDeferredState:(SKPaymentTransaction *)transaction {
    NSLog(@"ISN: Transaction  Deferred for: %@", transaction.payment.productIdentifier);

    UnitySendMessage("IOSInAppPurchaseManager", "onProductStateDeferred", [ISNDataConvertor NSStringToChar:transaction.payment.productIdentifier]);
}

- (void)provideContent:(SKPaymentTransaction *)transaction  isRestored:(BOOL)isRestored{
    
    NSLog(@"ISN: provideContent for: %@", transaction.payment.productIdentifier);

    lastTransaction = [self encodeBase64:(uint8_t *)transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length];
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendString:transaction.payment.productIdentifier];
    [data appendString:@"|"];
    if(isRestored) {
        [data appendString:@"0"];
    } else {
        [data appendString:@"1"];
    }
    
    [data appendString:@"|"];
    [data appendString: [self getReceipt:transaction]];
    
    NSString *str = [data copy] ;
    #if UNITY_VERSION < 500
    [str autorelease];
    #endif
    
    
    UnitySendMessage("IOSInAppPurchaseManager", "onProductBought", [ISNDataConvertor NSStringToChar:str]);

    
}





- (void)completeTransaction:(SKPaymentTransaction *)transaction {
    NSLog(@"ISN: completeTransaction...");
    
    
    
    ISN_Reachability* reachability = [ISN_Reachability reachabilityWithHostName:@"www.apple.com"];
    NetworkStatus remoteHostStatus = [reachability currentReachabilityStatus];
    
    if(remoteHostStatus == NotReachable) {
        NSLog(@"ISN: apple.com not reachable, sending tracnsactio finish canseled");
    } else {
        NSLog(@"ISN: apple.com reachable sending tracnsactio finish");
        [self provideContent:transaction isRestored:false];
        [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    }
    
    
   
    
}

- (void)restoreTransaction:(SKPaymentTransaction *)transaction {
    NSLog(@"ISN: restoreTransaction...");
    
   [self provideContent:transaction isRestored:true];
   [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    
}

- (void)failedTransaction:(SKPaymentTransaction *)transaction {
    NSLog(@"ISN: Transaction Failed with code : %i", transaction.error.code);
    NSLog(@"ISN: Transaction error: %@", transaction.error.description);
    
    NSString *erroCode;
    switch (transaction.error.code) {
        case SKErrorClientInvalid:
            erroCode = @"1";
            break;
        case SKErrorPaymentCancelled:
            erroCode = @"2";
            break;
        case SKErrorPaymentInvalid:
            erroCode = @"3";
            break;
        case SKErrorPaymentNotAllowed:
            erroCode = @"4";
            break;
        case SKErrorStoreProductNotAvailable:
            erroCode = @"4";
            break;
        default:
             erroCode = @"0";
    }
    
    


    
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    
    [data appendString:transaction.payment.productIdentifier];
    [data appendString:@"|"];
    
    
    if(transaction.error.localizedDescription != NULL) {
         [data appendString:transaction.error.localizedDescription];
    } else {
        if(transaction.error.description != NULL) {
            [data appendString:transaction.error.description];
        } else {
            [data appendString:@"Unknown Transaction Error"];
        }
    }
    [data appendString:@"|"];
    [data appendString:erroCode];
   
    
    NSString *str = [data copy] ;
    #if UNITY_VERSION < 500
    [str autorelease];
    #endif
    
    UnitySendMessage("IOSInAppPurchaseManager", "onTransactionFailed", [ISNDataConvertor NSStringToChar:str]);
    
    
    
}


- (void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error {
    NSLog(@"ISN: paymentQueue %@",error);
    UnitySendMessage("IOSInAppPurchaseManager", "onRestoreTransactionFailed", [ISNDataConvertor NSStringToChar:@""]);
}

- (void) paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
    NSLog(@"ISN: received restored transactions: %i", queue.transactions.count);
    
    if (queue.transactions.count == 0) {
        NSLog(@"ISN: Nothing to restore, fail event sanded");
        UnitySendMessage("IOSInAppPurchaseManager", "onRestoreTransactionFailed", [ISNDataConvertor NSStringToChar:@""]);
        return;
    }
    
    for (SKPaymentTransaction *transaction in queue.transactions) {
        NSString *productID = transaction.payment.productIdentifier;
        NSLog(@"ISN: restored: %@",productID);
    }
    
    UnitySendMessage("IOSInAppPurchaseManager", "onRestoreTransactionComplete", [ISNDataConvertor NSStringToChar:@""]);
    
}


@end

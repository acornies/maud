//
//  SKProduct+LocalizedPrice.h
//
//  Created by Osipov Stanislav on 1/16/13.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface SKProduct (LocalizedPrice)

@property (nonatomic, readonly) NSString *localizedPrice;

@end

//
//  AppEventListener.h
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 5/31/14.
//
//

#import <Foundation/Foundation.h>
#import "ISNDataConvertor.h"

@interface AppEventListener : NSObject

+ (id) sharedInstance;

-(void) subscribe;


@end

//
//  ISNSharedApplication.h
//  Unity-iPhone
//
//  Created by Lacost on 6/24/14.
//
//

#import <Foundation/Foundation.h>
#include "ISNDataConvertor.h"

@interface ISNSharedApplication : NSObject

+ (id)  sharedInstance;

- (void) checkUrl:(NSString*)url;
- (void) openUrl:(NSString*)url;


@end

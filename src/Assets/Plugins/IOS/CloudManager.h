//
//  CloudManager.h
//  CloudTest
//
//  Created by lacost on 10/2/13.
//  Copyright (c) 2013 cariboo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "ISNDataConvertor.h"

@interface CloudManager : NSObject


 + (id) sharedInstance;

- (void) initialize;
- (void) setString:(NSString*) val key:(NSString*) key;
- (void) setDouble:(double) val key:(NSString*) key;
- (void) setData:(NSData*) val key:(NSString*) key;

 -(void) requestDataForKey:(NSString*) key;

@end

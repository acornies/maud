//
//  CloudManager.m
//  CloudTest
//
//  Created by lacost on 10/2/13.
//  Copyright (c) 2013 cariboo. All rights reserved.
//

#import "CloudManager.h"

@implementation CloudManager

static CloudManager *_sharedInstance;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}


-(void) initialize {
    
    [[NSNotificationCenter defaultCenter]
     addObserver: self
     selector: @selector (iCloudAccountAvailabilityChanged:)
     name: NSUbiquityIdentityDidChangeNotification
     object: nil];
    
    
    
    NSFileManager*  fileManager = [NSFileManager defaultManager];
    id currentToken = [fileManager ubiquityIdentityToken];
    bool isSignedIntoICloud = (currentToken!=nil);

    if(isSignedIntoICloud) {
        NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(storeDidChange:)
                                                     name:NSUbiquitousKeyValueStoreDidChangeExternallyNotification
                                                   object:store];
        [store synchronize];
        
         UnitySendMessage("iCloudManager", "OnCloudInit", [ISNDataConvertor NSStringToChar:@""]);

    } else {
        UnitySendMessage("iCloudManager", "OnCloudInitFail", [ISNDataConvertor NSStringToChar:@""]);
   }
    
    NSLog(@"initialize");
    
}

-(void)setString:(NSString *)val key:(NSString *)key {
    NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
    [store setString:val forKey:key];
    
    [store synchronize];
}

-(void) setData:(NSData *)val key:(NSString *)key {
     NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
    [store setData:val forKey:key];
    
    [store synchronize];
}

-(void) setDouble:(double)val key:(NSString *)key {
    NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
    [store setDouble:val forKey:key];
    
    [store synchronize];
    
}


-(void) requestDataForKey:(NSString *)key {
    NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
    
    id data = [store objectForKey:key];
    
    
    
    NSMutableString * array = [[NSMutableString alloc] init];
    [array appendString:key];
    [array appendString:@"|"];
    

    NSString* stringData;
    
    if(data != nil) {
        if([data isKindOfClass:[NSString class]]) {
            stringData = (NSString*) data;
        }
        
        if([data isKindOfClass:[NSData class]]) {
            
            NSData *b = (NSData*) data;
            
            NSMutableString *str = [[NSMutableString alloc] init];
            const char *db = (const char *) [b bytes];
            for (int i = 0; i < [b length]; i++) {
                if(i != 0) {
                    [str appendFormat:@","];
                }
                
                [str appendFormat:@"%i", (unsigned char)db[i]];
            }
            
            stringData = str;
            
        }
        
        if([data isKindOfClass:[NSNumber class]]) {
            NSNumber* n = (NSNumber*) data;
            stringData = [n stringValue];
        }

    } else {
        stringData = @"null";
    }
   
    
    [array appendString:stringData];
    
    NSLog(@"data: %@", stringData);
    
    
    NSString *package = [[array copy] autorelease];
    
    if(data == nil) {
        UnitySendMessage("iCloudManager", "OnCloudDataEmpty", [ISNDataConvertor NSStringToChar:package]);
    } else {
        UnitySendMessage("iCloudManager", "OnCloudData", [ISNDataConvertor NSStringToChar:package]);

    }
    
   
    
}



- (void)storeDidChange:(NSNotification *)notification {
    UnitySendMessage("iCloudManager", "OnCloudDataChanged", [ISNDataConvertor NSStringToChar:@""]);
}

-(void) iCloudAccountAvailabilityChanged {
    
    NSLog(@"iCloudAccountAvailabilityChanged:");
}

@end



extern "C" {
    void _initCloud ()  {
        [[CloudManager sharedInstance] initialize];
    }
    
    void _setString(char* key, char* val) {
        NSString* k = [ISNDataConvertor charToNSString:key];
        NSString* v = [ISNDataConvertor charToNSString:val];
        
        [[CloudManager sharedInstance] setString:v key:k];
    }
    
    
    void _setDouble(char* key, float val) {
        NSString* k = [ISNDataConvertor charToNSString:key];
        double v = (double) val;
        
        [[CloudManager sharedInstance] setDouble:v key:k];
    }
    
    void _setData(char* key, char* val) {
        NSString* k = [ISNDataConvertor charToNSString:key];
        NSString* v = [ISNDataConvertor charToNSString:val];
        
        NSArray *bytes = [v componentsSeparatedByString:@","];
        
        
        NSMutableData* d = [[NSMutableData alloc] init];
        for(NSString* s in bytes) {
            int v = [s intValue];
            char * c = (char*)(&v);
            [d appendBytes:c length:1];
            
        }
        
        [[CloudManager sharedInstance] setData:d key:k];
        
    }
    
    
    void _requestDataForKey(char* key) {
        NSString* k = [ISNDataConvertor charToNSString:key];
        [[CloudManager sharedInstance] requestDataForKey:k];
    }
    
    
}

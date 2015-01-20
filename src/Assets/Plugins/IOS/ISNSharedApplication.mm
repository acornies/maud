//
//  ISNSharedApplication.m
//  Unity-iPhone
//
//  Created by Lacost on 6/24/14.
//
//

#import "ISNSharedApplication.h"

@implementation ISNSharedApplication

static ISNSharedApplication *_sharedInstance;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}

- (void) checkUrl:(NSString *)url {
    NSURL *uri = [NSURL URLWithString:url];
    BOOL canOpenURL = [[UIApplication sharedApplication] canOpenURL:uri];
    
    if(canOpenURL) {
        UnitySendMessage("IOSSharedApplication", "UrlCheckSuccess", [ISNDataConvertor NSStringToChar:url]);
    } else {
        UnitySendMessage("IOSSharedApplication", "UrlCheckFailed", [ISNDataConvertor NSStringToChar:url]);
    }
    
}

-(void) openUrl:(NSString *)url {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:url]];
}



extern "C" {
    
    
    //--------------------------------------
	//  IOS Native Plugin Section
	//--------------------------------------
    
  
    
    void _ISN_checkUrl(char* url) {
        NSString *uri = [ISNDataConvertor charToNSString:url];
        [[ISNSharedApplication sharedInstance] checkUrl:uri];
    }
    
    void _ISN_openUrl(char* url) {
        NSString *uri = [ISNDataConvertor charToNSString:url];
        [[ISNSharedApplication sharedInstance] openUrl:uri];
    }
    
}



@end

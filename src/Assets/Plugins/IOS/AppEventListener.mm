//
//  AppEventListener.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 5/31/14.
//
//

#import "AppEventListener.h"

@implementation AppEventListener

static AppEventListener *_sharedInstance;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}


- (void) subscribe {
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidBecomeActive:)   name:UIApplicationDidBecomeActiveNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationWillResignActive:) name:UIApplicationWillResignActiveNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidEnterBackground:) name:UIApplicationDidEnterBackgroundNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationWillTerminate:) name:UIApplicationWillTerminateNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidReceiveMemoryWarning:) name:UIApplicationDidReceiveMemoryWarningNotification object:nil];

}


+ (void) sendEvent: (NSString* ) event {
    UnitySendMessage("IOSNativeAppEvents", [ISNDataConvertor NSStringToChar:event], [ISNDataConvertor NSStringToChar:@"null"]);
}


- (void)applicationDidBecomeActive:(NSNotification *)notification {
    [AppEventListener sendEvent:@"applicationDidBecomeActive"];
}


- (void) applicationWillResignActive:(NSNotification *)notification {
    [AppEventListener sendEvent:@"applicationWillResignActive"];
}

- (void) applicationDidEnterBackground:(NSNotification *)notification {
    [AppEventListener sendEvent:@"applicationDidEnterBackground"];
}

- (void) applicationWillTerminate:(NSNotification *)notification {
    [AppEventListener sendEvent:@"applicationWillTerminate"];
}

- (void) applicationDidReceiveMemoryWarning:(NSNotification *)notification {
    [AppEventListener sendEvent:@"applicationDidReceiveMemoryWarning"];
}


extern "C" {
    void _ISNsubscribe ()  {
        [[AppEventListener sharedInstance] subscribe];
    }
}
@end




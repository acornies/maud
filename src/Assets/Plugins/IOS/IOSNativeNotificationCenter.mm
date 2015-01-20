//
//  IOSNativeNotificationCenter.m
//  Unity-iPhone
//
//  Created by lacost on 11/9/13.
//
//

#import "ISNDataConvertor.h"
#import "IOSNativeNotificationCenter.h"

@implementation IOSNativeNotificationCenter


static IOSNativeNotificationCenter *sharedHelper = nil;

+ (IOSNativeNotificationCenter *) sharedInstance {
    if (!sharedHelper) {
        sharedHelper = [[IOSNativeNotificationCenter alloc] init];
        
    }
    return sharedHelper;
}

- (void) RegisterForNotifications {
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        [[UIApplication sharedApplication] registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert|UIUserNotificationTypeBadge|UIUserNotificationTypeSound categories:nil]];

    }
}


-(void) scheduleNotification:(int)time message:(NSString *)messgae sound:(bool *)sound alarmID:(NSString *)alarmID badges:(int)badges {
    
    
    
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        UIUserNotificationSettings* NotificationSettings = [[UIApplication sharedApplication] currentUserNotificationSettings];
        
        NSLog(@"IOS 8 detected");
 
        if((NotificationSettings.types & UIUserNotificationTypeAlert) == 0) {
            NSLog(@"ISN: user disabled local notification for this app, sending fail event");
            
            NSMutableString * data = [[NSMutableString alloc] init];
            [data appendString: @"0" ];
            [data appendString:@"|"];
            [data appendString:  [NSString stringWithFormat:@"%u",[[UIApplication sharedApplication] currentUserNotificationSettings].types]];
            
            UnitySendMessage("IOSNotificationController", "OnNotificationScheduleResultAction", [ISNDataConvertor NSStringToChar:data]);
            
            [self RegisterForNotifications];
            return;
        }
        
        if((NotificationSettings.types & UIUserNotificationTypeBadge) == 0) {
           
            if(badges > 0) {
                 NSLog(@"ISN: no bages allowed for this user. notification bage disabled ");
                 badges = 0;
            }
           
            
        }
        NSLog(@"ISN: sss %u", NotificationSettings.types & UIUserNotificationTypeSound);
        
        if((NotificationSettings.types & UIUserNotificationTypeSound) == 0) {
            if(sound) {
                NSLog(@"ISN: no sound allowed for this user. notification sound disabled ");
                 #if UNITY_VERSION < 500
                sound = false;
                #endif
            }

        }
    }
   
    
    UILocalNotification* localNotification = [[UILocalNotification alloc] init];
    localNotification.fireDate = [NSDate dateWithTimeIntervalSinceNow:time];
    localNotification.alertBody = messgae;
    localNotification.timeZone = [NSTimeZone defaultTimeZone];
   
    if (badges > 0)
        localNotification.applicationIconBadgeNumber = badges;
    
    if(sound) {
        localNotification.soundName = UILocalNotificationDefaultSoundName;
    }
    
  
    
    NSMutableDictionary *userInfo = [NSMutableDictionary dictionary];
    [userInfo setObject:alarmID forKey:@"AlarmKey"];
    // Set some extra info to your alarm
    localNotification.userInfo = userInfo;
    
    NSLog(@"ISN: scheduleNotification AlarmKey: %@", alarmID);
    
    [[UIApplication sharedApplication] scheduleLocalNotification:localNotification];
    
    
     NSMutableString * data = [[NSMutableString alloc] init];
    [data appendString: @"1" ];
    [data appendString:@"|"];
    
     if ([[vComp objectAtIndex:0] intValue] >= 8) {
         [data appendString:  [NSString stringWithFormat:@"%u",[[UIApplication sharedApplication] currentUserNotificationSettings].types]];
     } else {
         [data appendString:@"7"];
     }
    
    
     UnitySendMessage("IOSNotificationController", "OnNotificationScheduleResultAction", [ISNDataConvertor NSStringToChar:data]);

    
}

- (UILocalNotification *)existingNotificationWithAlarmID:(NSString *)alarmID {
    for (UILocalNotification *notification in [[UIApplication sharedApplication] scheduledLocalNotifications]) {
        if ([[notification.userInfo objectForKey:@"AlarmKey"] isEqualToString:alarmID]) {
            return notification;
        }
    }
    
    return nil;
}

- (void)cleanUpLocalNotificationWithAlarmID:(NSString *)alarmID {
    NSLog(@"cleanUpLocalNotificationWithAlarmID AlarmKey: %@", alarmID);
    
    UILocalNotification *notification = [self existingNotificationWithAlarmID:alarmID];
    if (notification) {
          NSLog(@"notification canceled");
        [[UIApplication sharedApplication] cancelLocalNotification:notification];
    }
}




-(void) showNotificationBanner:(NSString *)title message:(NSString *)message {
    [GKNotificationBanner showBannerWithTitle:title message:message completionHandler:^{}];
}

- (void) cancelNotifications {
    [[UIApplication sharedApplication] cancelAllLocalNotifications];
}

- (void) applicationIconBadgeNumber:(int) badges {
    [UIApplication sharedApplication].applicationIconBadgeNumber = badges;
}


@end

extern "C" {
    
    void _ISN_CancelNotifications() {
        [[IOSNativeNotificationCenter sharedInstance] cancelNotifications];
    }
    
    
    void _ISN_CancelNotificationById(char* nId) {
        NSString* alarmID = [ISNDataConvertor charToNSString:nId];
        [[IOSNativeNotificationCenter sharedInstance] cleanUpLocalNotificationWithAlarmID:alarmID];
    }
    
    void  _ISN_RequestNotificationPermitions ()  {
        [[IOSNativeNotificationCenter sharedInstance] RegisterForNotifications];
    }
    
    
    void  _ISN_ScheduleNotification (int time, char* messgae, bool* sound, char* nId, int badges)  {
        NSString* alarmID = [ISNDataConvertor charToNSString:nId];
        [[IOSNativeNotificationCenter sharedInstance] scheduleNotification:time message:[ISNDataConvertor charToNSString:messgae] sound:sound alarmID:alarmID badges:badges];
    }
    
    void _ISN_ShowNotificationBanner (char* title, char* messgae)  {
        [[IOSNativeNotificationCenter sharedInstance] showNotificationBanner:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:messgae]];
    }
    void _ISN_ApplicationIconBadgeNumber (int badges) {
        [[IOSNativeNotificationCenter sharedInstance] applicationIconBadgeNumber:badges];
    }
    
    
    void _ISN_RegisterForRemoteNotifications(int types) {
          NSLog(@"_ISN_RegisterForRemoteNotifications");
       
       // UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:types categories:Nil];
       // [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
        
        //[[UIApplication sharedApplication] registerForRemoteNotifications];
        
        
        UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert |  UIUserNotificationTypeBadge | UIUserNotificationTypeSound categories:nil];
        [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
        [[UIApplication sharedApplication] registerForRemoteNotifications];
        
        
    }
}

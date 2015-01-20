//
//  IOSNativeNotificationCenter.h
//  Unity-iPhone
//
//  Created by lacost on 11/9/13.
//
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

@interface IOSNativeNotificationCenter : NSObject


+ (IOSNativeNotificationCenter *)sharedInstance;
- (void) scheduleNotification: (int) time message: (NSString*) messgae sound: (bool *)sound alarmID:(NSString *)alarmID badges: (int)badges;
- (void) cleanUpLocalNotificationWithAlarmID: (NSString *)alarmID;
- (void) showNotificationBanner: (NSString*) title message: (NSString*) message ;
- (void) cancelNotifications;
- (void) applicationIconBadgeNumber: (int)badges;
- (void) RegisterForNotifications;

@end

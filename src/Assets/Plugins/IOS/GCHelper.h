//
//  GCHelper.h
//  CatRace
//
//  Created by Ray Wenderlich on 4/23/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "ISNDataConvertor.h"

@protocol GCHelperDelegate
- (void)matchStarted;
- (void)matchEnded;
- (void)match:(GKMatch *)match didReceiveData:(NSData *)data fromPlayer:(NSString *)playerID;
@end

@interface GCHelper : NSObject <GKMatchmakerViewControllerDelegate, GKMatchDelegate> {
    BOOL isInited;
    
    UIView *currentView;
    GKMatch *match;
    GKTurnBasedMatch* TBMatch;
}


@property (retain) UIViewController *presentingViewController;
@property (retain) GKMatch *match;
@property (retain) GKTurnBasedMatch *TBMatch;

+ (GCHelper *)sharedInstance;

- (void) initNotificationHandler;
- (void) disconnect;


- (void) sendDataBytes: (NSData*) data requestType: (int) requestType;
- (void) sendDataBytes: (NSData*) data toPlayers:(NSArray*) toPlayers requestType:(int) requestType;

- (void)findMatchWithMinPlayers:(int)minPlayers maxPlayers:(int)maxPlayers playerGroup:(int)playerGroup;

@end

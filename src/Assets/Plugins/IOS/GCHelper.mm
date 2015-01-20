
//  GCHelper.m
//  CatRace
//
//  Created by Ray Wenderlich on 4/23/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "GCHelper.h"

@implementation GCHelper

@synthesize presentingViewController;
@synthesize match;
@synthesize TBMatch;

#pragma mark Initialization

static GCHelper *sharedHelper = nil;
+ (GCHelper *) sharedInstance {
    if (!sharedHelper) {
        sharedHelper = [[GCHelper alloc] init];
        
    }
    return sharedHelper;
}



- (id)init {
    if ((self = [super init])) {
        isInited = false;
    }
    return self;
}



#pragma mark User functions



-(void) initNotificationHandler {
    if(isInited) {
        return;
    }
    
    isInited = TRUE;
    
    NSLog(@"initNotificationHandler");
    [GKMatchmaker sharedMatchmaker].inviteHandler = ^(GKInvite *acceptedInvite, NSArray *playersToInvite) {
        // Insert game-specific code here to clean up any game in progress.
        
        
        if (acceptedInvite)
        {
            GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithInvite:acceptedInvite];
            mmvc.matchmakerDelegate = self;
            [self startMatchViewController:mmvc];
        }
        else if (playersToInvite)
        {
            GKMatchRequest *request = [[GKMatchRequest alloc] init];
            request.minPlayers = 2;
            request.maxPlayers = 2;
            request.playersToInvite = playersToInvite;
            
            GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithMatchRequest:request];
            mmvc.matchmakerDelegate = self;
            [self startMatchViewController:mmvc];
        }
    };
    
}

- (void)startMatchViewController:(GKMatchmakerViewController*) mmvc   {
    self.match = nil;
    
    CGSize screenSize = [[UIScreen mainScreen] bounds].size;
    UIWindow* window = [UIApplication sharedApplication].keyWindow;
    
    
    UIViewController *glView2 = [[UIViewController alloc] init];
    currentView = glView2.view;
    [window addSubview: glView2.view];
    [glView2 presentViewController: mmvc animated: YES completion:nil];
    
    
    mmvc.view.transform = CGAffineTransformMakeRotation(0.0f);
    [mmvc.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
    mmvc.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
    
    
}

- (void)findMatchWithMinPlayers:(int)minPlayers maxPlayers:(int)maxPlayers playerGroup:(int)playerGroup {
    
    
    NSLog(@"findMatchWithMinPlayers");
    
    GKMatchRequest *request = [[GKMatchRequest alloc] init];
    request.minPlayers = minPlayers;
    request.maxPlayers = maxPlayers;
    request.playerGroup = playerGroup;
    
    GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithMatchRequest:request];
    mmvc.matchmakerDelegate = self;
    
    [self startMatchViewController:mmvc];
    
}

/*
 
 -(void) findTurnBasedMatchWithMinPlayers:(int)minPlayers maxPlayers:(int)maxPlayers {
 GKMatchRequest *request = [[GKMatchRequest alloc] init];
 request.minPlayers = 2;
 request.maxPlayers = 2;
 
 GKTurnBasedMatchmakerViewController *mmvc = [[GKTurnBasedMatchmakerViewController alloc] initWithMatchRequest:request];
 mmvc.turnBasedMatchmakerDelegate = self;
 
 [self startTurnBasedMutchViewController:mmvc];
 
 }
 
 
 - (void)startTurnBasedMutchViewController:(GKTurnBasedMatchmakerViewController*) mmvc   {
 self.match = nil;
 
 CGSize screenSize = [[UIScreen mainScreen] bounds].size;
 UIWindow* window = [UIApplication sharedApplication].keyWindow;
 
 
 UIViewController *glView2 = [[UIViewController alloc] init];
 currentView = glView2.view;
 [window addSubview: glView2.view];
 [glView2 presentViewController: mmvc animated: YES completion:nil];
 
 
 mmvc.view.transform = CGAffineTransformMakeRotation(0.0f);
 [mmvc.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
 mmvc.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
 }
 
 
 
 */

-(void) dismissView: (GKMatchmakerViewController *)vc {
    if(currentView != NULL) {
        [currentView removeFromSuperview];
    }
    
    [vc dismissViewControllerAnimated:YES completion:nil];
    [vc.view.superview removeFromSuperview];
    [vc release];
}


-(void) dismissViewTB: (GKTurnBasedMatchmakerViewController *)vc {
    if(currentView != NULL) {
        [currentView removeFromSuperview];
    }
    
    [vc dismissViewControllerAnimated:YES completion:nil];
    [vc.view.superview removeFromSuperview];
    [vc release];
}


-(void)sendDataBytes:(NSData *)data requestType:(int)requestType {
    
    NSError *error;
    
    if(requestType == 0) {
        [match sendDataToAllPlayers: data withDataMode: GKMatchSendDataReliable error:&error];
    } else {
        [match sendDataToAllPlayers: data withDataMode: GKMatchSendDataUnreliable error:&error];
    }
    
    if (error != nil) {
        [self HandleSendError:error];
    }
}


- (void) sendDataBytes:(NSData *)data toPlayers:(NSArray *)toPlayers requestType:(int)requestType {
    
    
    NSError *error;
    
    if(requestType == 0) {
        [match sendData:data toPlayers:toPlayers withDataMode:GKMatchSendDataReliable error:&error];
    } else {
        [match sendData:data toPlayers:toPlayers withDataMode:GKMatchSendDataUnreliable error:&error];
    }
    
    
    if (error != nil) {
        [self HandleSendError:error];
    }
}


-(void) HandleSendError : (NSError *) error {
    // Handle the error.
}

-(void) StartMatch : (GKMatch*) theMatch {
    self.match = theMatch;
    match.delegate = self;
    if (match.expectedPlayerCount == 0) {
        
        NSLog(@"Ready to start match!");
        
        NSMutableString * array = [[NSMutableString alloc] init];
        
        for (NSString* playerID in theMatch.playerIDs) {
            [array appendString:playerID];
            [array appendString:@"|"];
        }
        
        
        [array appendString:@"endofline"];
        
        NSString *data = [[array copy] autorelease];
        UnitySendMessage("GameCenterMultiplayer", "OnGameCenterMatchStarted", [ISNDataConvertor NSStringToChar:data]);
    }
}

-(void) StartTBMatch : (GKTurnBasedMatch*) theMatch {
    
    self.TBMatch = theMatch;
    
    match.delegate = self;
    if (match.expectedPlayerCount == 0) {
        
        NSLog(@"Ready to start tb match!");
        

    }
}

-(void) disconnect {
    if([self match] != nil) {
        [[self match] disconnect];
    }
}

#pragma mark GKMatchmakerViewControllerDelegate

- (void)matchmakerViewControllerWasCancelled:(GKMatchmakerViewController *)viewController {
    [self dismissView:viewController];
}

- (void)matchmakerViewController:(GKMatchmakerViewController *)viewController didFailWithError:(NSError *)error {
    [self dismissView:viewController];
}


- (void)matchmakerViewController:(GKMatchmakerViewController *)viewController didFindMatch:(GKMatch *)theMatch {
    [self dismissView:viewController];
    [self StartMatch:theMatch];
}



#pragma mark GKTurnBasedMatchmakerViewController

/*
 - (void)turnBasedMatchmakerViewControllerWasCancelled:(GKTurnBasedMatchmakerViewController *)viewController {
 [self dismissViewTB:viewController];
 }
 
 
 - (void)turnBasedMatchmakerViewController:(GKTurnBasedMatchmakerViewController *)viewController didFailWithError:(NSError *)error {
 [self dismissViewTB:viewController];
 }
 
 - (void)turnBasedMatchmakerViewController:(GKTurnBasedMatchmakerViewController *)viewController didFindMatch:(GKTurnBasedMatch *)theMatch {
 [self dismissViewTB:viewController];
 [self StartTBMatch:theMatch];
 }
 */


#pragma mark GKMatchDelegate

// The match received data sent from the player.
- (void)match:(GKMatch *)theMatch didReceiveData:(NSData *)data fromPlayer:(NSString *)playerID {
    if (match != theMatch) return;
    
    
    NSMutableString *str = [[NSMutableString alloc] init];
    const char *db = (const char *) [data bytes];
    for (int i = 0; i < [data length]; i++) {
        if(i != 0) {
            [str appendFormat:@","];
        }
        
        [str appendFormat:@"%i", (unsigned char)db[i]];
    }
    
    
    
    NSMutableString * array = [[NSMutableString alloc] init];
    [array appendString:playerID];
    [array appendString:@"| "];
    [array appendString:str];
    
    
    NSString *package = [[array copy] autorelease];
    
    
    UnitySendMessage("GameCenterMultiplayer", "OnMatchDataReceived", [ISNDataConvertor NSStringToChar:package]);
    
}


// The player state changed (eg. connected or disconnected)
- (void)match:(GKMatch *)theMatch player:(NSString *)playerID didChangeState:(GKPlayerConnectionState)state {
    
    if (match != theMatch) return;
    
    switch (state) {
        case GKPlayerStateConnected:
            // handle a new player connection.
            NSLog(@"Player connected!");
            
            UnitySendMessage("GameCenterMultiplayer", "OnGameCenterPlayerConnected", [ISNDataConvertor NSStringToChar:playerID]);
            
            
            if (theMatch.expectedPlayerCount == 0) {
                [self StartMatch:theMatch];
            }
            
            break;
        case GKPlayerStateDisconnected:
            // a player just disconnected.
            NSLog(@"Player disconnected!");
            UnitySendMessage("GameCenterMultiplayer", "OnGameCenterPlayerDisconnected", [ISNDataConvertor NSStringToChar:playerID]);
            
            
            break;
    }
    
}


@end






extern "C" {
    
    
    void _findMatch (int minPlayers, int maxPlayers, int playerGroup) {
        [[GCHelper sharedInstance] findMatchWithMinPlayers:minPlayers maxPlayers:maxPlayers playerGroup:playerGroup];
    }
    
    
    
    void _sendDataToAll(char* data, int requestType) {
        
        NSString* str = [ISNDataConvertor charToNSString:data];
        NSArray *bytes = [str componentsSeparatedByString:@","];
        
        
        NSMutableData* d = [[NSMutableData alloc] init];
        for(NSString* s in bytes) {
            int v = [s intValue];
            char * c = (char*)(&v);
            [d appendBytes:c length:1];
            
        }
        
        
        [[GCHelper sharedInstance] sendDataBytes:d requestType:requestType];
    }
    
    
    
    void _sendDataToPlayers(char* data, char * playersID, int requestType) {
        
        NSString* str = [ISNDataConvertor charToNSString:playersID];
        NSArray *players = [str componentsSeparatedByString:@","];
        
        
        NSString* data_str = [ISNDataConvertor charToNSString:data];
        NSArray *bytes = [data_str componentsSeparatedByString:@","];
        
        
        NSMutableData* d = [[NSMutableData alloc] init];
        for(NSString* s in bytes) {
            int v = [s intValue];
            char * c = (char*)(&v);
            [d appendBytes:c length:1];
            
        }
        
        
        [[GCHelper sharedInstance] sendDataBytes:d toPlayers:players requestType:requestType];
    }
    
    void _disconnect() {
        [[GCHelper sharedInstance] disconnect];
    }
    
}

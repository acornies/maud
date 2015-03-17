#import "IOSGameCenterManager.h"
#import "ISNDataConvertor.h"

@implementation IOSGameCenterManager

static NSMutableArray *loadedPlayersIds;

- (id)init {
    self = [super init];
    if (self) {
        requestedLeaderboardId = NULL;
        isAchievementsWasLoaded = FALSE;
        loadedPlayersIds = [[NSMutableArray alloc] init];
        #if UNITY_VERSION < 500
        [loadedPlayersIds retain];
        #endif
        gameCenterManager = [[GameCenterManager alloc] init];
        NSLog(@"IOSGameCenterManager initialized");
    }
    
    return self;
}



- (void)dealloc {
      #if UNITY_VERSION < 500
    [super dealloc];
     #endif
}

-(void) getSignature {
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    [localPlayer generateIdentityVerificationSignatureWithCompletionHandler:^(NSURL *publicKeyUrl, NSData *signature, NSData *salt, uint64_t timestamp, NSError *error) {
        NSLog(@"generateIdentityVerificationSignatureWithCompletionHandler");
        
        if(error != nil) {
            
            NSLog(@"error: %@", error.description);

            NSMutableString * ErrorData = [[NSMutableString alloc] init];
            [ErrorData appendFormat:@"%i", error.code];
            [ErrorData appendString:@"| "];
            [ErrorData appendString:error.description];

            
            
            
             NSString *ErrorDataString = [ErrorData copy];
             #if UNITY_VERSION < 500
                [ErrorDataString autorelease];
             #endif
             UnitySendMessage("GameCenterManager", "VerificationSignatureRetrieveFailed", [ISNDataConvertor NSStringToChar:ErrorDataString]);
             return;
        }
        
        NSMutableString *sig = [[NSMutableString alloc] init];
        const char *db = (const char *) [signature bytes];
        for (int i = 0; i < [signature length]; i++) {
            if(i != 0) {
                [sig appendFormat:@","];
            }
            
            [sig appendFormat:@"%i", (unsigned char)db[i]];
        }
        
        
        NSMutableString *slt = [[NSMutableString alloc] init];
        const char *db2 = (const char *) [salt bytes];
        for (int i = 0; i < [signature length]; i++) {
            if(i != 0) {
                [slt appendFormat:@","];
            }
            
            [slt appendFormat:@"%i", (unsigned char)db2[i]];
        }

        
        
       
        
        NSString *path = [[NSString alloc] initWithString:[publicKeyUrl path]];
        NSLog(@"publicKeyUrl: %@", path);
        
        

        
        
        NSMutableString * array = [[NSMutableString alloc] init];
        [array appendString:path];
        [array appendString:@"| "];
        [array appendString:sig];
        [array appendString:@"| "];
        [array appendString:slt];
        [array appendString:@"| "];
        [array appendFormat:@"%llu", timestamp];
        
        
         NSString *str = [array copy];
#if UNITY_VERSION < 500
        [str autorelease];
#endif
        UnitySendMessage("GameCenterManager", "VerificationSignatureRetrieved", [ISNDataConvertor NSStringToChar:str]);
        
        
        
    }];
}




- (void) reportScore: (long long) score forCategory: (NSString*) category {
    NSLog(@"reportScore: %lld", score);
    NSLog(@"category %@", category);
    

    
    GKScore *scoreReporter = [[GKScore alloc] initWithCategory:category] ;
#if UNITY_VERSION < 500
    [scoreReporter autorelease];
#endif
    scoreReporter.value = score;
    
    [scoreReporter reportScoreWithCompletionHandler: ^(NSError *error) {
        if (error != nil) {
            UnitySendMessage("GameCenterManager", "onScoreSubmittedFailed", [ISNDataConvertor NSStringToChar:@""]);
            NSLog(@"Error while submitting score: %@", error.description);
        } else {
            NSLog(@"new high score sumbitted success: %lld", score);
            
            
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            
            [data appendString:category];
            [data appendString:@","];
            [data appendString:[NSString stringWithFormat: @"%lld", score]];
 
    
            NSString *str = [data copy];
            
#if UNITY_VERSION < 500
            [str autorelease];
#endif
            
            UnitySendMessage("GameCenterManager", "onScoreSubmittedEvent", [ISNDataConvertor NSStringToChar:str]);
            
           
        }
        
    }];
}


-(void) resetAchievements {
    [gameCenterManager resetAchievements];
}



-(void) submitAchievement:(double)percentComplete identifier:(NSString *)identifier  notifyComplete: (BOOL) notifyComplete {
    [gameCenterManager submitAchievement:identifier percentComplete:percentComplete notifyComplete:notifyComplete];
}




-(void) showLeaderboard:(NSString *)leaderboardId scope:(int)scope {
    
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    if(localPlayer.isAuthenticated) {

        requestedLeaderboardId = leaderboardId;
        
#if UNITY_VERSION < 500
        [requestedLeaderboardId retain];
#endif


        lbscope = scope;
        [self showLeaderboardPopUp];
    } else {
        NSLog(@"ISN showLeaderboard requires player to be authenticated.  Call ignored");
    }
}



- (void) showAchievements {
    
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    if(localPlayer.isAuthenticated) {
        [self showAchievementsPopUp];
    } else {
       NSLog(@"ISN showAchievements requires player to be authenticated.  Call ignored");
    }
}

- (void) showAchievementsPopUp {
    
    GKAchievementViewController *achievements = [[GKAchievementViewController alloc] init];
    if(achievements != NULL) {
        
        achievements.achievementDelegate = self;
        
        CGSize screenSize = [[UIScreen mainScreen] bounds].size;
        
        UIViewController *vc =  UnityGetGLViewController();
        
        [vc presentViewController: achievements animated: YES completion:nil];
        
        achievements.view.transform = CGAffineTransformMakeRotation(0.0f);
        [achievements.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
        achievements.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
        
    }
}

- (void) showLeaderboardsPopUp {
    GKGameCenterViewController *leaderboardController = [[GKGameCenterViewController alloc] init];
    
    
    NSLog(@"showLeaderboardsPopUp");
    leaderboardController.gameCenterDelegate = self;
    
    
    CGSize screenSize = [[UIScreen mainScreen] bounds].size;

    UIViewController *vc =  UnityGetGLViewController();

    [vc presentViewController: leaderboardController animated: YES completion:nil];
    leaderboardController.view.transform = CGAffineTransformMakeRotation(0.0f);
    [leaderboardController.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
    leaderboardController.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
    
    
}

- (void) showLeaderboardPopUp {
    
    GKGameCenterViewController *leaderboardController = [[GKGameCenterViewController alloc] init];
    if (leaderboardController != NULL)
    {
        NSLog(@"requested LB: %@", requestedLeaderboardId);
        
        leaderboardController.leaderboardCategory = requestedLeaderboardId;
        
        switch (lbscope) {
            case 2:
                leaderboardController.leaderboardTimeScope = GKLeaderboardTimeScopeAllTime;
                break;
            case 1:
                leaderboardController.leaderboardTimeScope = GKLeaderboardTimeScopeWeek;
                break;
            case 0:
                leaderboardController.leaderboardTimeScope = GKLeaderboardTimeScopeToday;
                break;
                
            default:
                leaderboardController.leaderboardTimeScope = GKLeaderboardTimeScopeAllTime;
                break;
        }
       
        leaderboardController.gameCenterDelegate = self;
        
        
        CGSize screenSize = [[UIScreen mainScreen] bounds].size;
        
        
        UIViewController *vc =  UnityGetGLViewController();
        
        [vc presentViewController: leaderboardController animated: YES completion:nil];
        leaderboardController.view.transform = CGAffineTransformMakeRotation(0.0f);
        [leaderboardController.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
        leaderboardController.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);

#if UNITY_VERSION < 500
        [requestedLeaderboardId release];
#endif

        
    }
    
    requestedLeaderboardId = NULL;
}


- (void)gameCenterViewControllerDidFinish:(GKGameCenterViewController *)vc {
    [self dismissGameCenterView:vc];
}


- (void)achievementViewControllerDidFinish:(GKAchievementViewController *)vc; {
    [self dismissGameCenterView:vc];
}


-(void) dismissGameCenterView: (GKGameCenterViewController *)vc {
    
    if (![vc isBeingPresented] && ![vc isBeingDismissed]) {
        [vc dismissViewControllerAnimated:YES completion:nil];
        [vc.view.superview removeFromSuperview];
    }
    
#if UNITY_VERSION < 500
    [vc release];
#endif
    
    
    UnitySendMessage("GameCenterManager", "OnGameCenterViewDismissed", [ISNDataConvertor NSStringToChar:@""]);
}


- (void) authenticateLocalPlayer {
    NSLog(@"authenticateLocalPlayer call");
    
    if([self isGameCenterAvailable]){
        GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
        
        if(localPlayer.authenticated == NO) {
           
           
            // [localPlayer setAuthenticateHandler:^(UIViewController *viewcontroller, NSError *error) {
            [localPlayer authenticateWithCompletionHandler:^(NSError *error){ //OLD Code
                if (localPlayer.isAuthenticated){
                    NSLog(@"PLAYER AUTHENTICATED");
                    
                    NSMutableString * data = [[NSMutableString alloc] init];
                    
                    if(localPlayer.playerID != nil) {
                         [data appendString:localPlayer.playerID];
                    } else {
                         [data appendString:@""];
                    }
                    [data appendString:@","];
                    
                    
                    if(localPlayer.displayName != nil) {
                        [data appendString:localPlayer.displayName];
                    } else {
                        [data appendString:@""];
                    }
                    [data appendString:@","];
                    
                    
                    if(localPlayer.alias != nil) {
                        [data appendString:localPlayer.alias];
                    } else {
                        [data appendString:@""];
                    }
                    
                
                    
                    NSString *str = [data copy];
                    
#if UNITY_VERSION < 500
                    [str autorelease];
#endif
                    
                    
                    UnitySendMessage("GameCenterManager", "onAuthenticateLocalPlayer", [ISNDataConvertor NSStringToChar:str]);
                    
                    NSLog(@"PLAYER AUTHENTICATED %@", localPlayer.alias);
                    
                    [[GCHelper sharedInstance] initNotificationHandler];
                    
                    if(!isAchievementsWasLoaded) {
                        [self loadAchievements];
                    }
                    
                    //loadin more info (avatar);
                    [self loadUserData:localPlayer.playerID];
                    
                } else {
                   
                    NSLog(@"Error: %@", error);
                    if(error != nil) {
                        NSLog(@"Error descr: %@", error.description);
                    }
                    NSLog(@"PLAYER NOT AUTHENTICATED");
                    UnitySendMessage("GameCenterManager", "onAuthenticationFailed", [ISNDataConvertor NSStringToChar:@""]);
                }
            }];
        } else {
            NSLog(@"Do nothing - Player is already authenticated");
        }
    }
}
- (void) loadAchievements {
    [GKAchievement loadAchievementsWithCompletionHandler:^(NSArray *achievements, NSError *error) {
        if (error == nil) {
            NSLog(@"loadAchievementsWithCompletionHandler");
            NSLog(@"count %lu", (unsigned long)achievements.count);
            
            
            isAchievementsWasLoaded = TRUE;
            NSMutableString * data = [[NSMutableString alloc] init];
            BOOL first = YES;
            for (GKAchievement* achievement in achievements) {
                
                
                if(!first) {
                    [data appendString:@","];
                }
                
                [data appendString:achievement.identifier];
                [data appendString:@","];
                
                NSLog(@"achievement.percentComplete:  %f", achievement.percentComplete);
                
                [data appendString:[NSString stringWithFormat:@"%f", achievement.percentComplete]];
                
                first = NO;
                
            }
            
            NSString *str = [data copy];
            
#if UNITY_VERSION < 500
            [str autorelease];
#endif
            
            UnitySendMessage("GameCenterManager", "onAchievementsLoaded", [ISNDataConvertor NSStringToChar:str]);
        } else {
              NSLog(@"loadAchievements failed:  %@", error.description);
             UnitySendMessage("GameCenterManager", "onAchievementsLoadedFailed", "");
        }
    }];
}


- (BOOL)isGameCenterAvailable {
    BOOL localPlayerClassAvailable = (NSClassFromString(@"GKLocalPlayer")) != nil;
    NSString *reqSysVer = @"4.1";
    NSString *currSysVer = [[UIDevice currentDevice] systemVersion];
    BOOL osVersionSupported = ([currSysVer compare:reqSysVer options:NSNumericSearch] != NSOrderedAscending);
    return (localPlayerClassAvailable && osVersionSupported);
}

-(void) retriveScores:(NSString *)category scope:(int)scope collection: (int) collection from:(int)from to:(int)to {
    NSLog(@"retrieveScores ");
    
    
    GKLeaderboard *board = [[GKLeaderboard alloc] init];
    
    if(board != nil) {

        
        board.range = NSMakeRange(from, to);
        board.category = category;
        switch (scope) {
            case 2:
                board.timeScope = GKLeaderboardTimeScopeAllTime;
                break;
            case 1:
                board.timeScope = GKLeaderboardTimeScopeWeek;
                break;
            case 0:
                board.timeScope = GKLeaderboardTimeScopeToday;
                break;
                
            default:
                board.timeScope = GKLeaderboardTimeScopeAllTime;
                break;
        }
        
        switch (collection) {
            case 1:
                board.playerScope = GKLeaderboardPlayerScopeGlobal;
                break;
            case 0:
                board.playerScope = GKLeaderboardPlayerScopeFriendsOnly;
                break;
                
            default:
                board.playerScope = GKLeaderboardPlayerScopeFriendsOnly;
                break;
        }
        
        
        [board loadScoresWithCompletionHandler: ^(NSArray *scores, NSError *error) {
            if (error != nil) {
                // handle the error.
                NSLog(@"Error retrieving score: %@", error.description);
                
                UnitySendMessage("GameCenterManager", "onLeaderboardScoreListLoadFailed", [ISNDataConvertor NSStringToChar:@""]);
                return;
                
            }
            
            
            if (scores != nil) {
                
                 NSLog(@"scores loaded");
                
                NSMutableString * data = [[NSMutableString alloc] init];
                [data appendString:category];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", scope]];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", collection]];
                [data appendString:@","];

                
                BOOL first = YES;
                
                NSEnumerator *e = [scores objectEnumerator];
                id object;
                while (object = [e nextObject]) {
                    GKScore* s =((GKScore*) object);
    
                    
                    if(!first) {
                        [data appendString:@","];
                        
                    }
                    
                    [data appendString:s.playerID];
                    [data appendString:@","];
                    
                    [data appendString:[NSString stringWithFormat:@"%lld", s.value]];
                    [data appendString:@","];
                    
                    [data appendString:[NSString stringWithFormat:@"%d", s.rank]];
                    
                    NSLog(@"id %@", s.playerID);
                    [self loadUserData:s.playerID];
                    
                    
                     first = NO;
                    
                }
                

                NSString *str = [data copy];
                
#if UNITY_VERSION < 500
                [str autorelease];
#endif
                UnitySendMessage("GameCenterManager", "onLeaderboardScoreListLoaded", [ISNDataConvertor NSStringToChar:str]);

                
                
            } else {
                NSLog(@"No scores to load");
                UnitySendMessage("GameCenterManager", "onLeaderboardScoreListLoadFailed", [ISNDataConvertor NSStringToChar:@""]);
            }
  
        }];
    }
    
#if UNITY_VERSION < 500
   [board release];
#endif
    
    
    

}

-(void) loadUserData:(NSString *)uid {
    
    NSLog(@"loadUserData %@", uid);

    
    
    if([loadedPlayersIds indexOfObject:uid] != NSNotFound) {
         NSLog(@"Player data was already loaded, call ignored");
        return;
    }
    
   
    
    NSArray *playerIdArray = [NSArray arrayWithObject:uid];
  
   

    [GKPlayer loadPlayersForIdentifiers:playerIdArray withCompletionHandler:^(NSArray *players, NSError *error) {
        
        GKPlayer *player = [players objectAtIndex:0];
        if (error != nil) {
            NSLog(@"%@", error.localizedDescription);
            
            UnitySendMessage("GameCenterManager", "onUserInfoLoadFailed", [ISNDataConvertor NSStringToChar:player.playerID]);
            return;
        }

 
        
        [loadedPlayersIds addObject:player.playerID];
        
        [player loadPhotoForSize:GKPhotoSizeSmall withCompletionHandler:^(UIImage *photo, NSError *error) {
        
                NSString *encodedImage = @"";
            
                
                if (photo == nil) {
                    NSLog(@"no photo for user with ID: %@", uid);
                } else {
                    NSData *imageData = UIImagePNGRepresentation(photo);
                    NSLog(@"imageData.length:  %i", imageData.length);
                    encodedImage = [imageData base64Encoding];
                    //  NSLog(@"encodedImage for user: %@", encodedImage);
                }
            
            
            
            
                
            
                NSMutableString * data = [[NSMutableString alloc] init];
                [data appendString:player.playerID];
                [data appendString:@","];
                [data appendString:encodedImage];
                [data appendString:@","];
                [data appendString:player.alias];
                [data appendString:@","];
                [data appendString:player.displayName];
            
            
                
                
                NSLog(@"User Data Loaded for ID:  %@", uid);
                NSString *str = [data copy];
            
#if UNITY_VERSION < 500
            [str autorelease];
#endif
                UnitySendMessage("GameCenterManager", "onUserInfoLoaded", [ISNDataConvertor NSStringToChar:str]);

            
            
        }];
         
    }];
}



-(void) sendAchievementChallengeWithFriendsPicker:(NSString *)achievementId message:(NSString *)message {
     GKAchievement *achievement = [[GKAchievement alloc] initWithIdentifier: achievementId];
    
#if UNITY_VERSION < 500
    [achievement autorelease];
#endif
    
     UIViewController *composeVC = [achievement challengeComposeControllerWithPlayers:nil message:message completionHandler:^(UIViewController *composeController, BOOL didIssueChallenge, NSArray *sentPlayerIDs) {
        [composeController dismissViewControllerAnimated:YES completion:nil];
        [composeController.view.superview removeFromSuperview];
     }];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController: composeVC animated: YES completion:nil];
}


-(void) sendAchievementChallenge:(NSString *)achievementId message:(NSString *)message playerIds:(NSArray *)playerIds {
    GKAchievement *achievement = [[GKAchievement alloc] initWithIdentifier: achievementId];
#if UNITY_VERSION < 500
      [achievement autorelease];
#endif
    
    [achievement issueChallengeToPlayers:playerIds message:message];
}




- (void) sendLeaderboardChallengeWithFriendsPicker:(NSString *)leaderboardId message:(NSString *)message {
    
    

    GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
    [leaderboardRequest autorelease];
#endif
    
    leaderboardRequest.category = leaderboardId;

    leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
    
    NSLog(@"leaderboardId %@", leaderboardId);
    
    
    if (leaderboardRequest != nil) {
        
        [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
            if (error != nil) {
                NSLog(@"Error challenge scores loading %@", error.description);
            }  else {
                
                UIViewController *composeVC = [leaderboardRequest.localPlayerScore challengeComposeControllerWithPlayers:nil message:message completionHandler:^(UIViewController *composeController, BOOL didIssueChallenge, NSArray *sentPlayerIDs){
                    
                    
                    [composeController dismissViewControllerAnimated:YES completion:nil];
                    
                    [composeController.view.superview removeFromSuperview];
                    
                }];
                
                
                UIViewController *vc =  UnityGetGLViewController();
                
                [vc presentViewController: composeVC animated: YES completion:nil];

                
             
                
            }
        }];
    }
    
}


-(void) sendLeaderboardChallenge:(NSString *)leaderboardId message:(NSString *)message playerIds:(NSArray *)playerIds {
    
    

    GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
    [leaderboardRequest autorelease];
#endif
    leaderboardRequest.category = leaderboardId;

    leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
    

    if (leaderboardRequest != nil) {
        
        [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
            if (error != nil) {
                NSLog(@"Error challenge scores loading");
            }  else {
                
                [leaderboardRequest.localPlayerScore issueChallengeToPlayers:playerIds message:message];
                
            }
        }];
    }

    
    
}


-(void) retrieveScoreForLocalPlayerWithCategory:(NSString *)category scope:(int)scope collection:(int)collection{


    GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
    [leaderboardRequest autorelease];
#endif
    leaderboardRequest.category = category;

    
    switch (scope) {
        case 2:
            leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
            break;
        case 1:
            leaderboardRequest.timeScope = GKLeaderboardTimeScopeWeek;
            break;
        case 0:
            leaderboardRequest.timeScope = GKLeaderboardTimeScopeToday;
            break;
            
        default:
            leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
            break;
    }
    
    switch (collection) {
        case 1:
        leaderboardRequest.playerScope = GKLeaderboardPlayerScopeGlobal;
        break;
        case 0:
        leaderboardRequest.playerScope = GKLeaderboardPlayerScopeFriendsOnly;
        break;
        
        default:
        leaderboardRequest.playerScope = GKLeaderboardPlayerScopeFriendsOnly;
        break;
    }

    
    
    if (leaderboardRequest != nil) {
        NSMutableString * data = [[NSMutableString alloc] init];
        
        [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
            if (error != nil) {
                NSLog(@"Error scores loading %@", error.description);
                UnitySendMessage("GameCenterManager", "onLeaderboardScoreFailed", "");

            }  else {
                [data appendString:category];
                [data appendString:@","];
                
            
                
                [data appendString:[NSString stringWithFormat:@"%lld", leaderboardRequest.localPlayerScore.value]];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", leaderboardRequest.localPlayerScore.rank]];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", scope]];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", collection]];


                NSString *str = [data copy];
#if UNITY_VERSION < 500
        [str autorelease];
#endif
                UnitySendMessage("GameCenterManager", "onLeaderboardScore", [ISNDataConvertor NSStringToChar:str]);

                
                
                
                NSLog(@"Retrieved localScore:%lld",leaderboardRequest.localPlayerScore.value);
            }
        }];
    }
}


-(void) retrieveFriends {
    GKLocalPlayer *lp = [GKLocalPlayer localPlayer];
    if (lp.authenticated) {
        [lp loadFriendsWithCompletionHandler:^(NSArray *friends, NSError *error) {
            
            if(error != NULL) {
                NSLog(@"Error loading friends: %@", error.description);
                UnitySendMessage("GameCenterManager", "onFriendListFailedToLoad", [ISNDataConvertor NSStringToChar:@""]);
                return;
            }
            
             if (friends != nil) {
                 BOOL first = YES;
                 NSMutableString * data = [[NSMutableString alloc] init];
                 for (NSString *fid in friends) {
                     
                     if(!first) {
                         [data appendString:@"|"];
                     }
                     first = NO;
                     
                     [data appendString:fid];
                     [self loadUserData:fid];
                      NSLog(@"fid %@", fid);
                     
                 }
                 
                 NSString *str = [data copy];
#if UNITY_VERSION < 500
                 [str autorelease];
#endif
                 UnitySendMessage("GameCenterManager", "onFriendListLoaded", [ISNDataConvertor NSStringToChar:str]);
                 
             } else {
                 NSLog(@"User has no friends, sending fail event");
                 UnitySendMessage("GameCenterManager", "onFriendListFailedToLoad", [ISNDataConvertor NSStringToChar:@""]);
             }
             
         }];
    } else {
        NSLog(@"User friends cannot be loaded before player auth, sending fail event");
        UnitySendMessage("GameCenterManager", "onFriendListFailedToLoad", [ISNDataConvertor NSStringToChar:@""]);
    }
    
}



@end


static IOSGameCenterManager *GCManager = NULL;

extern "C" {
    void _initGameCenter ()  {
        GCManager = [[IOSGameCenterManager alloc] init];
        [GCManager authenticateLocalPlayer];
    }
    
    
    void _showLeaderboard(char* leaderboardId, int scope) {
        [GCManager showLeaderboard:[ISNDataConvertor charToNSString:leaderboardId] scope:scope];
    }
    
    void _showLeaderboards() {
        [GCManager showLeaderboardsPopUp];
    }
    
    void _getLeaderboardScore(char* leaderboardId, int scope, int collection) {
        [GCManager retrieveScoreForLocalPlayerWithCategory:[ISNDataConvertor charToNSString:leaderboardId] scope:scope collection:collection];
    }
    
    void _loadLeaderboardScore(char* leaderboardId, int scope, int collection, int from, int to) {
       
        [GCManager retriveScores:[ISNDataConvertor charToNSString:leaderboardId] scope:scope  collection: collection from:from to:to];
    }
    
    void _showAchievements() {
        //[GCManager authenticateLocalPlayer];
        [GCManager showAchievements];
    }
    
    void _ISN_issueLeaderboardChallenge(char* leaderboardId, char* message, char* playerIds) {
        
        
        NSString* str = [ISNDataConvertor charToNSString:playerIds];
        NSArray *ids = [str componentsSeparatedByString:@","];
        
        [GCManager sendLeaderboardChallenge:[ISNDataConvertor charToNSString:leaderboardId] message:[ISNDataConvertor charToNSString:message] playerIds:ids];

    }
    
    void _ISN_issueLeaderboardChallengeWithFriendsPicker(char* leaderboardId, char* message) {
        
        NSString* lid = [ISNDataConvertor charToNSString:leaderboardId];
        NSString* mes = [ISNDataConvertor charToNSString:message];
        
        [GCManager sendLeaderboardChallengeWithFriendsPicker:lid message:mes];

    }

    
    void _ISN_issueAchievementChallenge(char* achievementId, char* message, char* playerIds) {
        
        NSString* str = [ISNDataConvertor charToNSString:playerIds];
        NSArray *ids = [str componentsSeparatedByString:@","];
        
        [GCManager sendAchievementChallenge:[ISNDataConvertor charToNSString:achievementId] message:[ISNDataConvertor charToNSString:message] playerIds:ids];
    }
    
    void _ISN_issueAchievementChallengeWithFriendsPicker(char* leaderboardId, char* message, char* playerIds) {
        
        NSString* lid = [ISNDataConvertor charToNSString:leaderboardId];
        NSString* mes = [ISNDataConvertor charToNSString:message];
        
        [GCManager sendAchievementChallengeWithFriendsPicker:lid message:mes];
    }

    
  
    
    void _reportScore(char* score, char* leaderboardId) {
 
        NSString* lid = [ISNDataConvertor charToNSString:leaderboardId];
        NSString* scoreValue = [ISNDataConvertor charToNSString:score];
         
        [GCManager reportScore:[scoreValue longLongValue] forCategory:lid];
    }
    
    void _submitAchievement(float percents, char* achievementID, BOOL notifyComplete) {
        double v = (double) percents;
        [GCManager submitAchievement:v identifier:[ISNDataConvertor charToNSString:achievementID] notifyComplete:notifyComplete];
    }
    
    void _resetAchievements() {
        [GCManager resetAchievements];
    }
    
    void _loadGCUserData(char* uid) {
        [GCManager loadUserData:[ISNDataConvertor charToNSString:uid]];
    }
    
    void _gcRetrieveFriends() {
        [GCManager retrieveFriends];
    }
    
    
    void _ISN_getSignature() {
        [GCManager getSignature];
    }
    
}


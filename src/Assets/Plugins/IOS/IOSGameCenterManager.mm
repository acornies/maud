#import "IOSGameCenterManager.h"
#import "ISNDataConvertor.h"

@implementation IOSGameCenterManager

static NSMutableArray *loadedPlayersIds;

- (id)init {
    self = [super init];
    if (self) {
        requestedLeaderBordId = NULL;
        isAchievementsWasLoaded = FALSE;
        loadedPlayersIds = [[NSMutableArray alloc] init];
        [loadedPlayersIds retain];
        
        gameCenterManager = [[GameCenterManager alloc] init];
        NSLog(@"IOSGameCenterManager inited");
    }
    
    return self;
}



- (void)dealloc {
    [super dealloc];
}




- (void) reportScore: (long long) score forCategory: (NSString*) category {
    NSLog(@"reportScore: %lld", score);
    NSLog(@"category %@", category);
    

    
    GKScore *scoreReporter = [[[GKScore alloc] initWithCategory:category] autorelease];
    scoreReporter.value = score;
    
    [scoreReporter reportScoreWithCompletionHandler: ^(NSError *error) {
        if (error != nil) {
            UnitySendMessage("GameCenterManager", "onScoreSubmitedFailed", [ISNDataConvertor NSStringToChar:@""]);
            NSLog(@"got error while score sibmiting: %@", error.description);
        } else {
            NSLog(@"new hing score sumbited succsess: %lld", score);
            
            
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            
            [data appendString:category];
            [data appendString:@","];
            [data appendString:[NSString stringWithFormat: @"%lld", score]];
 
    
            NSString *str = [[data copy] autorelease];
            
            
            
            UnitySendMessage("GameCenterManager", "onScoreSubmitedEvent", [ISNDataConvertor NSStringToChar:str]);
            
           
        }
        
    }];
}


-(void) resetAchievements {
    [gameCenterManager resetAchievements];
}



-(void) submitAchievement:(double)percentComplete identifier:(NSString *)identifier  notifayComplete: (BOOL) notifayComplete {
    [gameCenterManager submitAchievement:identifier percentComplete:percentComplete notifayComplete:notifayComplete];
}




-(void) showLeaderBoard:(NSString *)leaderBoradrId scope:(int)scope {
    
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    if(localPlayer.isAuthenticated) {
        requestedLeaderBordId = leaderBoradrId;
        [requestedLeaderBordId retain];

        lbscope = scope;
        [self showLeaderBoardPopUp];
    } else {
        NSLog(@"ISN showLeaderBoard requires player to be authed.  Call ignored");
    }
}



- (void) showAchievements {
    
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    if(localPlayer.isAuthenticated) {
        [self showAchievementsPopUp];
    } else {
       NSLog(@"ISN showAchievements requires player to be authed.  Call ignored");
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

- (void) showLeaderBoardsPopUp {
    GKGameCenterViewController *leaderboardController = [[GKGameCenterViewController alloc] init];
    
    
    NSLog(@"showLeaderBoardsPopUp");
    leaderboardController.gameCenterDelegate = self;
    
    
    CGSize screenSize = [[UIScreen mainScreen] bounds].size;

    UIViewController *vc =  UnityGetGLViewController();

    [vc presentViewController: leaderboardController animated: YES completion:nil];
    leaderboardController.view.transform = CGAffineTransformMakeRotation(0.0f);
    [leaderboardController.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
    leaderboardController.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
    
    
}

- (void) showLeaderBoardPopUp {
    
    GKGameCenterViewController *leaderboardController = [[GKGameCenterViewController alloc] init];
    if (leaderboardController != NULL)
    {
        NSLog(@"requested LB: %@", requestedLeaderBordId);
        
        leaderboardController.leaderboardCategory = requestedLeaderBordId;
        
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
        
        [requestedLeaderBordId release];
        
    }
    
    requestedLeaderBordId = NULL;
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
    
    [vc release];
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
                    NSLog(@"PLAYER AUTHENICATED");
                    
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
                    
                
                    
                    NSString *str = [[data copy] autorelease];
                    UnitySendMessage("GameCenterManager", "onAuthenticateLocalPlayer", [ISNDataConvertor NSStringToChar:str]);
                    
                    NSLog(@"PLAYER AUTHENICATED %@", localPlayer.alias);
                    
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
                    NSLog(@"PLAYER NOT AUTHENICATED");
                    UnitySendMessage("GameCenterManager", "onAuthenticationFailed", [ISNDataConvertor NSStringToChar:@""]);
                }
            }];
        } else {
            NSLog(@"Do nothing Player is already auntificated");
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
            
            NSString *str = [[data copy] autorelease];
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
    NSLog(@"retriveScores ");
    
    
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
                
                UnitySendMessage("GameCenterManager", "onLeaderBoardScoreListLoadFailed", [ISNDataConvertor NSStringToChar:@""]);
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
                
                NSString *str = [[data copy] autorelease];
                UnitySendMessage("GameCenterManager", "onLeaderBoardScoreListLoaded", [ISNDataConvertor NSStringToChar:str]);
                
                
            } else {
                NSLog(@"No scores to load");
                UnitySendMessage("GameCenterManager", "onLeaderBoardScoreListLoadFailed", [ISNDataConvertor NSStringToChar:@""]);
            }
  
        }];
    }
    [board release];
    

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
                NSString *str = [[data copy] autorelease];
                UnitySendMessage("GameCenterManager", "onUserInfoLoaded", [ISNDataConvertor NSStringToChar:str]);

            
            
        }];
         
    }];
}



-(void) sendAchievementChallengeWithFriendsPicker:(NSString *)achievementId message:(NSString *)message {
     GKAchievement *achievement = [[[GKAchievement alloc] initWithIdentifier: achievementId] autorelease];
    
     UIViewController *composeVC = [achievement challengeComposeControllerWithPlayers:nil message:message completionHandler:^(UIViewController *composeController, BOOL didIssueChallenge, NSArray *sentPlayerIDs) {
        [composeController dismissViewControllerAnimated:YES completion:nil];
        [composeController.view.superview removeFromSuperview];
     }];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController: composeVC animated: YES completion:nil];
}


-(void) sendAchievementChallenge:(NSString *)achievementId message:(NSString *)message playerIds:(NSArray *)playerIds {
    GKAchievement *achievement = [[[GKAchievement alloc] initWithIdentifier: achievementId] autorelease];
    [achievement issueChallengeToPlayers:playerIds message:message];
}




- (void) sendLeaderboardChallengeWithFriendsPicker:(NSString *)leaderBoradrId message:(NSString *)message {
    
    
    GKLeaderboard *leaderboardRequest = [[[GKLeaderboard alloc] init] autorelease];
    leaderboardRequest.category = leaderBoradrId;
    leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
    
    NSLog(@"leaderBoradrId %@", leaderBoradrId);
    
    
    if (leaderboardRequest != nil) {
        
        [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
            if (error != nil) {
                NSLog(@"Error chnalange scores loading %@", error.description);
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


-(void) sendLeaderboardChallenge:(NSString *)leaderBoradrId message:(NSString *)message playerIds:(NSArray *)playerIds {
    
    
    GKLeaderboard *leaderboardRequest = [[[GKLeaderboard alloc] init] autorelease];
    leaderboardRequest.category = leaderBoradrId;
    leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
    

    if (leaderboardRequest != nil) {
        
        [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
            if (error != nil) {
                NSLog(@"Error chnalange scores loading");
            }  else {
                
                [leaderboardRequest.localPlayerScore issueChallengeToPlayers:playerIds message:message];
                
            }
        }];
    }

    
    
}


-(void) retrieveScoreForLocalPlayerWithCategory:(NSString *)category scope:(int)scope collection:(int)collection{


    GKLeaderboard *leaderboardRequest = [[[GKLeaderboard alloc] init] autorelease];
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
                UnitySendMessage("GameCenterManager", "onLeaderBoardScoreFailed", "");

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

                NSString *str = [[data copy] autorelease];
                UnitySendMessage("GameCenterManager", "onLeaderBoardScore", [ISNDataConvertor NSStringToChar:str]);
                
                
                
                NSLog(@"Retrieved localScore:%lld",leaderboardRequest.localPlayerScore.value);
            }
        }];
    }
}



- (void)findMatchWithMinPlayers:(int)minPlayers maxPlayers:(int)maxPlayers {
    [[GCHelper sharedInstance] findMatchWithMinPlayers:minPlayers maxPlayers:maxPlayers];
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
                 
                 NSString *str = [[data copy] autorelease];
                 UnitySendMessage("GameCenterManager", "onFriendListLoaded", [ISNDataConvertor NSStringToChar:str]);
                 
             } else {
                 NSLog(@"User has no friends, sending fail event");
                 UnitySendMessage("GameCenterManager", "onFriendListFailedToLoad", [ISNDataConvertor NSStringToChar:@""]);
             }
             
         }];
    } else {
        NSLog(@"User friends can not be loaded before player auth, sending fail event");
        UnitySendMessage("GameCenterManager", "onFriendListFailedToLoad", [ISNDataConvertor NSStringToChar:@""]);
    }
    
}



@end


static IOSGameCenterManager *GCManager = NULL;

extern "C" {
    void _initGamaCenter ()  {
        GCManager = [[IOSGameCenterManager alloc] init];
        [GCManager authenticateLocalPlayer];
    }
    
    
    void _showLeaderBoard(char* leaderBoradrId, int scope) {
        [GCManager showLeaderBoard:[ISNDataConvertor charToNSString:leaderBoradrId] scope:scope];
    }
    
    void _showLeaderBoards() {
        [GCManager showLeaderBoardsPopUp];
    }
    
    void _getLeadrBoardScore(char* leaderBoradrId, int scope, int collection) {
        [GCManager retrieveScoreForLocalPlayerWithCategory:[ISNDataConvertor charToNSString:leaderBoradrId] scope:scope collection:collection];
    }
    
    void _loadLeadrBoardScore(char* leaderBoradrId, int scope, int collection, int from, int to) {
        [GCManager retriveScores:[ISNDataConvertor charToNSString:leaderBoradrId] scope:scope  collection: collection from:from to:to];
    }
    
    void _showAchievements() {
        //[GCManager authenticateLocalPlayer];
        [GCManager showAchievements];
    }
    
    void _ISN_issueLeaderboardChallenge(char* leaderBoradrId, char* message, char* playerIds) {
        
        
        NSString* str = [ISNDataConvertor charToNSString:playerIds];
        NSArray *ids = [str componentsSeparatedByString:@","];
        
        [GCManager sendLeaderboardChallenge:[ISNDataConvertor charToNSString:leaderBoradrId] message:[ISNDataConvertor charToNSString:message] playerIds:ids];

    }
    
    void _ISN_issueLeaderboardChallengeWithFriendsPicker(char* leaderBoradrId, char* message) {
        
        NSString* lid = [ISNDataConvertor charToNSString:leaderBoradrId];
        NSString* mes = [ISNDataConvertor charToNSString:message];
        
        [GCManager sendLeaderboardChallengeWithFriendsPicker:lid message:mes];

    }

    
    void _ISN_issueAchievementChallenge(char* achievementId, char* message, char* playerIds) {
        
        NSString* str = [ISNDataConvertor charToNSString:playerIds];
        NSArray *ids = [str componentsSeparatedByString:@","];
        
        [GCManager sendAchievementChallenge:[ISNDataConvertor charToNSString:achievementId] message:[ISNDataConvertor charToNSString:message] playerIds:ids];
    }
    
    void _ISN_issueAchievementChallengeWithFriendsPicker(char* leaderBoradrId, char* message, char* playerIds) {
        
        NSString* lid = [ISNDataConvertor charToNSString:leaderBoradrId];
        NSString* mes = [ISNDataConvertor charToNSString:message];
        
        [GCManager sendAchievementChallengeWithFriendsPicker:lid message:mes];
    }

    
  
    
    void _reportScore(char* score, char* leaderBoradrId) {
 
        NSString* lid = [ISNDataConvertor charToNSString:leaderBoradrId];
        NSString* scoreValue = [ISNDataConvertor charToNSString:score];
         
        [GCManager reportScore:[scoreValue longLongValue] forCategory:lid];
    }
    
    void _submitAchievement(float percents, char* achievementID, BOOL notifayComplete) {
        double v = (double) percents;
        [GCManager submitAchievement:v identifier:[ISNDataConvertor charToNSString:achievementID] notifayComplete:notifayComplete];
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
    
}


/*
 
 File: GameCenterManager.m
 Abstract: Basic introduction to GameCenter
 
 Version: 1.0
 
 Disclaimer: IMPORTANT:  This Apple software is supplied to you by Apple Inc.
 ("Apple") in consideration of your agreement to the following terms, and your
 use, installation, modification or redistribution of this Apple software
 constitutes acceptance of these terms.  If you do not agree with these terms,
 please do not use, install, modify or redistribute this Apple software.
 
 In consideration of your agreement to abide by the following terms, and subject
 to these terms, Apple grants you a personal, non-exclusive license, under
 Apple's copyrights in this original Apple software (the "Apple Software"), to
 use, reproduce, modify and redistribute the Apple Software, with or without
 modifications, in source and/or binary forms; provided that if you redistribute
 the Apple Software in its entirety and without modifications, you must retain
 this notice and the following text and disclaimers in all such redistributions
 of the Apple Software.
 Neither the name, trademarks, service marks or logos of Apple Inc. may be used
 to endorse or promote products derived from the Apple Software without specific
 prior written permission from Apple.  Except as expressly stated in this notice,
 no other rights or licenses, express or implied, are granted by Apple herein,
 including but not limited to any patent rights that may be infringed by your
 derivative works or by other works in which the Apple Software may be
 incorporated.
 
 The Apple Software is provided by Apple on an "AS IS" basis.  APPLE MAKES NO
 WARRANTIES, EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION THE IMPLIED
 WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 PURPOSE, REGARDING THE APPLE SOFTWARE OR ITS USE AND OPERATION ALONE OR IN
 COMBINATION WITH YOUR PRODUCTS.
 
 IN NO EVENT SHALL APPLE BE LIABLE FOR ANY SPECIAL, INDIRECT, INCIDENTAL OR
 CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
 GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 ARISING IN ANY WAY OUT OF THE USE, REPRODUCTION, MODIFICATION AND/OR
 DISTRIBUTION OF THE APPLE SOFTWARE, HOWEVER CAUSED AND WHETHER UNDER THEORY OF
 CONTRACT, TORT (INCLUDING NEGLIGENCE), STRICT LIABILITY OR OTHERWISE, EVEN IF
 APPLE HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
 Copyright (C) 2010 Apple Inc. All Rights Reserved.
 
 */

#import "GameCenterManager.h"
#import <GameKit/GameKit.h>

#import "ISNDataConvertor.h"



@implementation GameCenterManager

@synthesize earnedAchievementCache;
@synthesize delegate;

- (id) init
{
	self = [super init];
	if(self!= NULL)
	{
		earnedAchievementCache= NULL;
	}
	return self;
}

- (void) dealloc
{
	self.earnedAchievementCache= NULL;
	[super dealloc];
}




- (void) submitAchievement:(NSString *)identifier percentComplete:(double)percentComplete notifayComplete:(bool)notifayComplete {
    
	//GameCenter check for duplicate achievements when the achievement is submitted, but if you only want to report 
	// new achievements to the user, then you need to check if it's been earned 
	// before you submit.  Otherwise you'll end up with a race condition between loadAchievementsWithCompletionHandler
	// and reportAchievementWithCompletionHandler.  To avoid this, we fetch the current achievement list once,
	// then cache it and keep it updated with any new achievements.
    
    NSLog(@"submitAchievement %@", identifier);
    
	if(self.earnedAchievementCache == NULL) {
        NSLog(@"loading Achievements cash....");
		[GKAchievement loadAchievementsWithCompletionHandler: ^(NSArray *scores, NSError *error) {
			if(error == NULL)  {
				NSMutableDictionary* tempCache= [NSMutableDictionary dictionaryWithCapacity: [scores count]];
				for (GKAchievement* score in tempCache) {
					[tempCache setObject: score forKey: score.identifier];
				}
                
               

                
				self.earnedAchievementCache= tempCache;
                 NSLog(@"Achievements cash loaded, re submiting acguevment");
				[self submitAchievement:identifier percentComplete:percentComplete notifayComplete:notifayComplete];
			}
			else{
                  NSLog(@"Achievements cash load errod: %@", error.description);
			}

		}];
	} else {
		 //Search the list for the ID we're using...
		GKAchievement* achievement= [self.earnedAchievementCache objectForKey: identifier];
		if(achievement != NULL) {
			if((achievement.percentComplete >= 100.0) || (achievement.percentComplete >= percentComplete)) {
				//Achievement has already been earned so we're done.
				achievement= NULL;
			}
            
			achievement.percentComplete= percentComplete;
		} else {
			achievement= [[[GKAchievement alloc] initWithIdentifier: identifier] autorelease];
            achievement.showsCompletionBanner = notifayComplete;
            
            if(percentComplete > 100.0) {
                percentComplete = 100.0;
            }
            
			achievement.percentComplete= percentComplete;
            
			//Add achievement to achievement cache...
			[self.earnedAchievementCache setObject: achievement forKey: achievement.identifier];
		}
        
		if(achievement!= NULL) {
             NSLog(@"Submit the Achievement");
			//Submit the Achievement...
			[achievement reportAchievementWithCompletionHandler: ^(NSError *error) {
                if(error == NULL) {
                    NSMutableString * data = [[NSMutableString alloc] init];
                    [data appendString:achievement.identifier];
                    [data appendString:@","];
                    [data appendString:[NSString stringWithFormat:@"%f", achievement.percentComplete]];
                    
                    
                    NSString *str = [[data copy] autorelease];
                    
                    NSLog(@"Submitted");
                    NSLog(@"identifier: %@", achievement.identifier);
                    NSLog(@"percentComplete: %f", achievement.percentComplete);
                    
                    UnitySendMessage("GameCenterManager", "onAchievementProgressChanged", [ISNDataConvertor NSStringToChar:str]);
                } else {
                    NSLog(@"Submit failed with error %@", error.description);
                }
			}];
		}
	}
}

- (void) resetAchievements {
	self.earnedAchievementCache= NULL;
	[GKAchievement resetAchievementsWithCompletionHandler: ^(NSError *error)  {
        
        if(error != nil) {
            NSLog(@"resetAchievements failed: %@", error.description);
            UnitySendMessage("GameCenterManager", "onAchievementsResetFailed", "");
        } else {
            NSLog(@"resetAchievements complete");
            UnitySendMessage("GameCenterManager", "onAchievementsReset", "");

        }
        
		
        
	}];
}

@end

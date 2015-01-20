//
//  ISNVideo.h
//  Unity-iPhone
//
//  Created by Lacost on 8/27/14.
//
//

#import <Foundation/Foundation.h>
#include "ISNDataConvertor.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

#import <MediaPlayer/MediaPlayer.h>

@interface ISNVideo : NSObject

@property (strong, nonatomic) MPMoviePlayerViewController *streamPlayer;

+ (id) sharedInstance;

- (void) steamVideo:(NSString*)url;
- (void) openYoutubeVideo:(NSString*)url;

@end

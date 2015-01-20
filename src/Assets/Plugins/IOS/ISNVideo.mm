//
//  ISNVideo.m
//  Unity-iPhone
//
//  Created by Lacost on 8/27/14.
//
//

#import "ISNVideo.h"

@implementation ISNVideo

static ISNVideo *_sharedInstance;

+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}





-(void) steamVideo:(NSString *)url {
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    NSURL *streamURL = [NSURL URLWithString:url];
    
    _streamPlayer = [[MPMoviePlayerViewController alloc] initWithContentURL:streamURL];
    
    [vc presentMoviePlayerViewControllerAnimated:self.streamPlayer];
    
    [self.streamPlayer.moviePlayer play];
    
}

-(void) openYoutubeVideo:(NSString *)url {
    
    NSMutableString *str = [[NSMutableString alloc] init];
    [str appendString:@"http://www.youtube.com/v/"];
    [str appendString:url];
    
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:str]];
    return;
}



extern "C" {
    
    
    //--------------------------------------
	//  IOS Native Plugin Section
	//--------------------------------------
    
    
    void _ISN_SteamVideo(char* videoUrl) {
        NSString *url = [ISNDataConvertor charToNSString:videoUrl];
        [[ISNVideo sharedInstance] steamVideo:url];
    }
    
    void _ISN_OpenYoutubeVideo(char* videoUrl) {
        NSString *url = [ISNDataConvertor charToNSString:videoUrl];
        [[ISNVideo sharedInstance] openYoutubeVideo:url];
    }
    
    
}

@end

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





-(void) streamVideo:(NSString *)url {
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    NSURL *streamURL = [NSURL URLWithString:url];
    
    _streamPlayer = [[MPMoviePlayerViewController alloc] initWithContentURL:streamURL];
    
    [vc presentMoviePlayerViewControllerAnimated:self.streamPlayer];
    
    [self.streamPlayer.moviePlayer play];
    
}



-(void) openYouTubeVideo:(NSString *)url {
    NSLog(@"openYouTubeVideo");
    
    NSMutableString *str = [[NSMutableString alloc] init];
    [str appendString:@"http://www.youtube.com/v/"];
    [str appendString:url];
    
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:str]];
    
}

// Add this if you wish to add support Orientation support for picker








extern "C" {
    
    
    //--------------------------------------
	//  IOS Native Plugin Section
	//--------------------------------------
    
    
    void _ISN_StreamVideo(char* videoUrl) {
        NSString *url = [ISNDataConvertor charToNSString:videoUrl];
        [[ISNVideo sharedInstance] streamVideo:url];
    }
    
    void _ISN_OpenYouTubeVideo(char* videoUrl) {
        NSString *url = [ISNDataConvertor charToNSString:videoUrl];
        [[ISNVideo sharedInstance] openYouTubeVideo:url];
    }
    
    
}

@end

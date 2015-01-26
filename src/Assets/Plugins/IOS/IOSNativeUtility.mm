//
//  IOSNativeUtility.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 4/29/14.
//
//

#import "IOSNativeUtility.h"

@implementation IOSNativeUtility

static IOSNativeUtility *_sharedInstance;
static NSString* templateReviewURLIOS7  = @"itms-apps://itunes.apple.com/app/idAPP_ID";
NSString *templateReviewURL = @"itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=APP_ID";

+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}

-(void) redirectToRatigPage:(NSString *)appId {
#if TARGET_IPHONE_SIMULATOR
    NSLog(@"APPIRATER NOTE: iTunes App Store is not supported on the iOS simulator. Unable to open App Store page.");
#else
    
    
    NSString *reviewURL;
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    
    
    if ([[vComp objectAtIndex:0] intValue] >= 7) {
        reviewURL = [templateReviewURLIOS7 stringByReplacingOccurrencesOfString:@"APP_ID" withString:[NSString stringWithFormat:@"%@", appId]];
    }  else {
        reviewURL = [templateReviewURL stringByReplacingOccurrencesOfString:@"APP_ID" withString:[NSString stringWithFormat:@"%@", appId]];
    }
    
    NSLog(@"redirecting to iTunes page, IOS version: %i", [[vComp objectAtIndex:0] intValue]);
    NSLog(@"redirect URL: %@", reviewURL);
    
    
	
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:reviewURL]];
#endif
}




- (void) ShowSpiner {
    
    [[UIApplication sharedApplication] beginIgnoringInteractionEvents];
    
    if([self spinner] != nil) {
        return;
    }
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    [self setSpinner:[[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge]];

    
    [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];

    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        NSLog(@"IOS 8 detected");
        [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
    } else {
        if([[UIDevice currentDevice] orientation] == UIDeviceOrientationPortrait || [[UIDevice currentDevice] orientation] == UIDeviceOrientationPortraitUpsideDown) {
            NSLog(@"portarait detected");
            [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
            
        } else {
            NSLog(@"landscape detected");
            [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.height, vc.view.frame.size.width)];
        }

    }
    
  
    
  
    
    [self spinner].opaque = NO;
    [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];
    
    
    [UIView animateWithDuration:0.8 animations:^{
        [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
    }];
   
    
    
     
     [vc.view addSubview:[self spinner]];
     [[self spinner] startAnimating];
    
  //  [[self spinner] retain];

}



- (void) HideSpiner {
    
    if([self spinner] != nil) {
        [[UIApplication sharedApplication] endIgnoringInteractionEvents];
        
        [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
        [UIView animateWithDuration:0.8 animations:^{
            [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];

        } completion:^(BOOL finished) {
            [[self spinner] removeFromSuperview];
#if UNITY_VERSION < 500
            [[self spinner] release];
#endif
     
            [self setSpinner:nil];
        }];
        
       
    }
    
}

- (CGFloat) GetW {
    
    UIViewController *vc =  UnityGetGLViewController();
    
    bool IsLandscape;
    UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    if(orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight) {
        IsLandscape = true;
    } else {
        IsLandscape = false;
    }
    
    CGFloat w;
    if(IsLandscape) {
        w = vc.view.frame.size.height;
    } else {
        w = vc.view.frame.size.width;
    }
    
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        w = vc.view.frame.size.width;
    }

    
    return w;
}


- (void)DP_changeDate:(UIDatePicker *)sender {

    NSDateFormatter *dateFormatter = [[[NSDateFormatter alloc] init] autorelease];
    [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
    NSString *dateString = [dateFormatter stringFromDate:sender.date];
    
    NSLog(@"DateChangedEvent: %@", dateString);
    
    UnitySendMessage("IOSDateTimePicker", "DateChangedEvent", [ISNDataConvertor NSStringToChar:dateString]);
}

-(void) DP_PickerClosed:(UIDatePicker *)sender {
    NSDateFormatter *dateFormatter = [[[NSDateFormatter alloc] init] autorelease];
    [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
    NSString *dateString = [dateFormatter stringFromDate:sender.date];
    
    NSLog(@"DateChangedEvent: %@", dateString);
    
    UnitySendMessage("IOSDateTimePicker", "PickerClosed", [ISNDataConvertor NSStringToChar:dateString]);

}



UIDatePicker *datePicker;

- (void) DP_show:(int)mode {
    UIViewController *vc =  UnityGetGLViewController();
    
   
    
    
    
    
    CGRect toolbarTargetFrame = CGRectMake(0, vc.view.bounds.size.height-216-44, [self GetW], 44);
    CGRect datePickerTargetFrame = CGRectMake(0, vc.view.bounds.size.height-216, [self GetW], 216);
    CGRect darkViewTargetFrame = CGRectMake(0, vc.view.bounds.size.height-216-44, [self GetW], 260);
    
    UIView *darkView = [[[UIView alloc] initWithFrame:CGRectMake(0, vc.view.bounds.size.height, [self GetW], 260)] autorelease];
    darkView.alpha = 1;
    darkView.backgroundColor = [UIColor whiteColor];
    darkView.tag = 9;
    
    UITapGestureRecognizer *tapGesture = [[[UITapGestureRecognizer alloc] initWithTarget:self action:@selector(DP_dismissDatePicker:)] autorelease];
    [darkView addGestureRecognizer:tapGesture];
    [vc.view addSubview:darkView];
    
    
    datePicker = [[[UIDatePicker alloc] initWithFrame:CGRectMake(0, vc.view.bounds.size.height+44, [self GetW], 216)] autorelease];
    datePicker.tag = 10;
    
    
    [datePicker addTarget:self action:@selector(DP_changeDate:) forControlEvents:UIControlEventValueChanged];
    mode = 4;
    switch (mode) {
        case 1:
            datePicker.datePickerMode = UIDatePickerModeTime;
            break;
            
        case 2:
            datePicker.datePickerMode = UIDatePickerModeDate;
            break;
            
        case 3:
            datePicker.datePickerMode = UIDatePickerModeDateAndTime;
            break;
            
        case 4:
            datePicker.datePickerMode = UIDatePickerModeCountDownTimer;
            break;
            
        default:
            break;
    }
    

    
    [vc.view addSubview:datePicker];
    
    UIToolbar *toolBar = [[[UIToolbar alloc] initWithFrame:CGRectMake(0, vc.view.bounds.size.height, [self GetW], 44)] autorelease];
    toolBar.tag = 11;
    toolBar.barStyle = UIBarStyleDefault;
    UIBarButtonItem *spacer = [[[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace target:nil action:nil] autorelease];
    UIBarButtonItem *doneButton = [[[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(DP_dismissDatePicker:)] autorelease];
    [toolBar setItems:[NSArray arrayWithObjects:spacer, doneButton, nil]];
    [vc.view addSubview:toolBar];
    
    [UIView beginAnimations:@"MoveIn" context:nil];
    toolBar.frame = toolbarTargetFrame;
    datePicker.frame = datePickerTargetFrame;
    darkView.frame = darkViewTargetFrame;
   
    [UIView commitAnimations];

}

- (void)DP_removeViews:(id)object {
    UIViewController *vc =  UnityGetGLViewController();
    
    [[vc.view viewWithTag:9] removeFromSuperview];
    [[vc.view viewWithTag:10] removeFromSuperview];
    [[vc.view viewWithTag:11] removeFromSuperview];
}

- (void)DP_dismissDatePicker:(id)sender {
    UIViewController *vc =  UnityGetGLViewController();
    
    [self DP_PickerClosed:datePicker];
    
    CGRect toolbarTargetFrame = CGRectMake(0, vc.view.bounds.size.height, [self GetW], 44);
    CGRect datePickerTargetFrame = CGRectMake(0, vc.view.bounds.size.height+44, [self GetW], 216);
    CGRect darkViewTargetFrame = CGRectMake(0, vc.view.bounds.size.height, [self GetW], 260);
    
    
    [UIView beginAnimations:@"MoveOut" context:nil];
    [vc.view viewWithTag:9].frame = darkViewTargetFrame;
    [vc.view viewWithTag:10].frame = datePickerTargetFrame;
    [vc.view viewWithTag:11].frame = toolbarTargetFrame;
    [UIView setAnimationDelegate:self];
    [UIView setAnimationDidStopSelector:@selector(DP_removeViews:)];
    [UIView commitAnimations];
}




extern "C" {
    
    
    //--------------------------------------
    //  Date Time Picker
    //--------------------------------------
    
    void _ISN_ShowDP(int mode) {
        [[IOSNativeUtility sharedInstance] DP_show:mode];
    }
    
    
    //--------------------------------------
	//  IOS Native Plugin Section
	//--------------------------------------
    
    void _ISN_RedirectToAppStoreRatingPage(char* appId) {
        [[IOSNativeUtility sharedInstance] redirectToRatigPage: [ISNDataConvertor charToNSString:appId ]];
    }
    
    
    void _ISN_ShowPreloader() {
        [[IOSNativeUtility sharedInstance] ShowSpiner];
    }
    
    
    void _ISN_HidePreloader() {
        [[IOSNativeUtility sharedInstance] HideSpiner];
    }
    
    
    //--------------------------------------
	//  Native PopUps Plugin Section
	//--------------------------------------
    
    
    void _MNP_RedirectToAppStoreRatingPage(char* appId) {
        _ISN_RedirectToAppStoreRatingPage(appId);
    }
    
    
    void _MNP_ShowPreloader() {
        _ISN_ShowPreloader();
    }
    
    
    void _MNP_HidePreloader() {
        _ISN_HidePreloader();
    }
    
    
}
@end

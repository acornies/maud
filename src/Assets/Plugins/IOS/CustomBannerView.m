//
//  CustomBannerView.m
//  Unity-iPhone
//
//  Created by Lacost on 9/1/14.
//
//

#import "CustomBannerView.h"

@implementation CustomBannerView

- (id)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        // Initialization code
    }
    return self;
}


- (void)touchesBegan:(NSSet*)touches withEvent:(UIEvent*)event {
    
   // NSLog(@"touchesBegan");

}
- (void)touchesEnded:(NSSet*)touches withEvent:(UIEvent*)event
{
	 //NSLog(@"touchesEnded");
}
- (void)touchesCancelled:(NSSet*)touches withEvent:(UIEvent*)event
{
	// NSLog(@"touchesCancelled");
}
- (void)touchesMoved:(NSSet*)touches withEvent:(UIEvent*)event
{
	// NSLog(@"touchesMoved");
}


@end

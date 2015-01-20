//
//  ISNCamera.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 6/10/14.
//
//

#import "ISNCamera.h"


@implementation ISNCamera

static ISNCamera *_sharedInstance;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}

- (void) saveToCameraRoll:(NSString *)media {
    NSLog(@"saveToCameraRoll");
    NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
    UIImage *image = [[UIImage alloc] initWithData:imageData];
    
    
    UIImageWriteToSavedPhotosAlbum(image,
                                   self, // send the message to 'self' when calling the callback
                                   @selector(thisImage:hasBeenSavedInPhotoAlbumWithError:usingContextInfo:), // the selector to tell the method to call on completion
                                   NULL); // you generally won't need a contextInfo here
    
    
}

- (void)thisImage:(UIImage *)image hasBeenSavedInPhotoAlbumWithError:(NSError *)error usingContextInfo:(void*)ctxInfo {
    if (error) {
        NSLog(@"image not saved: %@", error.description);
        UnitySendMessage("IOSCamera", "OnImageSaveFailed", [ISNDataConvertor NSStringToChar:@""]);
        
    } else {
        NSLog(@"image saved");
        UnitySendMessage("IOSCamera", "OnImageSaveSuccess", [ISNDataConvertor NSStringToChar:@""]);
    }
    
    
}


-(void) GetImageFromAlbum {
    [self GetImage:UIImagePickerControllerSourceTypeSavedPhotosAlbum];
}


-(void) StartCameraImagePic {
    NSLog(@"StartCameraImagePic");
    [self GetImage:UIImagePickerControllerSourceTypeCamera];
}

-(void) GetImageFromCamera {
    BOOL cameraAvailableFlag = [UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera];
    if (cameraAvailableFlag) {
        [self performSelector:@selector(StartCameraImagePic) withObject:nil afterDelay:0.9];
    }
}


-(void) GetVideoPathFromAlbum {
    [self GetVideo];
}





- (void)GetVideo {
    UIViewController *vc =  UnityGetGLViewController();
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    picker.mediaTypes = [[NSArray alloc] initWithObjects:(NSString *)kUTTypeMovie,      nil];
    [vc presentViewController:picker animated:YES completion:nil];
    [picker release];
}




-(void) GetImage: (UIImagePickerControllerSourceType )source {
    UIViewController *vc =  UnityGetGLViewController();
    
    UIImagePickerController * picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    
    picker.sourceType = source;
    
    
    
    [vc presentViewController:picker animated:YES completion:nil];
    //	[vc presentModalViewController:picker animated:YES ];
    
    
    
    
}


-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    
    
    // added video support
    NSString *mediaType = [info objectForKey: UIImagePickerControllerMediaType]; // get media type
    // if mediatype is video
    if (CFStringCompare ((__bridge CFStringRef) mediaType, kUTTypeMovie, 0) == kCFCompareEqualTo) {
        NSURL *videoUrl=(NSURL*)[info objectForKey:UIImagePickerControllerMediaURL];
        NSString *moviePath = [videoUrl path];
        UnitySendMessage("IOSCamera", "OnVideoPickedEvent", [ISNDataConvertor NSStringToChar:moviePath]);
    } else{
        // it must be an image
        UIImage *photo = [info objectForKey:UIImagePickerControllerOriginalImage];
        NSString *encodedImage = @"";
        if (photo == nil) {
            NSLog(@"no photo");
        } else {
            // NSLog(@"MaxImageSize: %i", [self MaxImageSize]);
            //  NSLog(@"photo.size.width: %f", photo.size.width);
            
            
            
            if(photo.size.width > [self MaxImageSize] || photo.size.height > [self MaxImageSize] ) {
                NSLog(@"resizing image");
                CGSize s = CGSizeMake([self MaxImageSize], [self MaxImageSize]);
                
                if(photo.size.width > photo.size.height) {
                    CGFloat new_height = [self MaxImageSize] / (photo.size.width / photo.size.height);
                    s.height = new_height;

                } else {
                    CGFloat new_width = [self MaxImageSize] / (photo.size.height / photo.size.width);
                    s.width = new_width;

                }
                              
                photo =   [ISNCamera imageWithImage:photo scaledToSize:s];

            }
            
            NSData *imageData = nil;
            NSLog(@"ImageCompressionRate: %f", [self ImageCompressionRate]);
            if([self encodingType] == 0) {
                imageData = UIImagePNGRepresentation(photo);
            } else {
                imageData = UIImageJPEGRepresentation(photo, [self ImageCompressionRate]);
            }
            encodedImage = [imageData base64Encoding];
        }
        
        UnitySendMessage("IOSCamera", "OnImagePickedEvent", [ISNDataConvertor NSStringToChar:encodedImage]);
        
    }
    
}

+ (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 0.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    //[vc dismissModalViewControllerAnimated:YES];
    
    UnitySendMessage("IOSCamera", "OnImagePickedEvent", [ISNDataConvertor NSStringToChar:@""]);
}

extern "C" {
    
    
    //--------------------------------------
    //  IOS Native Plugin Section
    //--------------------------------------
    
    
    void _ISN_SaveToCameraRoll(char* encodedMedia) {
        NSString *media = [ISNDataConvertor charToNSString:encodedMedia];
        [[ISNCamera sharedInstance] saveToCameraRoll:media];
    }
    
    void _ISN_GetImageFromCamera() {
        [[ISNCamera sharedInstance] GetImageFromCamera];
    }
    
    void _ISN_GetVideoPathFromAlbum() {
        [[ISNCamera sharedInstance] GetVideoPathFromAlbum];
    }
    
    void _ISN_GetImageFromAlbum() {
        [[ISNCamera sharedInstance] GetImageFromAlbum];
    }
    
    void _ISN_InitCamerAPI(float compressionRate, int maxSize, int encodingType) {
        [[ISNCamera sharedInstance] setImageCompressionRate:compressionRate];
        [[ISNCamera sharedInstance] setMaxImageSize:maxSize / 2];
        [[ISNCamera sharedInstance] setEncodingType:encodingType];
    }
    
}


@end

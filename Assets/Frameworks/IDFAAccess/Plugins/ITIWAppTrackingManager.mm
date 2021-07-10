/****************************************************************************
 Copyright (c) 2010-2013 cocos2d-x.org
 Copyright (c) 2013-2017 Chukong Technologies Inc.
 
 http://www.cocos2d-x.org
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 ****************************************************************************/


#import "ITIWAppTrackingManager.h"

static ITIWAppTrackingManager *sharedInstance = nil;

@implementation ITIWAppTrackingManager

#pragma mark Singleton Methods
+ (ITIWAppTrackingManager*)sharedManager {
    if (sharedInstance == nil)
        sharedInstance = [[self alloc] init];
    
    return sharedInstance;
}

- (id)init
{
    self = [super init];
    if (self) {
        
    }
    return self;
}

-(void)requestTrackingAuthorization: (IDFAAccessRequestCallback*) callback
{
    if ([self shouldTrackingRequest]) {
        if (@available(iOS 14, *)) {
            [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
                // Tracking authorization completed. Start loading ads here.
                if(callback)callback(status == ATTrackingManagerAuthorizationStatusAuthorized);
            }];
            
        }
        else {
            if(callback)callback(true);
        }
    }
    else {
        if(callback)callback([self isAuthorized]);
    }
     
}

-(BOOL)isAuthorized
{
    if (@available(iOS 14, *)) {
        return ATTrackingManager.trackingAuthorizationStatus == ATTrackingManagerAuthorizationStatusAuthorized;
    } else {
        // Fallback on earlier versions
        return true;
    }
    return true;
}

-(BOOL)isDenied
{
    if (@available(iOS 14, *)) {
        return ATTrackingManager.trackingAuthorizationStatus == ATTrackingManagerAuthorizationStatusDenied;
    } else {
        // Fallback on earlier versions
        return false;
    }
    return false;
}

-(BOOL)isRestricted
{
    if (@available(iOS 14, *)) {
        return ATTrackingManager.trackingAuthorizationStatus == ATTrackingManagerAuthorizationStatusRestricted;
    } else {
        // Fallback on earlier versions
        return false;
    }
    return false;
}

-(BOOL)isNotDetermined
{
    if (@available(iOS 14, *)) {
        return ATTrackingManager.trackingAuthorizationStatus == ATTrackingManagerAuthorizationStatusNotDetermined;
    } else {
        // Fallback on earlier versions
        return false;
    }
    return false;
}

-(BOOL)shouldTrackingRequest
{
    if (@available(iOS 14, *)) {
        return (ATTrackingManager.trackingAuthorizationStatus == ATTrackingManagerAuthorizationStatusNotDetermined);
    } else {
        // Fallback on earlier versions
        return false;
    }
    return false;
}

-(void)showNativeAlert:(NSString *)_title message:(NSString *)_message callback:(NativeAlertResultCallback *)callback
{
    UIAlertController* alert = [UIAlertController alertControllerWithTitle: _title
                                   message:_message
                                   preferredStyle:UIAlertControllerStyleAlert];

    UIAlertAction* yesAction = [UIAlertAction actionWithTitle:@"Allow" style:UIAlertActionStyleDefault handler:^(UIAlertAction * action) {
        dispatch_async(dispatch_get_main_queue(), ^(void){
            //Run UI Updates
//            ITIWApplicationWrapper::sharedManager()->nativeAlertClicked(1); ---->
            if(callback)callback(true);
        });
    }];
    
    UIAlertAction* noAction = [UIAlertAction actionWithTitle:@"Not Now" style:UIAlertActionStyleDefault handler:^(UIAlertAction * action) {
        dispatch_async(dispatch_get_main_queue(), ^(void){
            //Run UI Updates
//            ITIWApplicationWrapper::sharedManager()->nativeAlertClicked(0); ---->
            if(callback)callback(false);
        });
    }];

    [alert addAction:yesAction];
    [alert addAction:noAction];
//    [alert autorelease];
    [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:alert animated:YES completion:nil];
}

@end

extern "C" {
    void ShowNativeAlertForIDFA (const char *title, const char *message, NativeAlertResultCallback callback) {
        
        NSString *nsTitle = [NSString stringWithUTF8String:title];
        NSString *nsMessage = [NSString stringWithUTF8String:message];
        
        [[ITIWAppTrackingManager sharedManager] showNativeAlert:nsTitle message:nsMessage callback:callback];
    }
}

extern "C" {
    void RequestIDFAAccess (IDFAAccessRequestCallback callback) {
        [[ITIWAppTrackingManager sharedManager] requestTrackingAuthorization: callback];
    }
}

extern "C" {
    bool IsAuthorized () {
        return [[ITIWAppTrackingManager sharedManager] isAuthorized];
    }
}

extern "C" {
    bool IsDenied () {
        return [[ITIWAppTrackingManager sharedManager] isDenied];
    }
}

#include "UnityFramework/UnityFramework-Swift.h"

extern "C" {
    void _GenerateHapticFromPattern (const char* pattern) {
        NSLog(@"Ashlog: Native iOS _GenerateHapticFromPattern");
        
        NSString *nsPattern = [NSString stringWithUTF8String:pattern];
        [[HapticSwiftBridge shared]generatePattern: nsPattern];
    }



    void _GenerateLightImpact () {
        NSLog(@"Ashlog: Native iOS _GenerateLightImpact");
        [[HapticSwiftBridge shared]impactLight];
    }
    void _GenerateMediumImpact () {
        NSLog(@"Ashlog: Native iOS _GenerateMediumImpact");
        [[HapticSwiftBridge shared]impactMedium];
    }
    void _GenerateHeavyImpact () {
        NSLog(@"Ashlog: Native iOS _GenerateHeavyImpact");
        [[HapticSwiftBridge shared]impactHeavy];
    }
    void _GenerateSoftImpact () {
        NSLog(@"Ashlog: Native iOS _GenerateSoftImpact");
        [[HapticSwiftBridge shared]impactSoft];
    }
    void _GenerateRigidImpact () {
        NSLog(@"Ashlog: Native iOS _GenerateRigidImpact");
        [[HapticSwiftBridge shared]impactRigid];
    }

    void _StartHapticEngine () {
        NSLog(@"Ashlog: Native iOS _StartHapticEngine");
        [[HapticSwiftBridge shared]startEngineIfNotRunning];
    }
    void _PlayCustomHaptic(float intensity, float sharpness, double duration, double startDelay){
        NSLog(@"Ashlog: Native iOS _PlayCustomHaptic");
        [[HapticSwiftBridge shared]playCustomHaptic:intensity sharpness:sharpness duration:duration startDelay:startDelay];
    }


    void _PlayWalkHaptic(double duration){
        NSLog(@"Ashlog: Native iOS _PlayWalkHaptic");
        if([[HapticSwiftBridge shared]haveHapticSupport]){
            [[HapticSwiftBridge shared]playCustomHaptic:0.245 sharpness:0.3 duration:0 startDelay:0];
        }
        else{
            [[HapticSwiftBridge shared]impactSoft];
        }
    }

    void _PlayTreeChopHaptic(double duration){
        NSLog(@"Ashlog: Native iOS _PlayTreeChopHaptic");
        if([[HapticSwiftBridge shared]haveHapticSupport]){
            [[HapticSwiftBridge shared]playCustomHaptic:0.6 sharpness:1 duration:0.15 startDelay:0.1];
        }
        else{
            [[HapticSwiftBridge shared]generatePattern:@"-o-o"];
        }
    }
    void _PlayTreeChopCompletedHaptic(double duration){
        NSLog(@"Ashlog: Native iOS _PlayTreeChopCompletedHaptic");
        if([[HapticSwiftBridge shared]haveHapticSupport]){
//            [[HapticSwiftBridge shared]playCustomHaptic:0.25 sharpness:0.1 duration:0.15 startDelay:0.2];
//            [[HapticSwiftBridge shared]playCustomHaptic:0.4 sharpness:0.25 duration:0 startDelay:0.2];
        }
        else{
            
        }
    }

    void _PlayBuildHaptic(double duration){
        NSLog(@"Ashlog: Native iOS _PlayBuildHaptic");
        if([[HapticSwiftBridge shared]haveHapticSupport]){
            [[HapticSwiftBridge shared]playCustomHaptic:0.49 sharpness:0.50 duration:0 startDelay:0.1];
        }
        else{
            [[HapticSwiftBridge shared]generatePattern:@"-O"];
        }
    }
    void _PlayBuildCompletedHaptic(double duration){
        NSLog(@"Ashlog: Native iOS _PlayBuildCompletedHaptic");
        if([[HapticSwiftBridge shared]haveHapticSupport]){
            [[HapticSwiftBridge shared]playCustomHaptic:0.63 sharpness:0.6 duration:0.15 startDelay:0.1];
        }
        else{
            [[HapticSwiftBridge shared]generatePattern:@"-O-O-O-o"];
        }
    }

    void _PlayShootEnemyHaptic(double duration){
        NSLog(@"Ashlog: Native iOS _PlayShootEnemyHaptic");
        if([[HapticSwiftBridge shared]haveHapticSupport]){
            [[HapticSwiftBridge shared]playCustomHaptic:0.35 sharpness:0.4 duration:0 startDelay:0];
        }
        else{
            [[HapticSwiftBridge shared]impactMedium];
        }
    }
    void _PlayEnemyDestroyedHaptic(double duration){
        NSLog(@"Ashlog: Native iOS _PlayEnemyDestroyedHaptic");
        if([[HapticSwiftBridge shared]haveHapticSupport]){
            [[HapticSwiftBridge shared]playCustomHaptic:0.49 sharpness:0.5 duration:0.1 startDelay:0];
        }
        else{
            [[HapticSwiftBridge shared]impactRigid];
        }
    }
}

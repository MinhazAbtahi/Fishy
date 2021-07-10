import Foundation
import Haptica
import CoreHaptics

@objc public class HapticSwiftBridge : NSObject {
    
    @objc public static let shared = HapticSwiftBridge()
    
    
    @objc public func generatePattern(_ pattern : String) {
        
        print("HapticSwiftBridge -> generatePattern. pattern: \(pattern)")
        Haptic.play(pattern, delay: 0.1)
    }
    
    public func impact(_ style : HapticFeedbackStyle) {
        
        print("HapticSwiftBridge -> impact. style: \(style)")
        Haptic.impact(style).generate()
    }
    
    @objc public func impactLight() {
        impact(.light)
    }
    @objc public func impactMedium() {
        impact(.medium)
    }
    @objc public func impactHeavy() {
        impact(.heavy)
    }
    @objc public func impactSoft() {
        if #available(iOS 13.0, *) {
            impact(.soft)
        } else {
            impact(.light)
        }
    }
    @objc public func impactRigid() {
        if #available(iOS 13.0, *) {
            impact(.rigid)
        } else {
            impact(.heavy)
        }
    }
    
    @objc public func haveHapticSupport() -> Bool{
        if #available(iOS 13.0, *) {
            return HapticExp.shared.supportsHaptics
        } else {
            return false
        }
    }
    
    @objc public func startEngineIfNotRunning(){
        if #available(iOS 13.0, *) {
            HapticExp.shared.startEngineIfNotRunning()
        } else {
        }
    }

    @objc public func playCustomHaptic(_ intensity: Float, sharpness: Float, duration: Double, startDelay: Double){
        if #available(iOS 13.0, *) {
            HapticExp.shared.playCustomHaptic(intensity, sharpness: sharpness, duration: duration, startDelay: startDelay)
        } else {
        }
    }
}


@available(iOS 13.0, *)
public class HapticExp : NSObject {
    
    
    // Haptic Engine & State:
    private var engine: CHHapticEngine!
    private var engineNeedsStart = true
    var supportsHaptics: Bool = {
        return CHHapticEngine.capabilitiesForHardware().supportsHaptics
    }()
    
    public static let shared = HapticExp()
    
    public func startEngineIfNotRunning(){
        if (engineNeedsStart){
            createAndStartHapticEngine()
        }
    }
    
    private func createAndStartHapticEngine() {
        guard supportsHaptics else { return }
        
        // Create and configure a haptic engine.
        do {
            engine = try CHHapticEngine()
        } catch let error {
            fatalError("Engine Creation Error: \(error)")
        }
        
        // The stopped handler alerts engine stoppage.
        engine.stoppedHandler = { reason in
            print("Stop Handler: The engine stopped for reason: \(reason.rawValue)")
            switch reason {
            case .audioSessionInterrupt:
                print("Audio session interrupt.")
            case .applicationSuspended:
                print("Application suspended.")
            case .idleTimeout:
                print("Idle timeout.")
            case .notifyWhenFinished:
                print("Finished.")
            case .systemError:
                print("System error.")
            case .engineDestroyed:
                print("Engine destroyed.")
            case .gameControllerDisconnect:
                print("Controller disconnected.")
            @unknown default:
                print("Unknown error")
            }
            
            // Indicate that the next time the app requires a haptic, the app must call engine.start().
            self.engineNeedsStart = true
        }
        
        // The reset handler notifies the app that it must reload all its content.
        // If necessary, it recreates all players and restarts the engine in response to a server restart.
        engine.resetHandler = {
            print("The engine reset --> Restarting now!")
            
            // Tell the rest of the app to start the engine the next time a haptic is necessary.
            self.engineNeedsStart = true
        }
        
        // Start haptic engine to prepare for use.
        do {
            try engine.start()
            
            // Indicate that the next time the app requires a haptic, the app doesn't need to call engine.start().
            engineNeedsStart = false
        } catch let error {
            print("The engine failed to start with error: \(error)")
        }
    }
    
    func playCustomHaptic(_ intensity: Float, sharpness: Float, duration: Double, startDelay: Double) {
        guard supportsHaptics else { return }
        // Play haptic here.
        do {
            // Start the engine if necessary.
            if engineNeedsStart {
                try engine.start()
                engineNeedsStart = false
            }
            // Create a haptic pattern player from normalized magnitude.
            let hapticPlayer = try createPlayer(intensity, sharpness: sharpness, duration: duration, startDelay: startDelay)
            
            // Start player, fire and forget
            try hapticPlayer?.start(atTime: CHHapticTimeImmediate)
        } catch let error {
            print("Haptic Playback Error: \(error)")
        }
    }
    
    private func createPlayer(_ intensity: Float, sharpness: Float, duration: Double, startDelay: Double) throws -> CHHapticPatternPlayer? {

        var eventType = CHHapticEvent.EventType.hapticTransient
        
        if duration > 0{
            eventType = CHHapticEvent.EventType.hapticContinuous
        }
        
        let hapticEvent = CHHapticEvent(eventType: eventType, parameters: [
            CHHapticEventParameter(parameterID: .hapticSharpness, value: sharpness),
            CHHapticEventParameter(parameterID: .hapticIntensity, value: intensity)
        ], relativeTime: startDelay, duration: duration)

        let pattern = try CHHapticPattern(events: [hapticEvent], parameters: [])
        return try engine.makePlayer(with: pattern)
    }
    
    private func linearInterpolation(alpha: Float, min: Float, max: Float) -> Float {
        return min + alpha * (max - min)
    }
    
}



//    private func playerForMagnitude(_ magnitude: Float) throws -> CHHapticPatternPlayer? {
//        let volume = linearInterpolation(alpha: magnitude, min: 0.1, max: 0.4)
//        let decay: Float = linearInterpolation(alpha: magnitude, min: 0.0, max: 0.1)
//        let audioEvent = CHHapticEvent(eventType: .audioContinuous, parameters: [
//            CHHapticEventParameter(parameterID: .audioPitch, value: -0.15),
//            CHHapticEventParameter(parameterID: .audioVolume, value: volume),
//            CHHapticEventParameter(parameterID: .decayTime, value: decay),
//            CHHapticEventParameter(parameterID: .sustained, value: 0)
//        ], relativeTime: 0)
//
//        let sharpness = linearInterpolation(alpha: magnitude, min: 0.9, max: 0.5)
//        let intensity = linearInterpolation(alpha: magnitude, min: 0.375, max: 1.0)
//        let hapticEvent = CHHapticEvent(eventType: .hapticTransient, parameters: [
//            CHHapticEventParameter(parameterID: .hapticSharpness, value: sharpness),
//            CHHapticEventParameter(parameterID: .hapticIntensity, value: intensity)
//        ], relativeTime: 0)
//
//        let pattern = try CHHapticPattern(events: [audioEvent, hapticEvent], parameters: [])
//        return try engine.makePlayer(with: pattern)
//    }

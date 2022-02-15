#import <UIKit/UIKit.h>
#import <AudioToolBox/AudioToolBox.h>

extern "C" void IOSVibrator(int _n)
{
    AudioServicesPlaySystemSound(_n);
}

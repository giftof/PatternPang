using System;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

public class Vibrate
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass AndroidPlayer;
    public static AndroidJavaObject AndroidcurrentActivity;
    public static AndroidJavaObject AndroidVibrator;
#elif !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void IOSVibrator(int _n);
    private static int m_haptic_strength;
#endif

    private static readonly Lazy<Vibrate> m_instance = new Lazy<Vibrate>(()
    => new Vibrate());

    public static Vibrate Instance
        => m_instance.Value;

    private Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#elif !UNITY_EDITOR
        m_haptic_strength = 1519; // 1519: weak, 1520: strong
#endif
    }

    public void Do()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate");
#elif !UNITY_EDITOR
        IOSVibrator(m_haptic_strength);
#endif
    }

    public void Do(long milliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", milliseconds);
#elif !UNITY_EDITOR
        IOSVibrator(m_haptic_strength);
#endif
    }

    public void Do(long[] pattern, int repeat = -1)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", pattern, repeat);
#elif !UNITY_EDITOR
        IOSVibrator(m_haptic_strength);
#endif
    }

    public void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("cancel");
#endif
    }
}



//public static class Vibrate
//{
//#if UNITY_ANDROID && !UNITY_EDITOR
//    public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//    public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//    public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
//#elif !UNITY_EDITOR
//    [DllImport("__Internal")]
//    public static extern void IOSVibrator(int _n);
//    private static int m_haptic_strength = 1519; // 1519: weak, 1520: strong
//#endif

//    public static void Do()
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidVibrator.Call("vibrate");
//#elif !UNITY_EDITOR
//        IOSVibrator(m_haptic_strength);
//#endif
//    }

//    public static void Do(long milliseconds)
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidVibrator.Call("vibrate", milliseconds);
//#elif !UNITY_EDITOR
//        IOSVibrator(m_haptic_strength);
//#endif
//    }

//    public static void Do(long[] pattern, int repeat = -1)
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidVibrator.Call("vibrate", pattern, repeat);
//#elif !UNITY_EDITOR
//        IOSVibrator(m_haptic_strength);
//#endif
//    }

//    public static void Cancel()
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidVibrator.Call("cancel");
//#endif
//    }
//}
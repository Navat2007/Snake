using System;

public static class EventBus
{
    public static Action StartLevelEvent;
    public static Action GameOverEvent;
    public static Action<bool> ExitLevelEvent;
    
    public static Action PauseEvent;
    public static Action UnPauseEvent;
    
    public static Action AppleCollectEvent;
}
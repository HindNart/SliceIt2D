using System;

public static class CutEventManager
{
    public static event Action<float> OnCutPerformed;

    public static void NotifyCutPerformed(float ratio)
    {
        OnCutPerformed?.Invoke(ratio);
    }
}
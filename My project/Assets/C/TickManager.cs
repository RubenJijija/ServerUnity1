using Unity.Netcode;

public static class TickManager
{
    // Definimos ticks a 60 por segundo
    public static uint CurrentTick
    {
        get
        {
            // NetworkTime.LocalTime es el reloj sincronizado de Netcode
            float time = NetworkManager.Singleton.LocalTime.TimeAsFloat;
            return (uint)(time * 60f);
        }
    }
}
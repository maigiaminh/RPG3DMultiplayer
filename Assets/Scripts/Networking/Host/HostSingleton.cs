using UnityEngine;

public class HostSingleton : Singleton<HostSingleton>
{
    public HostManager HostManager { get; private set; }

    public void CreateHost()
    {
        HostManager = new HostManager();
    }


    protected override void OnDestroy() {
        base.OnDestroy();
        StopCoroutine(nameof(HostManager.SendHeartBeatLobby));
        HostManager.Dispose();
    }
}
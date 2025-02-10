using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : Singleton<ServerSingleton>
{
    public async void CreateServer()
    {
        await UnityServices.Instance.InitializeAsync();
    }
    
}

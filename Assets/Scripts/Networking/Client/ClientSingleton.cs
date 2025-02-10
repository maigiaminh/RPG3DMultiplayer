using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : Singleton<ClientSingleton>
{
    public ClientManager ClientManager;




    public async Task<bool> CreateClient()
    {
        ClientManager = new ClientManager();

        return await ClientManager.InitAsync();
    }


    protected override void OnDestroy() {
        base.OnDestroy();
        ClientManager.Dispose();
    }
}

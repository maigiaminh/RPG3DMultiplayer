using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class ApplicationController : Singleton<ApplicationController>
{
    [SerializeField] private ClientSingleton ClientSingleton;
    [SerializeField] private HostSingleton HostSingleton;
    [SerializeField] private ServerSingleton ServerSingleton;


    protected override async void Awake()
    {
        if (LoadingManager.Instance)
        {
            LoadingManager.Instance.loadingPanel.SetActive(true);
            LoadingManager.Instance.progressBarText.text = "Initializing Server...";
        }
        await LaunchInMode(SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        base.Awake();

        //await Task.Delay(5000);
        
        if (isDedicatedServer)
        {
            ServerSingleton serverSingleton = Instantiate(ServerSingleton);
            serverSingleton.CreateServer();
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(HostSingleton);
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(ClientSingleton);

            bool authenticated = await clientSingleton.CreateClient();
            if (authenticated)
            {
                Debug.Log("Authenticated");
            }
            else
            {
                Debug.Log("Not Authenticated");
            }
        }

        LoadNextScene();

    }
    private void LoadNextScene()
    {
        if (LoadingManager.Instance) LoadingManager.Instance.NewLoadScene("Login Scene");
    }

}

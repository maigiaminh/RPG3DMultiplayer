using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class TestStartHost : MonoBehaviour
{
    [SerializeField] private Button startHostBtn;

    private void Start() {
        startHostBtn.onClick.AddListener(StartHost);
    }
    public async void StartHost(){
        await HostSingleton.Instance.HostManager.StartHostAsync();
    }
}

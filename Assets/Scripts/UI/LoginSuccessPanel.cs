using UnityEngine;

public class LoginSuccessPanel : MonoBehaviour
{
    public Camera camera6th;
    public Camera camera7th;

    public void LoginSuccess(){
        camera6th.enabled = false;
        camera7th.enabled = true;
    }

    public void LogOut(){
        camera6th.enabled = true;
        camera7th.enabled = false;
    }
}

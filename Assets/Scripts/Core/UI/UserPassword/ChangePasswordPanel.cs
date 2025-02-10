using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangePasswordPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField currentPass;
    [SerializeField] TMP_InputField newPass;
    [SerializeField] TMP_InputField confirmPass;
    [SerializeField] TextMeshProUGUI resultTxt;

    public void ClearResultText(){
        resultTxt.text = "";
    }
    public async void ChangePass(){
        if(string.IsNullOrEmpty(currentPass.text)
        || string.IsNullOrEmpty(newPass.text)
        || string.IsNullOrEmpty(confirmPass.text)){
            resultTxt.text = "Please fill in all field";
            resultTxt.color = Color.red;
        }
        else if(!newPass.text.Equals(confirmPass.text)){
            resultTxt.text = "New Password and Confirm Password are not match";
            resultTxt.color = Color.red;
        }
        else{
            string result = await FirebaseAuthManager.Instance.ChangePasswordAsync(currentPass.text, newPass.text);
            resultTxt.text = result;
            if(result.Equals("Password updated successfully.")){
                resultTxt.color = Color.green;
                currentPass.text = "";
                newPass.text = "";
                confirmPass.text = "";
            }
            else resultTxt.color = Color.red;
        }
    }
}

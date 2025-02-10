using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System;
using Firebase.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Firebase.Extensions;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Firebase.RemoteConfig;
using Facebook.Unity;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    // Firebase variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference DBReference;
    private FirebaseRemoteConfig remoteConfig;

    // Login Variables
    [Space]

    [Space]
    [Header("Login")]
    public TextMeshProUGUI loginResponseText;
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public TextMeshProUGUI registerResponseText;
    public TMP_InputField nameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;

    // Change Password Variables
    [Space]
    [Header("Change Password")]
    public TMP_InputField emailChangePasswordField;
    public TextMeshProUGUI stateChangePasswordText;


    public bool isAutoLogin = false;
    public Button AutoSkipBtn;

    private const string DATABASEURL = "https://duancntt-f5790-default-rtdb.asia-southeast1.firebasedatabase.app/";


    private const string GAME_SCENE = "MAINGAMESCENE";
    private const string CHOOSE_CHARACTER_SCENE_NAME = "ChooseCharacterScene";

    private const int _defaultGold = 0;
    private const int _defaultMisc = 0;
    private const int _defaultXp = 0;
    private const int _defaultLevel = 1;
    private const int _defaultMaxLevel = 1;
    private const string _defaultCharacterClassName = "";
    private const string _defaultAvatarName = "";
    private const string InitialSkin = "SM_Chr_Soldier_Female_02";
    private const string InitialHair = "SM_Chr_Hair_Female_01";
    private const string InitialBeard = "SM_Chr_Hair_Beard_01";
    private const string InitialHelmet = "SM_Chr_Attach_Crown_Leaf_01";
    private string ClientID;
    private string ClientSecret;
    private string FacebookAppID;
    private string FacebookAppSecret;
    public List<ItemData> InitialItems = new List<ItemData>();

    public bool IsUserDataSaving = false;
    public bool IsInventorySaving = false;
    public bool IsResourceSaving = false;
    public bool IsSaving
    {
        get => IsUserDataSaving || IsInventorySaving || IsResourceSaving;
        set { IsUserDataSaving = value; IsInventorySaving = value; IsResourceSaving = value; }
    }
    private readonly Dictionary<string, object> defaults =
        new Dictionary<string, object>
        {
            { "clientId", "default_client_id" },
            { "clientSecret", "default_client_secret" },
            { "facebook_app_id", "default_facebook_app_id" },
            { "facebook_app_secret", "default_facebook_app_secret" }
        };

    private void Start() => StartCoroutine(CheckAndFixCheckAndFixDependenciesAsync());

    // private void OnDisable()
    // {
    //     StartCoroutine(SaveDataAndQuit());
    // }

    // private IEnumerator SaveDataAndQuit()
    // {
    //     SaveData();

    //     yield return new WaitForSeconds(1f);
    //     Application.Quit();
    // }

    private IEnumerator CheckAndFixCheckAndFixDependenciesAsync()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        dependencyStatus = dependencyTask.Result;

        if (dependencyStatus == DependencyStatus.Available)
        {
            remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            InitializeFirebase();
            InitializeRemoteConfig();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
        }
        else
        {
            Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
        }

        if (!FB.IsInitialized) {
            FB.Init(InitCallback, OnHideUnity);
        } else {
            FB.ActivateApp();
        }
    }
    private void InitCallback ()
    {
        if (FB.IsInitialized) {
            FB.ActivateApp();
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity (bool isGameShown)
    {
        if (!isGameShown) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

    void InitializeFirebase()
    {
        //Set the default instance object
        auth = FirebaseAuth.DefaultInstance;

        FirebaseApp app = FirebaseApp.DefaultInstance;

        // Chỉ định DatabaseURL
        DBReference = FirebaseDatabase.GetInstance(app, DATABASEURL).RootReference;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private void InitializeRemoteConfig()
    {
        remoteConfig.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Default values set.");
            FetchRemoteConfig();
        });
    }

    private void FetchRemoteConfig()
    {
        // Tải giá trị từ Firebase Remote Config
        remoteConfig.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Remote Config fetched successfully.");
                remoteConfig.ActivateAsync().ContinueWithOnMainThread(activateTask =>
                {
                    if (activateTask.Result)
                    {
                        Debug.Log("Remote Config activated.");
                        UseRemoteConfigValues();
                    }
                });
            }
            else
            {
                Debug.LogError("Failed to fetch Remote Config.");
            }
        });
    }

    private void UseRemoteConfigValues()
    {
        string clientId = remoteConfig.GetValue("clientId").StringValue;
        string clientSecret = remoteConfig.GetValue("clientSecret").StringValue;
        string facebookAppId = remoteConfig.GetValue("facebook_app_id").StringValue;
        string facebookAppSecret = remoteConfig.GetValue("facebook_app_secret").StringValue;

        ClientID = clientId;
        ClientSecret = clientSecret;
        FacebookAppID = facebookAppId;
        FacebookAppSecret = facebookAppSecret;
    }

    private IEnumerator CheckForAutoLogin()
    {
        if (!isAutoLogin) yield break;
        user = null;
        if (user != null)
        {
            var reloadUserTask = user.DeleteAsync();

            yield return new WaitUntil(() => reloadUserTask.IsCompleted);
            Debug.Log("User is auto login + " + user.UserId);

            AutoLogin();
        }
        else
        {
            LoginRegisterUIManager.Instance.ActivateLoginRegisterPanel(true);
        }
    }

    private void AutoLogin()
    {
        if (user == null) LoginRegisterUIManager.Instance.ActivateLoginRegisterPanel(true);

        if (user.IsEmailVerified)
        {

            AutoSkipBtn.onClick.Invoke();
            UserContainer.Instance.SetName(user.DisplayName);
            // OpenGameScene();
            LoginRegisterUIManager.Instance.ShowGamePanel();
        }
        else
        {
            SendEmailForVerification();
        }

    }



    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                LoginRegisterUIManager.Instance.ActivateLoginRegisterPanel(true);
                ClearLoginRegisterFields();
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }


    public void Login()
    {
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failedMessage = "Login Failed! Because ";
            string temp = "";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    temp = "Email is invalid";
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    temp = "Wrong Password";
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    temp = "Email is missing";
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    temp = "Password is missing";
                    failedMessage += "Password is missing";
                    break;
                default:
                    temp = "Login Failed";
                    failedMessage = "Login Failed";
                    break;
            }

            Debug.Log(failedMessage);
            loginResponseText.text = temp;
        }
        else
        {
            user = loginTask.Result.User;

            Debug.LogFormat("You Are Successfully Logged In {0}", user.DisplayName);
            loginResponseText.text = "You Are Successfully Logged In " + user.DisplayName;

            if (user.IsEmailVerified)
            {
                LoginRegisterUIManager.Instance.ShowGamePanel();
                LoadData();
            }
            else
            {
                SendEmailForVerification();
            }

        }
    }

    public void Register()
    {
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (name == "")
        {
            registerResponseText.text = "User Name is empty";
            Debug.LogError("User Name is empty");
            yield break;
        }
        if (email == "")
        {
            registerResponseText.text = "email field is empty";
            Debug.LogError("email field is empty");
            yield break;
        }
        if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            registerResponseText.text = "Password does not match";
            Debug.LogError("Password does not match");
            yield break;
        }

        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogError(registerTask.Exception);

            FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Registration Failed! Because ";
            string temp = "";
            switch (authError)
            {
                case AuthError.InvalidEmail:
                    temp = "Email is invalid";
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    temp = "Wrong Password";
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    temp = "Email is missing";
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    temp = "Password is missing";
                    failedMessage += "Password is missing";
                    break;
                default:
                    temp = "Registration Failed";
                    failedMessage = "Registration Failed";
                    break;
            }

            Debug.Log(failedMessage);
            registerResponseText.text = temp;
            yield break;
        }

        // Get The User After Registration Success
        user = registerTask.Result.User;

        UserProfile userProfile = new UserProfile
        {
            DisplayName = name
        };

        var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

        yield return new WaitUntil(() => updateProfileTask.IsCompleted);

        if (updateProfileTask.Exception != null)
        {
            // Delete the user if user update failed
            user.DeleteAsync();

            Debug.LogError(updateProfileTask.Exception);

            FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failedMessage = "Profile update Failed! Becuase ";
            string temp = "";
            switch (authError)
            {
                case AuthError.InvalidEmail:
                    temp = "Email is invalid";
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    temp = "Wrong Password";
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    temp = "Email is missing";
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    temp = "Password is missing";
                    failedMessage += "Password is missing";
                    break;
                default:
                    temp = "Profile update Failed";
                    failedMessage = "Profile update Failed";
                    break;
            }
            Debug.Log(failedMessage);
            registerResponseText.text = temp;
        }
        else
        {
            Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
            registerResponseText.text = "Registration Sucessful Welcome " + user.DisplayName;
            // UIManager.Instance.OpenLoginPanel();
        }
    }


    public void ForgetPasswordSubmit()
    {
        string email = emailChangePasswordField.text;
        auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                stateChangePasswordText.text = "Email not found";
                return;
            }

            stateChangePasswordText.text = "Password reset email sent successfully to " + email;
        });
    }

    public async Task<string> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        if (user == null)
        {
            return "User not signed in.";
        }

        try
        {
            await auth.SignInWithEmailAndPasswordAsync(user.Email, currentPassword);

            await user.UpdatePasswordAsync(newPassword);
            return "Password updated successfully.";
        }
        catch (AggregateException ex)
        {
            foreach (var innerException in ex.InnerExceptions)
            {
                if (innerException is FirebaseException firebaseEx)
                {
                    switch ((AuthError)firebaseEx.ErrorCode)
                    {
                        case AuthError.WrongPassword:
                            return "Current password is incorrect.";
                        case AuthError.UserNotFound:
                            return "User not found.";
                        default:
                            return "Password update failed: " + firebaseEx.Message;
                    }
                }
            }
            return "An unexpected error occurred.";
        }
        catch (Exception ex)
        {
            return "Unexpected error: " + ex.Message;
        }
    }

    public void SendEmailForVerification()
    {
        StartCoroutine(SendEmailForVerificationAsync());
    }

    IEnumerator SendEmailForVerificationAsync()
    {
        if (user != null)
        {
            var sendEmailTask = user.SendEmailVerificationAsync();

            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            if (sendEmailTask.Exception != null)
            {
                FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;

                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string errorMessage = "Unknown Error : Please try again later";

                switch (authError)
                {
                    case AuthError.Cancelled:
                        errorMessage = "Email Verification Cancelled";
                        break;
                    case AuthError.TooManyRequests:
                        errorMessage = "Too Many Requests";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        errorMessage = "Invalid Recipient Email";
                        break;

                }
                LoginRegisterUIManager.Instance.ShowEmailVertificationPanel(false, user.Email, errorMessage);
            }
            else
            {
                Debug.Log("Email Verification Sent");
                if (user.IsEmailVerified)
                {
                    LoginRegisterUIManager.Instance.ActivateLoginRegisterPanel(true);
                }
                else
                {
                    SendEmailForVerification();
                }
            }
        }
    }

    public void OpenGameScene()
    {
        string sceneName;
        if (UserContainer.Instance == null || UserContainer.Instance.UserData == null || UserContainer.Instance.UserData.CharacterClassName == "")
        {
            sceneName = CHOOSE_CHARACTER_SCENE_NAME;
        }
        else
        {
            sceneName = GAME_SCENE;
        }

        LoadingManager.Instance.NewLoadScene(sceneName);

        UpdateAttributeBeforeGameStart();
    }

    private void UpdateAttributeBeforeGameStart()
    {
        StartCoroutine(UpdateAvatar(AvatarManager.Instance.GetCurrentChooseAvatarItemName()));
    }

    public void LogOut()
    {
        if (auth != null && user != null)
        {
            UserContainer.Instance.SetName("");
            UserContainer.Instance.SetUserData(new UserData()
            {
                Name = "",
                Xp = 0,
                Level = 0,
                MaxLevel = 0,
                Gold = 0,
                Misc = 0,
                CharacterClassName = "",
                AvatarName = ""
            });
            auth.SignOut();
        }
    }


    private void ClearLoginRegisterFields()
    {
        registerResponseText.text = "";
        loginResponseText.text = "";
        emailLoginField.text = "";
        passwordLoginField.text = "";
        nameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        confirmPasswordRegisterField.text = "";
    }






    #region Save Data
    public void SaveUserData()
    {
        if (PlayerLevelManager.Instance == null || ResourceManager.Instance == null)
        {
            return;
        }
        int xp = PlayerLevelManager.Instance ? PlayerLevelManager.Instance.Experience : _defaultXp;
        int level = PlayerLevelManager.Instance ? PlayerLevelManager.Instance.Level : _defaultLevel;
        int maxLevel = PlayerLevelManager.Instance ? PlayerLevelManager.Instance.MaxLevel : _defaultMaxLevel;
        int gold = ResourceManager.Instance ? ResourceManager.Instance.Gold : _defaultGold;
        int misc = ResourceManager.Instance ? ResourceManager.Instance.Misc : _defaultMisc;

        StartCoroutine(UpdateCharacterStats(xp, level, maxLevel));
        StartCoroutine(UpdateResource(gold, misc));
    }


    public void SaveUserInitData()
    {
        UserContainer.Instance.SetUserData(new UserData()
        {
            Name = user.DisplayName,
            Xp = _defaultXp,
            Level = _defaultLevel,
            MaxLevel = _defaultMaxLevel,
            Gold = _defaultGold,
            Misc = _defaultMisc,
            AvatarName = _defaultAvatarName,
            CharacterClassName = _defaultCharacterClassName
        });

        StartCoroutine(UpdateResource(_defaultGold, _defaultMisc));
        StartCoroutine(UpdateAvatar(_defaultAvatarName));
    }
    public async void SaveInventoryInitData()
    {
        await UpdateInventoryItemsAsync(InitialItems.ToDictionary(item => item, item => 1));
        InventoryContainer.Instance.PlayerItemMap = InitialItems.ToDictionary(item => item, item => 1);
    }

    public void SaveOutitInitData()
    {
        StartCoroutine(UpdateCharacterOutfits(new OutfitData()
        {
            SkinName = InitialSkin,
            HairName = InitialHair,
            BeardName = InitialBeard,
            HelmetName = InitialHelmet
        }));
        OutfitContainer.Instance.SetOutfitData(new OutfitData()
        {
            SkinName = InitialSkin,
            HairName = InitialHair,
            BeardName = InitialBeard,
            HelmetName = InitialHelmet
        });
    }

    public void SaveSkillLeveData(SkillLevelData skillLevelData)
    {
        if (skillLevelData == null)
        {
            Debug.LogError("Skill Level Data is null");
            return;
        }
        StartCoroutine(UpdateSkillLevelData(skillLevelData));
    }

    public void SaveQuest(Quest quest)
    {
        StartCoroutine(UpdateQuest(quest));
    }


    public async void SaveInventoryItems(Dictionary<ItemData, int> inventoryItems)
    {
        await UpdateInventoryItemsAsync(inventoryItems);
    }

    IEnumerator UpdateSkillLevelData(SkillLevelData skillLevelData)
    {
        // Chuyển SkillLevelData thành chuỗi JSON
        string jsonData = JsonUtility.ToJson(skillLevelData);

        // Lưu dữ liệu chuỗi JSON vào Firebase
        var DBTask = DBReference.Child("Users").Child(user.UserId).Child("Skill").Child("SkillLevel")
            .SetRawJsonValueAsync(jsonData);
        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(DBTask.Exception);
        }
        else
        {
            Debug.Log("Skill Level Data Updated");
        }

    }


    IEnumerator UpdateCharacterStats(int xp, int level, int maxLevel)
    {
        IsUserDataSaving = true;
        // Lưu tên người chơi vào Firebase
        var DBTaskName = DBReference.Child("Users").Child(user.UserId).Child("Username").SetValueAsync(user.DisplayName);

        yield return new WaitUntil(() => DBTaskName.IsCompleted);

        if (DBTaskName.Exception != null)
        {
            Debug.LogError(DBTaskName.Exception);
        }
        else
        {
            Debug.Log("Username Updated");
        }


        // Lưu dữ liệu Character Stats
        string characterStatPath = "Users/" + user.UserId + "/CharacterStats/";

        CharacterStat characterStat = PlayerStatManager.Instance.GetCharacterStat();
        string serializedData = JsonUtility.ToJson(characterStat);

        var DBTaskStats = DBReference.Child(characterStatPath).SetRawJsonValueAsync(serializedData);

        yield return new WaitUntil(() => DBTaskStats.IsCompleted);

        if (DBTaskStats.Exception != null)
        {
            Debug.LogError(DBTaskStats.Exception);
        }
        else
        {
            Debug.Log("Character Stats Updated");
        }

        // Lưu dữ liệu XP
        var DBTaskXp = DBReference.Child("Users").Child(user.UserId).Child("Xp").SetValueAsync(xp);

        yield return new WaitUntil(() => DBTaskXp.IsCompleted);

        if (DBTaskXp.Exception != null)
        {
            Debug.LogError(DBTaskXp.Exception);
        }
        else
        {
            Debug.Log("XP Updated");
        }

        // Lưu dữ liệu Level
        if (UserContainer.Instance.UserData != null && level > UserContainer.Instance.UserData.MaxLevel)
        {
            // Nếu level hiện tại lớn hơn max level, cập nhật max level

            Task DBTaskMaxLevel = DBReference.Child("Users").Child(user.UserId).Child("MaxLevel").SetValueAsync(maxLevel);

            yield return new WaitUntil(() => DBTaskMaxLevel.IsCompleted);

            if (DBTaskMaxLevel.Exception != null)
            {
                Debug.LogError(DBTaskMaxLevel.Exception);
            }
            else
            {
                Debug.Log("Max Level Updated");
            }
        }

        var DBTaskLevel = DBReference.Child("Users").Child(user.UserId).Child("Level").SetValueAsync(level);

        yield return new WaitUntil(() => DBTaskLevel.IsCompleted);

        if (DBTaskLevel.Exception != null)
        {
            Debug.LogError(DBTaskLevel.Exception);
        }
        else
        {
            Debug.Log("Level Updated");
        }

        IsUserDataSaving = false;
    }

    IEnumerator UpdateResource(int gold, int misc)
    {
        IsResourceSaving = true;


        var DBTaskGold = DBReference.Child("Users").Child(user.UserId).Child("Gold").SetValueAsync(gold);
        yield return new WaitUntil(() => DBTaskGold.IsCompleted);

        if (DBTaskGold.Exception != null)
        {
            Debug.LogError(DBTaskGold.Exception);
        }
        else
        {
            Debug.Log("Gold Updated");
        }


        var DBTaskMisc = DBReference.Child("Users").Child(user.UserId).Child("Misc").SetValueAsync(misc);

        yield return new WaitUntil(() => DBTaskMisc.IsCompleted);

        if (DBTaskMisc.Exception != null)
        {
            Debug.LogError(DBTaskMisc.Exception);
        }
        else
        {
            Debug.Log("Misc Updated");
        }
        IsResourceSaving = false;
    }


    IEnumerator UpdateQuest(Quest quest)
    {
        string questPath = "Users/" + user.UserId + "/Quests/" + quest.questInfo.id; // Đường dẫn "Users/{userId}/Quests/{questId}"
                                                                                     // Lấy dữ liệu quest và chuyển đổi thành JSON
        QuestData questData = quest.GetQuestData();
        string serializedData = JsonUtility.ToJson(questData);

        // Lưu quest data vào Firebase
        var DBTask = DBReference.Child(questPath).SetRawJsonValueAsync(serializedData);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(DBTask.Exception);
        }
        else
        {
            Debug.Log("Quest Updated for User: " + user.UserId);
        }
    }


    public async Task UpdateInventoryItemsAsync(Dictionary<ItemData, int> inventoryItems)
    {
        IsInventorySaving = true;

        string inventoryPath = "Users/" + user.UserId + "/Inventory";

        try
        {
            // Xóa toàn bộ dữ liệu cũ
            await DBReference.Child(inventoryPath).RemoveValueAsync();
            Debug.Log("Old inventory cleared.");

            // Tạo và chờ tất cả tác vụ thêm mới
            List<Task> tasks = new List<Task>();
            foreach (var item in inventoryItems)
            {
                string itemPath = inventoryPath + "/" + item.Key.itemName;
                tasks.Add(DBReference.Child(itemPath).SetValueAsync(item.Value));
            }

            await Task.WhenAll(tasks);

            Debug.Log("All inventory items updated.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating inventory: {e.Message}");
        }

        IsInventorySaving = false;
    }


    IEnumerator UpdateAvatar(string avatarName)
    {
        var DBTask = DBReference.Child("Users").Child(user.UserId).Child("Avatar").SetValueAsync(avatarName);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(DBTask.Exception);
        }
        else
        {
            Debug.Log("Avatar Updated");
        }

        UserContainer.Instance.UserData.AvatarName = avatarName;
    }

    public IEnumerator UpdateCharacterClass(string className)
    {
        var DBTask = DBReference.Child("Users").Child(user.UserId).Child("Class").SetValueAsync(className);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(DBTask.Exception);
            yield break;
        }

        UserContainer.Instance.UserData.CharacterClassName = className;
    }

    public IEnumerator UpdateCharacterOutfits(OutfitData outfitData)
    {
        string path = "Users/" + user.UserId + "/Outfits";
        string serializedData = JsonUtility.ToJson(outfitData);

        // Lưu quest data vào Firebase
        var DBTask = DBReference.Child(path).SetRawJsonValueAsync(serializedData);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(DBTask.Exception);
        }
        else
        {
            Debug.Log("Outfit Updated for User: " + user.UserId);
        }
    }


    #endregion

    #region Load Data

    public void LoadData()
    {
        StartCoroutine(LoadRankingBoardDataFromFirebase());
        StartCoroutine(LoadUserData());
        StartCoroutine(LoadOutfitData());
        StartCoroutine(LoadCharacterStat());
        StartCoroutine(LoadInventoryItems());
        StartCoroutine(CreateQuestMapFromFirebase());
        StartCoroutine(LoadSkillLevelData());
    }


    public void LoadRankingBoardData()
    {
        StartCoroutine(LoadRankingBoardDataFromFirebase());
    }

    IEnumerator LoadUserData()
    {
        var DBTask = DBReference.Child("Users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(DBTask.Exception);
            yield break;
        }
        if (DBTask.Result.Value == null)
        {
            SaveUserInitData();
            yield break;
        }
        DataSnapshot dataSnapshot = DBTask.Result;
        string className = "";
        string avatarName = "";
        int maxLevel = _defaultMaxLevel;
        int xp = _defaultXp;
        int level = _defaultLevel;
        int gold = _defaultGold;
        int misc = _defaultMisc;
        if (dataSnapshot.Child("Class").Value != null)
            className = dataSnapshot.Child("Class").Value.ToString();
        if (dataSnapshot.Child("Avatar").Value != null)
            avatarName = dataSnapshot.Child("Avatar").Value.ToString();
        if (dataSnapshot.Child("MaxLevel").Value != null)
            maxLevel = int.Parse(dataSnapshot.Child("MaxLevel").Value.ToString());
        if (dataSnapshot.Child("Xp").Value != null)
            xp = int.Parse(dataSnapshot.Child("Xp").Value.ToString());
        if (dataSnapshot.Child("Level").Value != null)
            level = int.Parse(dataSnapshot.Child("Level").Value.ToString());
        if (dataSnapshot.Child("Gold").Value != null)
            gold = int.Parse(dataSnapshot.Child("Gold").Value.ToString());
        if (dataSnapshot.Child("Misc").Value != null)
            misc = int.Parse(dataSnapshot.Child("Misc").Value.ToString());

        UserContainer.Instance.SetUserData(new UserData()
        {
            Name = user.DisplayName,
            Xp = xp,
            Level = level,
            Gold = gold,
            Misc = misc,
            MaxLevel = maxLevel,
            AvatarName = avatarName,
            CharacterClassName = className,
        });
    }

    IEnumerator LoadOutfitData()
    {
        var DBTask = DBReference.Child("Users").Child(user.UserId).Child("Outfits").GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        try
        {
            var serializedData = DBTask.Result.GetRawJsonValue();
            if (string.IsNullOrEmpty(serializedData))
            {
                SaveOutitInitData();
            }
            else
            {
                OutfitData outfitData = JsonUtility.FromJson<OutfitData>(serializedData);
                OutfitContainer.Instance.SetOutfitData(outfitData);
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load outfit data: " + e.Message);
        }
    }


    IEnumerator LoadCharacterStat()
    {
        var DBTask = DBReference.Child("Users").Child(user.UserId).Child("CharacterStats").GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(DBTask.Exception);
            yield break;
        }
        try
        {
            if (DBTask.Exception != null)
            {
                Debug.LogError("Failed to load quest from Firebase: " + DBTask.Exception);
            }
            else if (DBTask.Result.Exists)
            {
                string serializedData = DBTask.Result.GetRawJsonValue();
                CharacterStat characterStat = JsonUtility.FromJson<CharacterStat>(serializedData);
                UserContainer.Instance.SetCharacterStat(characterStat);
            }
            else
            {
                UserContainer.Instance.SetCharacterStat(null);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest data: " + e.Message);
        }
    }

    private IEnumerator LoadQuestFromFirebase(string questId, QuestInfo questInfo, Action<Quest> onQuestLoaded)
    {
        Quest quest = null;

        var DBTask = DBReference.Child("Users").Child(user.UserId).Child("Quests").Child(questId).GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        try
        {
            if (DBTask.Exception != null)
            {
                Debug.LogError("Failed to load quest from Firebase: " + DBTask.Exception);
            }
            else if (DBTask.Result.Exists)
            {
                // Chuyển dữ liệu JSON thành QuestData
                string serializedData = DBTask.Result.GetRawJsonValue();
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);

                Debug.Log("Quest loaded: " + questData.questState);
                Debug.Log("Quest loaded: " + questData.currentQuestStepIndex);
                Debug.Log("Quest loaded: " + questData.questStepStates);

                if (questData.currentQuestStepIndex < 0 || questData.currentQuestStepIndex >= questData.questStepStates.Length)
                {
                    questData.currentQuestStepIndex = 0;
                }

                // Tạo Quest từ QuestData
                quest = new Quest(questInfo, questData.questState, questData.currentQuestStepIndex, questData.questStepStates);
                Debug.Log("Quest loaded: " + quest.CurrentQuestStepState.state);
            }
            else
            {
                // Nếu không có dữ liệu trong Firebase, tạo quest mặc định
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest data: " + e.Message);
        }

        // Gọi callback để trả về quest đã tải
        onQuestLoaded?.Invoke(quest);
    }

    private IEnumerator CreateQuestMapFromFirebase()
    {
        QuestInfo[] allQuests = Resources.LoadAll<QuestInfo>("Quests");
        Dictionary<string, Quest> questMap = new Dictionary<string, Quest>();

        foreach (var questInfo in allQuests)
        {
            if (questMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate quest id found: " + questInfo.id);
                continue;
            }

            // Load quest từ Firebase
            bool isLoaded = false;
            StartCoroutine(LoadQuestFromFirebase(questInfo.id, questInfo, (loadedQuest) =>
            {
                if (loadedQuest != null)
                {
                    questMap.Add(questInfo.id, loadedQuest);
                }
                isLoaded = true;
            }));

            // Chờ tới khi quest được load xong
            yield return new WaitUntil(() => isLoaded);
        }

        // Gán bản đồ quest vào biến toàn cục
        QuestContainer.Instance.QuestMap = questMap;
    }


    private IEnumerator LoadInventoryItems()
    {
        string inventoryPath = "Users/" + user.UserId + "/Inventory";
        var DBTask = DBReference.Child(inventoryPath).GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        Dictionary<ItemData, int> inventoryItems = new Dictionary<ItemData, int>();

        if (DBTask.Exception != null)
        {
            Debug.LogError("Failed to load inventory: " + DBTask.Exception);
        }
        else if (DBTask.Result.Exists)
        {
            // Parse dữ liệu từ Firebase
            DataSnapshot snapshot = DBTask.Result;
            foreach (var childSnapshot in snapshot.Children)
            {
                string itemName = childSnapshot.Key;
                int itemQuantity = int.Parse(childSnapshot.Value.ToString());

                // Tìm ItemData từ Resources hoặc danh sách đã load trước
                ItemData itemData = FindItemDataByName(itemName);
                if (itemData != null)
                {
                    inventoryItems.Add(itemData, itemQuantity);
                }
                else
                {
                    Debug.LogWarning("Item not found: " + itemName);
                }
            }
            InventoryContainer.Instance.PlayerItemMap = inventoryItems;
        }
        else
        {

            SaveInventoryInitData();

            Debug.Log("No inventory data found for user: " + user.UserId);
        }
    }



    private ItemData FindItemDataByName(string itemName)
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("ItemData");
        foreach (var item in allItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        return null;
    }

    private IEnumerator LoadSkillLevelData()
    {
        // Lấy dữ liệu từ Firebase
        var DBTask = DBReference.Child("Users").Child(user.UserId).Child("Skill").Child("SkillLevel").GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        // Kiểm tra nếu có lỗi trong khi truy cập Firebase
        if (DBTask.Exception != null)
        {
            Debug.LogError($"Error loading Skill Level Data: {DBTask.Exception}");
            yield break;
        }

        // Kiểm tra nếu dữ liệu không tồn tại
        if (DBTask.Result.Value == null)
        {
            Debug.Log("Skill Level Data not found. Initializing default data...");

            // Tạo dữ liệu mặc định và gán vào SkillContainer
            SkillLevelData defaultSkillLevelData = new SkillLevelData();
            SkillContainer.Instance.SkillLevelData = defaultSkillLevelData;

            // Lưu dữ liệu mặc định lên Firebase
            UpdateSkillLevelData(defaultSkillLevelData);

            yield break;
        }

        // Nếu có dữ liệu, đọc và gán dữ liệu
        try
        {
            // Lấy chuỗi JSON từ Firebase
            string serializedData = DBTask.Result.GetRawJsonValue();

            if (string.IsNullOrEmpty(serializedData))
            {
                Debug.LogError("Serialized data is null or empty!");
                yield break;
            }

            // Deserialize JSON thành đối tượng SkillLevelData
            SkillLevelData skillLevelData = JsonUtility.FromJson<SkillLevelData>(serializedData);

            if (skillLevelData == null)
            {
                Debug.LogError("Skill Level Data deserialization returned null!");
                yield break;
            }

            // Gán dữ liệu vào SkillContainer
            SkillContainer.Instance.SkillLevelData = skillLevelData;

            Debug.Log("Skill Level Data loaded successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing Skill Level Data: {ex.Message}");
        }
    }

    IEnumerator LoadRankingBoardDataFromFirebase()
    {
        var DBTask = DBReference.Child("Users").GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(DBTask.Exception);
            yield break;
        }
        else
        {
            Debug.Log("Ranking Board Data Loaded");
        }

        if (DBTask.Result == null)
        {
            Debug.Log("No data found");
            yield break;
        }
        else
        {
            Debug.Log("Data found");
        }

        DataSnapshot dataSnapshot = DBTask.Result;

        List<RankingLevelData> tempRankingLevelItems = new List<RankingLevelData>();

        foreach (var snapshot in dataSnapshot.Children)
        {

            string username = snapshot.Child("Username").Value != null ? snapshot.Child("Username").Value.ToString() : "Missing";
            string avatarName = snapshot.Child("Avatar").Value != null ? snapshot.Child("Avatar").Value.ToString() : "Missing";
            int level = snapshot.Child("Level").Value != null ? int.Parse(snapshot.Child("Level").Value.ToString()) : 1;

            RankingLevelData rankingLevelItem = new RankingLevelData()
            {
                Name = username,
                Level = level,
                AvatarName = avatarName
            };

            tempRankingLevelItems.Add(rankingLevelItem);
        }

        tempRankingLevelItems = tempRankingLevelItems.OrderByDescending(x => x.Level).ToList();

        RankingBoardContainer.Instance.SetRankingLevelItems(tempRankingLevelItems);
    }



    #endregion



    #region LOGIN BY OTHER SERVICES
    public void SignInWithGoogle()
    {
        Debug.Log("Sign In With Gooogle Pressed");

        string redirectUri = "http://localhost";

        var url = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={ClientID}&redirect_uri={redirectUri}&response_type=code&scope=email%20profile";
        Application.OpenURL(url);

        StartCoroutine(HandleGoogleSignIn());
    }

    private IEnumerator HandleGoogleSignIn()
    {
        Task<string> codeTask = ListenForAuthCode();
        yield return new WaitUntil(() => codeTask.IsCompleted);

        if (codeTask.IsFaulted || codeTask.IsCanceled || codeTask.Exception != null || string.IsNullOrEmpty(codeTask.Result))
        {
            Debug.LogError("Google Sign-In failed: No auth code received.");
            yield break;
        }

        string code = codeTask.Result;

        Task<TokenResponse> tokenTask = ExchangeCodeForToken(code);
        yield return new WaitUntil(() => tokenTask.IsCompleted);

        if (tokenTask.IsFaulted || tokenTask.IsCanceled || tokenTask.Exception != null || tokenTask.Result == null)
        {
            Debug.LogError("Failed to exchange auth code for token.");
            yield break;
        }

        SignInWithFirebase(tokenTask.Result);
    }

    private async Task<string> ListenForAuthCode()
    {
        using (var listener = new HttpListener())
        {
            try
            {
                listener.Prefixes.Add("http://localhost/");
                listener.Start();

                var context = await listener.GetContextAsync();
                var code = context.Request.QueryString["code"];

                string responseString = @"
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                <title>Login Successful</title>
                                <style>
                                    body {
                                        font-family: Arial, sans-serif;
                                        text-align: center;
                                        background-color: #f4f4f9;
                                        color: #333;
                                        margin: 0;
                                        padding: 0;
                                        display: flex;
                                        flex-direction: column;
                                        justify-content: center;
                                        align-items: center;
                                        height: 100vh;
                                    }
                                    .container {
                                        max-width: 500px;
                                        padding: 20px;
                                        background: #fff;
                                        border-radius: 8px;
                                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                    }
                                    h1 {
                                        color: #4CAF50;
                                        margin-bottom: 20px;
                                    }
                                    p {
                                        font-size: 16px;
                                        margin-bottom: 20px;
                                    }
                                    button {
                                        background-color: #4CAF50;
                                        color: white;
                                        border: none;
                                        padding: 10px 20px;
                                        border-radius: 5px;
                                        cursor: pointer;
                                        font-size: 16px;
                                    }
                                    button:hover {
                                        background-color: #45a049;
                                    }
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <h1>Login with Google account successfully!</h1>
                                    <p>You can close this window and return to the game.</p>
                                </div>
                            </body>
                            </html>";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();

                listener.Stop();
                return code;
            }
            catch (Exception ex)
            {
                Debug.LogError($"HttpListener error: {ex.Message}");
                return null;
            }
            finally
            {
                listener.Stop();
                listener.Close();
            }
        }
    }

    private async Task<TokenResponse> ExchangeCodeForToken(string code)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", ClientID },
            { "client_secret", ClientSecret },
            { "redirect_uri", "http://localhost" },
            { "grant_type", "authorization_code" }
        };

        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));

                if (!response.IsSuccessStatusCode)
                {
                    Debug.LogError($"Error exchanging code for token: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                Debug.Log($"Token Response: {json}");
                return JsonConvert.DeserializeObject<TokenResponse>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ExchangeCodeForToken error: {ex.Message}");
                return null;
            }
        }
    }

    private void SignInWithFirebase(TokenResponse tokenResponse)
    {
        Debug.Log("TokenResponse: " + tokenResponse);
        if (tokenResponse == null)
        {
            Debug.LogError("Null token response. Cannot sign in with Firebase.");
            return;
        }

        if (string.IsNullOrEmpty(tokenResponse.IdToken))
        {
            Debug.LogError("ID Token is Null. Cannot sign in with Firebase.");
            return;
        }

        var credential = GoogleAuthProvider.GetCredential(tokenResponse.IdToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Google Sign-In failed: " + task.Exception);
                return;
            }

            user = task.Result;
            Debug.Log($"User signed in with Google: {user.DisplayName}");
            LoginRegisterUIManager.Instance.ShowGamePanel();
            LoadData();
        });
    }

    public void SignInWithFacebook(){
        string redirectUri = "http://localhost/";
        string scope = "public_profile,email";
        string loginUrl = $"https://www.facebook.com/v15.0/dialog/oauth?client_id={FacebookAppID}&redirect_uri={redirectUri}&response_type=code&scope={scope}";

        Application.OpenURL(loginUrl);

        StartCoroutine(HandleFacebookSignIn());
    }

    private IEnumerator HandleFacebookSignIn(){
        Task<string> codeTask = ListenForAccessTokenAsync();
        yield return new WaitUntil(() => codeTask.IsCompleted);

        if (codeTask.IsFaulted || codeTask.IsCanceled || codeTask.Exception != null || string.IsNullOrEmpty(codeTask.Result))
        {
            Debug.LogError("Facebook Sign-In failed: No auth code received.");
            yield break;
        }

        string code = codeTask.Result;

        Task<string> accessTokenTask = ExchangeCodeForAccessToken(code);
        yield return new WaitUntil(() => accessTokenTask.IsCompleted);

        string accessToken = accessTokenTask.Result;
        if (!string.IsNullOrEmpty(accessToken))
        {
            Debug.Log("Access token received: " + accessToken);
            SignInWithFacebook(accessToken);
        }
        else
        {
            Debug.LogError("Access token is null or empty.");
        }
    }

    private async Task<string> ListenForAccessTokenAsync()
    {
        using (var listener = new HttpListener())
        {
            try
            {
                listener.Prefixes.Add("http://localhost/");
                listener.Start();

                Debug.Log("Listening for Facebook redirect...");
                var context = await listener.GetContextAsync();
                Debug.Log($"Received request URL: {context.Request.Url}");

                var query = System.Web.HttpUtility.ParseQueryString(context.Request.Url.Query);
                string code = query["code"];

                if (string.IsNullOrEmpty(code))
                {
                    Debug.LogError("Code is null or empty. Authentication failed.");
                    return null;
                }

                string responseString = @"
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                <title>Login Successful</title>
                                <style>
                                    body {
                                        font-family: Arial, sans-serif;
                                        text-align: center;
                                        background-color: #f4f4f9;
                                        color: #333;
                                        margin: 0;
                                        padding: 0;
                                        display: flex;
                                        flex-direction: column;
                                        justify-content: center;
                                        align-items: center;
                                        height: 100vh;
                                    }
                                    .container {
                                        max-width: 500px;
                                        padding: 20px;
                                        background: #fff;
                                        border-radius: 8px;
                                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                    }
                                    h1 {
                                        color: #4CAF50;
                                        margin-bottom: 20px;
                                    }
                                    p {
                                        font-size: 16px;
                                        margin-bottom: 20px;
                                    }
                                    button {
                                        background-color: #4CAF50;
                                        color: white;
                                        border: none;
                                        padding: 10px 20px;
                                        border-radius: 5px;
                                        cursor: pointer;
                                        font-size: 16px;
                                    }
                                    button:hover {
                                        background-color: #45a049;
                                    }
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <h1>Login with Facebook account successfully!</h1>
                                    <p>You can close this window and return to the game.</p>
                                </div>
                            </body>
                            </html>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            
                Debug.Log("Access Token: " + code);
                return code;
            }
            catch (Exception ex)
            {
                Debug.LogError($"HttpListener error: {ex.Message}");
                return null;
            }
            finally
            {
                listener.Stop();
                listener.Close();
            }
        }
    }

    private async Task<string> ExchangeCodeForAccessToken(string code)
    {
        string redirectUri = "http://localhost/";

        string tokenUrl = $"https://graph.facebook.com/v15.0/oauth/access_token" +
                        $"?client_id={FacebookAppID}" +
                        $"&redirect_uri={redirectUri}" +
                        $"&client_secret={FacebookAppSecret}" +
                        $"&code={code}";

        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.GetAsync(tokenUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.LogError($"Error exchanging code for token: {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                Debug.Log($"Token Response: {json}");

                var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                return tokenData.ContainsKey("access_token") ? tokenData["access_token"] : null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error exchanging code for token: {ex.Message}");
                return null;
            }
        }
    }

    private void SignInWithFacebook(string accessToken)
    {
        var credential = FacebookAuthProvider.GetCredential(accessToken);

        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Sign-in failed: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            
            Debug.Log($"User signed in successfully: {newUser.DisplayName} ({newUser.UserId})");
        
            LoginRegisterUIManager.Instance.ShowGamePanel();
            LoadData();
        });
    }

    #endregion
}

[Serializable]
public class TokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    [JsonProperty("id_token")]
    public string IdToken { get; set; }
}

[Serializable]
public class DeviceLogin {
    public string access_token;
    public string scope;
}



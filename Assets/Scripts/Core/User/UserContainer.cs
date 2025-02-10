using UnityEngine;

public class UserContainer : Singleton<UserContainer>
{
    public UserData UserData { get; private set; }
    public CharacterStat CharacterStat { get; private set; }

    public void SetName(string name)
    {
        if (UserData == null)
        {
            UserData = new UserData();
        }
        UserData.Name = name;
    }

    public void SetUserData(UserData userData)
    {
        if(UserData == null)
        {
            UserData = new UserData();
        }
        
        UserData = userData;
    }

    public void SetUserCharacterClassData(string className)
    {
        UserData.CharacterClassName = className;
    }

    public void SetCharacterStat(CharacterStat characterStat)
    {
        CharacterStat = characterStat;
    }

    public void SetUserAuthId(string authId)
    {
        UserData.AuthId = authId;
    }
}

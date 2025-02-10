using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthenticationWrapper : MonoBehaviour
{
    public static AuthState authState { get; private set; } = AuthState.NotAuthenticated;
    public static async Task<AuthState> DoAuth(int maxRetries = 5)
    {
        if (authState == AuthState.Authenticated) return AuthState.Authenticated;
        if (authState == AuthState.Authenticating) return await Authenticating();

        await SignInAnonymusAsync(maxRetries);
        return authState;
    }

    private static async Task SignInAnonymusAsync(int maxRetries)
    {
        authState = AuthState.Authenticating;
        int retries = 0;
        while (authState == AuthState.Authenticating && retries < maxRetries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    authState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException e)
            {
                authState = AuthState.Error;
                Debug.LogError(e);
            }
            catch (RequestFailedException e)
            {
                authState = AuthState.Error;
                Debug.LogError(e);
            }

            retries++;
            await Task.Delay(100);
        }

        if (authState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully after {retries} retries");
            authState = AuthState.TimeOut;
        }
        Debug.Log("AuthState: " + authState);
    }
    private static async Task<AuthState> Authenticating()
    {
        while (authState == AuthState.Authenticating || authState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
        return authState;
    }


}
public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
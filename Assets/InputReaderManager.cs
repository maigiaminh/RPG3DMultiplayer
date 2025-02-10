using UnityEngine;

public class InputReaderManager : Singleton<InputReaderManager>
{
    [SerializeField] private InputReader inputReader;
    public InputReader InputReader => inputReader;
}

using System;

public class MainMenuRequester
{
    public static event Action MainMenuRequested;

    public void RequestMainMenu()
    {
        MainMenuRequested?.Invoke();
    }
    }

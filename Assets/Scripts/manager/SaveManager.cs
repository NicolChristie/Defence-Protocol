using UnityEngine;

public static class SaveManager
{
    private static int finishedAmount = 0;

    // Optional: call this at game startup
    public static void Initialize()
    {
        finishedAmount = 0;
        Debug.Log("SaveManager initialized. finishedAmount set to 0.");
    }

    public static void SaveFinishedAmount(int amount)
    {
        finishedAmount = amount;
    }

    public static int LoadFinishedAmount()
    {
        return finishedAmount;
    }

    public static void ResetFinishedAmount()
    {
        finishedAmount = 0;
    }
}

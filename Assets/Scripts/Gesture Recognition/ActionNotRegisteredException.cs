[System.Serializable]

/// <summary>
/// Exception thrown when the detected gesture ID has had no method bound to it through <see cref="GestureResolver.RegisterAction(int, System.Action)"/>
/// </summary>
public class ActionNotRegisteredException : System.Exception
{
    public ActionNotRegisteredException() { }
    public ActionNotRegisteredException(int id) : base("The recognized gesture (ID \"" + id + "\") has no registered action") { }
}
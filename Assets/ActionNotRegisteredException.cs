[System.Serializable]
public class ActionNotRegisteredException : System.Exception
{
    public ActionNotRegisteredException() { }
    public ActionNotRegisteredException(int id) : base("The recognized gesture (ID \"" + id + "\") has no registered action") { }
}
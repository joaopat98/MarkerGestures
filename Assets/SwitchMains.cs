using UnityEngine;

public class SwitchMains : MonoBehaviour
{

    public Transform[] mainTargets;
    int current = 0;

    void manageActive()
    {
        for (int i = 0; i < mainTargets.Length; i++)
        {
            if (i == current) mainTargets[i].gameObject.SetActive(true);
            else mainTargets[i].gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        manageActive();
    }

    public void switchTarget()
    {
        current = (current + 1) % mainTargets.Length;
        manageActive();
        Vuforia.TrackerManager.Instance.GetStateManager().ReassociateTrackables();
    }
}

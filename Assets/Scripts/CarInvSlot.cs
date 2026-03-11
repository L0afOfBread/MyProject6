using UnityEngine;

public class CarInvSlot : MonoBehaviour
{
    public int lastChildCount = 0;
    public static int partsCount = 0;
    public GameObject repairCarButtton;

    private void OnTransformChildrenChanged()
    {
        if (transform.childCount > lastChildCount)
        {
            partsCount++;
            lastChildCount++;
        }
        else if (transform.childCount < lastChildCount)
        {
            partsCount--;
            lastChildCount--;
        }
    }

    private void Update()
    {
        if (partsCount == 3)
        {
            repairCarButtton.SetActive(true);
        }
        else
        {
            repairCarButtton.SetActive(false);
        }

    }
}

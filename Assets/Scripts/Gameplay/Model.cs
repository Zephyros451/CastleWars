using UnityEngine;

public class Model : MonoBehaviour
{
    [SerializeField] private SphereCollider collider;

    public bool IsActive;
    public int SegmentsTravelled;
    public Allegiance Allegiance;
    public Type type;

    public void ActivateCollider()
    {
        collider.enabled = true;
    }
}

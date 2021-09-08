using UnityEngine;

public class Model : MonoBehaviour
{
    [SerializeField] private SphereCollider collider;

    public bool IsActive;
    public int SegmentsTravelled;
    public Allegiance Allegiance;
    public TowerType type;
    public Unit unit;

    public void ActivateCollider()
    {
        collider.enabled = true;
    }
}

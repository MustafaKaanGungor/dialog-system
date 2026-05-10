using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NPCHeadTracking : MonoBehaviour
{
    [SerializeField] private Transform aimController;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private MultiAimConstraint headAimConstraint;

    void Update()
    {
        float weight = (aimTarget == null) ? 0 : 1f;
        Vector3 pos = (aimTarget == null) ? transform.position + transform.forward + Vector3.up : aimTarget.position + Vector3.up;

        headAimConstraint.weight = Mathf.Lerp(headAimConstraint.weight, weight, .05f);
        aimController.position = Vector3.Lerp(aimController.position, pos, 0.3f);    
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            aimTarget = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            aimTarget = null;
        }
    }
}


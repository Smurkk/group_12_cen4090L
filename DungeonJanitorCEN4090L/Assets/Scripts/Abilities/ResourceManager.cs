using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Dev stuff")]
    [SerializeField, Tooltip("This serves only to give us a declarative statement on what this resource is. Currently, this is not read or used for anything.")] 
        private string resourceType;
    

    [Header("Resources")]
    [SerializeField] private float maxResource;
    [SerializeField] private float currentResource;

    [Header("Regeneration")]
    [SerializeField] private float regenRate;
    [SerializeField] private bool regenerate;


    public float CurrentAmount => currentResource;
    public float Max => maxResource;
    public float RegenRate => regenRate;
    public bool Regenerate => regenerate; // Probably won't need, but in case we decide to display to the user, or disable under certain conditions.


    private void Update()
    {
        if (!regenerate)
            return;

        if (currentResource < maxResource)
        {
            currentResource += regenRate * Time.deltaTime;
            currentResource = Mathf.Clamp(currentResource, 0, maxResource);
        }
    }

    public bool HasSufficientResources(float amount)
    {
        return currentResource >= amount;
    }

    public bool SpendResource(float amount)
    {
        if (!HasSufficientResources(amount))
            return false;

        currentResource -= amount;
        return true;
    }

    public void AddResource(float amount)
    {
        currentResource = Mathf.Clamp(currentResource + amount, 0, maxResource);
    }
    
}

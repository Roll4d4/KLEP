using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
[CreateAssetMenu(fileName = "NewLock", menuName = "KLAP/Lock")]
public class KLEPLock : ScriptableObject
{
    [SerializeField]
    public string LockName;
    [SerializeField]
    public float Attractiveness;

    [SerializeField]
    public List<KLEPCondition> conditions;

    [SerializeField]
    private bool uniqueInstanceRequired = false; // Toggle for unique instance creation

    [SerializeField]
    public List<SLASHkeyLoader> keyLoader = new List<SLASHkeyLoader>();

    private bool isOpen = false; // State to track if the lock is currently open

    // Method to update all conditions checking a specific key

    public KLEPLock CreateUniqueInstance()
    {
        KLEPLock clone = ScriptableObject.CreateInstance<KLEPLock>();
        clone.LockName = this.LockName;
        clone.Attractiveness = this.Attractiveness;
        clone.conditions = this.conditions.Select(cond => cond.Clone()).ToList();
        clone.keyLoader = new List<SLASHkeyLoader>(this.keyLoader); // Shallow copy is fine here since SLASHkeyLoader instances are shared.
        return clone;
    }


    public bool IsSatisfiedByAgent(HashSet<KLEPKey> elements)
    {
        // If the lock is already open, return true
        if (isOpen) return true;

        // Assume no conditions means the lock is always satisfied.
        if (conditions == null || !conditions.Any()) return true;

        // Separate OR and Non-OR conditions for targeted evaluation.
        var orConditions = conditions.Where(cond => cond.isOrCondition);
        var nonOrConditions = conditions.Where(cond => !cond.isOrCondition);

        // If there are OR conditions, evaluate them first.
        if (orConditions.Any())
        {
            // The lock is satisfied if any OR condition is true.
            if (orConditions.Any(cond => cond.EvaluateCondition(elements)))
            {
                isOpen = true; // Open the lock if an OR condition is satisfied
                return true;
            }

            // If none of the OR conditions are met, and there are no non-OR conditions, the lock is not satisfied.
            if (!nonOrConditions.Any()) return false;
        }

        // If there are no OR conditions or none are satisfied, all non-OR conditions must be met.
        bool allNonOrConditionsMet = nonOrConditions.All(cond => cond.EvaluateCondition(elements));
        if (allNonOrConditionsMet) isOpen = true; // Open the lock if all non-OR conditions are met

        return allNonOrConditionsMet;
    }

    public void Lock()
    {
        isOpen = false; // Explicitly lock the lock
    }

    public void Unlock()
    {
        isOpen = true; // Explicitly unlock the lock
    }
}


[Serializable]
public class KLEPCondition
{
    [SerializeField]
    public string keyName; // Name of the key this condition is targeting.
    [SerializeField]
    public bool keyMustExist; // True if the key must exist, false if it must not exist.
    [SerializeField]
    public bool isOrCondition; // Determines if this condition is an OR condition.

    public KLEPCondition Clone()
    {
        return new KLEPCondition
        {
            keyName = this.keyName,
            keyMustExist = this.keyMustExist,
            isOrCondition = this.isOrCondition
        };
    }

    public string ReturnKeyName()
    {
        return keyName;
    }

    // Evaluates the condition against the provided set of elements.
    public bool EvaluateCondition(HashSet<KLEPKey> elements)
    {
        bool keyExists = elements.Any(k => k.KeyName == keyName);
        return keyMustExist ? keyExists : !keyExists;
    }

    public bool IsOrCondition()
    {
        return isOrCondition;
    }

}

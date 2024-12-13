using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// From KLEPAgent.cs
// Core feature: The Agent as the decision-maker guiding the neuron’s actions.
// It selects from available executables, updates Q-values based on feedback,
// and optionally refrains from choosing any action if no attractive option is available.

public class KLEPAgent : MonoBehaviour
{
    private KLEPNeuron parentNeuron;
    private KLEPExecutableBase currentAction = null;
    private Dictionary<string, float> qValues = new Dictionary<string, float>();
    public float certaintyThreshold = 0;
    private int inTandemRecheckLimit = 900;
    private bool recheckInTandemExecutables = true;
    private Dictionary<KLEPGoal, KLEPExecutableBase> goalSoloExeMappings = new Dictionary<KLEPGoal, KLEPExecutableBase>();

    public void Initialize(KLEPNeuron neuron)
    {
        parentNeuron = neuron;
        parentNeuron.bridge.RegisterEvent("KeyAdded", OnKeyAddedEvent);
    }

    public void InitializeTriggers()
    {
        parentNeuron.bridge.RegisterEvent("FeedAgent", HandleFeedAgentEvent);
        parentNeuron.bridge.RegisterEvent("HurtAgent", HandleHurtAgentEvent);
    }

    private void OnKeyAddedEvent(string eventName, object eventData)
    {
        if (eventData is KLEPKey)
        {
            // A Key was added, trigger a recheck of in-tandem executables
            recheckInTandemExecutables = true;
        }
    }

    private void HandleFeedAgentEvent(string eventName, object eventData)
    {
        // Positive feedback event
        if (eventData != null)
        {
            Feed(eventData.ToString());
        }
    }

    private void HandleHurtAgentEvent(string eventName, object eventData)
    {
        // Negative feedback event
        if (eventData != null)
        {
            Hurt(eventData.ToString());
        }
    }

    public void Feed(string executableName)
    {
        UpdateQValues(executableName, 1.0f, 0.1f); // Sample parameters
    }

    public void Hurt(string executableName)
    {
        UpdateQValues(executableName, -1.0f, 0.1f); // Sample parameters
    }

    private void UpdateQValues(string executableName, float reward, float learningRate)
    {
        if (!qValues.ContainsKey(executableName))
        {
            qValues[executableName] = 0.0f;
        }

        qValues[executableName] += learningRate * (reward - qValues[executableName]);
    }

    public void AgentUpdate()
    {
        if (parentNeuron == null)
        {
            parentNeuron = this.GetComponent<KLEPNeuron>();
            if (parentNeuron == null) return;
        }

        var currentKeys = parentNeuron.heldKeys;
        var allExecutables = parentNeuron.heldExecutables;

        // Executables that run alongside other actions
        var inTandemExecutables = allExecutables.Where(exe => !exe.IsManagedByGoal && exe.InTandem).ToList();
        // Action-like executables that stand alone and are not managed by goals
        var soloExecutables = allExecutables.Where(exe => !exe.InTandem && !exe.IsManagedByGoal).ToList();

        // Perform periodic updates on in-tandem executables
        foreach (var exe in inTandemExecutables)
        {
            if (exe.parentNeuron == null || !exe.gameObject.activeSelf) continue;
            exe.ExecutableUpdates();
        }

        ExecuteInTandemExecutables(inTandemExecutables);
        DecideAndExecuteSoloAction(soloExecutables, currentKeys);
    }

    public void AgentFixedUpdate(HashSet<KLEPKey> fixedKeys)
    {
        if (parentNeuron == null)
        {
            parentNeuron = this.GetComponent<KLEPNeuron>();
            if (parentNeuron == null) return;
        }

        var allExecutables = parentNeuron.heldExecutables;
        var inTandemExecutables = allExecutables.Where(exe => !exe.IsManagedByGoal && exe.InTandem).ToList();
        var soloExecutables = allExecutables.Where(exe => !exe.InTandem && !exe.IsManagedByGoal).ToList();

        foreach (var exe in inTandemExecutables)
        {
            if (exe.parentNeuron == null || !exe.gameObject.activeSelf) continue;
            exe.ExecutableFixedUpdate();
        }

        ExecuteInTandemExecutablesFixed(inTandemExecutables, fixedKeys);
        DecideAndExecuteSoloActionFixed(soloExecutables, fixedKeys);
    }

    private void ExecuteInTandemExecutables(List<KLEPExecutableBase> inTandemExecutables)
    {
        recheckInTandemExecutables = true;
        int val = 0;

        while (recheckInTandemExecutables)
        {
            recheckInTandemExecutables = false;

            if (val > inTandemRecheckLimit) break;

            List<KLEPExecutableBase> executedExecutables = new List<KLEPExecutableBase>();

            foreach (var executable in inTandemExecutables.ToList())
            {
                if (executable.parentNeuron == null || !executable.gameObject.activeSelf)
                    continue;

                bool canValidate = executable.CanValidate(parentNeuron.heldKeys);
                bool canExecute = executable.CanExecute(parentNeuron.heldKeys);

                if (canValidate && canExecute)
                {
                    executable.Execute();
                    executedExecutables.Add(executable);
                    recheckInTandemExecutables = true;
                }
            }

            foreach (var executedExecutable in executedExecutables)
            {
                inTandemExecutables.Remove(executedExecutable);
            }

            if (inTandemExecutables.Count == 0) recheckInTandemExecutables = false;
            val++;
        }
    }

    private void ExecuteInTandemExecutablesFixed(List<KLEPExecutableBase> inTandemExecutables, HashSet<KLEPKey> fixedKeys)
    {
        recheckInTandemExecutables = true;
        int val = 0;

        while (recheckInTandemExecutables)
        {
            recheckInTandemExecutables = false;

            if (val > inTandemRecheckLimit) break;

            List<KLEPExecutableBase> executedExecutables = new List<KLEPExecutableBase>();

            foreach (var executable in inTandemExecutables.ToList())
            {
                if (executable.parentNeuron == null || !executable.gameObject.activeSelf)
                    continue;

                bool canValidate = executable.CanValidate(fixedKeys);
                bool canExecute = executable.CanExecute(fixedKeys);

                if (canValidate && canExecute)
                {
                    executable.ExecutableFixedUpdate();
                    executable.FixedExecute();
                    executedExecutables.Add(executable);
                    recheckInTandemExecutables = true;
                }
            }

            foreach (var executedExecutable in executedExecutables)
            {
                inTandemExecutables.Remove(executedExecutable);
            }

            if (inTandemExecutables.Count == 0) recheckInTandemExecutables = false;
            val++;
        }
    }

    private void DecideAndExecuteSoloAction(List<KLEPExecutableBase> soloActions, HashSet<KLEPKey> currentElements)
    {
        PopulateGoalSoloExecutableMappings(soloActions);

        var allGoals = soloActions.OfType<KLEPGoal>().ToList();

        // Gather non-goal actions
        var updatedSoloActions = soloActions.Where(action => !(action is KLEPGoal)).ToList();

        // Insert stand-in executables from goals
        foreach (var goal in allGoals)
        {
            KLEPExecutableBase standInOrGoal = goal.QueryGoalForSoloExe();
            if (standInOrGoal != null)
            {
                updatedSoloActions.Add(standInOrGoal);
            }
        }

        // If no attractive actions are found, do nothing ("patient" state)
        if (updatedSoloActions.Count == 0) return;

        KLEPExecutableBase bestAction = SelectBestAction(currentElements, updatedSoloActions);

        // If no action is selected (none are attractive enough), remain patient
        if (bestAction == null) return;

        var goalForBestAction = goalSoloExeMappings.FirstOrDefault(pair => pair.Value == bestAction).Key;
        if (goalForBestAction != null)
        {
            bestAction = goalForBestAction;
        }

        ExecuteAction(bestAction);
    }

    private void DecideAndExecuteSoloActionFixed(List<KLEPExecutableBase> soloActions, HashSet<KLEPKey> fixedKeys)
    {
        PopulateGoalSoloExecutableMappings(soloActions);

        var allGoals = soloActions.OfType<KLEPGoal>().ToList();

        var updatedSoloActions = soloActions.Where(action => !(action is KLEPGoal)).ToList();

        foreach (var goal in allGoals)
        {
            KLEPExecutableBase standInOrGoal = goal.QueryGoalForSoloExe();
            if (standInOrGoal != null)
            {
                updatedSoloActions.Add(standInOrGoal);
            }
        }

        // If no attractive actions are found, do nothing ("patient" state)
        if (updatedSoloActions.Count == 0) return;

        KLEPExecutableBase bestAction = SelectBestAction(fixedKeys, updatedSoloActions);

        // If no action is selected (none are attractive enough), remain patient
        if (bestAction == null) return;

        var goalForBestAction = goalSoloExeMappings.FirstOrDefault(pair => pair.Value == bestAction).Key;
        if (goalForBestAction != null)
        {
            bestAction = goalForBestAction;
        }

        FixedExecuteAction(bestAction);
    }

    private void PopulateGoalSoloExecutableMappings(List<KLEPExecutableBase> soloActions)
    {
        goalSoloExeMappings.Clear();

        foreach (var e in soloActions)
        {
            if (e is KLEPGoal goal)
            {
                KLEPExecutableBase standIn_EXE = goal.QueryGoalForSoloExe();
                if (standIn_EXE != null)
                {
                    goalSoloExeMappings[goal] = standIn_EXE;
                }
            }
        }
    }

    private KLEPExecutableBase SelectBestAction(HashSet<KLEPKey> currentElements, List<KLEPExecutableBase> soloActions)
    {
        float highestValue = float.MinValue;
        KLEPExecutableBase selectedAction = null;

        foreach (var action in soloActions)
        {
            if (!action.gameObject.activeInHierarchy) continue;

            float attractionValue = action.CalculateAttraction(currentElements);
            float qValue = qValues.ContainsKey(action.GetExecutableName()) ? qValues[action.GetExecutableName()] : 0;
            float combinedValue = attractionValue + qValue;

            // Introduce a threshold: if combinedValue is too low, we might not select any action at all
            if (combinedValue > highestValue && combinedValue > certaintyThreshold)
            {
                highestValue = combinedValue;
                selectedAction = action;
            }
        }

        return selectedAction; // May return null if no action met the threshold
    }

    private void ExecuteAction(KLEPExecutableBase action)
    {
        if (action.parentNeuron == null || !action.gameObject.activeSelf)
            return;

        if (currentAction != action)
        {
            currentAction?.OnExitState();
            action.OnEnterState();
        }

        currentAction = action;
        currentAction.ExecutableUpdates();
        currentAction.Execute();
    }

    private void FixedExecuteAction(KLEPExecutableBase action)
    {
        if (action.parentNeuron == null || !action.gameObject.activeSelf)
            return;

        if (currentAction != action)
        {
            currentAction?.OnExitState();
            action.OnEnterState();
        }

        currentAction = action;
        currentAction.ExecutableFixedUpdate();
        currentAction.FixedExecute();
    }

    public float GetQValue(string executableName)
    {
        return qValues.TryGetValue(executableName, out var qValue) ? qValue : 0.0f;
    }

    public Dictionary<string, float> GetQTable()
    {
        return qValues;
    }

    public KLEPExecutableBase ReturnCurrentEXE()
    {
        return currentAction;
    }
}

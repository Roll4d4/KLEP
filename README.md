Visit https://roll4d4.github.io/KLEP/ for the philosophies behind KLEP, this page will cover the practical implementation of KLEP into your project.

# KLEP: The Key-Lock Executable Process AI Framework

KLEP is a modular, symbolic AI framework designed for simplicity, transparency, and flexibility. By using **keys**, **locks**, and **executables**, KLEP enables developers to create dynamic, adaptive systems for games, simulations, or experimental AI research. Whether you're building intelligent NPCs or testing symbolic reasoning, KLEP provides a robust foundation.

---

## **What Is KLEP?**

KLEP models decision-making as a flow of **keys** (inputs), **locks** (conditions), and **executables** (actions). 

- **Keys**: Represent concepts, actions, or conditions. Keys are the "currency" of decision-making.
- **Locks**: Define the conditions required for behaviors to activate.
- **Executables**: Encapsulate actions or processes that influence the environment or release new keys.

KLEP supports **runtime modification**, making it ideal for real-time experimentation and development.

---

## **Getting Started with KLEP**

### **To Install**
Simply download the KLEP folder and move it into your project. Its symbolic logic, so no need for heavy multi GB models of training data. 
Unless thats your thing.

### **1. Initialize KLEP**
To start using KLEP, drag the `KLEPNeuron` script onto an empty GameObject and hit play. This auto-initializes the system.

![GIF: Initializing KLEP](link-to-your-gif-here)

---

### **2. Add a Sensor**
KLEP works with sensors to inject information into the neuron. For example, the **Keyboard Sensor** interprets user input as keys.

1. While the simulation is running, drag the `KeyboardSensor` script onto the entity.
2. Input keys and see them propagate through the neuron.

![GIF: Adding a Keyboard Sensor](link-to-your-gif-here)

---

### **3. Create Keys and Locks**
KLEP allows you to create custom keys and locks to define and trigger behaviors.

1. Create a key (e.g., `"JumpKey"`) in your sensor or through code.
2. Define a lock that looks for `"JumpKey"` in an executable.

![GIF: Creating Keys and Locks](link-to-your-gif-here)

---

## **Using KLEP in Code**

### **Writing an Executable**
An **executable** is the core of KLEP's behavior system. Here's how to create a custom executable:

1. Inherit from `KLEPExecutableBase`.
2. Override `Execute()` to define the action.
3. Use `CanValidate()` and `CanExecute()` to check for required keys.

```csharp
public class JumpAction : KLEPExecutableBase
{
    public override void Execute()
    {
        if (CanValidate(parentNeuron.heldKeys) && CanExecute(parentNeuron.heldKeys))
        {
            Debug.Log("Jumping!");
            // Example of a clean release:
            PushKey(MakeKey("LandedKey", 1.0f));
        }
    }

    public override bool IsComplete()
    {
        // Signal that the action has finished.
        return true;
    }
}
```

---

### **Releasing Keys**
Keys can be **cleanly released** (on behavior completion) or **dirty released** (at any time):

#### **Clean Release**
Occurs when keys are pushed on behavior completion:
```csharp
PushKey(MakeKey("CleanKey", 1.0f));
```

#### **Dirty Release**
Occurs when keys are pushed at arbitrary moments:
```csharp
PushKey(MakeKey("DirtyKey", 0.5f));
```

Use dirty releases sparingly to maintain behavior predictability.

---

### **Checking Keys in Code**
You can check for the presence of a key in the neuron:
```csharp
if (IsKeyPresent("JumpKey", KeyCheckType.inNeuron))
{
    Debug.Log("JumpKey is present!");
}
```

---

### **Cleanup**
Executables call `Cleanup()` during their lifecycle if needed. For example:
```csharp
public override void Cleanup()
{
    // Reset or clear any state after execution.
    Debug.Log("Cleaning up JumpAction");
}
```
You can find the cleanup behavior in the `KLEPAgent` script to customize how and when it is triggered.

---

## **Putting It All Together**

KLEP enables you to:
1. Dynamically create behaviors through executables.
2. Introduce sensors to feed keys into the system.
3. Use locks to conditionally activate actions.
4. Monitor and adapt key flow in real time.

This modular approach allows for endless experimentation and applications, from intelligent NPCs to real-world problem solvers.

---

## **Future Possibilities**
KLEP is designed as a foundation for exploration. Developers can:
- Integrate KLEP with **neuroevolution**, **genetic algorithms**, or other learning systems.
- Experiment with symbolic reasoning combined with traditional AI methods.
- Develop adaptive systems capable of self-modifying behaviors.

---

## **Join the KLEP Community**
Want to contribute or share your experiments? Visit the [KLEP GitHub Repository](your-link-here) and join the conversation.

Let's build the future of AI together—whether that's saving lives, creating compelling games, or simply making the coolest tech around.


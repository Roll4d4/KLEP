# **Locks in the KLEP System**

In the KLEP system, **locks** represent an abstract mechanism for verifying and validating information. They are conceptual tools designed to answer a simple question: **"Is this condition satisfied?"** A lock doesn’t inherently control access to something—it simply provides a mechanism for determining whether the current state meets a set of predefined conditions.

### **Why Locks?**

Locks abstract the concept of verification and make it extensible, adaptable, and modular. Whether you’re checking if a key exists, validating its data, or querying dynamic properties like coroutine states, locks provide a flexible system to handle these tasks. By implementing locks as **ScriptableObjects**, we enable developers to:

- **Modify locks visually** through the Unity Inspector.
- **Create and manage reusable configurations** for behaviors.
- **Dynamically modify locks at runtime** through scripting.

---

## **How Locks Work**

At their core, locks evaluate a series of **conditions** to determine if they are satisfied. These conditions can range from simple checks (e.g., "Does this key exist?") to complex logic involving data comparison or state validation.

### **Conditions: The Heart of a Lock**

Conditions are modular, allowing for easy extension and customization. Each condition defines:

- **A target key** to evaluate.
- **A rule** (e.g., "The key must exist" or "The key must not exist").
- **A logical grouping** (e.g., OR conditions or mandatory checks).

The lock evaluates conditions using the following steps:

1. **OR Conditions**: If any OR condition is satisfied, the lock is considered open.
2. **Mandatory Conditions**: If no OR conditions exist or none are satisfied, all mandatory conditions must be met for the lock to open.

This logical structure makes locks versatile while maintaining simplicity.

---

### **Lifecycle of a Lock**

Locks can toggle their state between **open** and **closed** based on the evaluation of conditions:

1. **Initialization**: Locks are created as instances of `ScriptableObject` with a unique name, a list of conditions, and additional metadata (like attractiveness).
2. **Evaluation**: Locks evaluate their conditions against the **current state of keys** (e.g., keys held by a neuron).
3. **Dynamic Updates**: Conditions can be modified dynamically during runtime to adapt to gameplay needs.

Locks also include utility methods to explicitly lock or unlock them programmatically, bypassing condition evaluation if needed.

---

### **Extendable Validation**

The KLEP lock system is designed to be **extendable**. Conditions aren’t limited to checking a key’s existence—they can evaluate any property a key carries. Some examples include:

- **String validation**: Check if the key’s value matches a specific string.
- **Numeric comparison**: Validate ranges of integers or floats.
- **Coroutine states**: Determine if a coroutine managed by a key is running or completed.
- **Dynamic links**: Evaluate real-time data from another neuron via the key.

This flexibility empowers developers to create highly adaptive and intricate systems with minimal boilerplate code.

---

## **Example Usage**

Imagine a scenario where an AI agent wants to use a behavior but must first validate its environment. You can define a lock like this:

1. **Lock Name**: `CanActivateBehavior`.
2. **Conditions**:
   - A key named `EnvironmentSafe` must exist.
   - A key named `EnemyNearby` must not exist.
3. **Dynamic Evaluation**: If conditions are satisfied during the frame, the lock allows the behavior to proceed.

This kind of modular approach enables both simplicity for prototyping and depth for advanced systems.

---

### **Philosophy Behind Locks**

Locks are **information verifiers**, not gatekeepers. They don’t enforce rules directly but act as a framework for evaluating rules dynamically. By externalizing verification logic into reusable, easily modifiable assets, the KLEP system supports rapid iteration and experimentation.

The extensible design allows locks to serve as the foundation for **higher-level decision-making systems**, paving the way for emergent complexity while reducing development overhead.

---

## **What’s Next: Keys**

Locks alone wouldn’t function without their counterpart: **keys**. A key provides the data that locks evaluate, carrying information about the system’s state. In the next section, we’ll dive into how keys work, their role in the KLEP system, and how they integrate seamlessly with locks to enable powerful, modular AI.

# **Keys in the KLEP System**

In the KLEP system, **keys** are dynamic, abstract representations of information. They serve as the carriers of data, the verifiers of state, and the enablers of behavior. By design, they are versatile and extensible, capable of holding any information the system requires while providing a robust framework for real-time validation and modification.

---

## **The Philosophy of Keys**

Keys are more than simple data structures—they are conceptual entities that interact dynamically within the KLEP system. Each key:

- **Holds properties**: A key can carry any type of data, from simple primitives like integers and strings to complex structures like lists of transforms.
- **Interacts with locks**: Keys provide the data that locks evaluate, enabling fine-grained control over system behaviors.
- **Facilitates real-time updates**: Keys can be dynamically modified during runtime, allowing the system to adapt to changing states seamlessly.

By implementing keys as **ScriptableObjects**, the system allows developers to:

- **Modify keys visually** through the Unity Inspector for rapid prototyping and debugging.
- **Reuse and share key configurations** across different components and systems.
- **Dynamically initialize and update keys** via scripting during gameplay.

---

## **How Keys Work**

Each key is composed of several critical components:

1. **Key Name**: A unique identifier that distinguishes this key from others.
2. **Attractiveness**: A numerical value that can be used to prioritize or sort keys, providing a basis for decision-making.
3. **Properties**: The heart of the key, a collection of named values that can be queried, updated, and synchronized.
4. **Key Loader**: A reference to a `SLASHkeyLoader` that provides default property definitions for initializing the key.

---

### **Properties: The Data Carriers**

Keys utilize a **property manager** to hold and manipulate their properties. Each property has:

- **A name**: Either predefined or custom.
- **A type**: Such as `int`, `string`, `float`, or even complex types like `List<Vector3>`.
- **A value**: The current data associated with the property.

#### **Dynamic Property Management**

Keys are designed to handle properties dynamically:

- **Add or update properties** during runtime with `SetProperty`.
- **Retrieve property values** for use in behaviors with `GetProperty`.
- **Synchronize properties** with a loader to reset or update default values.

This flexibility allows keys to act as living, evolving entities within the system.

---

### **Initialization and Synchronization**

When a key is created or enabled, it:

1. **Loads default properties** from its assigned key loader, ensuring a consistent starting state.
2. **Maintains custom data**: If a property has been modified from its default, the system preserves the custom value during updates.
3. **Synchronizes properties**: Removes properties not present in the loader, ensuring consistency.

---

## **Advanced Features of Keys**

### **Extendable Properties**

The property system is built to accommodate a wide range of use cases. Developers can extend the `PropertyType` enumeration and implement new logic for handling additional data types, enabling the system to grow with project requirements.

### **Real-Time Adaptability**

Keys can hold real-time state information, such as:

- **Coroutine states**: Track whether certain processes are active or complete.
- **Dynamic links**: Reference other objects or systems, like neurons or behaviors, and provide real-time updates.
- **Changing values**: Continuously update properties based on gameplay events, such as player inputs or environmental changes.

---

## **Why Keys Are Essential**

Keys provide the **data foundation** of the KLEP system. They are versatile, extensible, and dynamic, enabling developers to manage complex systems with ease. By externalizing data into keys, the system becomes more modular, testable, and adaptable.

---

## **The Connection to Locks**

Locks and keys are intrinsically linked. While locks validate conditions, keys provide the data that conditions evaluate. This relationship makes the KLEP system inherently modular and intuitive. Developers can:

- Define locks that evaluate properties of keys.
- Use keys to enable or disable behaviors dynamically.
- Create emergent systems where locks and keys work together to drive gameplay.

---

## **Example Use Cases**

### **Dynamic Buff System**

Imagine a buff in a game that temporarily increases a character’s speed:

- **Key**: `SpeedBoostKey` with a property `duration` and `multiplier`.
- **Lock**: Validates that `SpeedBoostKey` exists and checks the `duration` property to determine if the buff is active.

### **AI Coordination**

An AI commander neuron could issue keys to subordinate agents:

- Each key carries properties like `targetPosition` or `attackPriority`.
- Subordinate agents evaluate these keys to decide their actions.

---

## **Why Keys and Locks Together Work**

The dynamic interplay between keys and locks is what makes the KLEP system so powerful. Keys provide the flexibility and adaptability, while locks ensure precision and control. Together, they form the backbone of a modular, emergent system capable of driving sophisticated gameplay experiences.


# **KLEPExecutableBase: The Foundation of Execution**

The **KLEPExecutableBase** class serves as the foundational structure for all executable behaviors in the KLEP system. It unifies the **Sensor-Action-Router-Goal (SARG)** architecture under a single framework, enabling seamless integration of diverse behaviors while maintaining clarity and modularity.

---

## **What is an Executable?**

In KLEP, an executable is any behavior that interacts with the environment or the system through **keys and locks**. It can:

- **Release keys**: Trigger actions or states by introducing new keys into the system.
- **Validate locks**: Determine if the required conditions are met to perform an action.
- **Execute behaviors**: Perform specific tasks, either by itself or as part of a larger goal.

Executables are self-contained and context-sensitive, dynamically adapting to the state of the system through their interactions with keys and locks.

---

## **The SARG System**

The **SARG (Sensor, Action, Router, Goal)** paradigm categorizes executables into four distinct types:

### **1. Sensors**
- **Role**: Generate keys based on external stimuli or internal states.
- **Example**: A proximity sensor detecting a player’s position and releasing a “PlayerNear” key.

### **2. Actions**
- **Role**: Modify the environment or system based on the keys they validate.
- **Example**: Opening a door if the “DoorKey” is validated.

### **3. Routers**
- **Role**: Transform keys into other keys or modify their properties.
- **Example**: Converting a “RawMaterial” key into a “RefinedMaterial” key.

### **4. Goals**
- **Role**: Manage a sequence of executables to achieve a larger objective.
- **Example**: A crafting goal managing sensors (to detect materials), actions (to process them), and routers (to finalize the product).

Goals are a special type of executable and can recursively manage other goals, creating a hierarchical structure for complex behaviors.

---

## **The Lifecycle of an Executable**

Executables operate within a **key-lock-executable cycle**:

1. **Key Creation**: Keys are created or modified, influencing the system’s state.
2. **Lock Validation**: Locks check if the required conditions are met, using the keys in the system.
3. **Execution**: If locks are satisfied, the executable performs its task and potentially releases new keys, restarting the cycle.

This cycle ensures behaviors are interdependent and dynamically responsive.

---

## **Core Features of KLEPExecutableBase**

### **1. Key and Lock Management**
Executables maintain their own sets of:
- **Validation Locks**: Conditions required to validate the executable.
- **Execution Locks**: Conditions required to execute the behavior.

Locks are instantiated as **unique instances** during initialization, ensuring that each executable operates independently.

### **2. Tandem Execution**
- Executables can specify whether they are **in tandem**, meaning they can execute alongside other behaviors in the same cycle.
- This flexibility allows for simultaneous, non-conflicting actions within the system.

### **3. Attraction-Based Decision Making**
- Each executable calculates an **attraction value** based on its locks and the keys available.
- This value informs the **KLEPAgent** about the executable’s desirability, helping prioritize actions in dynamic environments.

### **4. Centralized Updates**
- Executables provide hooks for **frame-based** and **physics-based** updates.
- This centralized approach ensures consistent timing and reduces redundant logic.

---

## **Executable Hierarchy**

The **KLEPExecutableBase** establishes a hierarchy where all executables share core functionality while allowing for customization through inheritance. Key methods include:

### **Initialization**
- `Awake`: Instantiates unique locks and prepares the executable for operation.
- `Init`: A virtual method for custom initialization.

### **Validation and Execution**
- `CanValidate`: Checks if all validation locks are satisfied.
- `CanExecute`: Checks if all execution locks are satisfied.
- `Execute`: Executes the behavior if conditions are met.

### **Attraction Calculation**
- `CalculateAttraction`: Combines lock and key attraction values to determine the executable’s priority.

### **Event Management**
- `RegisterExecutableEvents` and `UnregisterExecutableEvents`: Allow executables to communicate with the **SLASHBridge**, enabling asynchronous, event-driven interactions.

---

## **The Power of Modularity**

By adhering to the **SARG** framework, the KLEPExecutableBase facilitates the creation of highly modular systems:

- **Composable Behaviors**: Combine sensors, actions, routers, and goals to create complex interactions.
- **Hierarchical Goals**: Goals can manage other goals, forming a tree of interdependent objectives.
- **Dynamic Updates**: Executables adapt to new keys and conditions in real time, ensuring responsiveness.

---

## **A Developer-Friendly Approach**

To simplify the design of large systems, the **KLEPExecutableBase** employs a shorthand methodology:

- **Executable → Key → Lock → Executable**
- Each behavior connects to the next through keys and locks, forming a chain of interdependent processes.

Developers can write descriptions and conditions for each behavior **vertically**, creating a clear, cyberpunk-inspired visualization of the system’s flow.

---

## **Strengths and Limitations**

### **Strengths**
- **Modularity**: Supports a wide range of behaviors with minimal coupling.
- **Scalability**: Goals manage complex systems without losing clarity.
- **Real-Time Adaptation**: Responds to changes in the environment instantly.

### **Limitations**
- **Complexity**: Designing hierarchical goals and interdependent locks requires careful planning.
- **Incompleteness**: The current system lacks advanced features for higher-level cognition, such as automated goal prioritization.

---

## **Conclusion**

The **KLEPExecutableBase** is the backbone of the KLEP system, enabling dynamic, modular behaviors across a wide range of applications. Its integration of the **SARG** framework ensures that sensors, actions, routers, and goals work seamlessly together, providing a powerful toolkit for developers to build responsive, scalable systems.

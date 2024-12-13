# Welcome to KLEP

![KLEP Logo](images/logo.png)

KLEP (Key-Lock Executable Process) is a groundbreaking symbolic AI system.

---

## How to Use This Page

This is the official landing page for **KLEP**, a symbolic AI framework inspired by human cognition and designed to mimic human decision-making through symbolic logic.  

If fully realized, KLEP aims to simulate thought processes and decision-making like humans. Failing that, it will stand as a robust logical framework for use in systems and games.

Below, you’ll find sections detailing the various systems of KLEP. Each section has a flag indicating its status:

<div style="border: 1px solid #444; background-color: #000; padding: 5px; border-radius: 5px; color: #fff;">
  <ul>
    <li>🟢 <strong>Green</strong>: Tested and fully functional.</li>
    <li>🟡 <strong>Yellow</strong>: Code exists but requires more testing.</li>
    <li>🔴 <strong>Red</strong>: Theoretical; no code yet.</li>
  </ul>
</div>

Each section will contain a link to a more indepth discussion of the sytem and what it does and why it does what it does. Below that, you will find each section has a list of features that KLEP provides within that specific system. These features will follow the same color coding as the sections holistic compleation view to show you what needs work and what the system can do for you today.

### About This Page

This page explores the **philosophical** and **conceptual** aspects of KLEP, the foundations that inspire its logic, and the reflections on what it means to be a "thinking machine." This can be thought of as the **humanities side** of the project—an examination of the deeper questions behind the system.

Meanwhile, the **project page** focuses on the **practical implementation** of these ideas through code. This dual approach allows us to bridge theory and application, addressing both the "why" and the "how" of KLEP.

### Why Unity and C#?

I believe that we **find ourselves through play**, and so the code presented here is geared toward the Unity engine, written in C#. This choice reflects the intent to make KLEP not only an AI framework but also a tool for creativity, experimentation, and exploration. However, the concepts and logic behind KLEP are universal, making it applicable to any system requiring decision-making as agile as humans.

---

## The Components of KLEP

### Neuron (🟢)

View Neuron Details

  Key Management (🟢): Dynamically acquires, holds, and releases symbolic keys as events unfold.
  Executable Synchronization (🟢): Continuously detects and integrates executable components into the logic network, ensuring the neuron’s internal logic remains coherent and up-to-date.
  Event Integration (🟢): Interacts with a universal event bridge, allowing seamless communication across the KLEP framework, facilitating complex, multi-layered decision-making.
  Agent Coordination (🟢): Provides a stable and accessible interface for higher-level agents to guide and influence the neuron’s behavior, enabling scalable complexity in reasoning.
    
### Key-Lock System(🟢)

View Key-Lock Details

  Symbolic Property Management (🟢): Handles a set of properties representing various attributes of the Key, ensuring flexible and extensible data handling.
  Loader Integration (🟢): Synchronizes Key properties with a keyLoader resource, providing dynamic initialization and updates based on predefined defaults.
  Attractiveness and Prioritization (🟢): Incorporates "attractiveness" as a factor that can guide decision-making and sorting, aiding systems that must select among multiple keys.
  Runtime Property Adjustments (🟢): Allows adding, removing, and resetting properties at runtime, facilitating dynamic changes in logic and behavior.
  Seamless Lock Integration (🟢): Designed to fit into the Key→Lock pipeline, ensuring easy validation and control flow when passing Keys through Locks and Executables.

### Lock 🟢
Locks validate keys and control access to executables, ensuring proper sequencing in symbolic logic.

Agent (🟢)

View Agent Details

   Decision-Making Core (🟢): Selects actions (executables) based on Q-values and attraction values, learning from feedback and adjusting behavior over time.
   Q-Learning Integration (🟢): Maintains a Q-table to track and update the value of actions, reinforcing those that yield positive outcomes.
   Non-Forced Action Selection (🟢): Will not select any action if none meet a defined certainty threshold, modeling “patience” and waiting for more favorable conditions.
   Flexible Framework (🟢): Operates over a wide range of executables, supporting both “in-tandem” and “solo” actions, as well as goals that can provide substitute actions.
   State Transitions (🟢): Handles OnEnterState/OnExitState callbacks, enabling executables to have stateful logic that reacts to selection and deselection.

Executable (🟢)

View Executable Details

   Validation and Execution Flow (🟢): Before running, executables must pass validation locks and execution locks to ensure that required conditions are met. This prevents logic from firing prematurely and maintains a clean, predictable logic flow.
   Attraction Calculation (🟢): Computes an “attraction” score based on the properties of keys that satisfy its conditions. This allows the Agent or other decision-making layers to pick the most suitable action at any given time.
   Flexible Key Interaction (🟢): Capable of pushing keys into the neuron or buffering them for the next frame. This ensures a fluid exchange of symbolic data between different parts of the KLEP system.
   Modular Lifecycle (🟢): Provides hooks (OnEnterState, OnExitState, Execute, ExecutableUpdates, ExecutableFixedUpdate) that make it easy to extend behavior. Executables can be simple or complex, depending on the system’s needs.
   Compatibility with the Agent and Goals (🟢): Works with the Agent’s decision-making routines and can be managed by goals, forming layered logic structures and scenario-driven behavior.

Goal (🟡)

View Goal Details

   Layered Execution (🟡): Organizes executables into ordered layers, each with specific completion requirements. Goals can represent multi-step plans where each layer depends on the previous one.
   Conditional Advancement (🟡): Uses configurable execution requirements (AllMustFire, AnyCanFire, NoneNeedToFire) to determine when it’s time to proceed to the next layer, supporting complex, branching logic.
   Activation Keys (🟡): May issue special activation keys to signal that the goal’s conditions have been met, influencing other parts of the system.
   Progress Tracking (🟡): Supports “progress tracking” by remembering the current layer, optionally resetting or continuing from previous states, enabling more narrative or stateful logic flows.
   Integration with the Agent (🟡): Although goals are conceptually robust, they have not been extensively tested in all scenarios. Their logic should be considered stable enough for experimentation, but additional testing and refinement may be needed before confidently declaring them fully “Green.”

### Ethical and Emotional Layers 🟡
A developing system to simulate ethical reasoning and emotional states.

### Memory 🟡
A memory layer to retain information and guide decision-making over time.

### Angles and Demons 🟡
A conceptual layer exploring different perspectives (angles) and biases (demons) in decision-making.

### The Trainyard 🟢
A simulation tool used to test and debug the interactions between keys, locks, and agents.

### Theoretical Layers 🔴
Conceptual components (like creativity) that have no implemented code but provide a roadmap for future exploration.

---

## Learn More

Visit the [GitHub Repository](https://github.com/Roll4d4/KLEP) for practical usage and code.

[Follow us on Twitter](https://twitter.com/roll4d4)  
[Visit Roll4d4.com](https://roll4d4.com)

# Welcome to KLEP

![KLEP Logo](images/logo.png)

KLEP (Key-Lock Executable Process) is a symbolic AI system where decision-making emerges from the interaction of keys, locks, and executables—much like neurons firing in a brain.

---

## How to Use This Page

Welcome to KLEP, where AI can think with symbolic logic, adapt through experience, and generate decisions like a human mind. Each component of KLEP is introduced below, with a **one-line summary** and collapsible feature sections for clarity. The status indicators show how mature each component is:

<div style="border: 1px solid #444; background-color: #000; padding: 5px; border-radius: 5px; color: #fff;">
  <ul>
    <li>🟢 <strong>Green</strong>: Tested and fully functional.</li>
    <li>🟡 <strong>Yellow</strong>: Code exists but requires more testing.</li>
    <li>🔴 <strong>Red</strong>: Theoretical; no code yet.</li>
  </ul>
</div>

**Note:** This page covers the conceptual and philosophical side of KLEP. The [Project Page](project.md) delves deeper into the practical and technical details.

### Why Unity and C#?

KLEP is built for Unity (in C#) for interactive experimentation and rapid prototyping. The underlying logic, however, applies universally—use KLEP in any context that requires agile, human-like reasoning.

---

## Core Systems

### Neuron (🟢)
**What it does:** The Neuron acts as the central node, synchronizing keys and executables to maintain a coherent logic network.

[View Neuron Details](neuron.md)

<details>
<summary>Features (Click to expand)</summary>

- **Key Management (🟢)**: Dynamically acquires, holds, and releases symbolic keys as events unfold.  
- **Executable Synchronization (🟢)**: Continuously detects and integrates executable components, keeping logic coherent.  
- **Event Integration (🟢)**: Hooks into the Bridge to facilitate complex, multi-layered decision-making.  
- **Agent Coordination (🟢)**: Interfaces with higher-level agents, enabling scalable complexity in reasoning.
</details>

---

### Key-Lock System (🟢)
**What it does:** The Key-Lock system encapsulates data and conditions—Keys carry symbolic info, and Locks validate them, ensuring logical order.

[View Key-Lock Details](key-lock.md)

<details>
<summary>Features (Click to expand)</summary>

- **Symbolic Property Management (🟢)**: Keys have flexible, extensible properties.  
- **Loader Integration (🟢)**: Synchronize properties with `SLASHkeyLoader` for dynamic defaults.  
- **Attractiveness & Prioritization (🟢)**: Rank keys for decision-making.  
- **Runtime Adjustments (🟢)**: Add/remove properties on-the-fly.  
- **Seamless Lock Integration (🟢)**: Easy validation pipeline from Keys to Executables.
</details>

---

### SLASH System (🟢)
**What it does:** The SLASH system provides dynamic loaders and managers for keys, enabling runtime creation, updating, and serialization of key data.

[View SLASH System Details](slash.md)

<details>
<summary>Features (Click to expand)</summary>

- **Dynamic Key Creation (🟢)**: Generate new keys on demand with `SLASHkeyLoader`.  
- **Property Synchronization (🟢)**: Ensure keys always have the right defaults and attributes.  
- **Foreign Keys Allowed (🟢)**: Extend keys beyond their initial definitions if desired.  
- **Serialization Support (🟢)**: Save and load key states for persistence.  
- **Adaptive Loaders (🟢)**: Select loaders based on property matches, ensuring keys remain compatible over time.
</details>

---

### Agent (🟢)
**What it does:** The Agent makes decisions by evaluating actions through Q-learning and attraction values, and can choose patience if no action is worthwhile.

[View Agent Details](agent.md)

<details>
<summary>Features (Click to expand)</summary>

- **Decision-Making Core (🟢)**: Picks actions based on Q-values and attraction.  
- **Q-Learning Integration (🟢)**: Adapts over time, reinforcing successful actions.  
- **Non-Forced Action Selection (🟢)**: Can opt to wait rather than choose a poor action.  
- **Flexible Framework (🟢)**: Handles solo and in-tandem executables, plus goal logic.  
- **State Transitions (🟢)**: Manage lifecycles with OnEnter/OnExit callbacks.
</details>

---

### Executable (🟢)
**What it does:** Executables are the actions the system can take, firing when their conditions are met, guided by keys and locks.

[View Executable Details](executable.md)

<details>
<summary>Features (Click to expand)</summary>

- **Validation & Execution Flow (🟢)**: Run only when conditions are met.  
- **Attraction Calculation (🟢)**: Score potential actions for better decision-making.  
- **Key Interaction (🟢)**: Push or buffer keys dynamically.  
- **Modular Lifecycle (🟢)**: Hooks for custom OnEnter/OnExit/Execute behavior.  
- **Integration with Agent & Goals (🟢)**: Fits seamlessly into higher-level logic.
</details>

---

### Goal (🟡)
**What it does:** Goals provide layered, multi-step plans that coordinate complex scenarios, though they’re still under testing.

[View Goal Details](goal.md)

<details>
<summary>Features (Click to expand)</summary>

- **Layered Execution (🟡)**: Multi-step logic sequences.  
- **Conditional Advancement (🟡)**: Move forward only when conditions are met.  
- **Activation Keys (🟡)**: Signal progress to the rest of the system.  
- **Progress Tracking (🟡)**: Remember past states to shape future actions.  
- **Agent Integration (🟡)**: Conceptually sound but needs more testing before going Green.
</details>

---

### Bridge (🟢)
**What it does:** The Bridge is a central event bus, enabling loose coupling between all components.

[View Bridge Details](bridge.md)

<details>
<summary>Features (Click to expand)</summary>

- **Centralized Event System (🟢)**: Broadcast and listen without direct references.  
- **Frame-Specific Tracking (🟢)**: Know exactly what happened this frame.  
- **Flexible Registration (🟢)**: Add or remove listeners at will.  
- **Logging & Debugging (🟢)**: Built-in event log for transparency.  
- **Scalable Integration (🟢)**: Grows effortlessly with your project’s complexity.
</details>

---
###  The following are not present in the Github repository until further testing and developent has been conducted.
---

### 7. Ethical and Emotional Layers (🟡)
**What It Is:** An experimental framework to incorporate morality and emotional states into decision-making, going beyond raw logic or simple cost-benefit analysis. By providing AI with "values" or "feelings," decisions can become more relatable and narrative-driven—ideal for story-rich contexts.

**Status:** Early testing. The groundwork is there, but it’s not fully validated. Still, the vision is to have a system that can weigh ethical dilemmas, reflect preferences, and create emotionally resonant choices.

---

### 8. Memory (🟡)
**What It Is:** A concept to give KLEP a sense of history. The idea is that the system learns from past encounters (keys, actions, outcomes) and uses this history to guide future decisions. Instead of treating each moment in isolation, KLEP can recall and apply past lessons—laying the groundwork for more adaptive, long-term behavior.

**Status:** Prototype stage. The basic logic is understood, but more testing and refinement are needed before memory can reliably shape strategy in complex scenarios.

---

### 9. Angles and Demons (🟡)
**What It Is:** A metaphorical set of advisors—“Aron” and “Nora”—that embody opposing strategies for problem-solving. When KLEP encounters uncertain conditions (unfamiliar keys, low confidence in actions), these advisors weigh in. One angle might push for exploration (like MCTS: searching broader, trying new paths), while the other angle advocates exploitation (like A*: leaning on known successes to find reliable outcomes quickly).

This interplay allows KLEP to adapt its decision-making style: under uncertainty, it might give more weight to the explorer’s advice, cycling through more complex reasoning steps. Over time, as patterns emerge and confidence grows, the system can favor the exploitative strategy that yields consistent results. Combined with the SLASH system for communication, these “Angles and Demons” enable KLEP to dynamically adjust its approach to novel problems, balancing experimentation against known tactics.

**Status:** Conceptual testing. The roles of Aron (exploration) and Nora (exploitation) are defined, and the logic for shifting decision-making emphasis exists in theory. Further development is needed to see how well this system can truly refine KLEP’s strategy under real-world conditions.

---

### 10. The Trainyard (🟢)
**What It Is:** A stable, working test environment where all these concepts can be tried out and debugged. It’s the sandbox that ensures keys, locks, executables, and various decision-making strategies behave as intended before being deployed in a larger project.

[Learn More About The Trainyard](trainyard.md)

---


## Learn More

- [GitHub Repository](https://github.com/Roll4d4/KLEP) – Dive into the code and examples.
- [Follow on Twitter](https://twitter.com/roll4d4) – Latest updates and insights.

By integrating these systems—Neuron, Keys, Locks, Executables, Goals, the Bridge, and the SLASH dynamic loading layer—you can craft AI scenarios that are transparent, adaptable, and genuinely interesting. KLEP aims to merge theory with practice, giving you a new way to model and build thinking machines.  

[Visit Roll4d4.com](https://roll4d4.com)

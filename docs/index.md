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
.- .-.. .-..  ... -.-- ... - . -- ...  .- .-. .  .-..--  .. -.  -.. . ...- . .-.. --- .--. -- . -. -  .- -. -..  - . ... - .. -. --.  
.- .-.. .-..  ... -.-- ... - . -- ...  .- .-. .  .-..--  .. -.  -.. . ...- . .-.. --- .--. -- . -. -  .- -. -..  - . ... - .. -. --.  

###**WARNING:** The following systems are not present in the GitHub repository until further testing and development has been conducted.

.- .-.. .-..  ... -.-- ... - . -- ...  .- .-. .  .-..--  .. -.  -.. . ...- . .-.. --- .--. -- . -. -  .- -. -..  - . ... - .. -. --.  
.- .-.. .-..  ... -.-- ... - . -- ...  .- .-. .  .-..--  .. -.  -.. . ...- . .-.. --- .--. -- . -. -  .- -. -..  - . ... - .. -. --.  
---

### 7. Ethical and Emotional Layers (🟡)
**What It Is:** Envisions AI with moral and emotional underpinnings rather than purely logical choices—useful for narrative-rich or character-driven scenarios.

**Status:** In testing. The core idea exists, but it’s still evolving. Further experimentation and refinement are needed.

[Learn More (Philosophy)](ethics-emotions.md)

---

### 8. Memory (🟡)
**What It Is:** Introduces long-term recall, letting KLEP remember past encounters and learn from them, rather than treating each moment in isolation.

**Status:** Early prototype. Basic logic is known, but more validation is required before it influences decision-making reliably.

[Learn More (Philosophy)](memory-concepts.md)

---

### 9. Angles and Demons (🟡)
**What It Is:** A conceptual pair of advisors—one urging exploration (Aron) and the other favoring exploitation (Nora). They help KLEP adapt when facing uncertainty, toggling between broad searching (like MCTS) and known shortcuts (like A*).

**Status:** Conceptual stage. The theory is there, but practical tests are needed to confirm its effectiveness in guiding decisions.

[Learn More (Philosophy)](angles-demons.md)

---

### 10. The Trainyard (🟢)
**What It Is:** A metaphorical “lab” that helped shape the understanding of KLEP. Historically and conceptually important, it offers a testing ground to ensure that keys, locks, and executables interact as intended before real-world application.

**Status:** Stable and included as part of the system’s development history and conceptual framework.

[Learn More (Philosophy)](trainyard-Original.md)
[Learn More (Philosophy)](trainyard-Extended.md)

---

.- .-.. .-..  ... -.-- ... - . -- ...  .- .-. .  .-..--  .. -.  -.. . ...- . .-.. --- .--. -- . -. -  .- -. -..  - . ... - .. -. --.  
.- .-.. .-..  ... -.-- ... - . -- ...  .- .-. .  .-..--  .. -.  -.. . ...- . .-.. --- .--. -- . -. -  .- -. -..  - . ... - .. -. --.  

###**WARNING:** The following systems are not present in the GitHub repository and are theoretical.

.- .-.. .-..  ... -.-- ... - . -- ...  .- .-. .  .-..--  .. -.  -.. . ...- . .-.. --- .--. -- . -. -  .- -. -..  - . ... - .. -. --.  
.- .-.. .-..  ... -.-- ... - . -- ...  .- .-. .  .-..--  .. -.  -.. . ...- . .-.. --- .--. -- . -. -  .- -. -..  - . ... - .. -. --.  

---

### 11. KlepImagination (🔴)
**What It Is:** A conceptual future layer where keys and locks become a specialized “AI language.” Imagine training a large language model (akin to GPT) exclusively on KLEP’s symbolic framework—its keys, locks, and executables—so it can invent new behaviors and reasoning patterns internally. Instead of just following predefined logic, it would learn to “speak” in KLEP’s language of symbols and conditions, expanding its capabilities beyond manual coding.

**Status:** Purely theoretical. The vision is there, but no implementation exists yet. KlepImagination is a long-term goal that could fundamentally reshape how KLEP evolves its logic, potentially enabling a system to self-author its strategies.

[Learn More (Philosophy)](klepimagination-theory.md)

---

## Learn More

- [GitHub Repository](https://github.com/Roll4d4/KLEP) – Dive into the code and examples.
- [Follow on Twitter](https://twitter.com/roll4d4) – Latest updates and insights.

By integrating these systems—Neuron, Keys, Locks, Executables, Goals, the Bridge, and the SLASH dynamic loading layer—you can craft AI scenarios that are transparent, adaptable, and genuinely interesting. KLEP aims to merge theory with practice, giving you a new way to model and build thinking machines.  

[Visit Roll4d4.com](https://roll4d4.com)

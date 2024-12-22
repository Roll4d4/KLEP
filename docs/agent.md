# **KLEPAgent: The Decision-Maker**

The **KLEPAgent** is the cornerstone of decision-making within the KLEP system. This component is responsible for analyzing the current state, weighing available options, and selecting behaviors to execute. By design, the agent integrates seamlessly into game development and robotics workflows, balancing real-time responsiveness with computational efficiency.

---

## **The Role of the Agent**

The agent’s purpose is to **make choices**, dynamically adapting to the environment and ensuring that only the most relevant behaviors are executed at any given moment. Unlike traditional behavior systems, which may rely on predefined trees or scripts, the KLEPAgent operates on a **curated pool of behaviors** managed by the neuron. 

---

## **Update vs FixedUpdate: Separate Clocks**

In both game development and robotics, there are often two different update cycles:

- **Frame-based (Update)**: For visuals, UI, and general gameplay logic.
- **Physics-based (FixedUpdate)**: For physics simulations and time-sensitive interactions.

The KLEPAgent respects these distinct cycles by separating behavior execution into **frame-based** and **physics-based** workflows. This duality offers:

- **Parallel Execution**: Physics and non-physics behaviors can run side by side without interference.
- **Selective Optimization**: You can disable one cycle entirely if your application doesn’t need it, saving valuable computational resources.

---

## **Behavior Filtering and Dynamic Curation**

### **Need-to-Know Basis**
The KLEPAgent only evaluates behaviors that are **active and accessible**. Behaviors are curated by the neuron, ensuring that:

1. **Disabled Behaviors**: Inactive or irrelevant behaviors are skipped entirely, avoiding unnecessary computation.
2. **Dynamic Updates**: If a new key is created mid-cycle, previously skipped behaviors are rechecked automatically, mitigating race conditions and ensuring real-time adaptability.

This approach allows developers to segment behaviors and keep the agent’s focus on what truly matters.

---

## **Certainty and Higher Cognition**

At the heart of the agent’s decision-making process lies a **certainty threshold**. This threshold governs whether the agent acts confidently or seeks external guidance. 

### **The Certainty Threshold**
- When the agent’s confidence in a decision is high (based on Q-values, key attraction, and lock attraction), it selects a behavior autonomously.
- If the agent becomes uncertain, it looks for **external input** to guide its decisions.

---

## **The Role of Angels and Demons**

When the agent’s confidence falters, it relies on a system of **advisors**:

1. **The Angel**: Focuses on **exploitation**, pushing the agent to rely on known solutions and optimizing for immediate success.
2. **The Demon**: Encourages **exploration**, driving the agent to experiment with new paths and uncover potential opportunities.

These two entities exist in an **adversarial relationship**, with the agent favoring the advisor that proves more successful over time. This balance ensures that the agent can **crawl through the problem space**, seeking advantageous positions while avoiding stagnation.

---

## **Graph Theory and Problem Space Navigation**

The agent's exploration is informed by the **connections between behaviors**:

- **Behavioral Graphs**: Each behavior is analyzed for the keys it can produce and consume, forming a graph-like structure that maps potential pathways.
- **Lock Attraction**: By modifying lock attraction values, the agent can influence its own decisions, effectively "highlighting" certain pathways to make them more appealing.
- **Dynamic Plans**: Using this graph, the agent can form a loose plan, prioritizing behaviors that are likely to yield desirable outcomes.

---

## **Core Components of Decision-Making**

### **1. Weights and Biases**
The agent’s decisions are influenced by a combination of factors:
- **Q-values**: Represent learned values from past experiences, updated through reinforcement learning.
- **Key Attraction**: A measure of the desirability of a key, based on its properties and context.
- **Lock Attraction**: Evaluates how compelling a lock is, guiding the agent toward behaviors that unlock advantageous opportunities.

These values combine to form the **weight** of each decision, with the certainty threshold acting as a filter to ensure quality choices.

### **2. Adversarial Guidance**
When certainty is low:
- **The Angel** biases the weights toward behaviors that have historically succeeded.
- **The Demon** adjusts weights to encourage experimentation and discovery.

This interplay ensures the agent navigates the problem space effectively, balancing **risk and reward**.

### **3. Dynamic Curation**
The agent continuously updates its pool of available behaviors:
- **New Keys**: Trigger a recheck of skipped behaviors, integrating them into the decision pool if they become relevant.
- **Disabled Behaviors**: Are ignored entirely, reducing computational overhead.

---

## **Strengths of the KLEPAgent**

### **1. Flexibility**
The agent’s design supports both:
- **Physics-based environments**: Using FixedUpdate to manage real-world constraints.
- **Frame-based environments**: Leveraging Update for high-speed decisions.

### **2. Adaptability**
- Behaviors are checked dynamically, ensuring the agent adapts to new keys and environmental changes in real time.
- Segmented processing avoids redundant checks, optimizing performance.

### **3. Scalability**
- Higher-level cognition systems (e.g., Angels and Demons) can be plugged in without disrupting the core agent logic.
- Graph-based navigation allows for emergent, scalable behavior planning.

---

## **Limitations and Future Development**

While the current implementation excels at practical decision-making, the higher cognition systems (e.g., Angels, Demons, and behavioral graphs) remain **conceptual**. Future work will focus on:
- **Formalizing Graph Navigation**: Developing algorithms to efficiently traverse behavioral graphs.
- **Expanding Reinforcement Learning**: Refining Q-value updates to account for more complex reward structures.
- **Enhancing Certainty Calculations**: Introducing more sophisticated methods to evaluate confidence and trigger higher cognition.

---

## **Conclusion**

The KLEPAgent represents a significant step forward in dynamic, adaptive decision-making systems. By integrating frame-based and physics-based execution, leveraging weights and biases, and introducing adversarial guidance, it provides a robust framework for navigating complex problem spaces.

Whether you’re building a game AI, a robotic system, or an experimental simulation, the KLEPAgent offers the tools to make intelligent, scalable, and context-sensitive decisions.

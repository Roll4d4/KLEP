using System;
using System.Collections.Generic;
using UnityEngine;

public class SLASHBridge : MonoBehaviour
{
    
    public delegate void UniversalEventDelegate(string eventName, object eventData);    // Delegate definition for universal events.

    // Dictionary to map event names to their corresponding delegate actions.
    private Dictionary<string, UniversalEventDelegate> eventsDictionary = new Dictionary<string, UniversalEventDelegate>();
    private HashSet<string> currentFramesEvents = new HashSet<string>();                // List to keep track of events that occur in the current frame.
    private List<string> eventLog = new List<string>();                                 // For logging events
    public List<string> GetEventLog() => eventLog;
    public bool enableLogging = true; // Add this field to enable or disable logging

    // Method to handle universal events system -> UES
    public void UniversalEvent(string eventName, object eventData = null, string source = "NOT Filled out")
    {
        currentFramesEvents.Add(eventName);                             // add to current frames log

        if (eventsDictionary.TryGetValue(eventName, out var handler))   // see if we have that event registered
        {

            if (handler != null)                                        // null check
            {   handler.Invoke(eventName, eventData);                   // invoke the event
                LogEvent(eventName, source);   }                        // Log our event
            else LogEvent(eventName, source, false);                    // null indicates it's an unregistered event

        }
        else
        { LogEvent(eventName, source, false); Debug.Log($"Unregistered event : {eventName} called by {source}"); }                         // false indicates it's an unregistered event

    }

    // Method to check if a specific event occurred in the current frame.
    public bool CheckEventOccurredThisFrame(string eventName)
    { return currentFramesEvents.Contains(eventName); }

    // Method to check if a specific event is registered.
    public bool CheckEventIsRegistered(string eventName)
    { return eventsDictionary.ContainsKey(eventName);  }

    // called from neuron to handle update logic
    public void BridgeUpdate()
    { currentFramesEvents.Clear(); } // Clear the events of the current frame at the end of each frame.
    
    // Registers a delegate to an event. If the event does not exist in the dictionary, it is added. If it does exist, the delegate is appended.
    public void RegisterEvent(string eventName, UniversalEventDelegate eventDelegate)
    {

        if (!eventsDictionary.ContainsKey(eventName))       // Check if the event already exists in the dictionary
        { eventsDictionary[eventName] = eventDelegate; }    // If it doesn't, add it with the delegate
        else
        { eventsDictionary[eventName] += eventDelegate; }   // If it does, append the new delegate to the existing one
        
    }

    // Unregisters a delegate from an event. If the delegate is the last one registered to the event, the event is effectively unregistered.
    public void UnregisterEvent(string eventName, UniversalEventDelegate eventDelegate)
    {
        
        if (eventsDictionary.ContainsKey(eventName))        // Check if the event exists in the dictionary
        { eventsDictionary[eventName] -= eventDelegate; }   // If it does, remove the delegate from it
       
        if (eventsDictionary[eventName] == null || eventsDictionary[eventName].GetInvocationList().Length == 0)
        { eventsDictionary.Remove(eventName); }             // Prune dictionary if delegate is null
    }

    // Logs the occurrence of an event, along with the source and a timestamp, to the eventLog list.
    private void LogEvent(string eventName, string source, bool isRegistered = true)
    {

        if (!enableLogging) return; // Check this condition before logging
        string timestamp = DateTime.UtcNow.ToString("o"); // Get the current timestamp in ISO 8601 format
        // Prepare the log message. Different messages for registered and unregistered events.
        string logMessage = isRegistered ? $"Event Triggered: {eventName}, Source: {source}, Timestamp: {timestamp}" :
                                           $"Unregistered Event: {eventName}, Source: {source}, Timestamp: {timestamp}";        
        eventLog.Add(logMessage);                         // Add the log message to the eventLog list

    }
}

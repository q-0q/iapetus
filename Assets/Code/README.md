# FSMs

FSMs are the core game entity system for driving control flow for objects in response to game events. They are implemented with the Wasp engine but follow a particular pattern.

FSMs make heavy use of nested inheritance and for this reason the file structure should be kept consistent throughout the hierarchy to ensure readability and refactorability.

FSM file system:

# `_main.cs`

Contains declarations for the 
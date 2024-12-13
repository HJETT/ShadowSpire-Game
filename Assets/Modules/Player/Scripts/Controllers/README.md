# Controller Module
This module covers the controller system, how to use it, how to create new controllers for specific movements and what each component does.

## Inputs
This is where we will talk about the keybinds and how to add an input.

### Change keybinds
To change the actual keybind, you just need to edit [this file](./Input%20Assets/PlayerInput.inputactions).

### Add an input
To add an input to the game, you will need to do multiple things. 

1. Create a new keybind for it
2. Go inside `InputMaster`
3. Create a delegate for the input. If the input gives information (*like a direction*) or receives information, this is where you will add it
4. Create an event with the new delegate
5. Create a new method with the correct parameters. You will also need to call your delegate inside the method
6. Go inside `Controller`
7. Add a new method for your input. This has the same parameters as your delegate
8. Go back to `InputMaster`
9. Subscribe/Unsubscribe your method inside `InputMaster.Operations.+` and `InputMaster.Operations.-`

This is a long process, but it allows to centralize the inputs between controllers. **If, at any step, you are confused, you can look at the other methods or ask the authors**.

Of course, depending on the desired result, the process can change.

## Controllers
The magic is managed by `InputMaster` and `ControllerManager`. They both ease the addition of new controllers.

### How to add a new controller
To add a new controller, you need to inherit `Controller`. You can then override the desired functions to execute what you need.

Sometimes, the controller could actually hide the base function without any consequences. However, it is hard to predict what will be the needs for the future (*maybe we will need to execute stuff in the start method for every controller*).

### Controller Stack
`ControllerManager` works by using a stack to recover previous used controllers. For example, you can switch to a controller and easily exit to the previous controller, no matter the controller used.

By default, the controller on top of the stack is `PlayerController`. The system won't let you exit a controller if no other controller are present in the stack.

## Interact system
The controllers can also interact with the world. To make a controller able to interact, you will need to call the appropriate functions from `IInteractable`.

### Add an interactable object
To add an interactable object to the world, you need to create a script that implements the interface `IInteractable`. You will get a method that will be called upon an interaction. You also need to add a collider to the object and put its layer as `Interactable`.
Otherwise, the system will not see it.

If you want to make an interact blocker, you can omit to add the script whilst adding everything else.

### Authors:
- WarperSan
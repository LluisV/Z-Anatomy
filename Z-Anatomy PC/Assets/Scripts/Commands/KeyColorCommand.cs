using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyColorCommand : ICommand
{
    //Obj, prev position
    Action method;

    public KeyColorCommand(Action method)
    {
        this.method = method;
    }

    public bool Equals(ICommand command)
    {
        return command.GetType() == GetType() && (command as KeyColorCommand).method == method;
    }

    public void Execute()
    {
        method();
    }

    public void Undo()
    {
        method();
    }

    public bool IsEmpty()
    {
        return method == null;
    }
}

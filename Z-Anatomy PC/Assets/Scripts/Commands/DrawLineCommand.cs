using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineCommand : ICommand
{

    GameObject line;

    public DrawLineCommand(GameObject line)
    {
        this.line = line;
    }

    public bool Equals(ICommand command)
    {
        return command.GetType() == GetType() && (command as DrawLineCommand).line == line;
    }

    public void Execute()
    {
        Pencil.instance.InstantiateLine(line);
    }

    public bool IsEmpty()
    {
        return line == null;
    }

    public void Undo()
    {
        Pencil.instance.DestroyLine(line);
    }


}

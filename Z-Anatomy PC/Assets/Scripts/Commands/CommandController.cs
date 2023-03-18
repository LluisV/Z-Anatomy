using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
    bool Equals(ICommand command);
    bool IsEmpty();
}

public static class CommandController
{
    private static List<ICommand> commands = new List<ICommand>();
    private static int index;
    public static void AddCommand(ICommand command, bool autoExecute = false)
    {
        if(index < commands.Count)
        {
            commands.RemoveRange(index, commands.Count - index);
        }
        index++;
        commands.Add(command);
        if (autoExecute)
            command.Execute();
        //Debug.Log("Added: " + command + "\ncount: " + commands.Count + "\nindex: " + index);
    }

    public static void UndoCommand()
    {
        if (commands.Count == 0)
            return;
        if (index > 0)
        {
            index--;
            commands[index].Undo();
            if(index > 0 && commands[index - 1] is not DeleteCommand && commands[index - 1] is not MoveCommand && commands[index] is not DrawLineCommand && commands[index] is not MoveCommand && commands[index] is not KeyColorCommand)
                commands[index - 1].Execute();
        }
    }

    public static void RedoCommand()
    {
        if (commands.Count == 0)
            return;
        if(index < commands.Count)
        {
            commands[index].Execute();
            index++;
        }
    }

    public static bool UndoEnabled()
    {
        return index > 0;
    }

    public static bool RedoEnabled()
    {
        return index <= commands.Count - 1;
    }

    public static bool IsLastDifferent(ICommand command)
    {
        if (commands.Count == 0 || index == 0)
            return true;
        return !command.Equals(commands[index-1]);
    }

    public static void Reset()
    {
        commands = new List<ICommand>();
        index = 0;
    }
}

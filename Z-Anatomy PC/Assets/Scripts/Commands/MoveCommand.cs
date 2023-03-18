using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveCommand : ICommand
{
    //Obj, prev position
    List<TangibleBodyPart> objects = new List<TangibleBodyPart>();
    List<Vector3> delta = new List<Vector3>();

    public bool IsEmpty()
    {
        return objects == null || objects.Count == 0;
    }

    public MoveCommand(List<TangibleBodyPart> objects, List<Vector3> delta)
    {
        this.objects = new List<TangibleBodyPart>(objects);
        this.delta = delta;
    }

    public bool Equals(ICommand command)
    {
        return command.GetType() == GetType() && (command as MoveCommand).objects.SequenceEqual(objects)
            && (command as MoveCommand).delta.SequenceEqual(delta);
    }

    public void Execute()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Translate(delta[i]);
            objects[i].UpdateBounds();
        }
        TranslateObject.Instance.SetGizmoCenter();
    }

    public void Undo()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Translate(-delta[i]);
            objects[i].UpdateBounds();
        }
        TranslateObject.Instance.SetGizmoCenter();
    }
}

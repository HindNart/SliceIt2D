using UnityEngine;
using UnitySpriteCutter;

public class SliceCommand
{
    // private readonly IBzSliceable target;
    private readonly GameObject target;
    // private readonly Plane slicePlane;
    private readonly Vector2 start;
    private readonly Vector2 end;

    // public SliceCommand(IBzSliceable target, Plane slicePlane)
    // {
    //     this.target = target;
    //     this.slicePlane = slicePlane;
    // }

    public SliceCommand(GameObject target, Vector2 start, Vector2 end)
    {
        this.target = target;
        this.start = start;
        this.end = end;
    }

    public void Execute()
    {
        // target.Slice(slicePlane, null);
        SpriteCutter.Cut(new SpriteCutterInput
        {
            lineStart = start,
            lineEnd = end,
            gameObject = target,
            gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
        });
    }
}
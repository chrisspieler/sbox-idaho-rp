using Editor;
using Sandbox;

namespace IdahoRP.Entities.Mirror;

/// <summary>
/// </summary>
[Library("func_mirror")]
[HammerEntity]
[Title("Monitor"), Category("Effects"), Icon("monitor")]
public partial class SimpleMirrorEntity : BrushEntity
{

    [Net]
    internal string ModelName { get; set; }

    private SimpleMirrorSceneObject _so;

    public override void Spawn()
    {
        base.Spawn();

		ModelName = GetModelName();
		EnableDrawing = false;
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();

        _so = new SimpleMirrorSceneObject(Game.SceneWorld, this);
    }

    [Event.Client.Frame]
    public void OnFrame()
    {
		if ( !Game.IsClient )
			return;

		_so?.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _so?.Destroy();
    }
}

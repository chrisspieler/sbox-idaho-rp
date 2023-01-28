
using Sandbox;

namespace IdahoRP.Entities.Mirror;

public partial class SimpleMirrorSceneObject : SceneCustomObject
{
	private SimpleMirrorEntity _entity;
	private PlanarReflection _reflection;
	private Model _mirrorModel;

	public SimpleMirrorSceneObject( SceneWorld sceneWorld, SimpleMirrorEntity parent ) : base( sceneWorld )
	{
		_entity = parent;

		_mirrorModel = Model.Load( _entity.ModelName );
		_reflection = new PlanarReflection( Game.SceneWorld, _mirrorModel, _entity.Transform);

		AddChild( "planar_reflection", _reflection );
	}

	internal void Destroy()
	{
		if ( _reflection != null && _reflection.IsValid )
		{
			_reflection.Delete();
			_reflection = null;
		}
	}

	internal void Update()
	{
		Log.Info( $"{_entity.LocalRotation.Left}" );
		_reflection.Update(Vector3.Zero, Rotation.Up);
		_reflection.OnRender();
	}
}

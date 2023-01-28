/* Copied from the base project. */

using Sandbox;

namespace IdahoRP.Entities.Mirror;

public class PlanarReflection : ScenePortal
{
	public PlanarReflection( SceneWorld world, Model model, Transform transform) : base( world, model, transform, true, (int)System.Math.Min( 512.0f, System.Math.Max( Screen.Width, Screen.Height ) ) )
	{
	}

	/// <summary>
	///	Updates the reflection information before the render
	/// </summary>
	public void Update( Vector3 reflectionOffset, Vector3 reflectionNormal, float clipPlaneOffset = 0 )
	{
		if ( !Parent.IsValid() )
		{
			Log.Warning( "PlanarReflection does not have a Parent" );
			return;
		}

		// Clip plane
		Vector3 planeOffset = reflectionNormal * clipPlaneOffset;
		Plane p = new Plane( reflectionOffset + planeOffset, reflectionNormal );
		SetClipPlane( p );

		// Reflect
		SceneCamera mainCam = Camera.Main;
		Matrix viewMatrix = Matrix.CreateWorld( mainCam.Position, mainCam.Rotation.Forward, mainCam.Rotation.Up );
		Matrix reflectMatrix = ReflectMatrix( viewMatrix, p );

		// Apply Rotation
		Vector3 reflectionPosition = reflectMatrix.Transform( mainCam.Position );
		Rotation reflectionRotation = ReflectRotation( mainCam.Rotation, reflectionNormal );

		ViewPosition = reflectionPosition;
		ViewRotation = reflectionRotation;

		// TODO: When we exceed this distance, blend with SSR. After X distance of SSR, goto cubemap only
		ZFar = 1000.0f;
	}

	/// <summary>
	///	Update on the render thread
	/// </summary>
	public void OnRender()
	{
		UpdateAspectRatio();
	}

	/// <summary>
	///	Updates the aspect ratio of the reflection to match the view
	/// </summary>
	public void UpdateAspectRatio()
	{
		if ( !Graphics.IsActive )
		{
			return;
		}
		Aspect = Graphics.Viewport.Size.x / Graphics.Viewport.Size.y;

		// See: PerformS1FovHack
		FieldOfView = System.MathF.Atan( System.MathF.Tan( Camera.FieldOfView.DegreeToRadian() * 0.5f ) * (Aspect * 0.75f) ).RadianToDegree() * 2.0f;
	}

	/// <summary>
	/// Returns true if the renderer is currently rendering the world using a reflection view
	/// </summary>
	public static bool IsRenderingReflection()
	{
		// We shouldn't ship this, can't find any other way to check for this currently, this is called before we can check for "VrMonitor"
		return Graphics.Viewport.Size.x == Graphics.Viewport.Size.y;
	}


	/// <summary>
	/// Returns a reflected matrix given a plane ( Reflection normal and distance )
	/// </summary>
	public Matrix ReflectMatrix( Matrix m, Plane plane )
	{
		m.Numerics.M11 = 1.0f - 2.0f * plane.Normal.x * plane.Normal.x;
		m.Numerics.M21 = -2.0f * plane.Normal.x * plane.Normal.y;
		m.Numerics.M31 = -2.0f * plane.Normal.x * plane.Normal.z;
		m.Numerics.M41 = -2.0f * -plane.Distance * plane.Normal.x;

		m.Numerics.M12 = -2.0f * plane.Normal.y * plane.Normal.x;
		m.Numerics.M22 = 1.0f - 2.0f * plane.Normal.y * plane.Normal.y;
		m.Numerics.M32 = -2.0f * plane.Normal.y * plane.Normal.z;
		m.Numerics.M42 = -2.0f * -plane.Distance * plane.Normal.y;

		m.Numerics.M13 = -2.0f * plane.Normal.z * plane.Normal.x;
		m.Numerics.M23 = -2.0f * plane.Normal.z * plane.Normal.y;
		m.Numerics.M33 = 1.0f - 2.0f * plane.Normal.z * plane.Normal.z;
		m.Numerics.M43 = -2.0f * -plane.Distance * plane.Normal.z;

		m.Numerics.M14 = 0.0f;
		m.Numerics.M24 = 0.0f;
		m.Numerics.M34 = 0.0f;
		m.Numerics.M44 = 1.0f;

		return m;
	}

	/// <summary>
	/// Returns a reflected matrix given a reflection normal
	/// </summary>
	private Rotation ReflectRotation( Rotation source, Vector3 normal )
	{
		return Rotation.LookAt( Vector3.Reflect( source * Vector3.Forward, normal ), Vector3.Reflect( source * Vector3.Up, normal ) );
	}

};

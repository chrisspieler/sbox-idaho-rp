using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP;

public static class NavPathSegmentExtensions
{
	public static Vector3 GetEndPosition(this NavPathSegment segment)
		=> segment.Position + segment.Forward * segment.Length;
}

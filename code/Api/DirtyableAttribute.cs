using System;

namespace IdahoRP.Api;

[AttributeUsage( AttributeTargets.Property, Inherited = true, AllowMultiple = true )]
sealed class DirtyableAttribute : Attribute
{

}

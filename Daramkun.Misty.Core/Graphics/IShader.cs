﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Misty.Graphics
{
	[Flags]
	public enum ShaderType
	{
		Unknown = 0,

		VertexShader = 1 << 0,

		PixelShader = 1 << 1,
		FragmentShader = PixelShader,

		GeometryShader = 1 << 2,
	}

	public class ShaderOption
	{
		public string [] AttributeOrdering { get; set; }
	}

	public interface IShader : IDisposable
	{
		ShaderType ShaderType { get; }
		ShaderOption Option { get; set; }

		object Handle { get; }
	}
}

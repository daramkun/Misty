﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Daramkun.Misty.Common;

namespace Daramkun.Misty.Graphics
{
	class Effect : StandardDispose, IEffect
	{
		IGraphicsDevice graphicsDevice;
		IShader vertexShader;
		IShader pixelShader;

		public object Handle { get { return new [] { vertexShader, pixelShader }; } }

		public IShader VertexShader { get { return vertexShader; } }
		public IShader PixelShader { get { return pixelShader; } }
		public IShader GeometryShader { get { return null; } }

		public Effect ( IGraphicsDevice graphicsDevice, IShader vertexShader, IShader pixelShader, IShader geometryShader = null )
		{
			InitializeEffect ( graphicsDevice, vertexShader, pixelShader, geometryShader );
		}

		public Effect ( IGraphicsDevice graphicsDevice, XmlDocument xmlDoc )
		{
			foreach ( XmlNode node in xmlDoc.ChildNodes [ 1 ].ChildNodes )
			{
				if ( node.Name == "shader" )
				{
					if ( node.Attributes [ "language" ].Value != "hlsl" ) continue;
					if ( ( node.Attributes [ "version" ] != null && new Version ( node.Attributes [ "version" ].Value ) <= Core.GraphicsDevice.Information.ShaderVersion )
						|| node.Attributes [ "version" ] == null )
					{
						switch ( node.Attributes [ "type" ].Value )
						{
							case "vertex":
								if ( VertexShader != null ) break;
								vertexShader = new Shader ( graphicsDevice, ShaderType.VertexShader, node.ChildNodes [ 0 ].Value );
								break;
							case "pixel":
								if ( PixelShader != null ) break;
								pixelShader = new Shader ( graphicsDevice, ShaderType.PixelShader, node.ChildNodes [ 0 ].Value );
								break;
							case "geometry":
								throw new PlatformNotSupportedException ();
						}
					}
				}
			}

			InitializeEffect ( graphicsDevice, VertexShader, PixelShader, GeometryShader );
		}

		private void InitializeEffect ( IGraphicsDevice graphicsDevice, IShader VertexShader, IShader PixelShader, IShader GeometryShader )
		{
			this.graphicsDevice = graphicsDevice;

			vertexShader = VertexShader;
			pixelShader = PixelShader;
		}

		public void Begin ()
		{
			SharpDX.Direct3D9.Device device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;

			device.VertexShader = vertexShader.Handle as SharpDX.Direct3D9.VertexShader;
			device.PixelShader = pixelShader.Handle as SharpDX.Direct3D9.PixelShader;
		}

		public void End ()
		{
			SharpDX.Direct3D9.Device device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;

			device.VertexShader = null;
			device.PixelShader = null;
		}

		public void SetUniform<T> ( string name, T value ) where T : struct
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var constantTable = ( vertexShader.Handle as SharpDX.Direct3D9.VertexShader ).Function.ConstantTable;
			var handle = constantTable.GetConstantByName ( null, name );
			constantTable.SetValue<T> ( device, handle, value );
		}

		public void SetUniform ( string name, params int [] value )
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var constantTable = ( vertexShader.Handle as SharpDX.Direct3D9.VertexShader ).Function.ConstantTable;
			var handle = constantTable.GetConstantByName ( null, name );
			constantTable.SetValue<int> ( device, handle, value );
		}

		public void SetUniform ( string name, params float [] value )
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var constantTable = ( vertexShader.Handle as SharpDX.Direct3D9.VertexShader ).Function.ConstantTable;
			var handle = constantTable.GetConstantByName ( null, name );
			constantTable.SetValue<float> ( device, handle, value );
		}

		public void SetTextures ( params TextureArgument [] args )
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var constantTable = ( pixelShader.Handle as SharpDX.Direct3D9.PixelShader ).Function.ConstantTable;

			foreach ( TextureArgument texture in args )
			{
				var handle = constantTable.GetConstantByName ( null, texture.Uniform );
				var samplerIndex = constantTable.GetSamplerIndex ( handle );

				device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.MinFilter, ChangeFilter ( texture.Filter ) );
				device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.MagFilter, ChangeFilter ( texture.Filter ) );

				device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.AddressU, ChangeAddress ( texture.Addressing ) );
				device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.AddressV, ChangeAddress ( texture.Addressing ) );

				device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.MaxAnisotropy, texture.AnisotropicLevel );

				device.SetTexture ( samplerIndex, texture.Texture.Handle as SharpDX.Direct3D9.Texture );
			}
		}

		private int ChangeFilter ( TextureFilter textureFilter )
		{
			switch ( textureFilter )
			{
				case TextureFilter.Nearest: return ( int ) SharpDX.Direct3D9.TextureFilter.Point;
				case TextureFilter.Linear: return ( int ) SharpDX.Direct3D9.TextureFilter.Linear;
				case TextureFilter.Anisotropic: return ( int ) SharpDX.Direct3D9.TextureFilter.Anisotropic;
				default: throw new ArgumentException ();
			}
		}

		private int ChangeAddress ( TextureAddressing textureAddressing )
		{
			switch ( textureAddressing )
			{
				case TextureAddressing.Wrap: return ( int ) SharpDX.Direct3D9.TextureAddress.Wrap;
				case TextureAddressing.Mirror: return ( int ) SharpDX.Direct3D9.TextureAddress.Mirror;
				case TextureAddressing.Clamp: return ( int ) SharpDX.Direct3D9.TextureAddress.Clamp;
				default: throw new ArgumentException ();
			}
		}
	}
}

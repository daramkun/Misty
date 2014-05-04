﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Misty.Contents.Tables;
using Daramkun.Misty.Graphics;

namespace Daramkun.Misty.Contents.Loaders
{
	public class EffectContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( IEffect ); } }

		public IEnumerable<string> FileExtensions { get { yield return "sxml"; yield return "lfx"; yield return "xml"; } }

		public bool AutoStreamDispose { get { return false; } }

		public object Load ( Stream stream, ResourceTable resourceTable, params object [] args )
		{
			return Core.GraphicsDevice.CreateEffect ( stream );
		}
	}
}

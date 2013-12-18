﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Misty.Common.Json;

namespace Daramkun.Misty.Contents.Loaders
{
	public class JsonContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( JsonEntry ); } }
		public bool IsSelfStreamDispose { get { return false; } }
		public IEnumerable<string> FileExtensions { get { yield return "json"; } }

		public object Load ( Stream stream, params object [] args )
		{
			return JsonParser.Parse ( stream );
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrimCopy.Experiment.Models.GitHub
{
	public class RateLimit
	{
		public Resources resources { get; set; }
		public Rate rate { get; set; }
	}

	public class Resources
	{
		public Core core { get; set; }
		public Search search { get; set; }
	}

	public class Core
	{
		public int limit { get; set; }
		public int remaining { get; set; }
		public int reset { get; set; }
	}

	public class Search
	{
		public int limit { get; set; }
		public int remaining { get; set; }
		public int reset { get; set; }
	}

	public class Rate
	{
		public int limit { get; set; }
		public int remaining { get; set; }
		public int reset { get; set; }
	}
}
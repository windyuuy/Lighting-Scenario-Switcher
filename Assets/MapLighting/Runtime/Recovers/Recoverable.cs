using System;
using UnityEngine;

namespace MapLighting
{
	public class Recoverable
	{
		public static Recoverable<F> Conv<F>(Component comp, F recover) where F:class
		{
			return new Recoverable<F>(comp,recover);
		}
	}
	
	[Serializable]
	public class Recoverable<T> where T:class
	{
		public string path;
		public T recover;
		
		public void Deconstruct(out string path0, out T recover0)
		{
			path0 = path;
			recover0 = recover;
		}
		
		public Recoverable(Component comp, T recover0)
		{
			var path0 = RecoverTool.GetObjPath(comp.transform);
			this.path = path0;
			this.recover = recover0;
		}
		public Recoverable(string path0, T recover0)
		{
			this.path = path0;
			this.recover = recover0;
		}
	}
}
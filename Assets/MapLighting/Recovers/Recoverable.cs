using System;
using System.Text;
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
		
		public static string GetObjPath(Transform comp)
		{
			var strBuilder = new StringBuilder();
			var comp0 = comp;
			while(comp0.parent!=null)
			{
				strBuilder.Insert(0, comp0.gameObject.name);
				strBuilder.Insert(0, "/");
				comp0 = comp0.parent;
			}
			strBuilder.Insert(0, comp0.gameObject.name);

			return strBuilder.ToString();
		}
		public Recoverable(Component comp, T recover0)
		{
			var path0 = GetObjPath(comp.transform);
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
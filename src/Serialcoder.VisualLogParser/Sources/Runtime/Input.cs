using System;

namespace Serialcoder.VisualLogParser.Runtime
{
	/// <summary>
	/// Summary description for Input.
	/// </summary>
	[Serializable]
	public class Input
	{
		private string name;
		private string progId;
		private string typeName;
		
		public string Name
		{
			get { return this.name; }
			set { name = value; }
		}

		public string ProgId
		{
			get { return progId; }
			set { progId = value; }
		}

		public string TypeName
		{
			get { return typeName; }
			set {typeName = value; }
		}

		public Type Type
		{
			get
			{
				Type t = Type.GetType(string.Format("MSUtil.{0}, Interop.MSUtil", this.typeName));
				if (t == null)
				{
					throw new ApplicationException("Type " + this.typeName + " does not exists");
				}
				else
				{
					return t; 
				}
			}			
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <returns></returns>
		public object CreateInstance()
		{
			//string tmp = string.Format("MSUtil.{0}, Interop.MSUtil", this.typeName);
			//this._type = Type.GetType(tmp);		

			return Activator.CreateInstance(this.Type);
		}
	}
}

using System;

namespace Serialcoder.VisualLogParser.Runtime
{
	/// <summary>
	/// Summary description for Inputs.
	/// </summary>
	[Serializable]
	public class InputCollection : System.Collections.CollectionBase
	{
		public void Add(Input item)
		{
			List.Add(item);
		}

		public void Remove(Input item)
		{
			List.Remove(item);
		}

		public Input this[int index]
		{
			get 
			{
				return (Input) List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		public Input this[string inputName]
		{
			get
			{
				for(int i=0; i < List.Count; i++)
				{
					Input input = (Input) List[i];
					if (input.Name.Equals(inputName))
					{
						return input;
					}
				}
				throw new ApplicationException("There is no Input with named : " + inputName);
			}
		}

		/*
		public Input this[InputEntry name]
		{
			get
			{
				for(int i=0; i < List.Count; i++)
				{
					Input input = (Input) List[i];
					if (input.Name.Equals(name))
					{
						return input;
					}
				}
				return null;
			}
		}
		*/
	}
}

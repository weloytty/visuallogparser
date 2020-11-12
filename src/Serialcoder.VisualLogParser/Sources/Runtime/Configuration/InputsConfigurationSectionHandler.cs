using System;
using System.Xml;

namespace Serialcoder.VisualLogParser.Runtime.Configuration
{
	/// <summary>
	/// Summary description for InputsSectionHandler.
	/// </summary>
	public class InputsConfigurationSectionHandler : System.Configuration.IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			if (section.ChildNodes == null)
			{
				return null;
			}
			InputCollection list = new InputCollection();
			for (int i = 0; i < section.ChildNodes.Count ; i++)
			{
				XmlNode node = section.ChildNodes[i];
				if (node.NodeType == XmlNodeType.Element)
				{
					Input input = new Input();
					input.Name = node.Attributes["name"].InnerText;
					input.ProgId = node.Attributes["progId"].InnerText;
					input.TypeName =  node.Attributes["type"].InnerText;
					list.Add(input);
				}
			}
			return list;
		}

		#endregion
	}
}

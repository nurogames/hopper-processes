using UnityEngine;
using System.Reflection;
using System.Collections;
using System.IO;
using UnityEngine.UIElements;

namespace Nuro.Processes
{
	public class LoadTextureFromDllProcess : Process
	{
		// Private variables
		private Texture2D m_Result = null;
		private Assembly m_Assembly = null;
		private string m_Filename = null;

		// Getter
		public Texture2D Result => m_Result;

		// Constructor
		public LoadTextureFromDllProcess(Assembly assembly, string filename) : base($"Load texture from dll: {filename}")
		{
			m_Assembly = assembly;
			m_Filename = filename;
		}

		// Process function
		protected override IEnumerator ProcessFunc()
		{
			Stream stream = m_Assembly.GetManifestResourceStream(m_Filename);

			using (BinaryReader br = new BinaryReader(stream))
			{
				byte[] buffer = br.ReadBytes((int)stream.Length);

				Texture2D tex = new Texture2D(2, 2);
				tex.LoadImage(buffer);
				m_Result = tex;
			}

			if(m_Result == null || (m_Result.width == 2 && m_Result.height == 2))
			{
				Debug.LogError($"Failed to load texture [{m_Filename}] from dll [{m_Assembly.GetName().Name}]");
			}

			yield break;
		}
	}
}

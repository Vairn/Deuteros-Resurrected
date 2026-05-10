using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
	public partial class GameConfig
	{
		private const string ConfigPath = "user://settings.cfg";
		private readonly ConfigFile _config = new ConfigFile();

		public GameConfig()
		{
			Load();
		}

		public void Load()
		{
			if (FileAccess.FileExists(ConfigPath))
				_config.Load(ConfigPath);
		}

		public void Save()
		{
			_config.Save(ConfigPath);
		}

		public void SetValue(string section, string key, Variant value)
		{
			_config.SetValue(section, key, value);
			Save();
		}

		public Variant GetValue(string section, string key, Variant @default = default)
		{
			return _config.GetValue(section, key, @default);
		}

		public bool HasValue(string section, string key)
		{
			return _config.HasSectionKey(section, key);
		}
	}
}
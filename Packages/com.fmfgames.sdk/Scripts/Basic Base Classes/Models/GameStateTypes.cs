using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BaseSDK.Models {
	[Serializable]
	public class GameState {
		public int GameStateVersion = 0;
		public long LastLogout = 0;

		public void Save () => LastLogout = DateTime.Now.Ticks;

		public override string ToString () => JsonConvert.SerializeObject(this);
	}
}
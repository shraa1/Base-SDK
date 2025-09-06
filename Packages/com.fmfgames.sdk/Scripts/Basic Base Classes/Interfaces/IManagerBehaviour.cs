namespace BaseSDK {
	public interface IManagerBehaviour {
		void Save ();
		void Load ();
		void CheckForUpgrade ();
		void Upgrade (int oldVersion, int newVersion);
	}
}
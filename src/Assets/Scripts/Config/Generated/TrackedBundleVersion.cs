using System.Collections;

public class TrackedBundleVersion
{
	public static readonly string bundleIdentifier = "com.AndrewCornies.LegendPeak";

	public static readonly TrackedBundleVersionInfo Version_1_2_0 =  new TrackedBundleVersionInfo ("1.2.0", 0);
	
	public ArrayList history = new ArrayList ();

	public TrackedBundleVersionInfo current = new TrackedBundleVersionInfo ("1.2.0", 0);

	public  TrackedBundleVersion() {
		history.Add (current);
	}

}

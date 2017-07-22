package md5db078303234b2a78e4ae1fbac273029c;


public class MoleMassCalc
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("GlockChemAndroid.MoleMassCalc, GlockChemAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MoleMassCalc.class, __md_methods);
	}


	public MoleMassCalc () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MoleMassCalc.class)
			mono.android.TypeManager.Activate ("GlockChemAndroid.MoleMassCalc, GlockChemAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

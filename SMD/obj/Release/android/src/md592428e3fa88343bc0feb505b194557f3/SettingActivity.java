package md592428e3fa88343bc0feb505b194557f3;


public class SettingActivity
	extends android.preference.PreferenceActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_attachBaseContext:(Landroid/content/Context;)V:GetAttachBaseContext_Landroid_content_Context_Handler\n" +
			"n_onDestroy:()V:GetOnDestroyHandler\n" +
			"n_onStart:()V:GetOnStartHandler\n" +
			"";
		mono.android.Runtime.register ("SMD.SettingActivity, SMD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SettingActivity.class, __md_methods);
	}


	public SettingActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SettingActivity.class)
			mono.android.TypeManager.Activate ("SMD.SettingActivity, SMD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void attachBaseContext (android.content.Context p0)
	{
		n_attachBaseContext (p0);
	}

	private native void n_attachBaseContext (android.content.Context p0);


	public void onDestroy ()
	{
		n_onDestroy ();
	}

	private native void n_onDestroy ();


	public void onStart ()
	{
		n_onStart ();
	}

	private native void n_onStart ();

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

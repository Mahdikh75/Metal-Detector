package md592428e3fa88343bc0feb505b194557f3;


public class SensorLTActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		android.content.DialogInterface.OnClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_attachBaseContext:(Landroid/content/Context;)V:GetAttachBaseContext_Landroid_content_Context_Handler\n" +
			"n_onStart:()V:GetOnStartHandler\n" +
			"n_onClick:(Landroid/content/DialogInterface;I)V:GetOnClick_Landroid_content_DialogInterface_IHandler:Android.Content.IDialogInterfaceOnClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("SMD.SensorLTActivity, SMD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SensorLTActivity.class, __md_methods);
	}


	public SensorLTActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SensorLTActivity.class)
			mono.android.TypeManager.Activate ("SMD.SensorLTActivity, SMD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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


	public void onStart ()
	{
		n_onStart ();
	}

	private native void n_onStart ();


	public void onClick (android.content.DialogInterface p0, int p1)
	{
		n_onClick (p0, p1);
	}

	private native void n_onClick (android.content.DialogInterface p0, int p1);

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

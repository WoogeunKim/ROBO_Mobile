package crc640a67887a4134e062;


public class BoxModeChangedListener
	extends com.devexpress.editors.layout.OnBoxMarginChangedListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onChange:(Landroid/graphics/Rect;)V:GetOnChange_Landroid_graphics_Rect_Handler\n" +
			"";
		mono.android.Runtime.register ("DevExpress.XamarinForms.Editors.Wrappers.BoxModeChangedListener, DevExpress.XamarinForms.Editors.Android", BoxModeChangedListener.class, __md_methods);
	}


	public BoxModeChangedListener ()
	{
		super ();
		if (getClass () == BoxModeChangedListener.class)
			mono.android.TypeManager.Activate ("DevExpress.XamarinForms.Editors.Wrappers.BoxModeChangedListener, DevExpress.XamarinForms.Editors.Android", "", this, new java.lang.Object[] {  });
	}


	public void onChange (android.graphics.Rect p0)
	{
		n_onChange (p0);
	}

	private native void n_onChange (android.graphics.Rect p0);

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

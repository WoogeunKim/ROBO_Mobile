package mono.com.devexpress.editors.dropdown;


public class DXDropdownContainer_CoerceValueListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.devexpress.editors.dropdown.DXDropdownContainer.CoerceValueListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_coerceIsDropdownOpen:()V:GetCoerceIsDropdownOpenHandler:DevExpress.Xamarin.Android.Dropdown.DXDropdownContainer/ICoerceValueListenerInvoker, DevExpress.Xamarin.Android.Editors\n" +
			"";
		mono.android.Runtime.register ("DevExpress.Xamarin.Android.Dropdown.DXDropdownContainer+ICoerceValueListenerImplementor, DevExpress.Xamarin.Android.Editors", DXDropdownContainer_CoerceValueListenerImplementor.class, __md_methods);
	}


	public DXDropdownContainer_CoerceValueListenerImplementor ()
	{
		super ();
		if (getClass () == DXDropdownContainer_CoerceValueListenerImplementor.class)
			mono.android.TypeManager.Activate ("DevExpress.Xamarin.Android.Dropdown.DXDropdownContainer+ICoerceValueListenerImplementor, DevExpress.Xamarin.Android.Editors", "", this, new java.lang.Object[] {  });
	}


	public void coerceIsDropdownOpen ()
	{
		n_coerceIsDropdownOpen ();
	}

	private native void n_coerceIsDropdownOpen ();

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

using Gtk;
using System;

public class TagMenu : Menu {
	private TagStore tag_store;
	private MenuItem parent_item;

	public delegate void TagSelectedHandler (Tag t);
	public event TagSelectedHandler TagSelected;

	private class TagItem : Gtk.ImageMenuItem {
		public Tag Value;

		public TagItem (Tag t) : base (t.Name) {
			Value = t;
			if (t.Icon != null)
				this.Image = new Gtk.Image (t.Icon);
		}

		protected TagItem (IntPtr raw) : base (raw) {}
	}

	public TagMenu (MenuItem item, TagStore store) 
	{
		if (item != null) {
			item.Submenu = this;
			item.Activated += HandlePopulate;
			parent_item = item;
		}

		tag_store = store;
	}

	protected TagMenu (IntPtr raw) : base (raw) {}

	public void Populate () {
		Populate (tag_store.RootCategory, this);
	}

	public void Populate (Category cat, Gtk.Menu parent) {
		foreach (Widget w in parent.Children) {
			w.Destroy ();
		}

		foreach (Tag t in cat.Children) {
			TagItem item = new TagItem (t);
			parent.Append (item);
			item.ShowAll ();

			Category subcat = t as Category;
			if (subcat != null && subcat.Children.Length != 0) {
				Gtk.Menu submenu = new Menu ();
				Populate (t as Category, submenu);

				Gtk.SeparatorMenuItem sep = new Gtk.SeparatorMenuItem ();
				submenu.Prepend (sep);
				sep.ShowAll ();

				TagItem subitem = new TagItem (t);
				subitem.Activated += HandleActivate;
				submenu.Prepend (subitem);
				subitem.ShowAll ();

				item.Submenu = submenu;
			} else {
				item.Activated += HandleActivate;
			}
		} 
	}
	
	private void HandlePopulate (object obj, EventArgs args) {
		this.Populate ();
	}
	
	void HandleActivate (object obj, EventArgs args)
	{
		if (TagSelected != null) {
			TagItem t = obj as TagItem;
			if (t != null)
				TagSelected (t.Value);
			else 
				Console.WriteLine ("Item was not a TagItem");
		}
	}

}

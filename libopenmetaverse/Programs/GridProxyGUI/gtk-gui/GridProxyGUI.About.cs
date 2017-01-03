
// This file has been generated by the GUI designer. Do not modify.
namespace GridProxyGUI
{
	public partial class About
	{
		private global::Gtk.HBox hbox1;
		private global::Gtk.Image image3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView textview1;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget GridProxyGUI.About
			this.Name = "GridProxyGUI.About";
			this.Title = global::Mono.Unix.Catalog.GetString ("About Grid Proxy");
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("GridProxyGUI.libomv.png");
			this.TypeHint = ((global::Gdk.WindowTypeHint)(1));
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Modal = true;
			this.Resizable = false;
			this.AllowGrow = false;
			this.DestroyWithParent = true;
			// Internal child GridProxyGUI.About.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.image3 = new global::Gtk.Image ();
			this.image3.Name = "image3";
			this.image3.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("GridProxyGUI.wgp-logo.png");
			this.hbox1.Add (this.image3);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.image3]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textview1 = new global::Gtk.TextView ();
			this.textview1.Buffer.Text = "\nGrid Proxy GUI\nGtk# Interface for Grid Proxy\n\nBased on Win Grid Proxy by\nJim Rad" +
			"ford <jradford@npl.com>\n\nWritten by\nLatif Khalifa <latifer@streamgrid.net>\n\nCopy" +
			"right (c) 2015\nOpen Metaverse Foundation\n";
			this.textview1.WidthRequest = 250;
			this.textview1.CanFocus = true;
			this.textview1.Name = "textview1";
			this.textview1.Editable = false;
			this.textview1.CursorVisible = false;
			this.textview1.AcceptsTab = false;
			this.textview1.WrapMode = ((global::Gtk.WrapMode)(2));
			this.GtkScrolledWindow.Add (this.textview1);
			this.hbox1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.GtkScrolledWindow]));
			w4.Position = 1;
			w1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(w1 [this.hbox1]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Internal child GridProxyGUI.About.ActionArea
			global::Gtk.HButtonBox w6 = this.ActionArea;
			w6.Name = "dialog1_ActionArea";
			w6.Spacing = 10;
			w6.BorderWidth = ((uint)(5));
			w6.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w7 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w6 [this.buttonOk]));
			w7.Expand = false;
			w7.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 493;
			this.DefaultHeight = 394;
			this.Show ();
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}

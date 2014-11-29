using System;
using System.Linq;
using System.Runtime.InteropServices; // needed for DLL import
using System.Windows.Forms; // needed for MethodInvoker
using System.Drawing;

using System.ComponentModel;

using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;
using IceFlake.Client.Scripts;

namespace IceFlake.Scripts
{
    #region QKInfoMonitor

    public class QKMenuPluginSample : Script
    {
        public QKMenuPluginSample() 
            : base("_QKMenuPluginSample", "Test", Color.CornflowerBlue.ToArgb()) 
        { }

        private DevExpress.XtraEditors.XtraForm _Form;
        
        private ComponentResourceManager resources = new ComponentResourceManager(typeof(IceForm));

        // OnStart
        ///////////////////////////////////////////////////////////////
        //
        public override void OnStart()
        {
            _Form = new DevExpress.XtraEditors.XtraForm();
            _Form.BackColor = System.Drawing.Color.FromArgb(43, 43, 43);
            //_Form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            _Form.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

            //_Form.LookAndFeel.SkinName = "Visual Studio 2013 Dark";
            _Form.MaximizeBox = false;
            _Form.MinimizeBox = false;
            _Form.Name = "PluginForm";
            _Form.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            _Form.Text = "Plugin";

            _Form.ShowInTaskbar = false;
            _Form.TopMost = true;

            //_Form.

            Manager.InvokeGUIThread(_Form.Show);
        }

        // OnTick
        ///////////////////////////////////////////////////////////////
        //
        public override void OnTick()
        {

        }

        // OnTerminate
        ///////////////////////////////////////////////////////////////
        //
        public override void OnTerminate()
        {
            _Form.Dispose();
        }

        // End
        ///////////////////////////////////////////////////////////////
        //
    }

    #endregion
}
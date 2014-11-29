using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;
using IceFlake.Client.Scripts;
using IceFlake.DirectX;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using DevExpress.XtraEditors;

namespace IceFlake
{
    public partial class IceForm : DevExpress.XtraEditors.XtraForm, ILog
    {
        public IceForm()
        {
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.LookAndFeel.LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
            //this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseWheel);

            InitializeComponent();

            //this.Width = 400;
            //this.Height = 550;

            //this.Width = 480;
            //this.Height = 640;

            //this.Width = this.BackgroundImage.Width;
            //this.Height = this.BackgroundImage.Height;

            //Needed to make the custom shaped Form
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            //Slow version
            //this.Region = BitmapToRegion.getRegion((Bitmap)this.BackgroundImage, Color.FromArgb(0, 255, 0), 100);

            //Fast version
            //this.Region = AllForms.getRegionFast((Bitmap)this.BackgroundImage, Color.FromArgb(0, 255, 0), 100);

        }

        #region ILog Members

        public void WriteLine(LogEntry entry)
        {
            Color logColor;
            switch (entry.Type)
            {
                case LogType.Error:
                    logColor = Color.Red;
                    break;
                case LogType.Warning:
                    logColor = Color.OrangeRed;
                    break;
                case LogType.Critical:
                    logColor = Color.Yellow;
                    break;
                case LogType.Information:
                    //logColor = Color.SteelBlue;
                    logColor = Color.CornflowerBlue;
                    break;
                case LogType.Good:
                    logColor = Color.ForestGreen;
                    break;
                case LogType.Normal:
                default:
                    //logColor = Color.Black;
                    logColor = Color.SteelBlue;
                    break;
            }

            rbLogBox.Invoke((Action) (() =>
            {
                rbLogBox.SelectionColor = logColor;
                rbLogBox.AppendText(entry.FormattedMessage + Environment.NewLine);
                rbLogBox.ScrollToCaret();
            }));
        }

        #endregion

        private void IceForm_Load(object sender, EventArgs e)
        {
            Log.AddReader(this);
            Log.WriteLine(LogType.Good, "CoreForm loaded");
        }

        private void IceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.RemoveReader(this);

            foreach (Script s in Manager.Scripts.Scripts.Where(x => x.IsRunning))
                s.Stop();

            // Let's give us a chance to undo some damage.
            Direct3D.Shutdown();
        }

        private void GUITimer_Tick(object sender, EventArgs e)
        {
            //Log.WriteLine(LogType.Critical, lstScripts.Items.Count.ToString());
            //Log.WriteLine(LogType.Critical, lstScripts.ItemCount.ToString());

            // devexpress checkbox control list uses ".ItemCount" not ".Items.Count"  <- WARNING
            //if (lstScripts.Items.Count == 0)
            if (lstScripts.ItemCount == 0)
                if (Manager.Scripts != null)
                    SetupScripts();

            if (Manager.Scripts.Scripts.Where(s => s.IsRunning).Contains(SelectedScript))
            {
                btnScriptStart.Enabled = false;
                btnScriptStop.Enabled = true;
            }
            else
            {
                btnScriptStart.Enabled = true;
                btnScriptStop.Enabled = false;
            }

            /*/
            if (!Manager.ObjectManager.IsInGame)
                return;

            try
            {
                WoWLocalPlayer lp = Manager.LocalPlayer;
                lblHealth.Text = string.Format("{0}/{1} ({2:0}%)", lp.Health, lp.MaxHealth, lp.HealthPercentage);
                lblPowerText.Text = string.Format("{0}:", lp.PowerType);
                lblPower.Text = string.Format("{0}/{1} ({2:0}%)", lp.Power, lp.MaxPower, lp.PowerPercentage);
                lblLevel.Text = string.Format("{0}", lp.Level);
                lblZone.Text = string.Format("{0} ({1})", WoWWorld.CurrentZone ?? "<unknown>",
                    WoWWorld.CurrentSubZone ?? "<unknown>");
            }
            catch { }
            /*/

            if (!Manager.ObjectManager.IsInGame)
                return;

            try
            {
                //int _maxHp = 100;
                //int _currHp = 20;
                int _maxHp = (int)Manager.LocalPlayer.MaxHealth;
                int _currHp = (int)Manager.LocalPlayer.Health;
                int _percentHp = ((100 * _currHp) / _maxHp);

                if (_percentHp < 100 && _percentHp >= 60)
                    taskbarAssistant.ProgressMode = DevExpress.Utils.Taskbar.Core.TaskbarButtonProgressMode.Normal;

                if (_percentHp < 60 && _percentHp >= 35)
                    taskbarAssistant.ProgressMode = DevExpress.Utils.Taskbar.Core.TaskbarButtonProgressMode.Paused;

                if (_percentHp < 35)
                    taskbarAssistant.ProgressMode = DevExpress.Utils.Taskbar.Core.TaskbarButtonProgressMode.Error;

                if (_currHp < _maxHp)
                {
                    if (infoPlayerHpBar.Properties.Maximum != _maxHp)
                    {
                        infoPlayerHpBar.Properties.Maximum = _maxHp;
                        taskbarAssistant.ProgressMaximumValue = _maxHp;
                    }

                    if (infoPlayerHpBar.Position != _currHp)
                    {
                        infoPlayerHpBar.Position = _currHp;
                        taskbarAssistant.ProgressCurrentValue = _currHp;
                    }
                }
            }
            catch (Exception ex) { Log.WriteLine(LogType.Error, ex.ToString()); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var dbg = new IceDebug();
            //dbg.Show();
        }

        #region Debug tab

        private Location
            _pos1 = default(Location),
            _pos2 = default(Location);

        private void btnExecute_Click(object sender, EventArgs e)
        {
            //string lua2 = tbLUA.Text;
            //string lua = "print(\"\\124TInterface\\\\Minimap\\\\CompassNorthTag:14\\124t " + lua2 + "\")";

            string lua = tbLUA.Text;

            if (string.IsNullOrEmpty(lua))
                return;
            Manager.ExecutionQueue.AddExececution(() =>
            {
                Log.WriteLine(lua);
                List<string> ret = WoWScript.Execute(lua);
                for (int i = 0; i < ret.Count; i++)
                    Log.WriteLine("\t[{0}] = \"{1}\"", i, ret[i]);
            });
        }

        private void btnSpellCast_Click(object sender, EventArgs e)
        {
            try
            {
                if (Manager.LocalPlayer.Class != WoWClass.Shaman)
                    return;
                WoWSpell healingWave = Manager.Spellbook["Healing Wave"];
                if (healingWave == null || !healingWave.IsValid)
                    return;
                Manager.ExecutionQueue.AddExececution(() => { healingWave.Cast(); });
            }
            catch
            {
            }
        }

        private void lblPos1_Click(object sender, EventArgs e)
        {
            if (!Manager.ObjectManager.IsInGame)
                return;
            _pos1 = Manager.LocalPlayer.Location;
            lblPos1.Text = _pos1.ToString();
        }

        private void lblPos2_Click(object sender, EventArgs e)
        {
            if (!Manager.ObjectManager.IsInGame)
                return;
            _pos2 = Manager.LocalPlayer.Location;
            lblPos2.Text = _pos2.ToString();
        }

        private void btnGenPath_Click(object sender, EventArgs e)
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            if (_pos1 == default(Location) || _pos2 == default(Location))
                return;

            try
            {
                //string map = WoWWorld.CurrentMap;
                //Log.WriteLine("Generate path from {0} to {1} in {2}", _pos1, _pos2, map);
                //var mesh = new Pather("Kalimdor");
                //mesh.LoadAppropriateTiles(_pos1.ToVector3(), _pos2.ToVector3());
                //List<Vector3> path = mesh.DetourMesh.FindPath(_pos1.ToFloatArray(), _pos2.ToFloatArray(), false);
                //foreach (Vector3 point in path)
                //    Log.WriteLine("[{0}]", point.ToString());
            }
            catch (Exception ex)
            {
                Log.WriteLine("NavMesh: {0}", ex.Message);
            }
        }

        private void btnLoSTest_Click(object sender, EventArgs e)
        {
            const uint flags = 0x120171;

            WoWLocalPlayer me = Manager.LocalPlayer;
            if (me == null || !me.IsValid)
                return;
            Location start = me.Location;

            WoWObject target = me.Target;
            if (target == null || !target.IsValid)
                return;
            Location end = target.Location;

            start.Z += 1.3f;
            end.Z += 1.3f;

            Manager.ExecutionQueue.AddExececution(() =>
            {
                Location result;
                bool los = (WoWWorld.Traceline(start, end, out result, flags) & 0xFF) == 0;
                Log.WriteLine("LoSTest: {0} -> {1} = {2} @ {3}", me.Location, target.Location, los, result);
            });
        }

        #endregion

        #region Scripts tab

        private Script SelectedScript;

        private void SetupScripts()
        {
            Manager.Scripts.ScriptRegistered += OnScriptRegisteredEvent;

            foreach (Script s in Manager.Scripts.Scripts)
            {
                s.OnStartedEvent += OnScriptStartedEvent;
                s.OnStoppedEvent += OnScriptStoppedEvent;

                // Add Tile Items // for teh luls
                try
                {
                    TileItem item = new TileItem();
                    //item.ItemSize = TileItemSize.Small;
                    item.ItemSize = TileItemSize.Medium;
                    //item.ItemSize = TileItemSize.Wide;

                    //Log.WriteLine("" + s.TileColor);
                    if (s.TileColor != 0)
                    {
                        item.ItemSize = TileItemSize.Wide;
                        item.AppearanceItem.Normal.BackColor = Color.FromArgb(s.TileColor);
                        //item.AppearanceItem.Normal.BackColor2 = Color.FromArgb(s.TileColor);
                        //item.AppearanceItem.Normal.BorderColor = Color.FromArgb(s.TileColor);
                        //item.AppearanceItem.Normal.ForeColor = Color.FromArgb(s.TileColor);
                    }

                    //item.Text = s.Name;
                    item.Text = "<" + s.Category + "> \n" + s.Name;
                    tileGroup1.Items.Add(item);
                }
                catch (Exception ex)
                { Log.WriteLine(LogType.Warning, ex.ToString()); }
            }

            lstScripts.DataSource = Manager.Scripts.Scripts.OrderBy(x => x.Category).ToList();
            Log.WriteLine(LogType.Information, "Loaded {0} scripts.", Manager.Scripts.Scripts.Count);
        }

        private void btnScriptStart_Click(object sender, EventArgs e)
        {
            if (SelectedScript == null)
                return;

            SelectedScript.Start();
        }

        private void btnScriptStop_Click(object sender, EventArgs e)
        {
            if (SelectedScript == null)
                return;

            SelectedScript.Stop();
        }

        private void btnScriptCompile_Click(object sender, EventArgs e)
        {
            Manager.Scripts.CompileAsync();
        }


        private void lstScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var script = lstScripts.SelectedItem as Script;
            if (script == null)
                return;

            SelectedScript = script;
        }


        private void lstScripts_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            var script = lstScripts.GetItem(e.Index) as Script;

            if (e.State == CheckState.Checked)
                script.Start();
            else if (e.State == CheckState.Unchecked)
                script.Stop();
        }

        private void OnScriptRegisteredEvent(object sender, EventArgs e)
        {
            var script = sender as Script;
            script.OnStartedEvent += OnScriptStartedEvent;
            script.OnStoppedEvent += OnScriptStoppedEvent;
            lstScripts.Invoke((Action) (() =>
            {
                lstScripts.DataSource = Manager.Scripts.Scripts.OrderBy(x => x.Category).ToList();
                lstScripts.Invalidate();
            }));
        }

        private void OnScriptStartedEvent(object sender, EventArgs e)
        {
            //int idx = lstScripts.Items.IndexOf(sender);
            //lstScripts.Invoke((Action) (() => lstScripts.SetItemCheckState(idx, CheckState.Checked)));
        }

        private void OnScriptStoppedEvent(object sender, EventArgs e)
        {
            //int idx = lstScripts.Items.IndexOf(sender);
            //lstScripts.Invoke((Action) (() => lstScripts.SetItemCheckState(idx, CheckState.Unchecked)));
        }

        #endregion

        private void buttonGwen_Click(object sender, EventArgs e)
        {
            Gwen.Sample.OpenTK.SimpleWindow.Main();
        }

        private bool _ShowSettings = false;
        private void simpleButtonSettings_Click(object sender, EventArgs e)
        {
            switch (_ShowSettings)
            {
                case false:
                    xtraTabControlMain.Show();
                    _ShowSettings = true;
                    break;
                case true:
                    xtraTabControlMain.Hide();
                    _ShowSettings = false;
                    break;
            }
        }




















    }
}
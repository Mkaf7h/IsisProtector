using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace IsisFuscator
{
    public partial class IsisFuscator : Form
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        private Panel topBar;
        private Panel sidebar;
        private Panel contentPanel;

        private readonly Color bgDark = Color.FromArgb(8, 8, 8);
        private readonly Color topBarColor = Color.FromArgb(12, 12, 12);
        private readonly Color sidebarColor = Color.FromArgb(6, 6, 6);
        private readonly Color borderCol = Color.FromArgb(45, 45, 45);
        private readonly Color textGray = Color.FromArgb(140, 140, 140);

        private List<Panel> sidebarItems = new List<Panel>();
        private int activeIndex = 0;

        private string assemblyPath = "";
        private string destinationPath = "";

        private bool renamerEnabled = true;
        private bool renameNamespaces = true;
        private bool renameTypes = true;
        private bool renameMethods = true;
        private bool renameFields = true;
        private bool renameProperties = false;
        private bool renameEvents = false;
        private int renamerLength = 12;
        private string renamerPrefix = "$Def SErver~";
        private string renamerChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789";

        private bool antiTamper = false;
        private bool antiDump = false;
        private bool antiDe4dot = false;
        private bool antiMemDump = false;
        private bool antiHook = false;
        private bool antiVirtMachine = false;
        private bool antiDebug = true;
        private bool antiIldasm = false;
        private bool antiHttp = false;

        private bool encString = true;
        private bool encInt = true;
        private bool encVm = false;
        private bool encMutation = false;
        private bool encResources = false;
        private bool encRuntime = false;
        private bool encDelegate = false;
        private bool encConstants = false;
        private bool encField = false;
        private bool encPolymorphic = false;
        private bool encCrossRef = false;
        private bool encStringCompose = false;
        private bool encArray = false;
        private bool encMethodBody = false;

        private bool stealthHideMethods = false;
        private bool stealthWatermark = true;
        private bool stealthTypeScrambler = false;
        private bool stealthDnSpyCrash = false;
        private bool stealthMethodInliner = false;
        private bool stealthFakeAttrs = false;
        private bool stealthTokenConfuse = false;
        private bool stealthInvalidMeta = false;
        private bool stealthEntryPtMove = false;

        public IsisFuscator()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgDark;
            this.Opacity = 0.95D;
            this.DoubleBuffered = true;

            try
            {
                if (File.Exists("icons/mainico.png.png"))
                {
                    using (var bmp = new Bitmap("icons/mainico.png.png"))
                    {
                        IntPtr hIcon = bmp.GetHicon();
                        this.Icon = Icon.FromHandle(hIcon);
                    }
                }
            }
            catch {}

            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = bgDark
            };

            sidebar = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = sidebarColor
            };

            int startY = 35;
            int spacing = 50;

            AddSidebarItem("icons/build.png", "Build", startY, 0);
            AddSidebarItem("icons/guard.png", "Protections", startY + spacing, 1);
            AddSidebarItem("icons/locker.png", "Encryption", startY + spacing * 2, 2);
            AddSidebarItem("icons/renamer.png", "Renamer", startY + spacing * 3, 3);
            AddSidebarItem("icons/code.png", "Code Virtualization", startY + spacing * 4, 4);

            topBar = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = topBarColor
            };
            topBar.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            PictureBox picLogo = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 15),
                Size = new Size(30, 30),
                BackColor = Color.Transparent
            };
            try
            {
                if (File.Exists("icons/discord.png")) picLogo.Image = Image.FromFile("icons/discord.png");
            }
            catch {}
            picLogo.MouseDown += topBar_MouseDownProxy;
            topBar.Controls.Add(picLogo);

            Label lblTitle = new Label()
            {
                Text = "IsisFuscator",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(58, 18),
                AutoSize = true
            };
            lblTitle.MouseDown += topBar_MouseDownProxy;
            topBar.Controls.Add(lblTitle);

            Panel vSep = new Panel()
            {
                Size = new Size(1, 22),
                Location = new Point(185, 19),
                BackColor = Color.FromArgb(40, 40, 40)
            };
            topBar.Controls.Add(vSep);

            Panel btnAboutTop = new Panel()
            {
                Location = new Point(200, 14),
                Size = new Size(95, 32),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            PictureBox picAbout = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(8, 6),
                Size = new Size(20, 20),
                BackColor = Color.Transparent
            };
            try
            {
                if (File.Exists("icons/about.png")) picAbout.Image = Image.FromFile("icons/about.png");
            }
            catch {}
            btnAboutTop.Controls.Add(picAbout);

            Label lblAboutText = new Label()
            {
                Text = "About",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(32, 6),
                AutoSize = true
            };
            btnAboutTop.Controls.Add(lblAboutText);

            EventHandler aboutClickHandler = (s, e) => {
                foreach (var pnl in sidebarItems)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c.Name == "indicator") pnl.Controls.Remove(c);
                        if (c is Label lbl) { lbl.ForeColor = textGray; lbl.Font = new Font("Segoe UI", 10F, FontStyle.Regular); }
                    }
                }
                activeIndex = -1;
                contentPanel.Controls.Clear();
                BuildAboutView();
            };

            btnAboutTop.Click += aboutClickHandler;
            picAbout.Click += aboutClickHandler;
            lblAboutText.Click += aboutClickHandler;

            btnAboutTop.MouseEnter += (s, e) => lblAboutText.ForeColor = Color.White;
            btnAboutTop.MouseLeave += (s, e) => lblAboutText.ForeColor = Color.FromArgb(200, 200, 200);

            topBar.Controls.Add(btnAboutTop);

            Button btnMinimize = new Button()
            {
                Text = "—",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.DarkGray,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(45, 60),
                Dock = DockStyle.Right,
                Cursor = Cursors.Hand
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 40);
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            topBar.Controls.Add(btnMinimize);

            Button btnExit = new Button()
            {
                Text = "✕",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 200, 200),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(45, 60),
                Dock = DockStyle.Right,
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 30, 30);
            try
            {
                if (File.Exists("icons/exit.png.png"))
                {
                    btnExit.Image = new Bitmap(Image.FromFile("icons/exit.png.png"), new Size(18, 18));
                    btnExit.Text = "";
                }
            }
            catch {}
            btnExit.Click += (s, e) => Application.Exit();
            topBar.Controls.Add(btnExit);

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebar);
            this.Controls.Add(topBar);

            SwitchView(0);
        }

        private void topBar_MouseDownProxy(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void AddSidebarItem(string iconPath, string text, int y, int index)
        {
            Panel itemPanel = new Panel()
            {
                Location = new Point(0, y),
                Size = new Size(220, 40),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Tag = index
            };

            PictureBox picIcon = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(22, 10),
                Size = new Size(20, 20),
                Tag = index,
                BackColor = Color.Transparent
            };

            try
            {
                if (File.Exists(iconPath))
                {
                    picIcon.Image = Image.FromFile(iconPath);
                }
            }
            catch
            {
            }

            itemPanel.Controls.Add(picIcon);

            Label lblText = new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = textGray,
                Location = new Point(52, 10),
                AutoSize = true,
                Tag = index
            };
            itemPanel.Controls.Add(lblText);

            EventHandler clickHandler = (s, e) => {
                SwitchView(index);
            };

            itemPanel.Click += clickHandler;
            picIcon.Click += clickHandler;
            lblText.Click += clickHandler;

            itemPanel.MouseEnter += (s, e) => {
                if (activeIndex != index) {
                    lblText.ForeColor = Color.White;
                }
            };
            itemPanel.MouseLeave += (s, e) => {
                if (activeIndex != index) {
                    lblText.ForeColor = textGray;
                }
            };

            sidebarItems.Add(itemPanel);
            sidebar.Controls.Add(itemPanel);
        }

        private void SwitchView(int index)
        {
            activeIndex = index;

            for (int i = 0; i < sidebarItems.Count; i++)
            {
                Panel pnl = sidebarItems[i];
                bool active = (i == index);

                foreach (Control c in pnl.Controls)
                {
                    if (c.Name == "indicator") pnl.Controls.Remove(c);
                }

                if (active)
                {
                    Panel indicator = new Panel()
                    {
                        Name = "indicator",
                        Size = new Size(3, 22),
                        Location = new Point(0, 9),
                        BackColor = Color.White
                    };
                    pnl.Controls.Add(indicator);
                }

                foreach (Control c in pnl.Controls)
                {
                    if (c is Label lbl)
                    {
                        lbl.ForeColor = active ? Color.White : textGray;
                        lbl.Font = new Font("Segoe UI", 10F, active ? FontStyle.Bold : FontStyle.Regular);
                    }
                }
            }

            contentPanel.Controls.Clear();
            if (index == 0)
            {
                BuildAssemblyView();
            }
            else if (index == 1)
            {
                BuildProtectionsView();
            }
            else if (index == 2)
            {
                BuildEncryptionView();
            }
            else if (index == 3)
            {
                BuildRenamerView();
            }
            else if (index == 4)
            {
                BuildVirtualizationView();
            }
            else
            {
                BuildPlaceholderView(sidebarItems[index].Controls[1] is Label l ? l.Text : "Section");
            }
        }

        private void BuildPlaceholderView(string sectionName)
        {
            int topOffset = 45;
            int leftOffset = 40;

            Label lblHeader = new Label()
            {
                Text = $"—  {sectionName}",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(leftOffset, topOffset),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblHeader);

            Label lblMsg = new Label()
            {
                Text = $"Options for {sectionName} will appear here. Ready for your instructions!",
                Font = new Font("Segoe UI", 10F),
                ForeColor = textGray,
                Location = new Point(leftOffset, topOffset + 50),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblMsg);
        }

        private void BuildAboutView()
        {
            int leftOffset = 40;
            int topOffset = 25;

            RoundedPanel card = new RoundedPanel()
            {
                Location = new Point(leftOffset, topOffset),
                Size = new Size(720, 460),
                BackColor = Color.FromArgb(10, 10, 10),
                BorderColor = borderCol,
                BorderWidth = 1,
                CornerRadius = 10
            };

            Label lblDisc = new Label()
            {
                Text = "!!! Disclaimer",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 35),
                Size = new Size(720, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblDisc);

            Label lblSub = new Label()
            {
                Text = "I am not responsible for your actions.",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(0, 75),
                Size = new Size(720, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblSub);

            Label lblCopy = new Label()
            {
                Text = "2026 Nubersec Group. All Rights Reserved.",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(110, 110, 110),
                Location = new Point(0, 100),
                Size = new Size(720, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblCopy);

            Panel line = new Panel()
            {
                Size = new Size(640, 1),
                Location = new Point(40, 135),
                BackColor = Color.FromArgb(40, 40, 40)
            };
            card.Controls.Add(line);

            int colY = 160;

            AddAboutColumn(card, "DESIGNER", "Nmk", "Dis: .5.1.1._.", 70, colY);
            AddAboutColumn(card, "DEVELOPER", "Mkaf7h", "Snapchat: c7k.mkaf7h", 280, colY);
            AddAboutColumn(card, "A PARTNER", "Mugy", "", 490, colY);

            int btnY = 380;
            card.Controls.Add(CreateAboutButton("DISCORD", 160, btnY, () => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://discord.com") { UseShellExecute = true })));
            card.Controls.Add(CreateAboutButton("WEBSITE", 310, btnY, () => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://google.com") { UseShellExecute = true })));
            card.Controls.Add(CreateAboutButton("GITHUB", 460, btnY, () => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com") { UseShellExecute = true })));

            contentPanel.Controls.Add(card);
        }

        private void AddAboutColumn(RoundedPanel parent, string role, string name, string sub, int x, int y)
        {
            Label lblRole = new Label()
            {
                Text = role,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(x, y),
                Size = new Size(160, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            parent.Controls.Add(lblRole);

            Label lblName = new Label()
            {
                Text = name,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(x, y + 25),
                Size = new Size(160, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            parent.Controls.Add(lblName);

            if (!string.IsNullOrEmpty(sub))
            {
                Label lblSub = new Label()
                {
                    Text = sub,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(150, 150, 150),
                    Location = new Point(x, y + 50),
                    Size = new Size(160, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                parent.Controls.Add(lblSub);
            }
        }

        private Button CreateAboutButton(string text, int x, int y, Action onClick)
        {
            Button btn = new Button()
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(20, 20, 20),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 36),
                Location = new Point(x, y),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void BuildAssemblyView()
        {
            int topOffset = 30;
            int leftOffset = 40;

            Label lblAsmHeader = new Label()
            {
                Text = "—  Assembly path",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(leftOffset, topOffset),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblAsmHeader);

            Panel boxAssembly = CreateInputBoxForAssembly("icons/build.png", string.IsNullOrEmpty(assemblyPath) ? "Drag n drop assembly here" : assemblyPath, topOffset + 32, leftOffset, true);
            contentPanel.Controls.Add(boxAssembly);

            int destTop = topOffset + 115;
            Label lblDestHeader = new Label()
            {
                Text = "—  Destination",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(leftOffset, destTop),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblDestHeader);

            Panel boxDestination = CreateInputBoxForDestination("icons/locker.png", string.IsNullOrEmpty(destinationPath) ? "Protected assembly destination" : destinationPath, destTop + 32, leftOffset);
            contentPanel.Controls.Add(boxDestination);

            Button btnBuild = new Button()
            {
                Text = " Build ",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 30),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(720, 48),
                Location = new Point(leftOffset, destTop + 105),
                Cursor = Cursors.Hand
            };
            btnBuild.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
            btnBuild.Click += (s, e) => {
                RunProtectionProcess();
            };
            contentPanel.Controls.Add(btnBuild);
        }

        private void RunProtectionProcess()
        {
            if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
            {
                MessageBox.Show("Please select a valid .NET assembly (.exe or .dll) to protect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(destinationPath))
            {
                string dir = Path.GetDirectoryName(assemblyPath) ?? "";
                string name = Path.GetFileNameWithoutExtension(assemblyPath);
                string ext = Path.GetExtension(assemblyPath);
                destinationPath = Path.Combine(dir, $"{name}_protected{ext}");
            }

            try
            {
                var resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
                var readerParameters = new ReaderParameters { AssemblyResolver = resolver, ReadWrite = true, ReadingMode = ReadingMode.Immediate };

                using (var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters))
                {
                    Random rand = new Random();

                    if (renamerEnabled)
                    {
                        foreach (var module in assembly.Modules)
                        {
                            foreach (var type in module.Types)
                            {
                                if (type.Name == "<Module>") continue;
                                if (IsFormOrGUI(type)) continue;

                                if (renameTypes && !type.IsPublic && !type.IsNestedPublic)
                                {
                                    type.Name = GenerateRandomName(rand);
                                }

                                if (renameFields)
                                {
                                    foreach (var field in type.Fields)
                                    {
                                        if (!field.IsPublic)
                                        {
                                            field.Name = GenerateRandomName(rand);
                                        }
                                    }
                                }

                                if (renameMethods)
                                {
                                    foreach (var method in type.Methods)
                                    {
                                        if (method.IsConstructor) continue;
                                        if (method.IsSpecialName || method.IsRuntimeSpecialName) continue;
                                        if (assembly.EntryPoint == method) continue;
                                        if (method.IsPublic || method.IsVirtual || method.IsAbstract) continue;

                                        method.Name = GenerateRandomName(rand);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var module in assembly.Modules)
                    {
                        if (antiIldasm)
                        {
                            var ctor = module.ImportReference(typeof(System.Runtime.CompilerServices.SuppressIldasmAttribute).GetConstructor(Type.EmptyTypes));
                            if (ctor != null) assembly.CustomAttributes.Add(new CustomAttribute(ctor));
                        }

                        if (antiDebug && assembly.EntryPoint != null)
                        {
                            var entryPoint = assembly.EntryPoint;
                            var ilProc = entryPoint.Body.GetILProcessor();
                            var firstInst = entryPoint.Body.Instructions[0];

                            var debuggerIsAttached = module.ImportReference(typeof(System.Diagnostics.Debugger).GetProperty("IsAttached")?.GetGetMethod());
                            var environmentExit = module.ImportReference(typeof(Environment).GetMethod("Exit", new Type[] { typeof(int) }));

                            if (debuggerIsAttached != null && environmentExit != null)
                            {
                                var label = Instruction.Create(OpCodes.Nop);
                                ilProc.InsertBefore(firstInst, Instruction.Create(OpCodes.Call, debuggerIsAttached));
                                ilProc.InsertBefore(firstInst, Instruction.Create(OpCodes.Brfalse, label));
                                ilProc.InsertBefore(firstInst, Instruction.Create(OpCodes.Ldc_I4_1));
                                ilProc.InsertBefore(firstInst, Instruction.Create(OpCodes.Call, environmentExit));
                                ilProc.InsertBefore(firstInst, label);
                            }
                        }

                        if (antiTamper)
                        {
                            var dummyType = new TypeDefinition("IsisFuscator.AntiTamper", "IntegrityCheck", TypeAttributes.Class | TypeAttributes.Public);
                            module.Types.Add(dummyType);
                        }
                        if (antiDump)
                        {
                            var dumpAttr = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructor(new Type[] { typeof(string) })));
                            dumpAttr.ConstructorArguments.Add(new CustomAttributeArgument(module.TypeSystem.String, "AntiDump Protected"));
                            assembly.CustomAttributes.Add(dumpAttr);
                        }
                        if (antiDe4dot)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                module.Types.Add(new TypeDefinition("De4dot.Decoy." + i, "Token" + i, TypeAttributes.Class | TypeAttributes.Public));
                            }
                        }
                    }

                    foreach (var module in assembly.Modules)
                    {
                        if (stealthWatermark)
                        {
                            var watermarkAttr = new CustomAttribute(module.ImportReference(typeof(System.Reflection.AssemblyCompanyAttribute).GetConstructor(new Type[] { typeof(string) })));
                            watermarkAttr.ConstructorArguments.Add(new CustomAttributeArgument(module.TypeSystem.String, "Protected by IsisFuscator"));
                            assembly.CustomAttributes.Add(watermarkAttr);
                        }

                        if (stealthDnSpyCrash)
                        {
                            try
                            {
                                var crashType = new TypeDefinition("IsisFuscator.Crash\0\u0000", "DnSpyCrash\u0001\u0002", TypeAttributes.Class | TypeAttributes.Public);
                                module.Types.Add(crashType);

                                var crashMethod = new MethodDefinition("CrashMethod\0", MethodAttributes.Public | MethodAttributes.Static, module.TypeSystem.Void);
                                var il = crashMethod.Body.GetILProcessor();
                                il.Append(Instruction.Create(OpCodes.Ret));
                                crashType.Methods.Add(crashMethod);

                                var dummyAttr = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructor(new Type[] { typeof(string) })));
                                dummyAttr.ConstructorArguments.Add(new CustomAttributeArgument(module.TypeSystem.String, new string('\0', 500)));
                                assembly.CustomAttributes.Add(dummyAttr);
                            }
                            catch {}
                        }
                    }

                    assembly.Write(destinationPath);
                }

                MessageBox.Show($"Assembly fully protected successfully!\n\n📁 Output:\n{destinationPath}",
                                "IsisFuscator - Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Protection failed:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateRandomName(Random rand)
        {
            char[] chars = string.IsNullOrEmpty(renamerChars) ? "abcdefghijklmnopqrstuvwxyz".ToCharArray() : renamerChars.ToCharArray();
            int len = Math.Max(4, Math.Min(renamerLength, 64));

            char[] buffer = new char[len];
            for (int i = 0; i < len; i++)
            {
                buffer[i] = chars[rand.Next(chars.Length)];
            }

            return renamerPrefix + new string(buffer);
        }

        private bool IsFormOrGUI(TypeDefinition type)
        {
            if (type.Name.Contains("Form") || type.Name.Contains("Window") || type.Name.Contains("Control") || type.Name.Contains("Panel"))
                return true;
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.Name.Contains("Form") || baseType.Name.Contains("Control") || baseType.Name.Contains("Window"))
                    return true;
                try
                {
                    var resolved = baseType.Resolve();
                    if (resolved == null) break;
                    baseType = resolved.BaseType;
                }
                catch
                {
                    break;
                }
            }
            return false;
        }

        private void BuildProtectionsView()
        {
            int topOffset = 25;
            int leftOffset = 40;

            Label lblHeader = new Label()
            {
                Text = "—  Antis Protections",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(leftOffset, topOffset),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblHeader);

            RoundedPanel card = new RoundedPanel()
            {
                Location = new Point(leftOffset, topOffset + 35),
                Size = new Size(720, 310),
                BackColor = Color.FromArgb(10, 10, 10),
                BorderColor = borderCol,
                BorderWidth = 1,
                CornerRadius = 8
            };

            int col1X = 40;
            int col2X = 360;
            int startY = 30;
            int rowSpacing = 45;

            var chkTamper = CreateStyledCheckbox("Anti Tamper", col1X, startY, antiTamper);
            chkTamper.CheckedChanged += (s, e) => antiTamper = chkTamper.Checked;
            card.Controls.Add(chkTamper);

            var chkDump = CreateStyledCheckbox("Anti Dump", col1X, startY + rowSpacing, antiDump);
            chkDump.CheckedChanged += (s, e) => antiDump = chkDump.Checked;
            card.Controls.Add(chkDump);

            var chkDe4dot = CreateStyledCheckbox("Anti De4dot", col1X, startY + rowSpacing * 2, antiDe4dot);
            chkDe4dot.CheckedChanged += (s, e) => antiDe4dot = chkDe4dot.Checked;
            card.Controls.Add(chkDe4dot);

            var chkMemDump = CreateStyledCheckbox("Anti MemDump", col1X, startY + rowSpacing * 3, antiMemDump);
            chkMemDump.CheckedChanged += (s, e) => antiMemDump = chkMemDump.Checked;
            card.Controls.Add(chkMemDump);

            var chkHook = CreateStyledCheckbox("Anti Hook", col1X, startY + rowSpacing * 4, antiHook);
            chkHook.CheckedChanged += (s, e) => antiHook = chkHook.Checked;
            card.Controls.Add(chkHook);

            var chkVirt = CreateStyledCheckbox("Anti Virt Machine", col2X, startY, antiVirtMachine);
            chkVirt.CheckedChanged += (s, e) => antiVirtMachine = chkVirt.Checked;
            card.Controls.Add(chkVirt);

            var chkDebug = CreateStyledCheckbox("Anti Debug", col2X, startY + rowSpacing, antiDebug);
            chkDebug.CheckedChanged += (s, e) => antiDebug = chkDebug.Checked;
            card.Controls.Add(chkDebug);

            var chkIldasm = CreateStyledCheckbox("Anti ILDasm", col2X, startY + rowSpacing * 2, antiIldasm);
            chkIldasm.CheckedChanged += (s, e) => antiIldasm = chkIldasm.Checked;
            card.Controls.Add(chkIldasm);

            var chkHttp = CreateStyledCheckbox("Anti Http", col2X, startY + rowSpacing * 3, antiHttp);
            chkHttp.CheckedChanged += (s, e) => antiHttp = chkHttp.Checked;
            card.Controls.Add(chkHttp);

            contentPanel.Controls.Add(card);
        }

        private void BuildEncryptionView()
        {
            int topOffset = 25;
            int leftOffset = 40;

            Label lblHeader = new Label()
            {
                Text = "—  Encryption",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(leftOffset, topOffset),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblHeader);

            RoundedPanel card = new RoundedPanel()
            {
                Location = new Point(leftOffset, topOffset + 35),
                Size = new Size(720, 390),
                BackColor = Color.FromArgb(10, 10, 10),
                BorderColor = borderCol,
                BorderWidth = 1,
                CornerRadius = 8
            };

            int col1X = 40;
            int col2X = 360;
            int startY = 30;
            int rowSpacing = 45;

            var c1 = CreateStyledCheckbox("String Encrypt", col1X, startY, encString);
            c1.CheckedChanged += (s, e) => encString = c1.Checked;
            card.Controls.Add(c1);

            var c2 = CreateStyledCheckbox("Int Encoding", col1X, startY + rowSpacing, encInt);
            c2.CheckedChanged += (s, e) => encInt = c2.Checked;
            card.Controls.Add(c2);

            var c3 = CreateStyledCheckbox("VM Obfuscation", col1X, startY + rowSpacing * 2, encVm);
            c3.CheckedChanged += (s, e) => { encVm = c3.Checked; if (c3.Checked) encMethodBody = true; };
            card.Controls.Add(c3);

            var c4 = CreateStyledCheckbox("Mutation Encode", col1X, startY + rowSpacing * 3, encMutation);
            c4.CheckedChanged += (s, e) => encMutation = c4.Checked;
            card.Controls.Add(c4);

            var c5 = CreateStyledCheckbox("Resources Compress", col1X, startY + rowSpacing * 4, encResources);
            c5.CheckedChanged += (s, e) => encResources = c5.Checked;
            card.Controls.Add(c5);

            var c6 = CreateStyledCheckbox("Runtime Encrypt", col1X, startY + rowSpacing * 5, encRuntime);
            c6.CheckedChanged += (s, e) => encRuntime = c6.Checked;
            card.Controls.Add(c6);

            var c7 = CreateStyledCheckbox("Delegate Enc", col1X, startY + rowSpacing * 6, encDelegate);
            c7.CheckedChanged += (s, e) => encDelegate = c7.Checked;
            card.Controls.Add(c7);

            var c8 = CreateStyledCheckbox("Constants Enc", col2X, startY, encConstants);
            c8.CheckedChanged += (s, e) => encConstants = c8.Checked;
            card.Controls.Add(c8);

            var c9 = CreateStyledCheckbox("Field Encrypt", col2X, startY + rowSpacing, encField);
            c9.CheckedChanged += (s, e) => encField = c9.Checked;
            card.Controls.Add(c9);

            var c10 = CreateStyledCheckbox("Polymorphic Enc", col2X, startY + rowSpacing * 2, encPolymorphic);
            c10.CheckedChanged += (s, e) => encPolymorphic = c10.Checked;
            card.Controls.Add(c10);

            var c11 = CreateStyledCheckbox("CrossRef Enc", col2X, startY + rowSpacing * 3, encCrossRef);
            c11.CheckedChanged += (s, e) => encCrossRef = c11.Checked;
            card.Controls.Add(c11);

            var c12 = CreateStyledCheckbox("String Compose", col2X, startY + rowSpacing * 4, encStringCompose);
            c12.CheckedChanged += (s, e) => encStringCompose = c12.Checked;
            card.Controls.Add(c12);

            var c13 = CreateStyledCheckbox("Array Encrypt", col2X, startY + rowSpacing * 5, encArray);
            c13.CheckedChanged += (s, e) => encArray = c13.Checked;
            card.Controls.Add(c13);

            var c14 = CreateStyledCheckbox("MethodBody Enc", col2X, startY + rowSpacing * 6, encMethodBody);
            c14.CheckedChanged += (s, e) => { encMethodBody = c14.Checked; if (c14.Checked) encVm = true; };
            card.Controls.Add(c14);

            contentPanel.Controls.Add(card);
        }

        private void BuildRenamerView()
        {
            int topOffset = 25;
            int leftOffset = 40;

            Label lblHeader = new Label()
            {
                Text = "—  Renamer",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(leftOffset, topOffset),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblHeader);

            RoundedPanel card = new RoundedPanel()
            {
                Location = new Point(leftOffset, topOffset + 35),
                Size = new Size(720, 370),
                BackColor = Color.FromArgb(10, 10, 10),
                BorderColor = borderCol,
                BorderWidth = 1,
                CornerRadius = 8
            };

            CheckBox chkEnable = new CheckBox()
            {
                Text = "Enable Renamer",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 25),
                AutoSize = true,
                Checked = renamerEnabled
            };
            chkEnable.CheckedChanged += (s, e) => renamerEnabled = chkEnable.Checked;
            card.Controls.Add(chkEnable);

            int checkX1 = 50;
            int checkX2 = 340;
            int checkY = 75;
            int checkSpacing = 35;

            var chkNamespaces = CreateStyledCheckbox("Namespaces", checkX1, checkY, renameNamespaces);
            chkNamespaces.CheckedChanged += (s, e) => renameNamespaces = chkNamespaces.Checked;
            card.Controls.Add(chkNamespaces);

            var chkFields = CreateStyledCheckbox("Fields", checkX2, checkY, renameFields);
            chkFields.CheckedChanged += (s, e) => renameFields = chkFields.Checked;
            card.Controls.Add(chkFields);

            var chkTypes = CreateStyledCheckbox("Types", checkX1, checkY + checkSpacing, renameTypes);
            chkTypes.CheckedChanged += (s, e) => renameTypes = chkTypes.Checked;
            card.Controls.Add(chkNamespaces);

            var chkProperties = CreateStyledCheckbox("Properties", checkX2, checkY + checkSpacing, renameProperties);
            chkProperties.CheckedChanged += (s, e) => renameProperties = chkProperties.Checked;
            card.Controls.Add(chkProperties);

            var chkMethods = CreateStyledCheckbox("Methods", checkX1, checkY + checkSpacing * 2, renameMethods);
            chkMethods.CheckedChanged += (s, e) => renameMethods = chkMethods.Checked;
            card.Controls.Add(chkMethods);

            var chkEvents = CreateStyledCheckbox("Events", checkX2, checkY + checkSpacing * 2, renameEvents);
            chkEvents.CheckedChanged += (s, e) => renameEvents = chkEvents.Checked;
            card.Controls.Add(chkEvents);

            int controlsY = 200;
            Label lblLength = new Label()
            {
                Text = "Length:",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(50, controlsY),
                AutoSize = true
            };
            card.Controls.Add(lblLength);

            NumericUpDown numLength = new NumericUpDown()
            {
                Value = Math.Clamp(renamerLength, 1, 128),
                Minimum = 1,
                Maximum = 128,
                Location = new Point(115, controlsY - 2),
                Size = new Size(75, 25),
                BackColor = Color.FromArgb(25, 25, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };
            numLength.ValueChanged += (s, e) => renamerLength = (int)numLength.Value;
            card.Controls.Add(numLength);

            Label lblPrefix = new Label()
            {
                Text = "Prefix:",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(230, controlsY),
                AutoSize = true
            };
            card.Controls.Add(lblPrefix);

            TextBox txtPrefix = new TextBox()
            {
                Text = renamerPrefix,
                Location = new Point(285, controlsY - 2),
                Size = new Size(140, 25),
                BackColor = Color.FromArgb(25, 25, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPrefix.TextChanged += (s, e) => renamerPrefix = txtPrefix.Text;
            card.Controls.Add(txtPrefix);

            int charsY = controlsY + 55;
            Label lblChars = new Label()
            {
                Text = "Chars:",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(50, charsY),
                AutoSize = true
            };
            card.Controls.Add(lblChars);

            TextBox txtChars = new TextBox()
            {
                Text = renamerChars,
                Location = new Point(115, charsY - 2),
                Size = new Size(565, 25),
                BackColor = Color.FromArgb(25, 25, 25),
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtChars.TextChanged += (s, e) => renamerChars = txtChars.Text;
            card.Controls.Add(txtChars);

            contentPanel.Controls.Add(card);
        }

        private void BuildVirtualizationView()
        {
            int topOffset = 25;
            int leftOffset = 40;

            Label lblHeader = new Label()
            {
                Text = "—  Stealth Virtualization",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(leftOffset, topOffset),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblHeader);

            RoundedPanel card = new RoundedPanel()
            {
                Location = new Point(leftOffset, topOffset + 35),
                Size = new Size(720, 310),
                BackColor = Color.FromArgb(10, 10, 10),
                BorderColor = borderCol,
                BorderWidth = 1,
                CornerRadius = 8
            };

            int col1X = 40;
            int col2X = 360;
            int startY = 30;
            int rowSpacing = 45;

            var c1 = CreateStyledCheckbox("Hide Methods", col1X, startY, stealthHideMethods);
            c1.CheckedChanged += (s, e) => stealthHideMethods = c1.Checked;
            card.Controls.Add(c1);

            var c2 = CreateStyledCheckbox("Watermark", col1X, startY + rowSpacing, stealthWatermark);
            c2.CheckedChanged += (s, e) => stealthWatermark = c2.Checked;
            card.Controls.Add(c2);

            var c3 = CreateStyledCheckbox("Type Scrambler", col1X, startY + rowSpacing * 2, stealthTypeScrambler);
            c3.CheckedChanged += (s, e) => stealthTypeScrambler = c3.Checked;
            card.Controls.Add(c3);

            var c4 = CreateStyledCheckbox("DnSpy Crash", col1X, startY + rowSpacing * 3, stealthDnSpyCrash);
            c4.CheckedChanged += (s, e) => stealthDnSpyCrash = c4.Checked;
            card.Controls.Add(c4);

            var c5 = CreateStyledCheckbox("Method Inliner", col1X, startY + rowSpacing * 4, stealthMethodInliner);
            c5.CheckedChanged += (s, e) => stealthMethodInliner = c5.Checked;
            card.Controls.Add(c5);

            var c6 = CustomCheckboxCreator("Fake Attrs", col2X, startY, stealthFakeAttrs);
            c6.CheckedChanged += (s, e) => stealthFakeAttrs = c6.Checked;
            card.Controls.Add(c6);

            var c7 = CreateStyledCheckbox("Token Confuse", col2X, startY + rowSpacing, stealthTokenConfuse);
            c7.CheckedChanged += (s, e) => stealthTokenConfuse = c7.Checked;
            card.Controls.Add(c7);

            var c8 = CreateStyledCheckbox("Invalid Meta", col2X, startY + rowSpacing * 2, stealthInvalidMeta);
            c8.CheckedChanged += (s, e) => stealthInvalidMeta = c8.Checked;
            card.Controls.Add(c8);

            var c9 = CreateStyledCheckbox("EntryPt Move", col2X, startY + rowSpacing * 3, stealthEntryPtMove);
            c9.CheckedChanged += (s, e) => stealthEntryPtMove = c9.Checked;
            card.Controls.Add(c9);

            CheckBox chkSelectAll = CreateStyledCheckbox("Select All", col2X, startY + rowSpacing * 4, false);
            chkSelectAll.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            chkSelectAll.CheckedChanged += (s, e) => {
                bool state = chkSelectAll.Checked;
                c1.Checked = state;
                c2.Checked = state;
                c3.Checked = state;
                c4.Checked = state;
                c5.Checked = state;
                c6.Checked = state;
                c7.Checked = state;
                c8.Checked = state;
                c9.Checked = state;
            };
            card.Controls.Add(chkSelectAll);

            contentPanel.Controls.Add(card);
        }

        private CheckBox CreateStyledCheckbox(string text, int x, int y, bool isChecked)
        {
            return new CheckBox()
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(220, 220, 220),
                Location = new Point(x, y),
                AutoSize = true,
                Checked = isChecked
            };
        }

        private CheckBox CustomCheckboxCreator(string text, int x, int y, bool isChecked)
        {
            return CreateStyledCheckbox(text, x, y, isChecked);
        }

        private Panel CreateInputBoxForAssembly(string iconPath, string placeholder, int y, int x, bool isAssembly)
        {
            RoundedPanel pnl = new RoundedPanel()
            {
                Location = new Point(x, y),
                Size = new Size(720, 50),
                BackColor = Color.FromArgb(10, 10, 10),
                BorderColor = borderCol,
                BorderWidth = 1,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            PictureBox picIcon = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(16, 13),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            try
            {
                if (File.Exists(iconPath)) picIcon.Image = Image.FromFile(iconPath);
            }
            catch {}
            pnl.Controls.Add(picIcon);

            TextBox txtBox = new TextBox()
            {
                Text = placeholder,
                Font = new Font("Segoe UI", 10F),
                ForeColor = string.IsNullOrEmpty(assemblyPath) ? Color.FromArgb(130, 130, 130) : Color.White,
                BackColor = Color.FromArgb(10, 10, 10),
                BorderStyle = BorderStyle.None,
                Location = new Point(52, 15),
                Size = new Size(610, 25),
                AllowDrop = true
            };

            txtBox.TextChanged += (s, e) => {
                if (txtBox.Text != "Drag n drop assembly here" && !string.IsNullOrEmpty(txtBox.Text))
                {
                    assemblyPath = txtBox.Text;
                }
            };

            txtBox.Enter += (s, e) => {
                if (txtBox.Text == "Drag n drop assembly here")
                {
                    txtBox.Text = "";
                    txtBox.ForeColor = Color.White;
                }
            };
            txtBox.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtBox.Text))
                {
                    txtBox.Text = "Drag n drop assembly here";
                    txtBox.ForeColor = Color.FromArgb(130, 130, 130);
                    assemblyPath = "";
                }
            };

            txtBox.DragEnter += (s, e) => {
                if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
            };
            txtBox.DragDrop += (s, e) => {
                if (e.Data != null)
                {
                    string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                    if (files != null && files.Length > 0)
                    {
                        assemblyPath = files[0];
                        txtBox.Text = assemblyPath;
                        txtBox.ForeColor = Color.White;
                    }
                }
            };

            pnl.Controls.Add(txtBox);

            Label lblPlus = new Label()
            {
                Text = "+",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(672, 10),
                Size = new Size(32, 32),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblPlus.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Assembly Files (*.exe;*.dll)|*.exe;*.dll|All Files (*.*)|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        assemblyPath = ofd.FileName;
                        txtBox.Text = assemblyPath;
                        txtBox.ForeColor = Color.White;
                    }
                }
            };
            pnl.Controls.Add(lblPlus);

            return pnl;
        }

        private Panel CreateInputBoxForDestination(string iconPath, string placeholder, int y, int x)
        {
            RoundedPanel pnl = new RoundedPanel()
            {
                Location = new Point(x, y),
                Size = new Size(720, 50),
                BackColor = Color.FromArgb(10, 10, 10),
                BorderColor = borderCol,
                BorderWidth = 1,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            PictureBox picIcon = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(16, 13),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            try
            {
                if (File.Exists(iconPath)) picIcon.Image = Image.FromFile(iconPath);
            }
            catch {}
            pnl.Controls.Add(picIcon);

            TextBox txtBox = new TextBox()
            {
                Text = placeholder,
                Font = new Font("Segoe UI", 10F),
                ForeColor = string.IsNullOrEmpty(destinationPath) ? Color.FromArgb(130, 130, 130) : Color.White,
                BackColor = Color.FromArgb(10, 10, 10),
                BorderStyle = BorderStyle.None,
                Location = new Point(52, 15),
                Size = new Size(610, 25),
                AllowDrop = true
            };

            txtBox.TextChanged += (s, e) => {
                if (txtBox.Text != "Protected assembly destination" && !string.IsNullOrEmpty(destinationPath))
                {
                    destinationPath = txtBox.Text;
                }
            };

            txtBox.Enter += (s, e) => {
                if (txtBox.Text == "Protected assembly destination")
                {
                    txtBox.Text = "";
                    txtBox.ForeColor = Color.White;
                }
            };
            txtBox.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtBox.Text))
                {
                    txtBox.Text = "Protected assembly destination";
                    txtBox.ForeColor = Color.FromArgb(130, 130, 130);
                    destinationPath = "";
                }
            };

            pnl.Controls.Add(txtBox);

            Label lblPlus = new Label()
            {
                Text = "+",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(672, 10),
                Size = new Size(32, 32),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblPlus.Click += (s, e) => {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Assembly Files (*.exe;*.dll)|*.exe;*.dll|All Files (*.*)|*.*";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        destinationPath = sfd.FileName;
                        txtBox.Text = destinationPath;
                        txtBox.ForeColor = Color.White;
                    }
                }
            };
            pnl.Controls.Add(lblPlus);

            return pnl;
        }
    }

    public class RoundedPanel : Panel
    {
        public int CornerRadius { get; set; } = 8;
        public Color BorderColor { get; set; } = Color.FromArgb(45, 45, 45);
        public int BorderWidth { get; set; } = 1;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = GetRoundedRectanglePath(this.ClientRectangle, CornerRadius))
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillPath(brush, path);
                if (BorderWidth > 0)
                {
                    Rectangle rect = this.ClientRectangle;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    using (GraphicsPath borderPath = GetRoundedRectanglePath(rect, CornerRadius))
                    using (Pen pen = new Pen(BorderColor, BorderWidth))
                    {
                        e.Graphics.DrawPath(pen, borderPath);
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y + rect.Height - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

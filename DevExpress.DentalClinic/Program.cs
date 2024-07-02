using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.DentalClinic.Model;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Utils;
using DevExpress.Utils.MVVM;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;

namespace DevExpress.DentalClinic {
    static class Program {
        [STAThread]
        static void Main() {
            XtraEditors.WindowsFormsSettings.ForceDirectXPaint();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            MVVMContext.RegisterFlyoutDialogService();
            XtraMessageBoxServiceHelper.RegisterXtraMessageBoxService();
            XtraEditors.WindowsFormsSettings.LoadApplicationSettings();
            bool darkTheme = Properties.Settings.Default.DarkTheme;
            SplashScreenManager.ShowFluentSplashScreen(logoImageOptions: new ImageOptions() {
                SvgImage = Properties.Resources.DevExpress_Logo_Mono,
                SvgImageColorizationMode = SvgImageColorizationMode.None
            });
            ToolTipController.DefaultController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
            LayoutControl.AllowCustomizationDefaultValue = false;
            XtraEditors.WindowsFormsSettings.FilterCriteriaDisplayStyle = XtraEditors.FilterCriteriaDisplayStyle.Visual;
            RegisterEntities();
            // Initialize Data-Layer
            AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
#if RECREATE_DATA
            DataGenerator.Instance.RecreateData();
#endif
            Task rescheduleAppointments = InvoiceHelper.RescheduleAppointments();
            Application.Run(new MainForm(rescheduleAppointments));
        }
        static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args) {
            string partialName = AssemblyHelper.GetPartialName(args.Name).ToLower();
            if(partialName == "entityframework" || partialName == "system.data.sqlite" || partialName == "system.data.sqlite.ef6") {
                string path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "..\\..\\bin", partialName + ".dll");
                return Data.Internal.SafeTypeResolver.GetOrLoadAssemblyFrom(path);
            }
            return null;
        }
        static void RegisterEntities() {
            XpoTypesInfoHelper.GetXpoTypeInfoSource();
            XafTypesInfo.Instance.RegisterEntity(typeof(Doctor));
            XafTypesInfo.Instance.RegisterEntity(typeof(Employee));
            XafTypesInfo.Instance.RegisterEntity(typeof(EmployeeRole));
            XafTypesInfo.Instance.RegisterEntity(typeof(PermissionPolicyUser));
            XafTypesInfo.Instance.RegisterEntity(typeof(PermissionPolicyRole));
        }
    }
}

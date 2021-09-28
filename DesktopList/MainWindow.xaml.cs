using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // variables ---------------------------
        System.Windows.Forms.NotifyIcon ni;
        private List<string> lstString = new();
        private bool _bSetWidth = false;
        System.Windows.Threading.DispatcherTimer timSave = new System.Windows.Threading.DispatcherTimer();
        // -------------------------------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // this resizes the form for us to fit the contents.
                this.SizeToContent = SizeToContent.Height;

                // Notify Icon ------------------------------
                ni = new System.Windows.Forms.NotifyIcon();
                //Note: The icon has a 'build type' property of Resource.
                System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/DesktopList;component/Images/Notes.ico")).Stream;
                ni.Icon = new System.Drawing.Icon(iconStream);
                iconStream.Dispose();
                ni.Visible = true;

                System.Windows.Forms.ContextMenuStrip cms = new System.Windows.Forms.ContextMenuStrip();
                cms.Items.Add("Exit", null, WinForms_ContextMenu_Exit_Click);
                ni.ContextMenuStrip = cms;
                // --------------------------------------------

                timSave.Tick += timer_Tick; // tick event for save timer.

                claSettings.LoadSettings();
                this.Left = (double)claSettings.iSS.ListLeft;
                this.Top = (double)claSettings.iSS.ListTop;

                // this doesn't work in Loaded - sets it to 400 after, so it's done in Activated.
                //this.Width = (double)claSettings.iSS.ListWidth;

                claSettings.iMW = this;

                claItems.LoadItems();
                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "Window_Loaded: " + ex.Message.ToString());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (timSave.IsEnabled)
                {
                    timSave.Stop();
                    claItems.SaveItems();
                }
                claSettings.SaveSettings();
                ni.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "Window_Closing: " + ex.Message.ToString());
            }
        }

        private void WinForms_ContextMenu_Exit_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Redraw()
        {
            try
            {
                // DRAW ALL ITEMS ----------------------------------------
                spList.Children.Clear();
                claSettings.booLoading = true;
                foreach (claItem iItem in claItems.lstItems.OrderBy(x => x.TaskPos))
                {
                    ToDoListItem itd = new ToDoListItem();
                    itd.txtMainText.Text = iItem.TaskText;
                    itd.MyID = iItem.MyID;
                    spList.Children.Add(itd);
                }
                claSettings.booLoading = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "Redraw: " + ex.Message.ToString());
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (timSave.IsEnabled)
                {
                    timSave.Stop();
                    claItems.SaveItems();
                }

                // NEW ITEM ----------------------------------------------
                claSettings.booLoading = true;

                claItem iItem = new claItem();
                iItem.TaskText = "";
                iItem.MyID = Guid.NewGuid();
                iItem.TaskPos = 1;
                if (claItems.lstItems.Count > 0)
                {
                    iItem.TaskPos = claItems.lstItems.OrderByDescending(a => a.TaskPos).ToList()[0].TaskPos + 1;
                }
                claItems.lstItems.Add(iItem);
                claItems.SaveItems();

                // add it to the main window.
                ToDoListItem itd = new ToDoListItem();
                itd.txtMainText.Text = "";
                itd.MyID = iItem.MyID;
                spList.Children.Add(itd);

                // will put the caret there - but not actually focus.
                //FocusManager.SetFocusedElement(itd, itd.txtMainText);

                claSettings.booLoading = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "Image_MouseLeftButtonDown: " + ex.Message.ToString());
            }
            
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Sometimes it wouldn't resize the height correctly if we didn't [repeatedly] include this.
            this.SizeToContent = SizeToContent.Height;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            try
            {
                // highlight border with nice animation. -----------------
                ColorAnimation ca = new ColorAnimation();
                ca.From = (Color)ColorConverter.ConvertFromString("#FF000000");
                //ca.From = (Color)ColorConverter.ConvertFromString("#7F2D2D32");
                ca.To = (Color)ColorConverter.ConvertFromString("#FF0078D7");
                ca.Duration = TimeSpan.FromMilliseconds(250);
                borderMain.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);

                // this doesn't work in Loaded event (sets it to 400 after, so have to do it after that).
                if (_bSetWidth == false)
                {
                    this.Width = (double)claSettings.iSS.ListWidth;
                    _bSetWidth = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "Window_Activated: " + ex.Message.ToString());
            }

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            ColorAnimation ca = new ColorAnimation();
            ca.From = (Color)ColorConverter.ConvertFromString("#FF0078D7");
            //ca.To = (Color)ColorConverter.ConvertFromString("#7F2D2D32");
            ca.To = (Color)ColorConverter.ConvertFromString("#FF000000");
            ca.Duration = TimeSpan.FromMilliseconds(250);
            borderMain.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
        }

        double intX;
        double intY;
        bool resizeInProgress = false;
        bool bMoveInProgress = false;

        private void borderRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            intX = e.GetPosition(this).X;
            resizeInProgress = true;
            Rectangle ib = sender as Rectangle;
            ib.CaptureMouse();
        }

        private void borderRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizeInProgress)
            {
                Rectangle ib = sender as Rectangle;
                ib.CaptureMouse();
                this.Width = this.Width + (e.GetPosition(this).X - intX);
                intX = e.GetPosition(this).X;
            }
        }

        private void borderRight_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            resizeInProgress = false;
            Rectangle ib = sender as Rectangle;
            ib.ReleaseMouseCapture();
        }

        private void borderMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            intX = e.GetPosition(this).X;
            intY = e.GetPosition(this).Y;
            bMoveInProgress = true;
            Border ib = sender as Border;
            ib.CaptureMouse();
        }

        private void borderMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (bMoveInProgress)
            {
                Border ib = sender as Border;
                ib.CaptureMouse();
                this.Left = this.Left + (e.GetPosition(this).X - intX);
                this.Top = this.Top + (e.GetPosition(this).Y - intY);
            }
        }

        private void borderMain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bMoveInProgress = false;
            Border ib = sender as Border;
            ib.ReleaseMouseCapture();
        }

        public void SaveItemsAfterDelay()
        {
            try
            {
                if (timSave.IsEnabled)
                {
                    timSave.Stop();
                }
                timSave.Interval = TimeSpan.FromSeconds(3);
                timSave.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "SaveItemsAfterDelay: " + ex.Message.ToString());
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timSave.Stop();
                claItems.SaveItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "timer_Tick: " + ex.Message.ToString());
            }
        }
    }
}

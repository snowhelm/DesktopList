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
using System.Windows.Threading;

namespace DesktopList
{
    /// <summary>
    /// Interaction logic for ListItem.xaml
    /// </summary>
    public partial class ToDoListItem : UserControl
    {
        public Guid MyID;
        private DispatcherTimer timMouseHasLeft;

        public ToDoListItem()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtMainText.Width = dpMain.Width;
            imgDelete.Opacity = 0;
            timMouseHasLeft = new DispatcherTimer();
            timMouseHasLeft.Interval = TimeSpan.FromMilliseconds(500);
            timMouseHasLeft.Tick += timer_Tick;
        }

        private void txtMainText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (claSettings.booLoading == false)
                {
                    claItems.lstItems.Find(x => x.MyID == MyID).TaskText = txtMainText.Text;
                    claSettings.iMW.SaveItemsAfterDelay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "txtMainText_TextChanged: " + ex.Message.ToString());
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            // make the delete button appear.
            if (imgDelete.Opacity != 1.0)
            {
                DoubleAnimation iAnimation = new DoubleAnimation();
                iAnimation.From = imgDelete.Opacity;
                iAnimation.To = 1.0;
                iAnimation.Duration = TimeSpan.FromMilliseconds(500);
                imgDelete.BeginAnimation(UIElement.OpacityProperty, iAnimation);
            }
            if (timMouseHasLeft.IsEnabled == false)
            {
                timMouseHasLeft.Start();
            }
        }

        private void imgDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // delete.
                if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    claItems.lstItems.Remove(claItems.lstItems.Find(x => x.MyID == MyID));
                    claItems.SaveItems();
                    claSettings.iMW.spList.Children.Remove(this);
                    // need to remove this from memory - dispose of this technically.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(claSettings.strError + "imgDelete_MouseLeftButtonDown: " + ex.Message.ToString());
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timMouseHasLeft.Stop();
            // has mouse left? - hide delete button.
            if (this.IsMouseOver == false)
            {
                if (imgDelete.Opacity != 0)
                {
                    DoubleAnimation iAnimation = new DoubleAnimation();
                    iAnimation.From = imgDelete.Opacity;
                    iAnimation.To = 0;
                    iAnimation.Duration = TimeSpan.FromMilliseconds(500);
                    imgDelete.BeginAnimation(UIElement.OpacityProperty, iAnimation);
                }
            } else
            {
                timMouseHasLeft.Start();
            }
        }

    } // end of class
}

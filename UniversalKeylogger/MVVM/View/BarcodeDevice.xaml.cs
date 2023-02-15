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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UniversalKeylogger.MVVM.View
{
    /// <summary>
    /// Interaction logic for BarcodeDevice.xaml
    /// </summary>
    public partial class BarcodeDevice : UserControl
    {
        public BarcodeDevice()
        {
            InitializeComponent();
        }

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(BarcodeDevice));

        public string Barcode
        {
            get
            {
                return (string)GetValue(BarcodeProperty);
            }
            set
            {
                SetValue(BarcodeProperty, value);
            }
        }
        public static readonly DependencyProperty BarcodeProperty = DependencyProperty.Register("Barcode", typeof(string), typeof(BarcodeDevice));

        public string Icon
        {
            get
            {
                return (string)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(BarcodeDevice));

        public Color Background1
        {
            get
            {
                return (Color)GetValue(Background1Property);
            }
            set
            {
                SetValue(Background1Property, value);
            }
        }
        public static readonly DependencyProperty Background1Property = DependencyProperty.Register("Background1", typeof(Color), typeof(BarcodeDevice));

        public Color Background2
        {
            get
            {
                return (Color)GetValue(Background2Property);
            }
            set
            {
                SetValue(Background2Property, value);
            }
        }
        public static readonly DependencyProperty Background2Property = DependencyProperty.Register("Background2", typeof(Color), typeof(BarcodeDevice));

        public Color EllipseBackground1
        {
            get
            {
                return (Color)GetValue(EllipseBackground1Property);
            }
            set
            {
                SetValue(EllipseBackground1Property, value);
            }
        }
        public static readonly DependencyProperty EllipseBackground1Property = DependencyProperty.Register("EllipseBackground1", typeof(Color), typeof(BarcodeDevice));

        public Color EllipseBackground2
        {
            get
            {
                return (Color)GetValue(EllipseBackground2Property);
            }
            set
            {
                SetValue(EllipseBackground2Property, value);
            }
        }
        public static readonly DependencyProperty EllipseBackground2Property = DependencyProperty.Register("EllipseBackground2", typeof(Color), typeof(BarcodeDevice));
    }
}

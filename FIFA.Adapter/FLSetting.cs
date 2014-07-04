using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace FLTestAdapter
{
    /// <summary>
    /// Chutzpah Adapter options page
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("6C95E241-763D-4887-9F05-A6B95F36D031")]
    public class FLSettings : DialogPage, INotifyPropertyChanged
    {
        public FLSettings()
        {
            enable_learning = true;
            enable_autoselection = true;
            method_name = "Ochiai";
        }


        private bool enable_learning;
        private bool enable_autoselection;
        private string method_name;


        [Browsable(true)]
        [DisplayName("Enable Learning")]
        [Description("Enable FIFA to learn from past testing data.")]
        public bool EnableLearning
        {
            get { return enable_learning; }
            set
            {
                enable_learning = value;
                OnPropertyChanged("EnableLearning");
            }
        }

        [Browsable(true)]
        [DisplayName("Enable Autoselection")]
        [Description("Enable FIFA to select proper method for you.")]
        public bool EnableAutoselection
        {
            get { return enable_autoselection; }
            set
            {
                enable_autoselection = value;
                OnPropertyChanged("EnableAutoselection");
            }
        }


        [Browsable(true)]
        [DisplayName("Method Name")]
        [Description("Instruct FIFA to use a specific method.")]
        public string MethodName
        {
            get { return method_name; }
            set
            {
                method_name = value;
                OnPropertyChanged("MethodName");
            }
        }

        public override void ResetSettings()
        {
            enable_learning = true;
            enable_autoselection = true;
            method_name = "Ochiai";
            base.ResetSettings();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

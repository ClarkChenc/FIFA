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
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Net;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using FIFA.Framework.Test;
using FIFA.Framework.Analysis;

namespace FIFATestAdapter
{
    /// <summary>
    /// Interaction logic for CodeRankControl.xaml
    /// </summary>
    public partial class CodeRankControl : UserControl
    {
        public List<BasicBlockView> Views { set; get; }

        DataCollectionListener<BasicBlock> listener;
        private CodeRankControl()
        {
            InitializeComponent();
            Views = new List<BasicBlockView>();
            listener = new DataCollectionListener<BasicBlock>(Constant.IPPort, UpdateCollection, UpdateMessage);
            listener.Start();
            
        }

        static CodeRankControl instance = null;
        public static CodeRankControl GetInstance 
        {
            get
            {
                if (instance == null)
                {
                    instance = new CodeRankControl();
                }
                return instance;
            }
        }

        public void UpdateCollection(IEnumerable<BasicBlock> list)
        {
            Views = new List<BasicBlockView>();
            foreach(var bb in list)
            {
                Views.Add(new BasicBlockView(bb));
            }
            Views.Sort(new Comparison<BasicBlockView>((a, b) =>
                {
                    return -a.Cov.susp.CompareTo(b.Cov.susp);
                }));


            this.Dispatcher.Invoke(() =>
            {
                code_rank_grid.ItemsSource = Views;
            });
        }

        public void UpdateMessage(string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                msg_label.Content = msg;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void code_rank_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if(grid.SelectedIndex < 0)
            {
                return;
            }
            int index =grid.SelectedIndex;
            BasicBlock cov = Views[index].Cov;
            DTE dte = Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE)) as DTE;
            var window = dte.ItemOperations.OpenFile(cov.source_file_path);
            var tx = window.Document.Selection as EnvDTE.TextSelection;
            
            tx.MoveToLineAndOffset((int)cov.start_line, (int)cov.start_col, false);
            tx.MoveToLineAndOffset((int)cov.end_line, (int)cov.end_col, true);
            

           
        }

        
    }
}

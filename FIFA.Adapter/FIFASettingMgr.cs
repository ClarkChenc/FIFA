using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using FIFA.Framework.Analysis;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using FIFA.Framework.Test;
namespace FIFATestAdapter
{
    public class FIFASettingMgr
    {
        public LocatorSetting LctSetting { private set; get; }
        public TestSetting TstSetting { private set; get; }
        static string lct_file_name = "D9648E05-6682-4DDA-BBE9-4EC3A98ADE22";
        static string tst_file_name = "846AEB78-774E-4789-8487-5F710BC1DC3A";
        public static FIFASettingMgr LoadFromFile()
        {
            string lct_file_full_name = Environment.GetEnvironmentVariable("TEMP")
                + @"\" + 
                lct_file_name;
            string tst_file_full_name = Environment.GetEnvironmentVariable("TEMP")
                + @"\" +
                tst_file_name;
            FIFASettingMgr mgr = new FIFASettingMgr();
            try
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                FileStream lct_fs = File.OpenRead(lct_file_full_name);
                FileStream tst_fs = File.OpenRead(tst_file_full_name);
                mgr.LctSetting = (LocatorSetting)bformatter.Deserialize(lct_fs);
                mgr.TstSetting = (TestSetting)bformatter.Deserialize(tst_fs);
                lct_fs.Close();
                tst_fs.Close();
            } catch(Exception)
            {
                mgr.LctSetting = new LocatorSetting();
                mgr.TstSetting = new TestSetting();
            }
            return mgr;
        }
        
        private FIFASettingMgr()
        {

        }
        public FIFASettingMgr(FIFASettingUI ui)
        {
            this.LctSetting = new LocatorSetting();
            this.TstSetting = new TestSetting();
            this.LctSetting.Method = ui.Method;
        }

        public void Persistence()
        {
            string lct_file_full_name = Environment.GetEnvironmentVariable("TEMP")
    + @"\" +
    lct_file_name;
            string tst_file_full_name = Environment.GetEnvironmentVariable("TEMP")
                + @"\" +
                tst_file_name;
            try
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                FileStream lct_fs = File.OpenWrite(lct_file_full_name);
                FileStream tst_fs = File.OpenWrite(tst_file_full_name);
                bformatter.Serialize(lct_fs, this.LctSetting);
                bformatter.Serialize(tst_fs, this.TstSetting);
                lct_fs.Close();
                tst_fs.Close();
            }
            catch (Exception)
            {
                
            }
        }



    }
}

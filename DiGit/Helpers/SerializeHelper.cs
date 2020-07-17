using System;
using System.IO;
using System.Xml.Serialization;

namespace DiGit.Helpers
{
    public static class SerializeHelper
    {
        public static bool Save(object data, string strFile)
        {
            if (data != null)
            {
                try
                {
                    FileStream fs = null;
                    XmlSerializer xs = new XmlSerializer(data.GetType());
                    fs = new FileStream(strFile, FileMode.Create, FileAccess.Write);
                    xs.Serialize(fs, data);
                    fs.Close();
                    fs = null;

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Exception ex = new Exception("Data object is not initialized");
                ex.Source = "SerializeHelper.Save()";
                throw ex;
            }
        }

        public static object Load(Type type, string strFile)
        {

            FileStream fs = null;
            try
            {
                fs = new FileStream(strFile, FileMode.Open, FileAccess.Read);
                XmlSerializer xs = new XmlSerializer(type);
                return xs.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    if (fs != null)
                        fs.Close();
                }
                catch { }
                finally { fs = null; }
            }
        }

    }
}

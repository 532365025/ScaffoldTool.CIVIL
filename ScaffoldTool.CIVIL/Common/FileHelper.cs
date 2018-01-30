using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Common
{
    public class FileHelper
    {
        private string _path;
        public FileHelper(string path)
        {
            this._path = path;
        }

        /// <summary>
        /// 文件读取 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> FlieReadLines()
        {
            string[] values = System.IO.File.ReadAllLines(_path);
            return values.ToList();
        }

        /// <summary>
        /// 文件写入  
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void FileWriteLine(string value)
        {
            FileStream fs = new FileStream(_path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.WriteLine(value);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 文件追加写入  
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void FileAddWriteLine(string value)
        {
            StreamWriter sw = File.AppendText(_path);
            //开始写入
            sw.WriteLine(value);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
        }

        /// <summary>
        /// 清空Json文件中所有内容 
        /// </summary>
        public void Clear()
        {
            FileStream fs = new FileStream(_path, FileMode.Create, FileAccess.Write);
            fs.Close();
        }
    }
}

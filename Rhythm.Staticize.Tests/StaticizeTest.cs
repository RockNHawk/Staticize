using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using Rhythm;
using Rhythm.Staticize;

namespace Rhythm.Staticize
{
    [TestClass]
    public class StaticizeTest
    {
        /// <summary>
        /// 初始化代码
        /// </summary>
        [TestInitialize()]
        public void Initialize() { }

        /// <summary>
        /// 资源清理代码
        /// </summary>
        [TestCleanup]
        public void Cleanup() { }


        [TestMethod]
        public void StaticizeTest1()
        {
            // 编号
            String batchId = CreateBatchId();

            // 输出文件夹
            string outputDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, batchId);
            System.IO.Directory.CreateDirectory(outputDirectory);

            List<KeyValuePair<Uri, String>> pages = new List<KeyValuePair<Uri, string>>(10000);

            // 生成豆瓣小组页面：http://www.douban.com/group/13196/discussion?start=0
            // 需要说明，如果页面内的图片、CSS、JS 采用相对路径 即不含（http://host/），Staticize 能够自动下载并放在文件夹中
            // 但如果是绝对路径，如 http://img3.douban.com/misc/mixed_static/7011201580a8cbed.css ，则是不会下载的。
            {
                for (int i = 1; i < 100; i += 25)
                {
                    string address = string.Concat("http://www.douban.com/group/13196/discussion?start=0", i);
                    string outputFile = System.IO.Path.Combine(outputDirectory, string.Concat("shine-discussion-", i.ToString(), ".html"));
                    pages.Add(new KeyValuePair<Uri, String>(new Uri(address), outputFile));
                }
            }

            CreateDirectory(pages, outputDirectory);

            Staticizer staticize = new Staticizer();

            staticize.AddBehavior(
                new ImageResourcesDownloadBehavior(outputDirectory)
                );

            //staticize.AddValidation(
            //    //验证CSS文件是否存在
            //    ValidationProjection.HasCssLink("/resources/css/jquery-ui-themes.css"),
            //    ValidationProjection.HasCssLink("/resources/css/axure_rp_page.css"),
            //    //验证网页主要页面DOM元素(id)是否存在
            //    ValidationProjection.HasElement("main_container"),
            //    //验证JS文件是否存在
            //    ValidationProjection.HasScriptLink("/data/sitemap.js"),
            //    ValidationProjection.HasScriptLink("/resources/scripts/jquery-1.7.1.min.js"),
            //    ValidationProjection.HasScriptLink("/resources/scripts/axutils.js"),
            //    ValidationProjection.HasScriptLink("/resources/scripts/jquery-ui-1.8.10.custom.min.js"),
            //    ValidationProjection.HasScriptLink("/resources/scripts/axurerp_beforepagescript.js"),
            //    ValidationProjection.HasScriptLink("/resources/scripts/messagecenter.js")
            //    );

            //staticize.AddValidation(
            //    //验证 HTML Docuemnt 中引用的资源是否存在。
            //   ValidationProjection.ResourcesExisting(outputDirectory),
            //    //XPath
            //   ValidationProjection.XPathEquals("main_template.html", "main_container"),
            //   ValidationProjection.InternalALinkExisting(outputDirectory)
            //    );

            var stepTaken = new StaticizeStepStatus();

            var staticizeResults = staticize.Staticize(pages, stepTaken);

            var validationResults = staticizeResults.GetValidationResults();
            validationResults.Save(System.IO.Path.Combine(outputDirectory, "validationResults.txt"));
        }

        public KeyValuePair<Uri, string> CreateUri(string address, String outputDirectory)
        {
            var uri = new Uri(address);
            string fileName = DefaultUriToLocalFilePathReslover.Instance.ResloveLocalPath(uri);
            return new KeyValuePair<Uri, string>(uri, System.IO.Path.Combine(outputDirectory, fileName));
        }

        void CreateDirectory(IEnumerable<KeyValuePair<Uri, String>> pages, string outputDirectory)
        {
            foreach (var item in pages)
            {
                string pageDir = item.Key.GetFileDirectory();
                var dir = System.IO.Path.Combine(outputDirectory, pageDir.RemoveLastDirectorySeparator());
                System.IO.Directory.CreateDirectory(dir);
            }
        }

        public static string CreateBatchId()
        {
            String batchId = String.Format("{0}-{1}", System.DateTime.Now.ToString("yyyyMMddHHmmss"), Guid.NewGuid().ToString().Replace('-', new char()).Substring(0, 6));
            return batchId;
        }



    }
}

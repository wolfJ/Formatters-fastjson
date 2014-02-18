using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using DragonScale.Windows.Formatters;
using DragonScale.Portable.Formatters;

namespace DragonScale.Portable.Formatters.Test
{
    /// <summary>
    /// Summary description for WindowsDefaultContentTest
    /// </summary>
    [TestClass]
    public class WindowsDefaultSettingsTest
    {
        public WindowsDefaultSettingsTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestDefaultSettingAndUnReferanceLoog()
        {
            //--------------------------------------------------------------
            PeopleX peoplex;
            string pJson;
            peoplex = new PeopleX() { Age = 1, Name = "1", Father = new PeopleX() { Name = "F1" }, Mother = new PeopleX() { Name = "M1" } };
            peoplex.Childs = new List<PeopleX>();
            peoplex.Childs.Add(peoplex.Father);
            peoplex.Childs.Add(peoplex.Mother);
            //序列化,简单对象的引用，Portable的。
            pJson = peoplex.ToJson();
            Debug.WriteLine(string.Format(":::::::ToJson::::::: \n pJson is : \n{0}", pJson));
            //反序列化
            peoplex = pJson.ToObject<PeopleX>(ContentFormat.Json);
            Debug.WriteLine(string.Format(":::::::ToObject::::::: \n peoplex is : \n{0}", peoplex.ToString()));

            //--------------------------------------------------------------
            PersonXX personX;
            personX = new PersonXX() { Age = 1, Birthday = new DateTime() };
            //序列化,继承对象的引用
            pJson = personX.ToJson();
            Debug.WriteLine(string.Format(":::::::ToJson::::::: \n pJson is : \n{0}", pJson));
            //反序列化
            personX = pJson.ToObject<PersonXX>(ContentFormat.Json);
            Debug.WriteLine(string.Format(":::::::ToObject::::::: \n personX is : \n{0}", personX.ToString()));
            //--------------------------------------------------------------
        }

        [TestMethod]
        public void TestFullSettingAndUnReferanceLoog()
        {
            //--------------------------------------------------------------
            PeopleX peoplex;
            string pJson;
            peoplex = new PeopleX() { Age = 1, Name = "1", Father = new PeopleX() { Name = "F1" }, Mother = new PeopleX() { Name = "M1" } };
            //peoplex.Father.Father = peoplex.Father;// Set Referance Loop
            peoplex.Childs = new List<PeopleX>();
            peoplex.Childs.Add(peoplex.Father);
            peoplex.Childs.Add(peoplex.Mother);

            var settings1 = FullJsonFormatterSettings.Default;
            settings1.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings1.IsNullIgnored = true;
            //序列化,Windows类库下的简单设置。循环引用关闭。NULL值过滤。
            pJson = peoplex.ToJson(settings1);
            Debug.WriteLine(string.Format(":::::::ToJson::::::: \n pJson is : \n{0}", pJson));
            //反序列化
            peoplex = pJson.ToObject<PeopleX>(ContentFormat.Json);
            Debug.WriteLine(string.Format(":::::::ToObject::::::: \n peoplex is : \n{0}", peoplex.ToString()));

            //--------------------------------------------------------------

        }

        [TestMethod]
        public void TestFullSettingAndMetaDataSerializeBurst()
        {
            //--------------------------------------------------------------
            PersonXX personX;
            string pJson;
            personX = new PersonXX() { Age = 1, Name = "1", Father = new PeopleX() { Name = "F1" }, Mother = new PeopleX() { Name = "M1" } };
            personX.Childs = new List<PeopleX>();
            personX.Childs.Add(personX.Father);
            personX.Childs.Add(personX.Mother);

            //--------------------------------------------------------------
            var settings2 = FullJsonFormatterSettings.Default;
            settings2.SerializeMetaData = SerializeMetaData.SerializeBurst;
            //settings2.JsonOutputFormaterEnum = JsonOutputFormatterEnum.FormatterOutput;
            //序列化,设置SerializeMetaData。SerializeBurst为元数据爆炸式序列化，对象的元数据都会尽可能的描述在JSON中。
            pJson = personX.ToJson(settings2);
            Debug.WriteLine(string.Format(":::::::ToJson::::::: \n pJson is : \n{0}", pJson));

            var settings22 = FullJsonFormatterSettings.Default;
            settings22.SerializeMetaData = SerializeMetaData.SerializeBurst;
            //反序列化,对应的设置MetaData的序列化方式。
            personX = pJson.ToObject<PersonXX>(ContentFormat.Json, settings22);
            Debug.WriteLine(string.Format(":::::::ToObject::::::: \n personX is : \n{0}", personX.ToString()));

            //-------------------------------------------------------------- 
        }

        [TestMethod]
        public void TestFullSettingAndMetaDataSerializeTiny()
        {
            PersonXX personX;
            string pJson;
            //--------------------------------------------------------------
            var settings3 = FullJsonFormatterSettings.Default;
            settings3.SerializeMetaData = SerializeMetaData.SerializeTiny;
            personX = new PersonXX() { Age = 1, Name = "1", Father = new PeopleX() { Name = "F1" }, Mother = new PeopleX() { Name = "M1" } };
            personX.Childs = new List<PeopleX>();
            personX.Childs.Add(personX.Father);
            personX.Childs.Add(personX.Mother);

            //序列化,设置SerializeMetaData。SerializeBurst为元数据压缩式序列化，对象的元数据都会统一管理，减少JSON的数据量。
            pJson = personX.ToJson(settings3);
            string tinyMetadata = settings3.GetSerializeMetaDataModeJson();
            Debug.WriteLine(string.Format(":::::::ToJson::::::: \n pJson is : \n{0}", pJson));

            var settings33 = FullJsonFormatterSettings.Default;
            settings33.SerializeMetaData = SerializeMetaData.SerializeTiny;
            settings33.SetSerializeMetaDataModeJson(tinyMetadata);
            //反序列化,对应的设置MetaData的序列化方式。
            personX = pJson.ToObject<PersonXX>(ContentFormat.Json, settings33);
            //test replace type
            settings33.ReplaceMetaDataMode("0", typeof(PersonXXReplace));
            var pReplace = pJson.ToObject<PersonXXReplace>(ContentFormat.Json, settings33);
            Debug.WriteLine(string.Format(":::::::ToObject::::::: \n personX is : \n{0}", personX.ToString()));

            //--------------------------------------------------------------
        }


        [TestMethod]
        public void TestFullSettingAndRenameMapping()
        {
            string pJson;
            PersonXX personX;
            //--------------------------------------------------------------
            var setting1 = FullJsonFormatterSettings.Default;
            setting1.AddKeyRenameMappingObjToJson(typeof(PersonXX), "Age", "a");
            setting1.AddKeyRenameMappingObjToJson(typeof(PersonXX), "Birthday", "b");
            setting1.AddKeyRenameMappingObjToJson(typeof(PersonXX), "Name", "n");
            personX = new PersonXX() { Age = 1, Birthday = new DateTime(), Name = "P1" };
            //序列化,名字的映射
            pJson = personX.ToJson(setting1);
            Debug.WriteLine(string.Format(":::::::ToJson::::::: \n pJson is : \n{0}", pJson));

            var setting2 = FullJsonFormatterSettings.Default;
            setting2.AddKeyRenameMappingJsonToObj(typeof(PersonXX), "Age", "a");
            setting2.AddKeyRenameMappingJsonToObj(typeof(PersonXX), "Birthday", "b");
            setting2.AddKeyRenameMappingJsonToObj(typeof(PersonXX), "Name", "n");
            //反序列化,名字的映射
            personX = pJson.ToObject<PersonXX>(ContentFormat.Json, setting2);
            Debug.WriteLine(string.Format(":::::::ToObject::::::: \n personX is : \n{0}", personX.ToString()));
            //--------------------------------------------------------------
        }
    }

    internal class PeopleX
    {
        public int? Age;
        public Int16 Age16;
        public Int64 Age32 { get; set; }
        public Int64 Age64 { get; set; }
        public String Name;
        public PeopleX Father;
        public PeopleX Mother;
        [DragonScale.Portable.TransientAttribute]
        public List<PeopleX> Childs;
    }

    internal class PersonXX : PeopleX
    {
        public bool? Sex { get; set; }
        public DateTime? Birthday { get; set; }
    }

    internal class PersonXXReplace : PeopleX
    {
        public bool? Sex { get; set; }
        public DateTime? Birthday { get; set; }
    }


}

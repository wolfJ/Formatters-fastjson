using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using RoleGame;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DragonScale.Windows.Formatters;

namespace DragonScale.Portable.Formatters.Test
{
    [TestClass]
    public class FormatterTest
    {
        [TestMethod]
        public void TestToJsonEx()
        {
            Person p1 = new Person() { Age = 11, Name = "wolf11", Loop = new Person() { Age = 1, Name = "wolf1" } };
            Person p2 = new Person() { Age = 22, Name = "wolf22", Loop = new Person() { Age = 2, Name = "wolf2" } };
            p1.Loop.Loop = p2;
            p2.Loop.Loop = p1;
            PersonT<string> pt1 = new PersonT<string>()
            {
                Age = 11,
                Name = "wolf11",
                SubType = "test11",
                Loop = new PersonT<string>()
                {
                    Age = 1,
                    Name = "wolf1",
                    SubType = "test1"
                }
            };

            var roles = new List<Role>();
            var role_key1 = new Privilege<Treasure>("Key1", Treasure.Level1);
            var role_key2 = new Privilege<Treasure>("Key2", role_key1, Treasure.Level2);
            var role_key3 = new Privilege<Treasure>("Key3", role_key2, Treasure.Level3);
            var role_key4 = new Privilege<Treasure>("Key4", role_key3, Treasure.Level4);
            var role_key5 = new Privilege<Treasure>("Key5", role_key4, Treasure.Level5);
            roles.Add(role_key1);
            roles.Add(role_key2);
            roles.Add(role_key3);
            roles.Add(role_key4);
            roles.Add(role_key5);


            //BinaryFormatter b = new BinaryFormatter();
            //using (var stream = new MemoryStream())
            //{
            //    b.Serialize(stream, roles);

            //}

            var set = new JsonFormatterSettings() {  };
            set.ContentProvider = new FullContentProvider(set);
            using (var stream = File.OpenWrite("d:/json.json"))
            {
                var data = Encoding.UTF8.GetBytes(roles.ToJson(set));
                stream.Write(data, 0, data.Length);
                stream.Flush();
                stream.Close();
            }
            //using (var stream = File.OpenRead("d:/json.json"))
            //{
            //    var reader = new StreamReader(stream);
            //    var content = reader.ReadToEnd();
            //    Settings s = new JsonFormatterSettings() { ReferenceLoopHandling = DragonScale.Portable.Formatters.Core.ReferenceLoopHandling.Serialize };
            //    roles = content.ToObject<List<Role>>(ContentFormat.Json, s);
            //}



            //Settings settings = new JsonFormatterSettings() { ReferenceLoopHandling = Core.ReferenceLoopHandling.Serialize };
            //var pJson = pt1.ToJson(settings);

            //var objPer = pJson.ToObject<Person>(ContentFormat.Json, settings);
        }
    }

    internal class Person
    {
        private int age;
        private string name;
        private Person loop;

        public Person Loop
        {
            get { return this.loop; }
            set { this.loop = value; }
        }

        public int Age
        {
            //get;
            //set;
            get { return this.age; }
            set { this.age = value; }
        }

        public string Name
        {
            //get;
            //set;
            get { return this.name; }
            set { this.name = value; }
        }

    }

    internal class PersonT<T>
    {
        private int age;
        private string name;
        private PersonT<T> loop;
        private T subType;

        public Type Type
        {
            get { return typeof(T); }
        }

        public PersonT<T> Loop
        {
            get { return this.loop; }
            set { this.loop = value; }
        }

        public int Age
        {
            //get;
            //set;
            get { return this.age; }
            set { this.age = value; }
        }

        public string Name
        {
            //get;
            //set;
            get { return this.name; }
            set { this.name = value; }
        }


        public T SubType
        {
            //get;
            //set;
            get { return this.subType; }
            set { this.subType = value; }
        }
    }


    internal class TestType
    {
        public Type Typexxx
        {
            //get { return Type.GetType("system.");}
            get;
            set;
        }

    }
}

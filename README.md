Formatters-fastjson
===================

Portable Formatters for XML, JSON.  json is fork of http://fastjson.codeplex.com/

DragonScale.Portable.Formatters; portable 可用在.net的任意平台中。
    + DragonScale.Silverlight.Formatters; Silverlight下的库支持。
    + DragonScale.Windows.Formatters; Windows下的库支持。


DragonScale.Portable.Formatters.Test 含有测试类，简单的用法。

支持：
  与标准JSON对接。
  对象循环引用。
  可设置IgnoreProperty,IgnoreField.
  对象变量名的映射，可用于压缩，或避免重复构造对象。
  支持SerializeMetaData，对象的元数据信息也可序列化。有了对象的元数据，。net下的整合能力就会很强了。
  支持LoadTypeFromAssemblies。
  

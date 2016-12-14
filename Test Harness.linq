<Query Kind="Program">
  <Reference Relative="MAB.SimpleMapper\bin\Debug\MAB.SimpleMapper.dll">C:\Src\MAB.SimpleMapper\MAB.SimpleMapper\bin\Debug\MAB.SimpleMapper.dll</Reference>
  <Namespace>MAB.SimpleMapper</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

void Main()
{
    var tmp = new {
        Email = "test@test.com",
        ID = 100,
        NotAParam = true
    };

    tmp.MapToConstructor<TestClass1>().Dump();
    tmp.MapToConstructor<TestClass2>().Dump();
    tmp.MapToConstructor<TestClass3>().Dump();
    tmp.MapToConstructor<TestClass4>().Dump();
}

public class TestClass1
{
    public TestClass1() {}

    public TestClass1(int id)
    {
        ID = id;
    }
    
    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Other { get; private set; }
}

public class TestClass2
{
    public TestClass2() {}

    public TestClass2(int id, string email)
    {
        ID = id;
        Email = email;
    }
    
    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Other { get; private set; }
}

public class TestClass3
{
    public TestClass3() {}

    public TestClass3(int id, string name, string email)
    {
        ID = id;
        Name = name;
        Email = email;
    }

    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Other { get; private set; }
}

public class TestClass4
{
    public TestClass4() {}

    public TestClass4(int id, string name, string email, string other)
    {
        ID = id;
        Name = name;
        Email = email;
        Other = other;
    }

    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Other { get; private set; }
}
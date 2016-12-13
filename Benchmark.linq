<Query Kind="Program">
  <Reference Relative="MAB.SimpleMapper\bin\Debug\MAB.SimpleMapper.dll">E:\Src\MAB.SimpleMapper\MAB.SimpleMapper\bin\Debug\MAB.SimpleMapper.dll</Reference>
  <Namespace>MAB.SimpleMapper</Namespace>
</Query>

void Main()
{
    Mapper.ClearMappings();

    var entities = GetEntities(1000);

    var results = new List<Model>();

    var result1 = Benchmark.Perform(() => {
        foreach (var entity in entities)
        {
            // var model = new Model();
            // entity.MapTo(model);
            var model = entity.MapTo<Model>();
            results.Add(model);
        }
    });

    Mapper.ClearMappings();
    Mapper.UseCustomDelegate = true;

    var results2 = new List<Model>();

    var result2 = Benchmark.Perform(() => {
        foreach (var entity in entities)
        {
//            var model = new Model();
//            entity.MapTo(model);
            var model = entity.MapTo<Model>();
            results2.Add(model);
        }
    });

    result1.Dump("Activator.CreateInstance");

    results.Take(10).Dump();

    result2.Dump("Cached Delegate");

    results.Take(10).Dump();
    
    Mapper._activators.Dump();
}

public IEnumerable<Entity> GetEntities(int count)
{
    var rnd = new Random();

    for (var i = 0; i < count; i++)
    {
        var email = new String(Enumerable.Range(1, 5).Select(x => (char)rnd.Next(66, 91)).ToArray())
                  + "@" + new String(Enumerable.Range(1, 10).Select(x => (char)rnd.Next(66, 91)).ToArray())
                  + "." + new String(Enumerable.Range(1, 3).Select(x => (char)rnd.Next(66, 91)).ToArray());

        yield return new Entity
        {
            ID = rnd.Next(1, 500),
            Email = email.ToLower(),
            Created = DateTime.Now.AddDays((rnd.Next(1, 10000) * -1)),
            Active = (rnd.Next(1, 100) > 50)
        };
    }
}

public class Entity 
{
    public int ID { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public bool Active { get; set; }
}

public class Model 
{
    public int ID { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public bool Active { get; set; }
}

public class Benchmark 
{
    public static long Perform(Action action)
	{
		return Benchmark.Perform(action, 1);
	}

	public static long Perform(Action action, int iterations)
	{
		var stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < iterations; i++)
		{
			action();
		}
		stopwatch.Stop();
		return stopwatch.ElapsedMilliseconds;
	}
}
<Query Kind="Program">
  <Reference Relative="MAB.SimpleMapper\bin\Debug\MAB.SimpleMapper.dll">E:\Src\MAB.SimpleMapper\MAB.SimpleMapper\bin\Debug\MAB.SimpleMapper.dll</Reference>
  <Namespace>MAB.SimpleMapper</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

void Main()
{
    var tmp = new {
        ID = 100,
        Email = "test@test.com"
    };

    var tc1 = tmp.TUseToConstruct2<TestClass>();
    tc1.Dump();
    
    var tc2 = tmp.TUseToConstruct2<TestClass>();
    tc2.Dump();
}

public class TestClass 
{
//    public TestClass() {}
//
//    public TestClass(int id, string email)
//    {
//        ID = id;
//        Email = email;
//    }

    public TestClass(int id, string name, string email)
    {
        ID = id;
        Name = name;
        Email = email;
    }

    public TestClass(int id, string name, string email, string other)
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

public static class Extensions 
{
    public static TDestination TUseToConstruct<TDestination>(this object source) where TDestination : class
    {
        var sourceProperties = source.GetType().GetProperties().Select(p => p.GetValue(source)).ToArray();
        
        return (TDestination)Activator.CreateInstance(typeof(TDestination), sourceProperties);   
    }

    public static TDestination TUseToConstruct2<TDestination>(this object source) where TDestination: class
    {
        // Get all the constructors for the destination type
        var constructors = typeof(TDestination).GetConstructors();
        
        var constructorSignatures = new Dictionary<ConstructorInfo, IEnumerable<string>>();
        
        // For each constructor on this type, add a dictionary entry where the constructor itself is the key, and the 
        // value is a list of the parameters for the constructor, converted to strings in the form PROPNAME~TYPENAME
        foreach (var constructor in constructors)
        {
            var constructorSignature = constructor.GetParameters().Select(p => string.Format("{0}~{1}", GetNormalisedPropertyName(p.Name), p.ParameterType));
            
            constructorSignatures.Add(constructor, constructorSignature);
        }
        
        // Now we do the same with all the properties of the source type
        var sourceProperties = source.GetType().GetProperties();

        var sourcePropertySignature = sourceProperties.Select(p => string.Format("{0}~{1}", GetNormalisedPropertyName(p.Name), p.PropertyType));

        // This dictionary will hold a number telling us how different each constructor signature
        var rankedConstructorSignatures = new List<Tuple<ConstructorInfo, int>>();

        // Loop over each constructor signature and find the one which matches the most source type properties, by intersecting
        // the array of property signatures for each constructor with the property signature of the current source object
        foreach (var k in constructorSignatures.Keys)
            rankedConstructorSignatures.Add(new Tuple<ConstructorInfo, int>(k, sourcePropertySignature.Intersect(constructorSignatures[k]).Count()));
        
        // Pick the constructor with the largest intersection with the source type's properties
        var bestConstructor = rankedConstructorSignatures.OrderByDescending(x => x.Item2).First().Item1;
        
        // Create a delegate which will construct on object of the new type using the best matching constructor
        // TODO: We can also cache this in memory to improve performance
        var createObject = CreateObject<TDestination>(bestConstructor);
        
        Func<PropertyInfo, ParameterInfo, bool> find = (sp, p) => GetNormalisedPropertyName(sp.Name) == GetNormalisedPropertyName(p.Name) && sp.PropertyType == p.ParameterType;
        
        var args = bestConstructor.GetParameters().Select(p => sourceProperties.FirstOrDefault(sp => find(sp, p))?.GetValue(source)).ToArray(); 
        
        return createObject(args);
    }

    private delegate T CreateT<T>(params object[] args);

    private static CreateT<T> CreateObject<T>(ConstructorInfo ctor)
    {
        var type = ctor.DeclaringType;
        var ctorParams = ctor.GetParameters();

        //create a single param of type object[]
        var args = Expression.Parameter(typeof(object[]), "args");

        var argsExp = new Expression[ctorParams.Length];

        //pick each arg from the params array 
        //and create a typed expression of them
        for (int i = 0; i < ctorParams.Length; i++)
        {
            argsExp[i] = Expression.Convert(
                Expression.ArrayIndex(args, Expression.Constant(i)),
                ctorParams[i].ParameterType
            );
        }

        //make a NewExpression that calls the
        //ctor with the args we just created
        var newExp = Expression.New(ctor, argsExp);

        //create a lambda with the New
        //Expression as body and our param object[] as arg
        var lambda = Expression.Lambda(typeof(CreateT<T>), newExp, args);

        return (CreateT<T>)lambda.Compile();
    }

    private static string GetNormalisedPropertyName(string propertyName)
    {
        return Regex.Replace(propertyName.ToUpperInvariant(), "_", "");
    }
}

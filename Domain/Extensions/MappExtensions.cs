using System.Reflection;

namespace Domain.Extensions;

public static class MappExtensions
{
    public static TDestination MapTo<TDestination>(this object source)
    {
        //Tog hjälp från ChatGPT att uppgradera Mapextension.

        ArgumentNullException.ThrowIfNull(source, nameof(source));

        TDestination destination = Activator.CreateInstance<TDestination>()!;

        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destProp in destinationProperties)
        {
            var sourceProp = sourceProperties.FirstOrDefault(x => x.Name == destProp.Name);
            if (sourceProp == null || !destProp.CanWrite)
                continue;

            var sourceValue = sourceProp.GetValue(source);
            if (sourceValue == null)
                continue;

            // Om det är samma typ, tex. string, sätts det direkt.
            if (destProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
            {
                destProp.SetValue(destination, sourceValue);
            }
            // Om det är en komplex typ, tex. klass, görs rekursiv mapping. 
            else if (IsComplexType(destProp.PropertyType) && IsComplexType(sourceProp.PropertyType))
            {
                var mappedValue = sourceValue.MapTo(destProp.PropertyType);
                destProp.SetValue(destination, mappedValue);
            }
        }

        return destination;
    }


    // Returnerar true om det är en komplex typ, dvs. en klass som inte är en sträng, som AdressEntity tex.
    private static bool IsComplexType(Type type)
    {
        return type.IsClass && type != typeof(string); 
    }

    // Gör så att vi kan anropa MapTo när vi inte vet generiska typen, tex. AdressEntity till AddressModel
    private static object? MapTo(this object source, Type destinationType)
    {
        if (source == null)
            return null;

        var method = typeof(MappExtensions)
            .GetMethod(nameof(MapTo), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(destinationType);

        return method.Invoke(null, new[] { source });
    }
}


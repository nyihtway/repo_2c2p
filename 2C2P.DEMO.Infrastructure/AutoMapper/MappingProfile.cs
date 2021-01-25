using _2C2P.DEMO.Infrastructure.Interfaces;
using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace _2C2P.DEMO.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var runningAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Contains("2C2P.DEMO."));
            foreach (var assembly in runningAssemblies)
            {
                ApplyMappingsFromAssembly(assembly);
            }
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping") ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}

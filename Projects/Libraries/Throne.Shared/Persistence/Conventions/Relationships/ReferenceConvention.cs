using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Throne.Shared.Persistence.Conventions.Relationships
{
    internal sealed class ReferenceConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.NotFound.Exception();
        }
    }
}
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Throne.Framework.Persistence.Conventions.Relationships
{
    internal sealed class ReferenceConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.NotFound.Exception();
        }
    }
}
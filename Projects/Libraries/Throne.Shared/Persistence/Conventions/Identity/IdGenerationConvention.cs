using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Throne.Framework.Persistence.Conventions.Identity
{
    internal sealed class IdGenerationConvention : IIdConvention, IIdConventionAcceptance
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.GeneratedBy.Increment();
        }

        public void Accept(IAcceptanceCriteria<IIdentityInspector> criteria)
        {
            criteria.Expect(x => x.Generator, Is.Not.Set);
        }
    }
}
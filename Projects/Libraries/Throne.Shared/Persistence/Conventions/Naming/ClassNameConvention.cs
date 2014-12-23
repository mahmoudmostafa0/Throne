using System;
using System.Diagnostics.Contracts;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Throne.Framework.Persistence.Conventions.Naming
{
    internal sealed class ClassNameConvention : IClassConvention
    {
        public const string Record = "Record";

        public void Apply(IClassInstance instance)
        {
            string tableName = instance.TableName;
            int recIndex = tableName.IndexOf(Record, StringComparison.Ordinal);

            if (recIndex == -1)
                return;

            int recLength = Record.Length;
            Contract.Assume(recIndex + recLength < tableName.Length);
            instance.Table(tableName.Remove(recIndex, recLength));
        }
    }
}
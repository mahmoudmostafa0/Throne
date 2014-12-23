using NHibernate.Tool.hbm2ddl;

namespace Throne.Framework.Persistence.Schema
{
    public sealed class SchemaInfo
    {
        private readonly SchemaExport _schema;

        public SchemaInfo(DatabaseContext ctx)
        {
            _schema = new SchemaExport(ctx.Configuration);
        }

        public void Create()
        {
            _schema.Create(false, true);
        }

        public void Drop()
        {
            _schema.Drop(false, true);
        }

        public void Export(string file)
        {
            _schema.SetOutputFile(file);
            _schema.Create(false, false);
        }
    }
}
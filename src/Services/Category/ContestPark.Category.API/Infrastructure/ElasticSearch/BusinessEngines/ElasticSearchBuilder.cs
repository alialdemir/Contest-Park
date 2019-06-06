using Nest;
using System;
using System.Linq.Expressions;
using ContestPark.Category.API.Extensions;

namespace ContestPark.Category.API.Infrastructure.ElasticSearch.BusinessEngines
{
    public class ElasticSearchBuilder
    {
        internal string IndexName;
        internal int Size;
        internal int From;
        internal IQueryContainer QueryContainer;
        internal IElasticContext ElasticContext;

        public ElasticSearchBuilder(string indexName, IElasticContext elasticContext)
        {
            IndexName = indexName;
            ElasticContext = elasticContext;

            QueryContainer = new QueryContainer();
        }

        public ElasticSearchBuilder SetSize(int size)
        {
            Size = size;

            return this;
        }

        public ElasticSearchBuilder SetFrom(int from)
        {
            From = from;

            return this;
        }

        public ElasticSearchBuilder AddTermQuery<T>(Expression<Func<T, object>> expression, string term)
        {
            string field = expression.Body.GetMemberName<T>();
            field = field.Substring(0, 1).ToLower() + field.Substring(1);

            return AddTermQuery<T>(field, term);
        }

        public ElasticSearchBuilder AddTermQuery<T>(string field, string term)
        {
            QueryContainer.Term = new TermQuery()
            {
                Field = new Field(field),
                Value = term
            };

            return this;
        }

        public ElasticSearchEngine Build()
        {
            return new ElasticSearchEngine(this);
        }
    }
}
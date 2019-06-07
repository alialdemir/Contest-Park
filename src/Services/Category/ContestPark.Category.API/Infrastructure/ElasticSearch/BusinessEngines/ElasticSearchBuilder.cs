using Nest;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ContestPark.Category.API.Infrastructure.ElasticSearch.BusinessEngines
{
    public class ElasticSearchBuilder
    {
        #region Private Variables

        internal string IndexName;
        internal int Size;
        internal int From;
        internal IQueryContainer QueryContainer;
        internal IElasticContext ElasticContext;

        #endregion Private Variables

        #region Constructor

        public ElasticSearchBuilder(string indexName,
                                    IElasticContext elasticContext)
        {
            IndexName = indexName;
            ElasticContext = elasticContext;

            QueryContainer = new QueryContainer();
        }

        #endregion Constructor

        #region Methods

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

        public ElasticSearchBuilder AddTermQuery<T>(Expression<Func<T, object>> expression, object term)
        {
            QueryContainer.Term = new TermQuery()
            {
                Field = new Field(expression),
                Value = term
            };

            return this;
        }

        public ElasticSearchBuilder AddTermQuery(string field, object term)
        {
            QueryContainer.Term = new TermQuery()
            {
                Field = new Field(field),
                Value = term
            };

            return this;
        }

        public ElasticSearchBuilder AddTermsQuery(string field, params object[] term)
        {
            QueryContainer.Terms = new TermsQuery()
            {
                Field = new Field(field),
                Terms = term.AsEnumerable()
            };

            return this;
        }

        public ElasticSearchBuilder AddFilter<T>(string field, params string[] term)
        {
            var queryContainer = new QueryContainer[term.Length];
            for (int i = 0; i < term.Length; i++)
            {
                queryContainer[i] = new MatchQuery { Field = new Field(field), Query = term[i] };
            }

            QueryContainer.Bool = new BoolQuery
            {
                Filter = queryContainer
            };

            return this;
        }

        public ElasticSearchBuilder AddTermsQuery<T>(Expression<Func<T, object>> expression, params object[] term)
        {
            QueryContainer.Terms = new TermsQuery()
            {
                Field = new Field(expression),
                Terms = term
            };

            return this;
        }

        public ElasticSearchEngine Build()
        {
            return new ElasticSearchEngine(this);
        }

        #endregion Methods
    }
}
// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using Guan.Logic;

namespace Guan
{
    public class GuanQueryDispatcher
    {
        private readonly Module _module;

        public GuanQueryDispatcher(Module module)
        {
            _module = module;
        }

        public Task<Term?> SingleQueryAsync(string queryExpression)
        {
            var query = CreateQuery(queryExpression);

            // Execute the query. 
            // result will be () if there is no answer/result for supplied query.
            return query.GetNextAsync();
        }

        public async IAsyncEnumerable<Term> QueryAsync(string queryExpression, int maxResults = 1)
        {
            var query = CreateQuery(queryExpression);

            // Execute the query. 
            // result will be () if there is no answer/result for supplied query (see the simple external predicate rules, for example).
            if (maxResults == 1)
            {
                // Gets one result.
                Term result = await query.GetNextAsync();
                Console.WriteLine($"answer: {result}"); // () if there is no answer.
                yield return result;
            }
            else
            {
                // Gets multiple results, if possible, up to supplied maxResults value.
                var results = query.GetResultsAsync(maxResults);
                
                await foreach (var result in results)
                    yield return result;
            }
        }

        public async Task<bool> BoolQueryAsync(string queryExpression)
        {
            var result = await SingleQueryAsync(queryExpression);
            return result is not null;
        }

        private Query CreateQuery(string queryExpression)
        {
            // Required ModuleProvider instance. You created the module used in its construction in Program.cs.
            ModuleProvider moduleProvider = new ModuleProvider();
            moduleProvider.Add(_module);

            // Required QueryContext instance. You must supply moduleProvider (it implements IFunctorProvider).
            QueryContext queryContext = new QueryContext(moduleProvider);

            // The Query instance that will be used to execute the supplied query expression over the related rules.
            return Query.Create(queryExpression, queryContext);
        }
    }
}

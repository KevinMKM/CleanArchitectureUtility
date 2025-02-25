﻿namespace CleanArchitectureUtility.Core.Contract.ApplicationServices.Queries;

public interface IQueryHandler<TQuery, TData> where TQuery : class, IQuery<TData>
{
    Task<QueryResult<TData>> Handle(TQuery request, CancellationToken cancellationToken);
}
export interface PaginatedResultsDto<T>
{
  results: T[];
  page: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}

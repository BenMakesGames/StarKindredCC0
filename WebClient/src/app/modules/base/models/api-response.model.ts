export interface ApiResponseModel<T>
{
  messages: ApiMessage[];
  data: T;
}

export interface ApiMessage
{
  type: 'Info'|'Error'|'Success';
  text: string;
}

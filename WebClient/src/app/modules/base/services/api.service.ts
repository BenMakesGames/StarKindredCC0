import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";
import {BehaviorSubject, catchError, from, map, Observable, Subject, switchMap, tap, throwError} from "rxjs";
import {ApiResponseModel} from "../models/api-response.model";
import { stringify } from 'qs';
import {environment} from "../../../../environments/environment";
import {MessagesService} from "./messages.service";
import { Router } from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class ApiService
{
  private rootUrl = environment.apiEndpoint;

  public sessionId = new BehaviorSubject<string|null>(null);

  constructor(private http: HttpClient, private messages: MessagesService, private router: Router) {
    this.sessionId.next(localStorage.getItem('sessionId'));
  }

  setSessionId(sessionId: string|null)
  {
    if(sessionId) {
      localStorage.setItem('sessionId', sessionId);
      localStorage.setItem('sessionIdCreatedOn', new Date().toISOString());
    }
    else
      localStorage.removeItem('sessionId');

    this.sessionId.next(sessionId);
  }

  sessionIdIsOld()
  {
    const sessionIdCreatedOn = localStorage.getItem('sessionIdCreatedOn');

    if(!sessionIdCreatedOn)
      return true;

    const createdOn = new Date(sessionIdCreatedOn);
    const now = new Date();

    // session ids are good for 3 days, but let's be extra-good and get a new one every 24 hours
    return now.getTime() - createdOn.getTime() > 1000 * 60 * 60 * 24;
  }

  private options(): any
  {
    let headers = new HttpHeaders();

    if(this.sessionId.value)
      headers = headers.append('Authorization', 'Bearer ' + this.sessionId.value);

    return {
      headers: headers,
      responseType: 'json',
    };
  }

  get<T>(path: string, data: any = null): Observable<ApiResponseModel<T>>
  {
    return this.keepSessionIdFresh().pipe(
      switchMap(() => this.http.get<ApiResponseModel<T>>(this.rootUrl + path + (data ? '?' + stringify(data) : ''), <object>this.options())),
      catchError(r => this.commonErrorHandler<T>(r, () => {
        return this.http.get<ApiResponseModel<T>>(this.rootUrl + path + (data ? '?' + stringify(data) : ''), <object>this.options());
      })),
      map(r => this.commonDataHandler<T>(r)),
    );
  }

  post<T>(path: string, data: any = {}): Observable<ApiResponseModel<T>>
  {
    return this.keepSessionIdFresh().pipe(
      switchMap(() => this.http.post<ApiResponseModel<T>>(this.rootUrl + path, data, <object>this.options())),
      catchError(r => this.commonErrorHandler<T>(r, () => {
        return this.http.post<ApiResponseModel<T>>(this.rootUrl + path, data, <object>this.options());
      })),
      map(r => this.commonDataHandler<T>(r)),
    );
  }

  private onlyOneRenewSessionAtATimePlz: Subject<any>|null = null;

  private keepSessionIdFresh(): Observable<any>
  {
    if(!this.sessionId.value || !this.sessionIdIsOld())
      return from([null]);

    if(this.onlyOneRenewSessionAtATimePlz && !this.onlyOneRenewSessionAtATimePlz.closed)
      return this.onlyOneRenewSessionAtATimePlz;

    this.onlyOneRenewSessionAtATimePlz = new Subject<any>();

    return this.http.post<ApiResponseModel<{ sessionId: string }>>(this.rootUrl + 'accounts/renewSession', null, <object>this.options())
      .pipe(
        tap(r => {
          this.setSessionId(r.data.sessionId);
          this.onlyOneRenewSessionAtATimePlz?.next(null);
          this.onlyOneRenewSessionAtATimePlz?.complete();
        })
      )
    ;
  }

  private commonErrorHandler<T>(r: HttpErrorResponse, originalRequest: Function): Observable<ApiResponseModel<T>>
  {
    if(r.status === 0)
    {
      this.messages.add({
        type: 'Error',
        text: 'Could not connect to the StarKindred server. Either the server is down, or you are not connected to the internet.',
      });

      return throwError(() => r.error);
    }

    if(r.status == 401)
    {
      this.setSessionId(null);
      // noinspection JSIgnoredPromiseFromCall
      this.router.navigateByUrl('/');
    }

    const data = <ApiResponseModel<T>>r.error;

    this.processCommonData(data);

    return throwError(() => data);
  }

  private commonDataHandler<T>(r: ApiResponseModel<T>): ApiResponseModel<T>
  {
    this.processCommonData(r);

    return r;
  }

  private processCommonData(data: ApiResponseModel<any>)
  {
    if(data.messages)
    {
      data.messages.forEach(m => {
        this.messages.add(m);
      });
    }
  }
}

import { Plant } from './plant';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RemoteService {
  private url:string = "https://127.0.0.1:7189/api/plants";

  constructor(private http: HttpClient) {
    
  }

    // Get all plants from the API
  getPlants():Observable<Plant>{
    return this.http.get<Plant>(this.url);
  }
}

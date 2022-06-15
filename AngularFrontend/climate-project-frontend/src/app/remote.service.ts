import { Plant } from './plant';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Datalogger } from './datalogger';

@Injectable({
  providedIn: 'root'
})
export class RemoteService {
  private url:string = "https://127.0.0.1:7189/api";

  constructor(private http: HttpClient) {
    
  }

  // Get all plants from the API
  getPlants():Observable<Plant>{
    return this.http.get<Plant>(`${this.url}/plants`);
  }

  // Get all dataloggers from the API
  getDataloggers():Observable<Datalogger>{
    return this.http.get<Datalogger>(`${this.url}/dataloggers`);
  }
}

import { Component, OnInit } from '@angular/core';
import { RemoteService } from '../remote.service';

@Component({
  selector: 'app-plant-list',
  templateUrl: './plant-list.component.html',
  styleUrls: ['./plant-list.component.css']
})
export class PlantListComponent implements OnInit {
  
  constructor(private remoteService:RemoteService) { }

  public plants:any = [];

  ngOnInit(): void {
    this.loadPlants();
  }

  loadPlants() {
    return this.remoteService.getPlants().subscribe((data: {}) => {
      this.plants = data;
    });
  }
}
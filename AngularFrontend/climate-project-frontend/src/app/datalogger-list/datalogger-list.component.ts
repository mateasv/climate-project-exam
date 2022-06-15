import { Component, OnInit } from '@angular/core';
import { RemoteService } from '../remote.service';

@Component({
  selector: 'app-datalogger-list',
  templateUrl: './datalogger-list.component.html',
  styleUrls: ['./datalogger-list.component.css']
})
export class DataloggerListComponent implements OnInit {

  constructor(private remoteService:RemoteService) { }

  public dataloggers:any = [];

  ngOnInit(): void {
    this.loadDataloggers();
  }

  loadDataloggers() {
    return this.remoteService.getDataloggers().subscribe((data: {}) => {
      this.dataloggers = data;
    });
  }
}

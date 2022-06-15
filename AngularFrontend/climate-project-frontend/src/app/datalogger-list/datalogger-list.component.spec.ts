import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataloggerListComponent } from './datalogger-list.component';

describe('DataloggerListComponent', () => {
  let component: DataloggerListComponent;
  let fixture: ComponentFixture<DataloggerListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataloggerListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataloggerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

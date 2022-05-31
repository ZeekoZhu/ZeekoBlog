import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-edit-page',
  templateUrl: './edit-page.component.html',
  styleUrls: [ './edit-page.component.css' ],
})
export class EditPageComponent implements OnInit {
  handleBack() {
    this.location.back();
  }

  constructor(private location: Location) {}

  ngOnInit(): void {
  }

}

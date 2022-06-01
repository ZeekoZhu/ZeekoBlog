import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: [ './button.component.css' ],
})
export class ButtonComponent implements OnInit {

  @Input() loading: boolean | null = false;
  @Input() type?: HTMLButtonElement['type'];
  @Input() disabled?: boolean;

  constructor() { }

  ngOnInit(): void {
  }

}

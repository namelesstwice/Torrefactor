import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CoffeeRoaster } from '../_models/coffee-roaster';

@Component({
  selector: 'app-coffee-roaster-select-dialog',
  templateUrl: './coffee-roaster-select-dialog.component.html',
  styleUrls: ['./coffee-roaster-select-dialog.component.less']
})
export class CoffeeRoasterSelectDialogComponent implements OnInit {

  public selected: string;

  constructor(
    public dialogRef: MatDialogRef<CoffeeRoasterSelectDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CoffeeRoaster[]) {}

  ngOnInit(): void {
  }
}

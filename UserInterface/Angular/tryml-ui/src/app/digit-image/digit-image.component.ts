import { Component, OnInit } from '@angular/core';
import { HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { DigitImageService } from '../digit-image.service';
import { ServiceLayerCallResult } from '../utilities/service-layer-interface/service-layer-interface';

@Component({
  selector: 'app-digit-image',
  templateUrl: './digit-image.component.html',
  styleUrls: ['./digit-image.component.css']
})
/**
 * @name DigitImageComponent
 * @desc Component which displays the pixels of a digit image in a grid.
 */
export class DigitImageComponent implements OnInit {

  private digitDimension: number = 28;
  private pixelData: number[][];

  constructor(private digitImageService: DigitImageService) { 

    this.pixelData = new Array<Array<number>>();

    for (let i = 0; i < this.digitDimension; i++) {
      let thisRow: number[] = new Array<number>(0);
      for (let j = 0; j < this.digitDimension; j++) {
        thisRow.push(0);
      }
      this.pixelData.push(thisRow);
    } 
  }

  ngOnInit() {
  }

  /**
   * @name convertNumericShadeToHtmlColour
   * @desc Converts the inputted numeric value (positive byte) into an html hex color (e.g. 0 converts to #ffffff).
   *
   * @param {number} shade - The number to convert.
   * @return {string} A string containing an html hex colour (prefixed with '#').
   */
  public convertNumericShadeToHtmlColour(shade: number): string  {
    if (shade < 0 || shade > 255)
      throw new Error("Value of parameter 'shade' (" + shade.toString() + ") must be between 0 and 255 inclusive.");

    // Invert the value (so black is 0 rather than 255)
    shade = 255 - shade;
    // Convert to HTML hex colour string
    let hexValue: string = shade.toString(16);
    if (hexValue.length == 1) {
      hexValue = "0" + hexValue;
    }

    return "#" + hexValue + hexValue + hexValue;
  }

  /**
   * @name getDigitImage
   * @desc Gets a digit image from a remote API via the digit image service and populates the local 'pixelData' member.
   *
   * @param {number} imageId - The id number of the digit image.
   */
  public getDigitImage(imageId: number): void {

    /* 
    // For Observable
    this.digitImageService.getDigitImage(imageId)
      .subscribe(
        (returnData: number[][]) => {
          this.pixelData = returnData;
        }, 
        (httpErrorResponse: HttpErrorResponse) => { 
          console.log("An error happened"); 
          console.log(httpErrorResponse); 
        }
      );
    */

    // For Promise
    this.digitImageService.getDigitImage(imageId)
      .then((result: ServiceLayerCallResult) => {
        console.log("Everything worked"); 
        this.pixelData = result.Content
      })
      .catch((result: ServiceLayerCallResult) => { 
        console.log("An error happened"); 
        console.log(`ErrorType: ${result.ErrorType}`)
        console.log(`SystemErrorMessage: ${result.SystemErrorMessage}`)
      });
  }
}
